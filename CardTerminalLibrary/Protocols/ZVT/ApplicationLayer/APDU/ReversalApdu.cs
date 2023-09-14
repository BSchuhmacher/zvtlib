using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class ReversalApdu : ApduBase
    {
        private BCDNumberParameter _passwordParam = new BCDNumberParameter(0, 0, 0, 0, 0, 0);
        private PrefixedParameter<BCDNumberParameter> _receiptParam = new PrefixedParameter<BCDNumberParameter>(0x87, new BCDNumberParameter(0, 0, 0, 0));


        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0x30 }; }
        }

        public int ReceiptNr
        {
            get { return (int)_receiptParam.SubParameter.DecodeNumber(); }
            set { _receiptParam.SubParameter.SetNumber(value, 2); }
        }

        public ReversalApdu()
        {
            _parameters.Add(_passwordParam);
            _parameters.Add(_receiptParam);
        }
    }
}
