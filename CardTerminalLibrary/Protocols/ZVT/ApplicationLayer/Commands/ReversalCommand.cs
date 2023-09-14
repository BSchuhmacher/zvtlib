using System;
using System.Collections.Generic;
using System.Text;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Commands;
using Wiffzack.Diagnostic.Log;
using System.Xml;
using Wiffzack.Devices.CardTerminals.Common;
using Wiffzack.Services.Utils;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class ReversalCommand : CommandBase<ReversalApdu, PaymentResult>, IReversalCommand
    {
        
        private Logger _log = LogManager.Global.GetLogger("Wiffzack");

        /// <summary>
        /// Statusinformation that comes from the outside from a previous autorization request
        /// </summary>
        private StatusInformationApdu _outsideStatusInformation = null;

        #region ICommand Members
       

        public override void ReadSettings(XmlElement settings)
        {
            _outsideStatusInformation = StatusInformationApdu.CreateFromIData(settings);
            ReceiptNr = (int)_outsideStatusInformation.FindParameter<BCDNumberParameter>(StatusInformationApdu.StatusParameterEnum.ReceiptNr).DecodeNumber();
        }

        #endregion

        public int ReceiptNr
        {
            get { return _apdu.ReceiptNr; }
            set { _apdu.ReceiptNr = value; }
        }

        public ReversalCommand(IZvtTransport transport, ZVTCommandEnvironment commandEnvironment)
            : base(transport, commandEnvironment)
        {
            _apdu = new ReversalApdu();
        }

        public override PaymentResult Execute()
        {
            try
            {
                PaymentResult result = new PaymentResult();
                result.Success = true;

                if(_environment.RaiseAskOpenConnection())
                    _transport.OpenConnection();

                ApduCollection apdus = _commandTransmitter.TransmitAPDU(_apdu);
                CheckForAbortApdu(result, apdus);
				if(result.Success){
					//Contains the result (success or failure) and much information about the transaction
            		StatusInformationApdu statusInformation = apdus.FindFirstApduOfType<StatusInformationApdu>();
					result.Data=statusInformation;
				}

                result.PrintDocuments = _commandTransmitter.PrintDocuments;

                return result;
            }
            finally
            {
                if(_environment.RaiseAskCloseConnection())
                    _transport.CloseConnection();
            }
        }


    }
}
