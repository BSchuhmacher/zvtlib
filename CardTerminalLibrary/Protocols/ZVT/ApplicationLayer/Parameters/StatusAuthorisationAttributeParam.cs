namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters
{
    /// <summary>
    /// Represent the authorisation-attribute
    /// 
    /// TODO: interpretation
    /// </summary>
    public class StatusAuthorisationAttributeParam : FixedSizeParam
    {
        public StatusAuthorisationAttributeParam()
            :base(8)
        {
        }
    }
}
