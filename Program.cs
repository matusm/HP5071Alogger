using System;
using System.IO;

namespace HP5071Alogger
{
    class Program
    {
        static void Main(string[] args)
        {
            var logEntry = new LogEntry(PullDiagonsticString(@"diag1.txt"));
            Console.WriteLine(logEntry.ToCsvHeader());
            Console.WriteLine(logEntry.ToCsvString());

            logEntry = new LogEntry(PullDiagonsticString(@"diag2.txt"));
            Console.WriteLine(logEntry.ToCsvString());

            logEntry = new LogEntry(PullDiagonsticString(@"diag3.txt"));
            Console.WriteLine(logEntry.ToCsvString());

        }

        static string PullDiagonsticString(string fileName)
        {
            string sysString = File.ReadAllText(fileName);
            return sysString;
        }
    }
}
