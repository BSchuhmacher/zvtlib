using System;
using System.Collections.Generic;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters.TLV
{
	public class TLVTag {
		public byte[] Data { get; private set; }
		public TLVClass Class { get; private set; }
		public bool IsPrimitive { get; private set; }
		public ushort Number { get; private set; }

		/// <summary>
		/// Create a TLVTag where
		/// - byte 1 contains class, object-type and tag-number
		/// - byte 2 is optional and can contain the actual tag-number
		/// </summary>
		/// <param name="data"></param>
		public TLVTag(byte[] data) {
			Init(data);
		}

		public TLVTag(TagValue tag) {
			var tagValue = (ushort)tag;
			var data = new List<byte>();

			// Check if we have a two-byte tag
			if (tagValue >= byte.MaxValue) {
				data.Add((byte)(tagValue >> 8));
				data.Add((byte)tagValue);
			} else {
				data.Add((byte)tagValue);
			}

			Init(data.ToArray());
		}

		private void Init(byte[] data) {
			if (data == null) {
				throw new ArgumentNullException(nameof(data));
			}

			if (data.Length < 1) {
				throw new ArgumentOutOfRangeException(nameof(data), "Tag data is empty.");
			}

			if (data.Length > 2) {
				throw new ArgumentOutOfRangeException(nameof(data), "Implemented support for one or two byte tags only.");
			}

			var tagByte1 = data[0];
			var tagByte2 = data.Length > 1 ? data[1] : 0;

			if ((tagByte2 & 0b1000_0000) != 0) {
				throw new NotImplementedException($"Tag-field 0x{data:X4} indicates a third byte following but implemented support for one or two byte tags only.");
			}

			Data = data;
			Class = (TLVClass)(tagByte1 & 0b1100_0000);
			IsPrimitive = (tagByte1 & 0b0010_0000) == 0;
			Number = (tagByte1 & 0b11111) != 0b11111 ? (ushort)(tagByte1 & 0b11111) : (ushort)tagByte2;
		}
	}
}

