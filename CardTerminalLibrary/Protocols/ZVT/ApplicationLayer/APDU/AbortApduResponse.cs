using System;
using System.Collections.Generic;
using System.Text;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public class AbortApduResponse : ApduResponse
    {

        public StatusCodes.ErrorIDEnum ResultCode
        {
            get { return (StatusCodes.ErrorIDEnum)base._rawApduData[3]; }
        }

        public AbortApduResponse(params byte[] rawData)
            :base(rawData)
        {
        }

    }
}
