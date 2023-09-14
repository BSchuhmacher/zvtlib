using System;
using System.Net.Sockets;
using System.Net;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer;
using Wiffzack.Devices.CardTerminals.Common;
using System.Xml;
using Wiffzack.Services.Utils;
using Wiffzack.Diagnostic.Log;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer
{
    public class NetworkTransport : IZvtTransport
    {

        private class State
        {
            public const int BUFFER_SIZE = 4096;

            public byte[] _buffer = new byte[BUFFER_SIZE];
            public TcpClient _client;
            public SocketError _socketError;
            public AsyncCallback _callback;
        }
		private int _RECEIVE_RESPONSE_TIMEOUT;
         public int RECEIVE_RESPONSE_TIMEOUT
		{
			get { return _RECEIVE_RESPONSE_TIMEOUT; }
			set { _RECEIVE_RESPONSE_TIMEOUT =(int) value;}
		}
		private int _MASTER_RESPONES_TIMEOUT;
		public int MASTER_RESPONES_TIMEOUT
		{
			get { return _MASTER_RESPONES_TIMEOUT; }
			set { _MASTER_RESPONES_TIMEOUT =(int) value;}
		}
        /// <summary>
        /// If not in master mode always receive requests
        /// </summary>
        private volatile bool _masterMode = true;

        /// <summary>
        /// TCPClient 
        /// </summary>
        private TcpClient _client = null;

        private ByteBuffer _receiveBuffer = new ByteBuffer();

        /// <summary>
        /// Transport configuration
        /// </summary>
        private XmlElement _config;

        private Logger _log = LogManager.Global.GetLogger("Wiffzack");

        #region IZvtTransport Members

        public bool MasterMode
        {
            get { return _masterMode; }
            set { _masterMode = value; }
        }


        public NetworkTransport(XmlElement config)
        {
            _config = config;
			
        }

        public void OpenConnection()
        {
			MASTER_RESPONES_TIMEOUT = XmlHelper.ReadInt(_config,"TimeoutT4",180000);
			RECEIVE_RESPONSE_TIMEOUT = XmlHelper.ReadInt(_config,"TimeoutT3",5000);
			_log.Info("Setting Timeouts");
			_log.Info("T4 Timeout: "+MASTER_RESPONES_TIMEOUT);
			_log.Info("T3 Timeout: "+RECEIVE_RESPONSE_TIMEOUT);
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse(XmlHelper.ReadString(_config, "RemoteIP")), XmlHelper.ReadInt(_config, "RemotePort", 5577));
            StartReceive(null);
        }

        /// <summary>
        /// Starts a new receive 
        /// </summary>
        /// <param name="myState"></param>
        private void StartReceive(State myState)
        {
            if (myState == null)
            {
                myState = new State();
                myState._client = _client;
                myState._callback = new AsyncCallback(ReceiveCallback);
            }

            myState._client.Client.BeginReceive(myState._buffer, 0, myState._buffer.Length, SocketFlags.None, out myState._socketError,
                myState._callback, myState);


        }

        /// <summary>
        /// called on data receive
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            State myState = (State)ar.AsyncState;

            //Maybe the connection has been closed or so
            if (myState._client == null || myState._client.Client == null || myState._client.Connected == false)
                return;

            try
            {
                int read = myState._client.Client.EndReceive(ar);

                //connection closed
                if (read == 0)
                {
                    //eeeek
                }
                else
                {
                    _receiveBuffer.Add(myState._buffer, 0, read);
                    StartReceive(myState);
                }
            }
            catch (Exception e)
            {
                _log.Warning("Network Transport error: {0}", e.ToString()); 
            }
        }

        public void CloseConnection()
        {
            if(_client.Connected)
                _client.Close();
        }

        public IZvtTpdu CreateTpdu(IZvtApdu apdu)
        {
            return new NetworkTpdu(apdu);
        }

        public IZvtTpdu CreateTpdu(byte[] apdu)
        {
            return new NetworkTpdu(apdu);
        }

        public void Transmit(IZvtTpdu tpdu)
        {
            byte[] data = tpdu.GetTPDUData();
            _log.Debug("Transmitting TPDU: {0}", ByteHelpers.ByteToString(data));
            _client.GetStream().Write(data, 0, data.Length);
        }

        public void Transmit(byte[] apduData)
        {
            Transmit(CreateTpdu(apduData));
        }

        public byte[] ReceiveResponsePacket()
        {
            int start = Environment.TickCount;

            int myTimeout = RECEIVE_RESPONSE_TIMEOUT;

            if (!_masterMode)
                myTimeout = MASTER_RESPONES_TIMEOUT;
			int waitTimeLeft=-1;
            while (NetworkTpdu.CreateFromBuffer(_receiveBuffer, false) == null && ( Environment.TickCount - start < myTimeout))
            {
				if(waitTimeLeft<1){
					_receiveBuffer.WaitForByte(myTimeout, false);
				}else{
					_receiveBuffer.WaitForByte(waitTimeLeft, false);
				}
				/*
				 * the waitTimeLeft variable is used if one waitForByte call does not wait for the full timeout duration
				 * Without the waitTimeLeft variable the system would try to wait another full timeout duration instead of
				 * the time left.
				 */
				waitTimeLeft=myTimeout-(Environment.TickCount - start);
            }

            NetworkTpdu responseTpdu = NetworkTpdu.CreateFromBuffer(_receiveBuffer, true);

            if(responseTpdu != null)
                _log.Debug("Received TPDU: {0}", ByteHelpers.ByteToString(responseTpdu.GetTPDUData()));

            if (responseTpdu == null)
                throw new ConnectionTimeOutException();
            else
                return responseTpdu.GetAPDUData();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_client != null)
            {
                CloseConnection();
                _client = null;
            }
        }

        #endregion
    }
}
