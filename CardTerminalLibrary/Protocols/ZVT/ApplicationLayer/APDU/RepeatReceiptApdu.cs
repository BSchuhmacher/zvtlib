using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class RepeatReceiptApdu : ApduBase
    {
        /// <summary>
        /// Password parameter is required but not evaluated
        /// </summary>
        private BCDNumberParameter _passwordParam = new BCDNumberParameter(1, 2, 3, 4, 5, 6);

        /// <summary>
        /// Defines the service byte
        /// </summary>
        private OptionalParameter<PrefixedParameter<RepeatReceiptServiceByteParameter>> _serviceByte = 
            new OptionalParameter<PrefixedParameter<RepeatReceiptServiceByteParameter>>(false, 
                new PrefixedParameter<RepeatReceiptServiceByteParameter>(0x03, 
                    new RepeatReceiptServiceByteParameter()));


        public BCDNumberParameter Password
        {
            get { return _passwordParam; }
        }

        public RepeatReceiptServiceByteParameter ServiceByte
        {
            get { return _serviceByte.SubParameter.SubParameter; }
        }

        public bool EnableServiceByte
        {
            get { return _serviceByte.Enabled; }
            set { _serviceByte.Enabled = value; }
        }

        public RepeatReceiptApdu()
        {
            _parameters.Add(_passwordParam);
            _parameters.Add(_serviceByte);
        }

        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0x20 }; }
        }
    }
}
