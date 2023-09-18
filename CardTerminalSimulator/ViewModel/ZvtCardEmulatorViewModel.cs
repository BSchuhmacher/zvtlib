using System.ComponentModel;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using CardTerminalSimulator.Model;

namespace CardTerminalSimulator.ViewModel;

public class ZvtCardEmulatorViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly TerminalSettings _terminalSettings;

    private string _terminalIpAddress;

    public string TerminalIpAddress
    {
        get => _terminalIpAddress;
        set
        {
            _terminalIpAddress = value;
            _terminalSettings.TerminalIpAddress = value;
            OnPropertyChanged();
        }
    }

    private int _terminalPort;

    public int TerminalPort
    {
        get => _terminalPort;
        set
        {
            _terminalPort = value;
            _terminalSettings.TerminalPort = value;
            OnPropertyChanged();
        }
    }

    private ServerStatus _serverStatus;

    public ServerStatus ServerStatus
    {
        get => _serverStatus;
        set
        {
            _serverStatus = value;
            StartStopServer = value == ServerStatus.Offline ? "Start server" : "Stop server";
            OnPropertyChanged();
            OnPropertyChanged(nameof(ServerStatusColor));
        }
    }

    private string _logArea;

    public string LogArea
    {
        get => string.IsNullOrEmpty(_logArea) ? "Nothing happened yet!" : _logArea;
        set
        {
            _logArea = value;
            OnPropertyChanged();
        }
    }

    private string _startStopServer = "Start";

    public string StartStopServer
    {
        get => _startStopServer;
        set
        {
            _startStopServer = value;
            OnPropertyChanged();
        }
    }

    public SolidColorBrush ServerStatusColor => ServerStatus == ServerStatus.Offline ? Brushes.Red : Brushes.Green;

    public ICommand ToggleServerState { get; }

    private bool CanServerStateBeToggled(object arg)
    {
        return !string.IsNullOrWhiteSpace(_terminalIpAddress) && _terminalPort > 1024;
    }

    private void DoToggleServerState(object obj)
    {
        ServerStatus = ServerStatus == Model.ServerStatus.Offline ? ServerStatus.Online : ServerStatus.Offline;
    }

    public ZvtCardEmulatorViewModel()
    {
        _terminalSettings = new TerminalSettings();
        TerminalIpAddress = "127.0.0.1";
        TerminalPort = 65000;

        ToggleServerState = new RelayCommand(DoToggleServerState, CanServerStateBeToggled);
    }
}