using System;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class RefundApdu : ApduBase
    {

        /// <summary>
        /// The amount is 6 byte BCD-packed, amount in Euro-cents with leading zeros.
        /// </summary>
        private PrefixedParameter<BCDNumberParameter> _amountParam = new PrefixedParameter<BCDNumberParameter>(0x04, new BCDNumberParameter(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        /// <summary>
        /// CC
        /// </summary>
        private PrefixedParameter<CurrencyCodeParameter> _currencyCodeParam = new PrefixedParameter<CurrencyCodeParameter>(0x49, new CurrencyCodeParameter());

		/// <summary>
		/// Password parameter is required but not evaluated
		/// </summary>
		private BCDNumberParameter _Password = new BCDNumberParameter(0, 0, 0, 0, 0, 0);

		public Int64 CentAmount
        {
            get { return _amountParam.SubParameter.DecodeNumber(); }
            set { _amountParam.SubParameter.SetNumber(value, 6); }
        }

        public RefundApdu()
        {
			_parameters.Add(_Password);
            _parameters.Add(_amountParam);
            _parameters.Add(_currencyCodeParam);
        }

        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0x31 }; }
        }
    }
}
