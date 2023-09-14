﻿using Wiffzack.Devices.CardTerminals.PrintSupport;
using System.Xml;
using Wiffzack.Services.Utils;

namespace Wiffzack.Devices.CardTerminals.Commands
{
    public class CommandResult
    {
        /// <summary>
        /// Indicates if the Operation was successful
        /// </summary>
        protected bool _success;

        /// <summary>
        /// Error code for error tracement, in terms of the used provider
        /// </summary>
        protected int? _protocolSpecificErrorCode;


        /// <summary>
        /// Error description for simpler error tracement
        /// </summary>
        protected string _protocolSpecificErrorDescription;

        /// <summary>
        /// Contains all print documents that where generated by the command
        /// </summary>
        protected IPrintDocument[] _printDocuments;

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public int? ProtocolSpecificErrorCode
        {
            get { return _protocolSpecificErrorCode; }
            set { _protocolSpecificErrorCode = value; }
        }

        public string ProtocolSpecificErrorDescription
        {
            get { return _protocolSpecificErrorDescription; }
            set { _protocolSpecificErrorDescription = StringHelper.addSpaces(value); }
        }

        public IPrintDocument[] PrintDocuments
        {
            get { return _printDocuments; }
            set { _printDocuments = value; }
        }

        public CommandResult()
        {
        }

        public CommandResult(bool success, int? protocolSpecificErrorCode, string protocolSpecificErrorMessage)
        {
            _success = success;
            _protocolSpecificErrorCode = protocolSpecificErrorCode;
            _protocolSpecificErrorDescription = StringHelper.addSpaces(protocolSpecificErrorMessage);
        }

        public virtual void SerializeToXml(XmlElement rootNode)
        {
            XmlHelper.WriteBool(rootNode, "Success", Success);
			if(_protocolSpecificErrorCode!=null)
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", _protocolSpecificErrorCode);
			if(_protocolSpecificErrorDescription!=null && !_protocolSpecificErrorDescription.Equals(""))
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", _protocolSpecificErrorDescription);

            if (_printDocuments != null)
            {
                foreach (IPrintDocument document in _printDocuments)
                {
                    if(document!=null){
						XmlElement documentRoot = (XmlElement)rootNode.AppendChild(rootNode.OwnerDocument.CreateElement("Document"));
	                    document.SerializeToXml(documentRoot);
					}
                }
            }

        }
    }
}
