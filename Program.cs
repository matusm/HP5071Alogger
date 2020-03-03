using System;
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
            //*****
            csStandards[0] = new CsStandard(settings.Name1, settings.ComPort1);
            csStandards[0].SetFullyQualifiedFileName(settings.LogFilePath, settings.LogFileBaseName1, settings.LogFileExtension);
            //*****
            csStandards[1] = new CsStandard(settings.Name2, settings.ComPort2);
            csStandards[1].SetFullyQualifiedFileName(settings.LogFilePath, settings.LogFileBaseName2, settings.LogFileExtension);
            //*****
            csStandards[2] = new CsStandard(settings.Name3, settings.ComPort3);
            csStandards[2].SetFullyQualifiedFileName(settings.LogFilePath, settings.LogFileBaseName3, settings.LogFileExtension);
            //*****
            csStandards[3] = new CsStandard(settings.Name4, settings.ComPort4);
            csStandards[3].SetFullyQualifiedFileName(settings.LogFilePath, settings.LogFileBaseName4, settings.LogFileExtension);
            //*****
            csStandards[4] = new CsStandard(settings.Name5, settings.ComPort5);
            csStandards[4].SetFullyQualifiedFileName(settings.LogFilePath, settings.LogFileBaseName5, settings.LogFileExtension);
            //*****
        }
        //*****************************************************************************

        //*****************************************************************************
        private static void Loop()
        {
            DateTime timeStamp = DateTime.UtcNow;
            if (TimeForLogging(timeStamp))
            {
                for (int i = 0; i < numberOfCsStandards; i++)
                {
                    csStandards[i].WriteDataToLogFile();
                }
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

    }
}
