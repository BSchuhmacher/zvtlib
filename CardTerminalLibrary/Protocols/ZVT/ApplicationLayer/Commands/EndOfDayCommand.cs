using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Commands;
using System.Xml;
using Wiffzack.Diagnostic.Log;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class EndOfDayCommand : CommandBase<EndOfDayApdu, CommandResult>, IEndOfDayCommand
    {
        private Logger _log = LogManager.Global.GetLogger("Wiffzack");

        
        public EndOfDayCommand(IZvtTransport transport, ZVTCommandEnvironment commandEnvironment)
            : base(transport, commandEnvironment)
        {
            _apdu = new EndOfDayApdu();
        }

        public override CommandResult Execute()
        {
            try
            {
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

        public override void ReadSettings(XmlElement settings)
        {
            
        }

    }
}
