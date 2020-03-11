//******************************************************************************
//
// CsStandard
// Class to handle the file logging of a single 5071A Cs-standard
// 
// Author: Michael Matus, 2020
//
//
// Usage:
// 1.) instantiate class with five string parameters
//     (name, com port, directory, filename, and file name extension)
// 2.) call WriteDataToLogFile() at any time
//
// 
//******************************************************************************

using System;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace HP5071Alogger
{
    public class CsStandard
    {
        private const int DELAY = 100; // a delay in ms
        private SerialPort serialPort;

        #region Properties
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string ComPort { get; private set; }
        public string LogFileName { get; private set; }
        #endregion

        #region Ctor
        public CsStandard(string name, string comPort, string path, string baseName, string extension)
        {
            ComPort = comPort.Trim();
            Name = name.Trim();
            LogFileName = SetFullyQualifiedFileName(path, baseName, extension);
            serialPort = new SerialPort();
            IdentifyInstrument();
        }
        #endregion

        #region Methods
        public void WriteDataToLogFile()
        {
            string diagnosticString = PullDiagonsticString();
            var logEntry = new LogEntry(diagnosticString);
            Console.WriteLine($"Status summary for {Name}: {logEntry.StatusSummary}");
            WriteDataToLogFile(logEntry.ToCsvString());
        }

        public void WriteHeaderToLogFile()
        {
            string dummy = "   ";
            var logEntry = new LogEntry(dummy);
            if(!File.Exists(LogFileName))
                WriteDataToLogFile(logEntry.ToCsvHeader());
        }
        #endregion

        private void IdentifyInstrument()
        {
            ID = "*";
            if (!serialPort.IsOpen)
            {
                OpenComPort();
            }
            if (serialPort.IsOpen)
            {
                string command = "*IDN?";
                SendCommand(command);
                ID = ReceiveString();
                ID = ID.Replace('\n', ' ');
                ID = ID.Replace('\r', ' ');
                ID = ID.Replace(command, " ");
                ID = ID.Trim();
            }
        }

        private void WriteDataToLogFile(string csvLine)
        {
            try
            {
                StreamWriter writer = new StreamWriter(LogFileName, true);
                writer.WriteLine(csvLine);
                writer.Close();
                Console.WriteLine($"File {LogFileName} (for {Name}) updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"File {LogFileName} not updated.");
            }
        }

        private string PullDiagonsticString()
        {
            if (!serialPort.IsOpen)
            {
                OpenComPort();
            }
            if (serialPort.IsOpen)
            {
                SendCommand("SYSTEM:PRINT?");
                return ReceiveString();
            }
            return "";
        }

        private void SendCommand(string command)
        {
            serialPort.DiscardOutBuffer();
            serialPort.DiscardInBuffer();
            serialPort.WriteLine(command); 
            Thread.Sleep(DELAY);
        }

        private string ReceiveString()
        {
            StringBuilder sb = new StringBuilder();
            int numberOfTimeouts = 0;
            while (true)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    sb.AppendLine(message);
                }
                catch (TimeoutException)
                {
                    numberOfTimeouts++;
                    if (numberOfTimeouts > 4) break;
                }
            }
            return sb.ToString();
        }

        private void OpenComPort()
        {
            try
            {
                serialPort = new SerialPort(ComPort, 9600, Parity.None, 8, StopBits.One);
                serialPort.Handshake = Handshake.None;
                serialPort.ReadTimeout = 100;
                serialPort.WriteTimeout = 100;
                serialPort.RtsEnable = false;
                serialPort.DtrEnable = false;
                serialPort.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cant open port {ComPort}, keep trying.");
                Console.WriteLine($"Message received: {e.Message}");
            }
        }

        private string SetFullyQualifiedFileName(string path, string baseName, string extension)
        {
            return Path.ChangeExtension(Path.Combine(path, baseName), extension);
        }

        public override string ToString()
        {
            return $"[CsStandard - ID:{ID} Name:{Name} ComPort:{ComPort} Path:{LogFileName}]";
        }


    }
}
