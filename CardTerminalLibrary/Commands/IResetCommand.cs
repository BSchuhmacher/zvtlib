namespace Wiffzack.Devices.CardTerminals.Commands
{
    /// <summary>
    /// Resets the terminal
    /// </summary>
    public interface IResetCommand:ICommand
    {

        CommandResult Execute();
    }
}
