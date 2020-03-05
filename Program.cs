using System;
using System.Threading;
using HP5071Alogger.Properties;

namespace HP5071Alogger
{
    class Program
    {
        // global fields
        static Settings settings;
        static CsStandard[] csStandards = new CsStandard[5];
        static int numberOfCsStandards = 1;

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
            numberOfCsStandards = settings.NumberOfStandards;
            if (numberOfCsStandards < 1) numberOfCsStandards = 1;
            if (numberOfCsStandards > 5) numberOfCsStandards = 5;

            LogEntry.Delimiter = settings.CsvDelimiter;

            csStandards[0] = new CsStandard(
                settings.Name1, 
                settings.ComPort1,
                settings.LogFilePath, 
                settings.LogFileBaseName1, 
                settings.LogFileExtension);

            csStandards[1] = new CsStandard(
                settings.Name2,
                settings.ComPort2,
                settings.LogFilePath,
                settings.LogFileBaseName2,
                settings.LogFileExtension);

            csStandards[2] = new CsStandard(
                settings.Name3,
                settings.ComPort3,
                settings.LogFilePath,
                settings.LogFileBaseName3,
                settings.LogFileExtension);

            csStandards[3] = new CsStandard(
                settings.Name4,
                settings.ComPort4,
                settings.LogFilePath,
                settings.LogFileBaseName4,
                settings.LogFileExtension);

            csStandards[4] = new CsStandard(
                settings.Name5,
                settings.ComPort5,
                settings.LogFilePath,
                settings.LogFileBaseName5,
                settings.LogFileExtension);

            // some diagnostic output
            for (int i = 0; i < numberOfCsStandards; i++)
            {
                Console.WriteLine(csStandards[i]);
            }
            Console.WriteLine();

            // write header in for logfiles
            // TODO call only for new files!
            for (int i = 0; i < numberOfCsStandards; i++)
            {
                csStandards[i].WriteHeaderToLogFile();
            }

        }
        //*****************************************************************************

        //*****************************************************************************
        private static void Loop()
        {
            if (ItsTimeForLogging())
            {
                for (int i = 0; i < numberOfCsStandards; i++)
                {
                    csStandards[i].WriteDataToLogFile();
                }
                Thread.Sleep(1000 * settings.LogIntervallTolerance);
            }
            Thread.Sleep(250);
        }
        //*****************************************************************************

        private static bool ItsTimeForLogging()
        {
            DateTime timeStamp = DateTime.UtcNow;
            if (timeStamp.Second <= (settings.LogIntervallTolerance - 1))
            {
                if (timeStamp.Minute % settings.LogIntervall == 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
