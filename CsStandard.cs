using System;
using System.Text;
using System.IO.Ports;
using System.IO;

namespace HP5071Alogger
{
    public class CsStandard
    {
        private static SerialPort serialPort;

        public string Name { get; private set; }
        public string ComPort { get; private set; }
        public string LogFileName { get; private set; }

        public CsStandard(string name, string comPort)
        {
            ComPort = comPort.Trim();
            Name = name.Trim();
            serialPort = new SerialPort();
        }

        public void SetFullyQualifiedFileName(string path, string baseName, string extension)
        {
            LogFileName = Path.ChangeExtension(Path.Combine(path, baseName), extension);
        }

        public void WriteDataToLogFile()
        {
            string diagnosticString = PullDiagonsticString();
            var logEntry = new LogEntry(diagnosticString);
            WriteDataToLogFile(logEntry.ToCsvString());
            Console.WriteLine($"Standard {Name} logged.");
        }

        private void WriteDataToLogFile(string csvLine)
        {
            string diagnosticString = PullDiagonsticString();
            var logEntry = new LogEntry(diagnosticString);
            try
            {
                StreamWriter writer = new StreamWriter(LogFileName, true);
                writer.WriteLine(csvLine);
                writer.Close();
                Console.WriteLine($"File {LogFileName} updated.");
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
            RequestStatusReport();
            return ReadStatusReport();
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
                Console.WriteLine("Exit program!");
                Environment.Exit(1);
            }
        }







    }
}
