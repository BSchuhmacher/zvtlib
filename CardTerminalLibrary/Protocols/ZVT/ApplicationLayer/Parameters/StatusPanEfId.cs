using System;
using System.Collections.Generic;
using System.Text;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
    /// <summary>
    /// Represents PAN for magnet-stripe and ED_ID for chip
    /// 
    /// TODO: Data interpretation 
    /// </summary>
    public class StatusPanEfId : LVarBCDNumberParameter
    {
        public StatusPanEfId(params byte[] id)
            : base(2, id)
        {
        }
		/// <summary>
        /// Decodes and returns the encoded number
        /// </summary>
        /// <returns></returns>
        public static String BCDDecodeNumberAsString(byte[] decodedBytes)
        {
            String num ="";

            for (int i = 0; i < decodedBytes.Length; i++)
            {
				byte current = decodedBytes[i];
			    if(current==0x0E){
					num+="*";
				}else{
					if((current==0x0F) && (i>=decodedBytes.Length-1)){
					}else{
						num += (Int64)(current);
					}
				}
            }

            return num;
        }
		public String DecodeNumberAsString()
		{
			return BCDDecodeNumberAsString(BCDGetDecodedBytes(_bytes.ToArray()));
		}


    }
}
