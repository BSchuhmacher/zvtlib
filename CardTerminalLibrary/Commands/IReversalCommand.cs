namespace Wiffzack.Devices.CardTerminals.Commands
{
    /// <summary>
    /// Starts the process of reversal,
    /// keep in mind that the reversal process can fail for different protocols in different situations
    /// </summary>
    public interface IReversalCommand : ICommand
    {
        PaymentResult Execute();
    }
}
