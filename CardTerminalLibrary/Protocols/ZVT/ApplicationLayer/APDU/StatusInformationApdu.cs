﻿using System;
using System.Collections.Generic;
using System.Text;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;
using Wiffzack.Devices.CardTerminals.Commands;
using System.Xml;
using Wiffzack.Services.Utils;
using Wiffzack.Devices.CardTerminals.Common;

namespace Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.APDU
{
    /// <summary>
    /// Wrapper around the ApduResponse to easily parse status informations
    /// </summary>
    /// <remarks>
    /// The special thing about status informations
    /// </remarks>
    public class StatusInformationApdu : ApduResponse, IData
    {
        public static StatusInformationApdu CreateFromIData(XmlElement rootNode)
        {
            StatusInformationApdu status = new StatusInformationApdu();
            status.ReadXml(rootNode);
            return status;
        }

        public enum StatusParameterEnum : byte
        {
            ResultCode = 0x27,
            Amount = 0x04,
            TraceNr = 0x0B,
            OriginalTraceNr = 0x37,
            Time = 0x0C,
            Date = 0x0D,
            ExpiryDate = 0x0E,
            SequenceNumber = 0x17,
            PaymentType = 0x19,
            PanEfId = 0x22,
            TerminalId = 0x29,
            AuthorisationAttribute = 0x3B,
            CurrencyCode = 0x49,
            BlockedGoodsGroups = 0x4C,
            ReceiptNr = 0x87,
            CardType = 0x8A,
            CardTypeID = 0x8C,
            AdditionalCardDataForECCash = 0x92,
            GeldkartePaymentData = 0x9A,
            AIDParameter = 0xBA,
            EFInfo = 0xAF,
            ContractNumberForCC = 0x2A,
            AdditionalTextForCC = 0x3C,
            ResultCodeBinary = 0xA0,
            TurnoverNr = 0x88,
            CardTypeName = 0x8B,
			DiagnosisRequired = 0xC2,
			CommunicationFault=0x9B,
			SystemError=0xFF,
			NoConnection=0xA3,
			EndOfDayBatchNotPossible=0x77,
			CardNotReadable=0x64,
			CardDataNotPresent=0x65,
			ProcessingError=0x66,
			AbortViaTimeOutOrAbortKey=0x6C,
			CreditNotSufficient=0x71,
			CardExpired=0x78,
			CardNotYetValid=0x79,
			UnknownCard=0x7A,
			CommunicationError=0x7D,
			AllreadyReversed=0xB4,
			ReversalNotPossible=0xB5,
			OpenEndOfDayBatchPresent=0xF0,
			VoltageSupplyToLow=0xBF,
			InitialisationRequired=0x55,
			FunctionNotPossible=0x83

        }


        /// <summary>
        /// Saves the parsed paramters from the Apdu
        /// </summary>
        private Dictionary<StatusParameterEnum, IParameter> _parameters = new Dictionary<StatusParameterEnum, IParameter>();

        private StatusInformationApdu():base(null)
        {
        }

        public StatusInformationApdu(byte[] rawApduData)
            : base(rawApduData)
        {
            LoadParameters();
        }

        private void LoadParameters()
        {
            _parameters.Clear();

            LoadParameterHelper.LoadParameters(CreateParameterForBMP,
                (LoadParameterHelper.AddToParameterListDelegate)delegate(byte bmp, IParameter param)
                {
                    if (_parameters.ContainsKey((StatusParameterEnum)bmp))
                        _parameters[(StatusParameterEnum)bmp] = param;
                    else
                        _parameters.Add((StatusParameterEnum)bmp, param);
                },
                _rawApduData, 3);
        }

