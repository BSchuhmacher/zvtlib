﻿namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class DiagnosisApdu : ApduBase
    {
        protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0x70 }; }
        }
    }
}
