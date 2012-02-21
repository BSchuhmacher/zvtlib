using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.TransportLayer;
using Wiffzack.Diagnostic.Log;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer;
using System.IO.Ports;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Commands;
using Wiffzack.Devices.CardTerminals.Protocols.ZVT.ApplicationLayer.Parameters;
using Wiffzack.Devices.CardTerminals.Commands;
using Wiffzack.Services.Utils;
namespace Wiffzack.Devices.CardTerminals.Tests

{
	/// <summary>
	/// Report starter. 
	/// The report starter awaits a XML file that contains the configuration for transport and report. 
	/// It executes the command and saves the resulting XML file as /tmp/result.xml.
	/// </summary>
	public class PaymentStarter
	{
	
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main(string[] args){
			if(args[0].Equals("?")){
					LogManager.Global = new LogManager(true, new TextLogger(null, LogLevel.Everything, "Wiffzack", Console.Out));
					Logger _log= LogManager.Global.GetLogger("Wiffzack");
					_log.Info("<config.xml>");
					return;
			}
			LogManager.Global = new LogManager(true, new TextLogger(null, LogLevel.Everything, "Wiffzack", Starter.getFileLoggerStream()));
			//create XML file with result message
			XmlDocument resultXML = new XmlDocument();
			XmlElement rootNode=resultXML.CreateElement("Result");
			resultXML.AppendChild(rootNode);
			//check if the first argument is a file
			if(args.Length!=1 || !File.Exists(args[0])){
				LogManager.Global.GetLogger("Wiffzack").Info("Please pass a XML configuration file as first argument!");
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -1);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", "Please pass a XML configuration file as first argument!");
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;
			}
			
			//load the XML file
			XmlDocument config = new XmlDocument();
			try {
				config.Load(args[0]);
 			}
			//if any exception occur, the XML file could not be read and thus the program stops
			catch (Exception e){
				LogManager.Global.GetLogger("Wiffzack").Info("Your XML was probably bad...");
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -2);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", "Your XML was probably bad.");
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;
 			}
			//initialise environment with the configuration file and execute command
			try{
	     		ICommandEnvironment environment = new ZVTCommandEnvironment(config.DocumentElement);
				environment.StatusReceived += new IntermediateStatusDelegate(environment_StatusReceived);
				PaymentResult result = environment.CreatePaymentCommand((XmlElement)config.DocumentElement.SelectSingleNode("Payment")).Execute();
				//create XML file with result message
				result.SerializeToXml(resultXML.DocumentElement);
				result.Data.WriteXml(resultXML.DocumentElement);
				//save file in /tmp/result.xml
				resultXML.Save(Starter.result);
				//debug message --> remove later
				LogManager.Global.GetLogger("Wiffzack").Info("XML file created");
			}catch(System.ArgumentException se){
				LogManager.Global.GetLogger("Wiffzack").Info("Bad Xml Argument");
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -3);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", "Bad Xml Argument");
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;			
			}catch(System.FormatException fe){
				LogManager.Global.GetLogger("Wiffzack").Info("Bad Xml Argument");
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -3);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", fe.Message);
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;	
			}catch(System.Net.Sockets.SocketException ce){
				LogManager.Global.GetLogger("Wiffzack").Info("Connection Error: "+ce.Message);
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -4);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", ce.Message);
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;	
			}catch(Exception e){
				LogManager.Global.GetLogger("Wiffzack").Info("System Error:"+e.Message);
				XmlHelper.WriteBool(rootNode, "Success", false);
            	XmlHelper.WriteInt(rootNode, "ProtocolSpecificErrorCode", -255);
            	XmlHelper.WriteString(rootNode, "ProtocolSpecificErrorDescription", e.Message);
				//save file in /tmp/result.xml
				try{
					resultXML.Save(Starter.result);
				}catch(Exception saving){
					LogManager.Global.GetLogger("Wiffzack").Info("Error Saving Result");
					LogManager.Global.GetLogger("Wiffzack").Info(saving.Message);
				}
				return;
			}
			
		}
		  static void environment_StatusReceived(IntermediateStatus status)
        {
            LogManager.Global.GetLogger("Wiffzack").Info(status.ToString());
        }
	}
}
