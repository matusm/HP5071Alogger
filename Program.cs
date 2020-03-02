using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using HP5071Alogger.Properties;

namespace HP5071Alogger
{
    class Program
    {
        // global fields
        static string fileName;
        static SerialPort serialPort;
        static LogEntry logEntry;
        static Settings settings;

        //*****************************************************************************
        static void Main(string[] args)
        {
            // mimic an Arduino sketch
            Setup();
            while (true)
            {
                Loop();
            }
        }
        //*****************************************************************************

        //*****************************************************************************
        private static void Setup()
        {
            settings = new Settings();
            fileName = Path.Combine(settings.LogFilePath, settings.LogFileName);
            OpenComPort(settings.ComPort1);
            logEntry = new LogEntry(PullDiagonsticString());
            WriteDataToLogFile(fileName, logEntry.ToCsvHeader());
        }
        //*****************************************************************************

        //*****************************************************************************
        private static void Loop()
        {
            DateTime timeStamp = DateTime.UtcNow;
            if (TimeForLogging(timeStamp))
            {
                logEntry = new LogEntry(PullDiagonsticString());
                WriteDataToLogFile(fileName, logEntry.ToCsvString());
                Thread.Sleep(1000 * settings.LogIntervallTolerance);
            }
        }
        //*****************************************************************************

        private static bool TimeForLogging(DateTime timeStamp)
        {
            if (timeStamp.Second <= (settings.LogIntervallTolerance - 1))
            {
                if (timeStamp.Minute % settings.LogIntervall == 0)
                {
                    return true;
                }
            }
            return false;
        }

        static void WriteDataToLogFile(string fileName, string logLine)
        {
            try
            {
                StreamWriter writer = new StreamWriter(fileName, true);
                writer.WriteLine(logLine);
                writer.Close();
                Console.WriteLine($"File {fileName} updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"File {fileName} not updated.");
            }
        }

        static string PullDiagonsticString()
        {
            RequestStatusReport();
            string sysString = ReadStatusReport();
            return sysString;
        }

        static void RequestStatusReport()
        {
            serialPort.WriteLine("SYSTEM:PRINT?");
        }

        static string ReadStatusReport()
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

        static void OpenComPort(string comPort)
        {
            try
            {
                serialPort = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
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
