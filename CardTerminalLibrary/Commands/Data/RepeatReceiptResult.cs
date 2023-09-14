namespace Wiffzack.Devices.CardTerminals.Commands
{
    public class RepeatReceiptResult : CommandResult
    {
        /// <summary>
        /// Identifier of the payment process on the terminal
        /// </summary>
        private IData _data;

        public IData Data
        {
            get { return _data; }
        }

        public RepeatReceiptResult(bool success, int? protocolSpecificErrorCode, string protocolSpecificDescription, IData data)
        {
            _success = success;
            _protocolSpecificErrorCode = protocolSpecificErrorCode;
            _protocolSpecificErrorDescription = StringHelper.addSpaces(protocolSpecificDescription);

            _data = data;
        }

    }
}
