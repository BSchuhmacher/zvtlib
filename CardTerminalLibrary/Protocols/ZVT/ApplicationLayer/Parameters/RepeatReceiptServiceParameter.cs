namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
    public class RepeatReceiptServiceByteParameter : BitConfigParameter
    {
        /// <summary>
        /// xxxx xxx1 ECR requires Status-Information (as in the original transaction)
        /// xxxx xxx0 Do not send Status-Information
        /// omitted).
        /// </summary>
        public bool ECRrequiresStatusInformation
        {
            get { return GetBit(0); }
            set { SetBit(0, value); }
        }

        /// <summary>
        /// xxxx xx1x No print receipt (neither Print line commands sent from PT nor printed on PT itself).
        /// xxxx xx0x Print receipt (either Printline commands sent from PT or printed on PT itself)
        /// </summary>
        public bool DoNotPrintReceipt
        {
            get { return GetBit(1); }
            set { SetBit(1, value); }
        }

    }
}