        /// <summary>
        /// Returns the specific parameter or null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        public T FindParameter<T>(StatusParameterEnum parameterType) where T : class, IParameter
        {
            if (_parameters.ContainsKey(parameterType))
            {
                if (_parameters[parameterType] is PrefixedParameter<T>)
                    return ((PrefixedParameter<T>)_parameters[parameterType]).SubParameter;
                else
                    return (T)_parameters[parameterType];
            }
            else
                return null;
        }

        /// <summary>
        /// Creates an empty Parameter Object wich is then filled by ParseFromBytes
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        protected virtual IParameter CreateParameterForBMP(byte bmp)
        {
            switch ((StatusParameterEnum)bmp)
            {
                //Result Code
                case StatusParameterEnum.ResultCode:
                    return new PrefixedParameter<StatusInformationResultCode>(bmp, new StatusInformationResultCode());

                //Amount 6 Bytes
                case StatusParameterEnum.Amount:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

                //Trace Nr: 3 Bytes
                case StatusParameterEnum.TraceNr:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0, 0, 0));

                //Original Trace Nr (Only for reversal): 3 Bytes
                case StatusParameterEnum.OriginalTraceNr:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0, 0, 0));

                //time: 3bytes
                case StatusParameterEnum.Time:
                    return new PrefixedParameter<StatusTimeParameter>(bmp, new StatusTimeParameter(0, 0, 0));

                //date: 2bytes
                case StatusParameterEnum.Date:
                    return new PrefixedParameter<StatusDateParameter>(bmp, new StatusDateParameter(0, 0));

                //expiry date: 2bytes
                case StatusParameterEnum.ExpiryDate:
                    return new PrefixedParameter<StatusExpDateParameter>(bmp, new StatusExpDateParameter(0, 0));

                //Sequence Number: 2Bytes
                case StatusParameterEnum.SequenceNumber:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0));

                //Payment Type: 1Byte
                case StatusParameterEnum.PaymentType:
                    return new PrefixedParameter<StatusPaymentTypeParam>(bmp, new StatusPaymentTypeParam());

                case StatusParameterEnum.PanEfId:
                    return new PrefixedParameter<StatusPanEfId>(bmp, new StatusPanEfId());

                //Terminal ID: 4Bytes
                case StatusParameterEnum.TerminalId:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0, 0, 0, 0, 0));

                //Authorisation-Attribute: 8Bytes
                case StatusParameterEnum.AuthorisationAttribute:
                    return new PrefixedParameter<FixedSizeParam>(bmp, new FixedSizeParam(8));

                //CurrencyCode: 2Bytes
                case StatusParameterEnum.CurrencyCode:
                    return new PrefixedParameter<CurrencyCodeParameter>(bmp, new CurrencyCodeParameter());

                //Blocked-goods-groups: LLVar Encoded
                case StatusParameterEnum.BlockedGoodsGroups:
                    return new PrefixedParameter<LVarBCDNumberParameter>(bmp, new LVarBCDNumberParameter(2));

                //Receipt-nr.: 2 bytes
                case StatusParameterEnum.ReceiptNr:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0));

                //Card-Type: 1 Byte
                case StatusParameterEnum.CardType:
                    return new PrefixedParameter<SingleByteParameter>(bmp, new SingleByteParameter());

                //Card-Type-ID: 1 Byte
                case StatusParameterEnum.CardTypeID:
                    return new PrefixedParameter<SingleByteParameter>(bmp, new SingleByteParameter());

                //Additional card Data for EC-Cash: LLLVar
                case StatusParameterEnum.AdditionalCardDataForECCash:
                    return new PrefixedParameter<LVarParameter>(bmp, new LVarParameter(3));

                //Geldkarte payment data: LLLVar
                case StatusParameterEnum.GeldkartePaymentData:
                    return new PrefixedParameter<LVarParameter>(bmp, new LVarParameter(3));

                //AID-Parameter: 5bytes
                case StatusParameterEnum.AIDParameter:
                    return new PrefixedParameter<FixedSizeParam>(bmp, new FixedSizeParam(5));

                //EF-Info: LLLvar
                case StatusParameterEnum.EFInfo:
                    return new PrefixedParameter<LVarParameter>(bmp, new LVarParameter(3));

                //Contract Number for credit Cards: 15bytes
                case StatusParameterEnum.ContractNumberForCC:
                    return new PrefixedParameter<AsciiFixedSizeParameter>(bmp, new AsciiFixedSizeParameter(15));

                //Additional text for credit cards: LLLVar
                case StatusParameterEnum.AdditionalTextForCC:
                    return new PrefixedParameter<AsciiLVarParameter>(bmp, new AsciiLVarParameter(3));

                //result code which cannot be encoded in BCD: 1byte
                case StatusParameterEnum.ResultCodeBinary:
                    return new PrefixedParameter<SingleByteParameter>(bmp, new SingleByteParameter());

                //turnover Nr: 3byte
                case StatusParameterEnum.TurnoverNr:
                    return new PrefixedParameter<BCDNumberParameter>(bmp, new BCDNumberParameter(0, 0, 0, 0, 0, 0));

                //Name of the card type: LLVar
                case StatusParameterEnum.CardTypeName:
                    return new PrefixedParameter<AsciiLVarParameter>(bmp, new AsciiLVarParameter(2));
                
                default:
                    return null;

            }
        }


        #region IData Members

        public void WriteXml(XmlElement rootNode)
        {
            XmlHelper.WriteString(rootNode, "RawData", ByteHelpers.ByteToString(_rawApduData));
			BCDNumberParameter receiptNr=this.FindParameter<BCDNumberParameter>(StatusInformationApdu.StatusParameterEnum.ReceiptNr);
			if(receiptNr!=null)
				XmlHelper.WriteInt64(rootNode,"ReceiptNr",receiptNr.DecodeNumber());
			PrefixedParameter<AsciiLVarParameter> additional=this.FindParameter<PrefixedParameter<AsciiLVarParameter>>(StatusInformationApdu.StatusParameterEnum.AdditionalCardDataForECCash);
			if(additional!=null)
				XmlHelper.WriteString(rootNode,"Additional",additional.SubParameter.Text);
			PrefixedParameter<StatusPanEfId> panefid=this.FindParameter<PrefixedParameter<StatusPanEfId>>(StatusInformationApdu.StatusParameterEnum.PanEfId);
			if(panefid!=null)
				XmlHelper.WriteInt64(rootNode,"PanEfid",panefid.SubParameter.DecodeNumber());
			PrefixedParameter<BCDNumberParameter> amount=this.FindParameter<PrefixedParameter<BCDNumberParameter>>(StatusInformationApdu.StatusParameterEnum.Amount);
			if(amount!=null)
				XmlHelper.WriteInt64(rootNode,"Amount",amount.SubParameter.DecodeNumber());
			PrefixedParameter<AsciiLVarParameter> additionalInfo=this.FindParameter<PrefixedParameter<AsciiLVarParameter>>(StatusInformationApdu.StatusParameterEnum.AdditionalTextForCC);
		
			if(additionalInfo!=null){
				XmlHelper.WriteString(rootNode,"AdditionalInfo",additionalInfo.SubParameter.Text);
				Console.WriteLine(additionalInfo.SubParameter.Data.Length +"<-- length");
				Console.WriteLine("First: "+additionalInfo.SubParameter.Data[0]);
				Console.WriteLine("Last: "+additionalInfo.SubParameter.Data[additionalInfo.SubParameter.Data.Length-1]);
					
			}
		}
			

        public void ReadXml(XmlElement rootNode)
        {
            _rawApduData = ByteHelpers.ByteStringToByte(XmlHelper.ReadString(rootNode, "RawData"));
            LoadParameters();
        }


        public XmlElement ToXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("Data"));
            WriteXml(doc.DocumentElement);
            return doc.DocumentElement;
        }
        #endregion
    }
}
