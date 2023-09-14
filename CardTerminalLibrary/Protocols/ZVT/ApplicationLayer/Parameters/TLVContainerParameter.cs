using System;
using System.Collections.Generic;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
	class TLVContainerParameter : IParameter {
		public const byte BMP = 0x06;

		public readonly List<TLVItem> TLVItems;

		public int Length => GetBytes().Length;

		public TLVContainerParameter() : this(null) {
		}

		public TLVContainerParameter(List<TLVItem> tlvItems) {
			TLVItems = new List<TLVItem>();

			if (tlvItems != null) {
				TLVItems = tlvItems;
			}
		}

		public void ParseFromBytes(byte[] buffer, int offset) {
			throw new NotImplementedException();
		}

		public byte[] GetBytes() {
			var subItemData = new List<byte>();
			foreach (var item in TLVItems) {
				subItemData.AddRange(item.GetBytes());
			}

			var data = new List<byte>();
			data.Add(BMP);
			data.AddRange(TLVItem.GetLengthData(subItemData.Count));
			data.AddRange(subItemData);

			return data.ToArray();
		}

		public void AddToBytes(List<byte> buffer) {
			buffer.AddRange(GetBytes());
		}
	}
}
