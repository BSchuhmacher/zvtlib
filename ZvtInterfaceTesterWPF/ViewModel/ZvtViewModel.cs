using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CommonLibs;
using JetBrains.Annotations;

namespace ZvtInterfaceTesterWPF.ViewModel;

public class ZvtViewModel:INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public ICommand ClearLogCommand { get; }
    public ICommand ChooseIpConnection { get; }
    public ICommand ChooseSerialConnection { get; }
    
    private string _logArea="";

    public string LogArea
    {
        get => string.IsNullOrWhiteSpace(_logArea)?"Nothing logged yet":_logArea;
        set
        {
            _logArea = value;
            OnPropertyChanged();
        }
    }

    public bool IpConnection { get; private set; } = true;

    public bool SerialConnection => !IpConnection;

    public bool ClearLogEnabled => !string.IsNullOrEmpty(_logArea);

    public Visibility IpSettingsVisibility => IpConnection ? Visibility.Visible : Visibility.Collapsed;

    public Visibility SerialSettingsVisibility => IpConnection ? Visibility.Collapsed : Visibility.Visible;

    public static string[]? ComPorts => SerialPort.GetPortNames();

    private string _selectedComPort;
    public string SelectedComPort
    {
        get => ComPorts?[0] ?? "<none>";
        set => _selectedComPort = value;
    }

    public ZvtViewModel()
    {
        ClearLogCommand = new RelayCommand(ClearLog, CanClearLog);
        ChooseIpConnection = new RelayCommand(SetIpConnection, CanSetIpConnection);
        ChooseSerialConnection = new RelayCommand(SetSerialConnection, CanSetSerialConnection);
    }

    private bool CanSetSerialConnection(object arg)
    {
        return IpConnection;
    }

    private void SetSerialConnection(object obj)
    {
        IpConnection = false;
        ConnectionTypeChange();
    }

    private bool CanSetIpConnection(object arg)
    {
        return !IpConnection;
    }

    private void SetIpConnection(object obj)
    {
        IpConnection = true;
        ConnectionTypeChange();
    }

    private void ConnectionTypeChange()
    {
        OnPropertyChanged(nameof(IpConnection));
        OnPropertyChanged(nameof(SerialConnection));
        OnPropertyChanged(nameof(IpSettingsVisibility));
        OnPropertyChanged(nameof(SerialSettingsVisibility));
        OnPropertyChanged(nameof(ComPorts));
        OnPropertyChanged(nameof(SelectedComPort));
    }

    private bool CanClearLog(object arg)
    {
        return !string.IsNullOrEmpty(_logArea);
    }

    private void ClearLog(object obj)
    {
        LogArea = "";
    }
}