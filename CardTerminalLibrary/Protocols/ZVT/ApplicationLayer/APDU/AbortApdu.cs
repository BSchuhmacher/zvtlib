using System.Collections.Generic;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
	public class AbortApdu : ApduBase
	{
		public AbortApdu ()
		{
			_parameters=new List<IParameter>();
		}
		public override bool SendsCompletionPacket
        {
            get { return false; }
        }

         protected override byte[] ByteControlField
        {
            get { return new byte[] { 0x06, 0xB0 }; }
        }

	}
}

