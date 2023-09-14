using System.Text;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
    public class AsciiFixedSizeParameter : FixedSizeParam
    {

        public string Text
        {
            get { return Encoding.GetEncoding(28591).GetString(_myData); }
            set { _myData = Encoding.GetEncoding(28591).GetBytes(value); }
        }

        public AsciiFixedSizeParameter(int size)
            : base(size)
        {

        }

    }
}
