using System;
using System.Collections.Generic;
using Wiffzack.Devices.CardTerminals.Commands;
using System.Xml;
using Wiffzack.Services.Utils;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer
{
    /// <summary>
    /// Implements the ZVT-specific command environment,
    /// which manages the transport layer and command creation
    /// </summary>
    public class ZVTCommandEnvironment:ICommandEnvironment
    {
        public event AskConnectionDelegate CloseConnection;

        public event AskConnectionDelegate OpenConnection;

        /// <summary>
        /// Raised when an intermediate status is received
        /// </summary>
        public event IntermediateStatusDelegate StatusReceived;

        /// <summary>
        /// Contains the configuration of the environment
        /// </summary>
        private XmlElement _environmentConfig;

        /// <summary>
        /// TransportLayer to use
        /// </summary>
        private IZvtTransport _transport;

        public XmlElement RegistrationCommandConfig
        {
            get
            {
                XmlElement config = (XmlElement)_environmentConfig.SelectSingleNode("RegistrationCommand");
				
                if (config == null)
                {
                    config = (XmlElement)_environmentConfig.AppendChild(_environmentConfig.OwnerDocument.CreateElement("RegistrationCommand"));
                }

                return config;
            }
        }


        public ZVTCommandEnvironment(XmlElement environmentConfig)
        {
            _environmentConfig = environmentConfig;
            string transport = XmlHelper.ReadString(environmentConfig, "Transport");
            if (transport == null)
                throw new ArgumentException("No transport layer specified");

            if (transport.Equals("serial", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlElement serialConfig = (XmlElement)environmentConfig.SelectSingleNode("TransportSettings");
				
                if(serialConfig == null)
                    throw new ArgumentException("No serial configuration specified");

                _transport = new RS232Transport(serialConfig);
            }
            else if (transport.Equals("network", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlElement networkConfig = (XmlElement)environmentConfig.SelectSingleNode("TransportSettings");
                if (networkConfig == null)
                    throw new ArgumentException("No network configuration specified");
                _transport = new NetworkTransport(networkConfig);
            }
        }

        public void RaiseIntermediateStatusEvent(IntermediateStatus status) {
            StatusReceived?.Invoke(status);
        }

        private void ReadSettings(ICommand command, XmlElement settings)
        {
            if (settings != null)
                command.ReadSettings(settings);
        }

        #region ICommandEnvironment Members

        public IInitialisationCommand CreateInitialisationCommand(XmlElement settings) {
            return CreateInitialisationCommand(settings, null);
        }

        public IInitialisationCommand CreateInitialisationCommand(XmlElement settings, List<TLVItem> tlvParameters)
        {
            RegistrationCommand cmd = new RegistrationCommand(_transport, this, tlvParameters);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        public IPaymentCommand CreatePaymentCommand(XmlElement settings)
        {
            AuthorizationCommand cmd = new AuthorizationCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

		public IPaymentCommand CreateRefundCommand(XmlElement settings) {
			RefundCommand cmd = new RefundCommand(_transport, this);
			cmd.Status += RaiseIntermediateStatusEvent;
			ReadSettings(cmd, settings);
			return cmd;
		}

		public ITelePaymentCommand CreateTelePaymentCommand(XmlElement settings)
        {
            TeleAuthorizationCommand cmd = new TeleAuthorizationCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        public IReversalCommand CreateReversalCommand(XmlElement settings)
        {
            ReversalCommand cmd = new ReversalCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        public IReportCommand CreateReportCommand(XmlElement settings)
        {
            ReportCommand cmd =  new ReportCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        public IEndOfDayCommand CreateEndOfDayCommand(XmlElement settings)
        {
            EndOfDayCommand cmd = new EndOfDayCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        public IResetCommand CreateResetCommand(XmlElement settings)
        {
            ResetCommand cmd = new ResetCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }
		
		public IAbortCommand CreateAbortCommand(XmlElement settings)
        {
            AbortCommand cmd = new AbortCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }


        public IDiagnosisCommand CreateDiagnosisCommand(XmlElement settings)
        {
            NetworkDiagnosisCommand cmd = new NetworkDiagnosisCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }
		
		 public IRepeatReceiptCommand CreateRepeatReceiptCommand(XmlElement settings)
        {
            RepeatReceiptCommand cmd = new RepeatReceiptCommand(_transport, this);
            cmd.Status += RaiseIntermediateStatusEvent;
            ReadSettings(cmd, settings);
            return cmd;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_transport != null)
            {
                _transport.Dispose();
                _transport = null;
            }
        }

        #endregion

        public bool RaiseAskCloseConnection()
        {
            if (CloseConnection == null)
                return true;

            return CloseConnection();
        }

        public bool RaiseAskOpenConnection()
        {
            if (OpenConnection == null)
                return true;

            return OpenConnection();
        }
    }
}
