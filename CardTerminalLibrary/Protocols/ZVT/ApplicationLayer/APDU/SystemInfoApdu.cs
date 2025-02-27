﻿using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class SystemInfoApdu : ApduBase
    {
        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x0f, 0x11 }; }
        }

        public SystemInfoApdu()
        {
            _parameters.Add(new BCDNumberParameter(0, 0, 0, 0, 0, 0));
        }

    }
}
