namespace CardTerminalSimulator.Model;

public class TerminalSettings
{
    public string TerminalIpAddress
    {
        get;
        set;
    } = "127.0.0.1";

    public int TerminalPort { get; set; } = 65000;

    public ServerStatus ServerStatus { get; set; } = Model.ServerStatus.Offline;

}