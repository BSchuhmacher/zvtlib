using System.Collections;

namespace CardTerminalSimulator.Model;

public sealed class ZvtCommands : Hashtable
{
    public ZvtCommands()
    {
        Add("CommandCompletion", new byte[] { 0x80, 0x00, 0x00 });
        Add("Completion", new byte[] { 0x06, 0x0F, 0x00 });
        Add("EndOfDay", new byte[] { 0x06, 0x50 });
    }
}