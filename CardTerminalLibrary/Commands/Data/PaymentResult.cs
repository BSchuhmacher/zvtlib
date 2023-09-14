namespace Wiffzack.Devices.CardTerminals.Commands
{
    public class PaymentResult : CommandResult
    {
        /// <summary>
        /// Identifier of the payment process on the terminal
        /// </summary>
        private IData _data;

        public IData Data
        {
            get { return _data; }
			set { _data=(value); }
        }
		public PaymentResult(){
		}
        public PaymentResult(bool success, int? protocolSpecificErrorCode, string protocolSpecificDescription, IData data)
        {
            _success = success;
            _protocolSpecificErrorCode = protocolSpecificErrorCode;
            _protocolSpecificErrorDescription = StringHelper.addSpaces(protocolSpecificDescription);

            _data = data;
        }

    }
}
