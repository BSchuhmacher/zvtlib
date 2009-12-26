﻿using System;
using System.Collections.Generic;
using System.Text;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;
using Wiffzack.Devices.CardTerminals.Commands;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands
{
    public class AuthorizationCommand : IPaymentCommand
    {
        public event IntermediateStatusDelegate Status;

        /// <summary>
        /// Transportlayer to use
        /// </summary>
        private IZvtTransport _transport;

        /// <summary>
        /// Authorization APDU
        /// </summary>
        private AuthorizationApdu _apdu;

        private ICommandTransmitter _commandTransmitter;

        public AuthorizationCommand(IZvtTransport transport)
        {
            _transport = transport;
            _apdu = new AuthorizationApdu();
            _commandTransmitter = new MagicResponseCommandTransmitter(_transport);
            _commandTransmitter.ResponseReceived += new Action<IZvtApdu>(_commandTransmitter_ResponseReceived);
        }

        private void _commandTransmitter_ResponseReceived(IZvtApdu responseApdu)
        {

        }

     
        public PaymentResult Execute(Int64 centAmount)
        {
            _apdu.SetCentAmount(centAmount);
            _transport.OpenConnection();
            ApduCollection responses = _commandTransmitter.TransmitAPDU(_apdu);
            _transport.CloseConnection();

            //Contains the result (success or failure) and much information about the transaction
            StatusInformationApdu statusInformation = responses.FindFirstApduOfType<StatusInformationApdu>();

            //Completion is only sent if everything worked fine
            CompletionApduResponse completion = responses.FindFirstApduOfType<CompletionApduResponse>();

            //Abort is only sent if something went wrong
            AbortApduResponse abort = responses.FindFirstApduOfType<AbortApduResponse>();


            bool success = true;
            int? errorCode = null;
            string errorDescription = "";

            if (completion == null && abort != null)
            {
                success = false;
                errorCode = (byte)abort.ResultCode;
                errorDescription = abort.ResultCode.ToString();
            }
            else if (statusInformation != null)
            {
                StatusInformationResultCode result = statusInformation.FindParameter<StatusInformationResultCode>(StatusInformationApdu.StatusParameterEnum.ResultCode);

                if (result.ResultCode != StatusCodes.ErrorIDEnum.NoError)
                {
                    success = false;
                    errorCode = (byte)result.ResultCode;
                    errorDescription = result.ResultCode.ToString();
                }
            }

            PaymentResult authResult = new PaymentResult(success, errorCode, errorDescription, statusInformation);

            return authResult;

        }        
    }
}
