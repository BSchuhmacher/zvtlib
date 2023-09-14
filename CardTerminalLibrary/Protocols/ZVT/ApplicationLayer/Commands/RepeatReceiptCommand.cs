using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Commands;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;
using Wiffzack.Diagnostic.Log;
using System.Xml;
using Wiffzack.Services.Utils;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class RepeatReceiptCommand:CommandBase<RepeatReceiptApdu, RepeatReceiptResult> ,IRepeatReceiptCommand
    {
        /// <summary>
        /// RepeatReceipt APDU
        /// </summary>
        private RepeatReceiptApdu _repeatReceipt;
		

        private Logger _log = LogManager.Global.GetLogger("Wiffzack");

        public RepeatReceiptCommand(IZvtTransport transport, ZVTCommandEnvironment env)
        :base (transport,env)
		{
            _repeatReceipt = new RepeatReceiptApdu();
        }


        public override RepeatReceiptResult Execute()
        {         
			RepeatReceiptResult repResult=null;
            try
            {
                if(_environment.RaiseAskOpenConnection())
                    _transport.OpenConnection();
                ApduCollection responses = _commandTransmitter.TransmitAPDU(_repeatReceipt);
                //Contains the result (success or failure) and much information about the transaction
	            StatusInformationApdu statusInformation = responses.FindFirstApduOfType<StatusInformationApdu>();
	
	            //Completion is only sent if everything worked fine
	            CompletionApduResponse completion = responses.FindFirstApduOfType<CompletionApduResponse>();
	            //Abort is only sent if something went wrong
	            AbortApduResponse abort = responses.FindFirstApduOfType<AbortApduResponse>();
	
	            //If the terminal is not registered a application layer nack (0x84 XX XX) is sent
	            StatusApdu status = responses.FindFirstApduOfType<StatusApdu>();
	
	            bool success = true;
	            int? errorCode = null;
	            string errorDescription = "";
	
	            if (completion == null && abort != null)
	            {
	                success = false;
	                errorCode = (byte)abort.ResultCode;
	                errorDescription = abort.ResultCode.ToString();
	            }
	            else if (statusInformation != null)
	            {
	                StatusInformationResultCode result = statusInformation.FindParameter<StatusInformationResultCode>(StatusInformationApdu.StatusParameterEnum.ResultCode);
	
	                if (result.ResultCode != StatusCodes.ErrorIDEnum.NoError)
	                {
	                    success = false;
	                    errorCode = (byte)result.ResultCode;
	                    errorDescription = result.ResultCode.ToString();
	                }
	            }
	            else if (status != null && status.Status != StatusCodes.ErrorIDEnum.NoError)
	            {
	                success = false;
	                errorCode = (byte)status.Status;
	                errorDescription = status.Status.ToString();
	            }
	
	            repResult = new RepeatReceiptResult(success, errorCode, errorDescription, statusInformation);
	            repResult.PrintDocuments = _commandTransmitter.PrintDocuments;
				_log.Debug("Documents:"+_commandTransmitter.PrintDocuments.Length);
	            }
            finally
            {
                if(_environment.RaiseAskCloseConnection())
                    _transport.CloseConnection();
            }

            
            return repResult;


        }


        public override void ReadSettings(XmlElement settings)
        {
			_repeatReceipt.ServiceByte.ECRrequiresStatusInformation=XmlHelper.ReadBool(settings,"EnableStatusInformation",false);
			_repeatReceipt.ServiceByte.DoNotPrintReceipt=XmlHelper.ReadBool(settings,"DontPrint",false);
			if(_repeatReceipt.ServiceByte.ECRrequiresStatusInformation==true || _repeatReceipt.ServiceByte.DoNotPrintReceipt == true)
				_repeatReceipt.EnableServiceByte=true;
        }
      
    }
}
