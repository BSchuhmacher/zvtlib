﻿using System;
using System.Collections.Generic;
using System.Text;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Commands;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class ReportCommand :CommandBase<ReportApdu, CommandResult>, IReportCommand
    {
        private SystemInfoApdu _systemInfo = new SystemInfoApdu();

        /// <summary>
        /// Indicates if the system info should be printed
        /// </summary>
        private bool _printSystemInfo = false;

        /// <summary>
        /// Indicates if the Terminal report should be printed
        /// </summary>
        private bool _printReport = true;


        public bool PrintSystemInfo
        {
            get{ return _printSystemInfo;}
            set { _printReport = value; }
        }

        public bool PrintReport
        {
            get { return _printReport; }
            set { _printReport = value; }
        }


        public ReportCommand(IZvtTransport transport)
            : base(transport)
        {
            _apdu = new ReportApdu();
        }


        public override CommandResult Execute()
        {
            CommandResult result = new CommandResult();
            result.Success = true;

            try
            {
                _transport.OpenConnection();

                if (_printSystemInfo)
                {
                    ApduCollection apdus = _commandTransmitter.TransmitAPDU(_systemInfo);
                    CheckForAbortApdu(result, apdus);
                }

                if (_printReport && result.Success)
                {
                    ApduCollection apdus = _commandTransmitter.TransmitAPDU(_apdu);
                    CheckForAbortApdu(result, apdus);
                }
            }
            finally
            {
                _transport.CloseConnection();
            }

            return result;
        }
        

    }
}
