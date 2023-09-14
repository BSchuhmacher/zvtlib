using System;
using System.Collections.Generic;
using System.Linq;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters.TLV;
namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
	public enum TLVClass : byte {
		Universal = 0,
		Application = 0b01,
		ContextSpecific = 0b10,
		Private = 0b11
	}

	/// <summary>
	/// Available tag-field values for TLV objects.
	/// Tags with more than two bytes are not supported.
	/// The MSB represents byte 1 and contains class, object type and tag-number.
	/// The LSB represents byte 2 and contains tag-numbers > 0b11110 according to ZVT chapter 9.3.1.
	/// </summary>
	public enum TagValue : ushort {
		ZvtCommand = 0x0A,
		ListOfPermittedZvtCommands = 0x26,
		EMVConfigParameter = 0x40,
		TotalLengthOfFile = 0x1F00,
		Date = 0x1F0E,
		Time = 0x1F0F
	}

	public class TLVItem {
		public TLVTag Tag { get; }
		public List<byte> Data { get; }
		public List<TLVItem> SubItems { get; }
		
		public TLVItem(TLVTag tag, List<byte> data) : this(tag, data, null) {
		}

		public TLVItem(TLVTag tag, List<TLVItem> subItems = null) : this(tag, null, subItems) {
		}

		private TLVItem(TLVTag tag, List<byte> data = null, List<TLVItem> subItems = null) {
			Tag = tag;

			Data = new List<byte>();
			if (data != null) {
				Data.AddRange(data);
			}

			SubItems = new List<TLVItem>();
			if (subItems != null) {
				SubItems.AddRange(subItems);
			}
		}

		public byte[] GetBytes() {
			var data = new List<byte>();

			data.AddRange(Tag.Data);

			// Primitive data objects only contain custom Data
			if (Tag.IsPrimitive && Data.Any()) {
				data.AddRange(GetLengthData(Data.Count));
				data.AddRange(Data);	
			}

			// Constructed data objects only contain sub-data-objects
			if (!Tag.IsPrimitive && SubItems.Any()) {
				var subItemData = new List<byte>();
				foreach (var subItem in SubItems) {
					subItemData.AddRange(subItem.GetBytes());
				}

				data.AddRange(GetLengthData(subItemData.Count));
				data.AddRange(subItemData);
			}
	
			return data.ToArray();
		}

		public static byte[] GetLengthData(int length) {
			var data = new List<byte>();

			if (length <= sbyte.MaxValue) {
				// only first length byte required
				data.Add((byte)length);
			} else if (length <= byte.MaxValue) {
				// indicate additional length byte use second byte for actual length value
				data.Add(0b1000_0001);
				data.Add((byte)length);
			} else if (length <= ushort.MaxValue) {
				// indicate two additional length bytes use second and third byte for actual length value
				data.Add(0b1000_0010);
				data.Add((byte)(length >> 8));
				data.Add((byte)length);
			} else {
				throw new NotSupportedException($"TLV length of more than {ushort.MaxValue} bytes is not supported.");
			}

			return data.ToArray();
		}
	}
}

