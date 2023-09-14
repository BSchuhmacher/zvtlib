using System;
using System.Xml;

namespace Wiffzack.Devices.CardTerminals.Commands
{
    public delegate bool AskConnectionDelegate();

    /// <summary>
    /// Implemented by a protocol specific environment which manages 
    /// all objects (transport,...) need by the commands
    /// </summary>
    public interface ICommandEnvironment : IDisposable
    {
        event AskConnectionDelegate CloseConnection;
        event AskConnectionDelegate OpenConnection;

        event IntermediateStatusDelegate StatusReceived;

        #region Command factory
        IInitialisationCommand CreateInitialisationCommand(XmlElement settings);
        IPaymentCommand CreatePaymentCommand(XmlElement settings);
		ITelePaymentCommand CreateTelePaymentCommand(XmlElement settings);
        IReversalCommand CreateReversalCommand(XmlElement settings);
        IReportCommand CreateReportCommand(XmlElement settings);
        IEndOfDayCommand CreateEndOfDayCommand(XmlElement settings);
        IResetCommand CreateResetCommand(XmlElement settings);
        IDiagnosisCommand CreateDiagnosisCommand(XmlElement settings);
		IAbortCommand CreateAbortCommand(XmlElement settings);
		IRepeatReceiptCommand CreateRepeatReceiptCommand(XmlElement settings);
        #endregion



    }
}
