﻿//******************************************************************************
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

        private string SetFullyQualifiedFileName(string path, string baseName, string extension)
        {
            return Path.ChangeExtension(Path.Combine(path, baseName), extension);
        }       
        
        private void WriteDataToLogFile(string csvLine)
        {
            try
            {
                StreamWriter writer = new StreamWriter(LogFileName, true);
                writer.WriteLine(csvLine);
                writer.Close();
                Console.WriteLine($"File {LogFileName} updated with data from {Name}.");
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
            if (!serialPort.IsOpen)
            {
                RequestStatusReport();
                return ReadStatusReport();
            }
            return "";
        }

        private void RequestStatusReport()
        {
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
                }
                catch
                {
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
                serialPort.Open();
                serialPort.Handshake = Handshake.XOnXOff;
                serialPort.DiscardInBuffer();
                serialPort.ReadTimeout = 1000;
                serialPort.WriteTimeout = 1000;
            }
            catch (Exception e)
            {
                serialPort = null;
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Keep trying the next time.");
                //Environment.Exit(1);
            }
        }







    }
}
