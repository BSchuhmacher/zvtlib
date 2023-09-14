namespace Wiffzack.Devices.CardTerminals.Commands
{
    /// <summary>
    /// Performs a diagnose on the terminal device
    /// </summary>
    public interface IAbortCommand : ICommand
    {
        CommandResult Execute();
    }
}