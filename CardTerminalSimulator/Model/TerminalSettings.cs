namespace CardTerminalSimulator.Model;

public class TerminalSettings
{
    public string TerminalIpAddress
    {
        get;
        set;
    }

    public int TerminalPort { get; set; }

    public ServerStatus ServerStatus { get; set; }

}