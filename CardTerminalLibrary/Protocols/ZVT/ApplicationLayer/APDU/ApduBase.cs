﻿using System.Collections.Generic;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    public abstract class ApduBase:IZvtApdu
    {
        /// <summary>
        /// Stores all parameters associated with this apdu
        /// </summary>
        protected List<IParameter> _parameters = new List<IParameter>();


        /// <summary>
        /// Gets the Controlfield (first 2 bytes) of tha APDU
        /// </summary>
        protected abstract byte[] ByteControlField { get; }

        #region IZvtApdu Members

        public virtual bool SendsCompletionPacket
        {
            get { return true; }
        }

        public virtual ControlField ControlField
        {
            get { return new ControlField(ByteControlField); }
        }
        

        public virtual byte[] GetRawApduData()
        {
            List<byte> buffer = new List<byte>();

            foreach (IParameter param in _parameters)
                param.AddToBytes(buffer);
			int len=buffer.Count;
			byte[] lenarr=ParameterByteHelper.convertLength(len);
			for(int i=lenarr.Length-1;i>=0;i--){
				buffer.Insert(0,lenarr[i]);
			}
			if(lenarr.Length>=2)
				buffer.Insert(0,0xFF);
            buffer.InsertRange(0, ByteControlField);
            return buffer.ToArray();

        }

        #endregion

    }
}
