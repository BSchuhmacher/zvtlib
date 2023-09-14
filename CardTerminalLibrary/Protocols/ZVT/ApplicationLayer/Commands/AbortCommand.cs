using System;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Commands;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class AbortCommand : CommandBase<AbortApdu, CommandResult>, IAbortCommand
    {

        public AbortCommand(IZvtTransport transport, ZVTCommandEnvironment commandEnvironment)
            :base (transport, commandEnvironment)
        {
            base._apdu = new AbortApdu();
        }

        public override CommandResult Execute()
        {
            try
            {
				Console.WriteLine("Executing Abort Command");
                CommandResult result = new CommandResult();
                result.Success = true;

                if(_environment.RaiseAskOpenConnection())
                    _transport.OpenConnection();

                ApduCollection apdus = _commandTransmitter.TransmitAPDU(_apdu);
                CheckForAbortApdu(result, apdus);
                result.PrintDocuments = _commandTransmitter.PrintDocuments;
                return result;
            }
            finally
            {
                if(_environment.RaiseAskCloseConnection())
                    _transport.CloseConnection();
            }
        }

        #region ICommand Members

        public override void ReadSettings(System.Xml.XmlElement settings)
        {
        }

        #endregion
    }
}
