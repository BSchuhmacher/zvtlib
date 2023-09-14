using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class EndOfDayApdu : ApduBase
    {
        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0x50 }; }
        }

        public EndOfDayApdu()
        {
            _parameters.Add(new BCDNumberParameter(0, 0, 0, 0, 0, 0));
        }
    }
}
