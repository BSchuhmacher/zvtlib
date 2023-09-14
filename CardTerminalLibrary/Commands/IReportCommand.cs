namespace Wiffzack.Devices.CardTerminals.Commands
{
    /// <summary>
    /// Prints several terminal reports
    /// </summary>
    public interface IReportCommand : ICommand
    {
        CommandResult Execute();
    }
}
