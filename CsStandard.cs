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

namespace HP5071Alogger
{
    public class CsStandard
    {
        private SerialPort serialPort;

        public string Name { get; private set; }
        public string ComPort { get; private set; }
        public string LogFileName { get; private set; }

        public CsStandard(string name, string comPort, string path, string baseName, string extension)
        {
            ComPort = comPort.Trim();
            Name = name.Trim();
            LogFileName = SetFullyQualifiedFileName(path, baseName, extension);
            serialPort = new SerialPort();
        }

        public void WriteDataToLogFile()
        {
            string diagnosticString = PullDiagonsticString();
            var logEntry = new LogEntry(diagnosticString);
            WriteDataToLogFile(logEntry.ToCsvString());
        }

        public void WriteHeaderToLogFile()
        {
            string dummy = "   ";
            var logEntry = new LogEntry(dummy);
            WriteDataToLogFile(logEntry.ToCsvHeader());
        }

        public override string ToString()
        {
            return $"[CsStandard Name:{Name} ComPort:{ComPort} Path:{LogFileName}]";
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
                Console.WriteLine("*****DEBUG 1");
                RequestStatusReport();
                return ReadStatusReport();
            }
            return "";
        }

        private void RequestStatusReport()
        {
            serialPort.DiscardOutBuffer();
            serialPort.DiscardInBuffer();
            serialPort.WriteLine("SYSTEM:PRINT?");
        }

        private string ReadStatusReport()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    sb.AppendLine(message);
                    Console.WriteLine($"*****DEBUG 2 <{message}> ");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            return sb.ToString();
        }

        private void OpenComPort()
        {
            try
            {
                serialPort = new SerialPort(ComPort, 9600, Parity.None, 8, StopBits.One);
                //serialPort.Handshake = Handshake.XOnXOff;
                serialPort.Handshake = Handshake.None;
                serialPort.RtsEnable = true;
                //serialPort.DiscardInBuffer();
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
                int dummy = serialPort.BaudRate;
                serialPort.Open();
                Console.WriteLine($"****DEBUG serialPort.IsOpen: {serialPort.IsOpen}");
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message);
                Console.WriteLine($"Cant open port {ComPort}, keep trying.");
            }
        }

        private string SetFullyQualifiedFileName(string path, string baseName, string extension)
        {
            return Path.ChangeExtension(Path.Combine(path, baseName), extension);
        }





    }
}
