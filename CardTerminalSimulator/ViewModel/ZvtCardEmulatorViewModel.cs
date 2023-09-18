using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using CardTerminalSimulator.Model;
using CommonLibs;
using JetBrains.Annotations;
using SuperSimpleTcp;

namespace CardTerminalSimulator.ViewModel;

public class ZvtCardEmulatorViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly TerminalSettings _terminalSettings;
    private SimpleTcpServer? _tcpServer;
    private ZvtCommands _zvtCommands;

    private string? _terminalIpAddress;

    public string? TerminalIpAddress
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
        get => string.IsNullOrEmpty(_logArea) ? $"Nothing happened yet!{Environment.NewLine}" : _logArea;
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
        if (ServerStatus == ServerStatus.Offline)
        {
            _tcpServer = new SimpleTcpServer(_terminalIpAddress, _terminalPort);
            _tcpServer.Events.DataReceived += Events_DataReceived;
            _tcpServer.Start();
            LogArea +=
                $"Terminal Simulator started listening on IP {_terminalIpAddress}:{_terminalPort}.{Environment.NewLine}";
        }
        else
        {
            if (_tcpServer != null)
            {
                _tcpServer.Events.DataReceived -= Events_DataReceived;
                _tcpServer.Stop();
                _tcpServer.Dispose();
                _tcpServer = null;
                LogArea +=
                    $"Terminal Simulator stopped!{Environment.NewLine}";
            }
        }

        ServerStatus = ServerStatus == ServerStatus.Offline ? ServerStatus.Online : ServerStatus.Offline;
    }

    private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
    {
        if (_tcpServer == null)
        {
            return;
        }

        LogArea += $"Data received: {BitConverter.ToString(e.Data.ToArray())}";

        switch (e.Data.AsSpan())
        {
            // case var s when s.StartsWith(new byte[] { 0x06, 0x01 }):
            //     Thread.Sleep(500);
            //
            //     LogArea += $"Send Command Completion{Environment.NewLine}";
            //     var t = (byte[])_zvtCommands["CommandCompletion"]!;
            //     _tcpServer.Send(e.IpPort, (byte[])_zvtCommands["CommandCompletion"]!);
            //
            //     Thread.Sleep(1000);
            //
            //     LogArea += $"Send Completion{Environment.NewLine}";
            //     _tcpServer.Send(e.IpPort, (byte[])_zvtCommands["Completion"]!);
            //     break;
            default:
                LogArea += $"Unknown command received: {e.Data.ToString()}{Environment.NewLine}";
                break;
        }
    }

    public ZvtCardEmulatorViewModel()
    {
        _terminalSettings = new TerminalSettings();
        TerminalIpAddress = "127.0.0.1";
        TerminalPort = 65000;

        _tcpServer = null;
        _zvtCommands = new ZvtCommands();

        ToggleServerState = new RelayCommand(DoToggleServerState, CanServerStateBeToggled);

        _logArea = "";
    }
}