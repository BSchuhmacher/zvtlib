using System;
using System.IO.Ports;
using System.IO;
using System.Xml;
using Wiffzack.Services.Utils;

namespace Wiffzack.Communication
{
    public class SerialComm:ICommunication {
        private readonly object _SerialPortLock = new object();
        private const int BUFFER_SIZE = 256;

        /// <summary>
        /// Gibt an ob das serielle Port bei Konfiguration automatisch geöffnet werden soll
        /// (notwendig für ICommunication)
        /// </summary>
        public bool AutoOpen { get; set; } = true;

        private struct StateObj
        {
            public byte[] data;
        }

        private SerialPort _port;
        private XmlElement _config;

        public SerialComm()
        {
        }

        public SerialComm(XmlDocument config)
        {
            _config = config.DocumentElement;
            LoadConfig();
        }

        /// <summary>
        /// Wartet auf Daten am seriellen Port
        /// </summary>
        private void StartRead()
        {
            try
            {
                lock (_SerialPortLock)
                {
					_port.ReadTimeout=10000;
					_port.WriteTimeout=2000;
                    StateObj state;
                    state.data = new byte[BUFFER_SIZE];

                    if (_port.IsOpen) {
                        _port.BaseStream.BeginRead(state.data, 0, BUFFER_SIZE, ReadCallback, state);
                    }
                }
            }
            catch (Exception e)
            { Console.WriteLine(e.Message);}
        }

        /// <summary>
        /// Es wurden Daten gelesen
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int read;
                lock (_SerialPortLock) {
                    if (_port.IsOpen) {
                        read = _port.BaseStream.EndRead(ar);
                        StateObj state = (StateObj)ar.AsyncState;
                        OnDataReceived?.Invoke(state.data, read);

                    }
                }
                
                StartRead();
            }
            //Die Verbindung wurde beendet
            catch (IOException) {
                OnConnectionClosed?.Invoke(this);
                //_port.Close();
            }
            catch (Exception)
            { }
        }

        public void SetLines(bool RTS, bool DTR)
        {
            lock (_SerialPortLock)
            {
                _port.RtsEnable = RTS;
                _port.DtrEnable = DTR;
            }
        }

        #region ICommunication Members

        public event OnDataReceivedDelegate OnDataReceived;

        public event Action<ICommunication> OnConnectionEstablished;

        public event Action<ICommunication> OnConnectionClosed;

        public void SetupCommunication(XmlElement setup)
        {
            _config = setup;
            LoadConfig();           
        }


        public void LoadConfig()
        {
            lock (_SerialPortLock) {
                _port = new SerialPort(XmlHelper.ReadString(_config, "Port"),
                    XmlHelper.ReadInt(_config, "BaudRate", 9600),
                    XmlHelper.ReadEnum(_config, "Parity", Parity.Even),
                    XmlHelper.ReadInt(_config, "DataBits", 8),
                    XmlHelper.ReadEnum(_config, "StopBits", StopBits.One));

                _port.ReadBufferSize = Math.Max(4096, XmlHelper.ReadInt(_config, "ReadBuffer", 4096));
                _port.WriteBufferSize = Math.Max(4096, XmlHelper.ReadInt(_config, "WriteBuffer", 4096));
            }

            if(AutoOpen)
                Open();
        }


        public void SendData(byte[] data, int offset, int length)
        {
            lock (_SerialPortLock)
            {
                _port.BaseStream.Write(data, offset, length);
            }
        }


        public void Dispose()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
        }

        #endregion

        /// <summary>
        /// Achtung: Nicht Teil von ICommunication
        /// </summary>
        public void Open()
        {
            lock (_SerialPortLock) {
                if (!_port.IsOpen)
                    _port.Open();
            }

            OnConnectionEstablished?.Invoke(this);
            StartRead();
        }

        /// <summary>
        /// Achtung: Nicht Teil von ICommunication
        /// </summary>
        public void Close()
        {
            lock (_SerialPortLock) {
                if (_port != null && _port.IsOpen) {
                    _port.Close();
                }
            }
        }

    }
}
