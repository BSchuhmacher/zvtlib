using System;
using System.Collections.Generic;
using System.Text;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Diagnostic.Log;
namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.ApduHandlerDefinitions
{
    public class AckSenderApduHandler : IApduHandler
    {
        /// <summary>
        /// Apdus compatible with this handler
        /// </summary>
        private List<byte[]> _compatibleApdus = new List<byte[]>();

        /// <summary>
        /// Transport layer
        /// </summary>
        private IZvtTransport _transport;
		private Logger _log = LogManager.Global.GetLogger("Wiffzack");
        public AckSenderApduHandler(IZvtTransport transport)
        {
            _transport = transport;

            //Completion
            _compatibleApdus.Add(new byte[] { 0x06, 0x0f });

            //Status
            _compatibleApdus.Add(new byte[] { 0x04, 0x0f });

            //Intermediate Status
            _compatibleApdus.Add(new byte[] { 0x04, 0xff });

            //Abort
            _compatibleApdus.Add(new byte[] { 0x06, 0x1e });

            //Print Line
            _compatibleApdus.Add(new byte[] { 0x06, 0xd1 });

            //Print Text Block
            _compatibleApdus.Add(new byte[] { 0x06, 0xd3 });            
        }


        #region IApduHandler Members

        public void StartCommand()
        {
        }

        /// <summary>
        /// Checks if this handler is compatible with the specified apdu
        /// </summary>
        /// <param name="responseApdu"></param>
        /// <returns></returns>
        public bool IsCompatibleHandler(IZvtApdu responseApdu)
        {
            _log.Info("Received: {0:x2}, {1:x2}", responseApdu.ControlField.Class, responseApdu.ControlField.Instruction);
            foreach (byte[] compatibleApdu in _compatibleApdus)
            {
                if (responseApdu.ControlField.Equals(compatibleApdu))
                    return true;
            }

            return false;
        }

        public void Process(IZvtApdu requestApdu, IZvtApdu responseApdu)
        {
            _transport.Transmit(_transport.CreateTpdu(new StatusApdu()));
        }

        #endregion
    }
}
