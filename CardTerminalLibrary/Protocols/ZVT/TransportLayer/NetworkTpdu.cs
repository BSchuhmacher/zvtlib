using System;
using System.Collections.Generic;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer;
using Wiffzack.Devices.CardTerminals.Common;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer
{
    /// <summary>
    /// Encapsulates an Apdu for transmission over Network
    /// </summary>
    public class NetworkTpdu : TpduBase
    {
        /// <summary>
        /// If available, extracts one whole apdu and removes the data from buffer
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetworkTpdu CreateFromBuffer(ByteBuffer data, bool removeFromBuffer)
        {
            if (data.Count > 2)
            {
                List<byte> myData = new List<byte>();
                myData.Add(data[0]);
                myData.Add(data[1]);

                //Length calculation
                //There are 2 length modes, if the single length byte is 0xff
                //then the 2 following bytes are interpreted as length
                int length;
                byte bLength = data[2];
                myData.Add(data[2]);
                int dataStartIndex = 2;
                if (bLength == 0xFF)
                {
                    dataStartIndex += 2;
                    if (data.Count >= 5)
                    {
                        myData.Add(data[3]);
                        myData.Add(data[4]);
                        length = BitConverter.ToUInt16(new byte[] { data[3], data[4] }, 0);
                    }
                    else
                        return null;
                }
                else
                {
                    dataStartIndex++;
                    length = bLength;
                }


                if (data.Count >= dataStartIndex + length)
                {
                    for(int i = dataStartIndex; i<dataStartIndex+length; i++)
                        myData.Add(data[i]);

                    if(removeFromBuffer)
                        data.Remove(myData.Count);

                    return new NetworkTpdu(myData.ToArray());
                }
            }

            return null;
        }


        public NetworkTpdu(IZvtApdu apdu)
            :this(apdu.GetRawApduData())
        {
        }

        public NetworkTpdu(byte[] data)
        {
            this.ApduData = data;
        }

        private NetworkTpdu()
        {
        }
    }
}
