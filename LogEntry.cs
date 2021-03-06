﻿//******************************************************************************
//
// LogEntry
// Class to store all parameters of diagnostic data of a Cs frequency standard
// of type 5071A (HP, Agilent, Symmetricon, Microsemi)
// 
// Author: Michael Matus, 2020
//
//
// Usage:
// 1.) instantiate class with a single string
//     (the diagnostic string pulled from the instrument)
// 2.) consume the required parameters or a formatted string for logging 
//
// Methods:
// -- void ToCsvString()
//    generates a comma separated string of all parameters for logging to a file
// -- void ToCsvHeader()
//    generates a comma separated string for all parameter titles
//
// All the hard work of parsing is done in the constructor.
// The properties (getters only) can be used separately.
// However the main usage is to generate a formatted string for logging
//
// Example:
//  var entry = new LogEntry(diagString);
//  Console.WriteLine(entry.ToCsvString());
// 
//******************************************************************************

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HP5071Alogger
{
    public class LogEntry
    {
        private const string defaultDelimiter = " ; ";

        #region Properties
        // a static property for all instances of this class
        public static string Delimiter { get; set; } = defaultDelimiter;
        // standard parameters of type double
        public SingleParameter FrequencyOffset { get; private set; }
        public SingleParameter OscillatorControl { get; private set; }
        public SingleParameter RfAmplitude1 { get; private set; }
        public SingleParameter RfAmplitude2 { get; private set; }
        public SingleParameter ZeemanFrequency { get; private set; }
        public SingleParameter CFieldCurrent { get; private set; }
        public SingleParameter EMultiplier { get; private set; }
        public SingleParameter SignalGain { get; private set; }
        public SingleParameter CbtOven { get; private set; }
        public SingleParameter CbtOfenError { get; private set; }
        public SingleParameter OscillatorOven { get; private set; }
        public SingleParameter IonPump { get; private set; }
        public SingleParameter HwIonizer { get; private set; }
        public SingleParameter MassSpec { get; private set; }
        public SingleParameter SawTuning { get; private set; }
        public SingleParameter DroTuning { get; private set; }
        public SingleParameter Mhz87Pll { get; private set; }
        public SingleParameter UpClockPll { get; private set; }
        public SingleParameter P12VSupply { get; private set; }
        public SingleParameter M12VSupply { get; private set; }
        public SingleParameter P5VSupply { get; private set; }
        public SingleParameter Thermometer { get; private set; }
        // properties of various type
        public DateTime EntryDate { get; private set; }
        public string StatusSummary { get; private set; }
        public string PowerSource { get; private set; }
        public string LogStatus { get; private set; }
        public string CbtId { get; private set; }
        public int InternalMjd { get; private set; }
        public string InternalTime { get; private set; }
        #endregion

        #region Ctor
        public LogEntry(string systPrint)
        {
            ResetAllProperties();
            ParseDiagnosticString(systPrint);
        }
        #endregion

        #region Methods

        public string ToCsvString()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            StringBuilder sb = new StringBuilder();
            sb.Append($"{EntryDate.ToString("yyyy-MM-dd HH:mm:ss+000")}{Delimiter}");
            sb.Append($"{InternalMjd}{Delimiter}");
            sb.Append($"{InternalTime}{Delimiter}");
            sb.Append($"{CbtId}{Delimiter}");
            sb.Append($"{StatusSummary}{Delimiter}");
            sb.Append($"{PowerSource}{Delimiter}");
            sb.Append($"{LogStatus}{Delimiter}");
            sb.Append($"{FrequencyOffset.Value}{Delimiter}");
            sb.Append($"{OscillatorControl.Value:F2}{Delimiter}");
            sb.Append($"{RfAmplitude1.Value:F1}{Delimiter}");
            sb.Append($"{RfAmplitude2.Value:F1}{Delimiter}");
            sb.Append($"{ZeemanFrequency.Value:F0}{Delimiter}");
            sb.Append($"{CFieldCurrent.Value:F3}{Delimiter}");
            sb.Append($"{EMultiplier.Value:F0}{Delimiter}");
            sb.Append($"{SignalGain.Value:F1}{Delimiter}");
            sb.Append($"{CbtOven.Value:F1}{Delimiter}");
            sb.Append($"{CbtOfenError.Value:F2}{Delimiter}");
            sb.Append($"{OscillatorOven.Value:F1}{Delimiter}");
            sb.Append($"{IonPump.Value:F1}{Delimiter}");
            sb.Append($"{HwIonizer.Value:F1}{Delimiter}");
            sb.Append($"{MassSpec.Value:F1}{Delimiter}");
            sb.Append($"{SawTuning.Value:F1}{Delimiter}");
            sb.Append($"{DroTuning.Value:F1}{Delimiter}");
            sb.Append($"{Mhz87Pll.Value:F1}{Delimiter}");
            sb.Append($"{UpClockPll.Value:F1}{Delimiter}");
            sb.Append($"{P12VSupply.Value:F1}{Delimiter}");
            sb.Append($"{M12VSupply.Value:F1}{Delimiter}");
            sb.Append($"{P5VSupply.Value:F1}{Delimiter}");
            sb.Append($"{Thermometer.Value:F1}"); // last entry without delimiter
            return sb.ToString();
        }

        public string ToCsvHeader()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Time stamp{Delimiter}");
            sb.Append($"MJD (internal){Delimiter}");
            sb.Append($"Date (internal){Delimiter}");
            sb.Append($"CBT ID{Delimiter}");
            sb.Append($"Status summary{Delimiter}");
            sb.Append($"Power source{Delimiter}");
            sb.Append($"Log status{Delimiter}");
            sb.Append($"{FrequencyOffset.Title}{Delimiter}");
            sb.Append($"{OscillatorControl.Title}{Delimiter}");
            sb.Append($"{RfAmplitude1.Title}{Delimiter}");
            sb.Append($"{RfAmplitude2.Title}{Delimiter}");
            sb.Append($"{ZeemanFrequency.Title}{Delimiter}");
            sb.Append($"{CFieldCurrent.Title}{Delimiter}");
            sb.Append($"{EMultiplier.Title}{Delimiter}");
            sb.Append($"{SignalGain.Title}{Delimiter}");
            sb.Append($"{CbtOven.Title}{Delimiter}");
            sb.Append($"{CbtOfenError.Title}{Delimiter}");
            sb.Append($"{OscillatorOven.Title}{Delimiter}");
            sb.Append($"{IonPump.Title}{Delimiter}");
            sb.Append($"{HwIonizer.Title}{Delimiter}");
            sb.Append($"{MassSpec.Title}{Delimiter}");
            sb.Append($"{SawTuning.Title}{Delimiter}");
            sb.Append($"{DroTuning.Title}{Delimiter}");
            sb.Append($"{Mhz87Pll.Title}{Delimiter}");
            sb.Append($"{UpClockPll.Title}{Delimiter}");
            sb.Append($"{P12VSupply.Title}{Delimiter}");
            sb.Append($"{M12VSupply.Title}{Delimiter}");
            sb.Append($"{P5VSupply.Title}{Delimiter}");
            sb.Append($"{Thermometer.Title}"); // last entry without delimiter
            return sb.ToString();
        }

        #endregion

        #region Private stuff
        private void ResetAllProperties()
        {
            FrequencyOffset = new SingleParameter("Freq Offset", "");
            OscillatorControl = new SingleParameter("Osc. control", "%");
            RfAmplitude1 = new SingleParameter("RF amplitude 1", "%");
            RfAmplitude2 = new SingleParameter("RF amplitude 2", "%");
            ZeemanFrequency = new SingleParameter("Zeeman Freq", "Hz");
            CFieldCurrent = new SingleParameter("C-field curr", "mA");
            EMultiplier = new SingleParameter("E-multiplier", "V");
            SignalGain = new SingleParameter("Signal Gain", "%");
            CbtOven = new SingleParameter("CBT Oven", "V");
            CbtOfenError = new SingleParameter("CBT Oven Err", "oC");
            OscillatorOven = new SingleParameter("Osc. Oven", "V");
            IonPump = new SingleParameter("Ion Pump", "uA");
            HwIonizer = new SingleParameter("HW Ionizer", "V");
            MassSpec = new SingleParameter("Mass spec", "V");
            SawTuning = new SingleParameter("SAW Tuning", "V");
            DroTuning = new SingleParameter("DRO Tuning", "V");
            Mhz87Pll = new SingleParameter("87MHz PLL", "V");
            UpClockPll = new SingleParameter("uP Clock PLL", "V");
            P12VSupply = new SingleParameter("+12V supply", "V");
            M12VSupply = new SingleParameter("-12V supply", "V");
            P5VSupply = new SingleParameter("+5V supply", "V");
            Thermometer = new SingleParameter("Thermometer", "oC");
            EntryDate = DateTime.UtcNow;
            StatusSummary = "*";
            PowerSource = "*";
            LogStatus = "*";
            CbtId = "*";
            InternalMjd = -1;
            InternalTime = "*";
        }

        // from here on massive string banging happens
        private void ParseDiagnosticString(string systPrint)
        {
            // take the time of this method call as the entry date
            EntryDate = DateTime.UtcNow;
            // replace multiple spaces with a single space
            systPrint = Regex.Replace(systPrint, @" {2,}", " ");
            // split to separate lines
            string[] textLines = systPrint.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.RemoveEmptyEntries);
            // Grab all parameters line by line
            foreach (var textLine in textLines)
            {
                ParseSection1(textLine);
                ParseSection2(textLine);
            }
        }

        private void ParseSection1(string textLine)
        {
            if (textLine.Contains("MJD"))
            {
                string[] tokens = textLine.Split(
                    new[] { " " },
                    StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 3) return; // tokens[0]=="MJD"
                InternalMjd = int.Parse(tokens[1]);
                InternalTime = tokens[2].Trim();
            }
        }

        private void ParseSection2(string textLine)
        {
            string[] tokens = textLine.Split(
                new[] { ":" },
                StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2) return;
            if (tokens.Length == 2)
            {
                switch (tokens[0])
                {
                    case "CBT ID":
                        CbtId = tokens[1].Trim();
                        return;
                    case "Status summary":
                        StatusSummary = tokens[1].Trim();
                        return;
                    case "Power source":
                        PowerSource = tokens[1].Trim();
                        return;
                    case "Log status":
                        LogStatus = tokens[1].Trim();
                        return;
                    default:
                        return;
                }
            }
            if (tokens.Length == 3)
            {
                ParseSection3(tokens);
            }
        }

        private void ParseSection3(string[] tokens)
        {
            // the left hand side
            SetValueForKey(FrequencyOffset, tokens[0], tokens[1]);
            SetValueForKey(RfAmplitude1, tokens[0], tokens[1]);
            SetValueForKey(ZeemanFrequency, tokens[0], tokens[1]);
            SetValueForKey(EMultiplier, tokens[0], tokens[1]);
            SetValueForKey(CbtOven, tokens[0], tokens[1]);
            SetValueForKey(OscillatorOven, tokens[0], tokens[1]);
            SetValueForKey(HwIonizer, tokens[0], tokens[1]);
            SetValueForKey(SawTuning, tokens[0], tokens[1]);
            SetValueForKey(Mhz87Pll, tokens[0], tokens[1]);
            SetValueForKey(P12VSupply, tokens[0], tokens[1]);
            SetValueForKey(P5VSupply, tokens[0], tokens[1]);
            // the right hand side
            SetValueForKey(OscillatorControl, tokens[1], tokens[2]);
            SetValueForKey(RfAmplitude2, tokens[1], tokens[2]);
            SetValueForKey(CFieldCurrent, tokens[1], tokens[2]);
            SetValueForKey(SignalGain, tokens[1], tokens[2]);
            SetValueForKey(CbtOfenError, tokens[1], tokens[2]);
            SetValueForKey(IonPump, tokens[1], tokens[2]);
            SetValueForKey(MassSpec, tokens[1], tokens[2]);
            SetValueForKey(DroTuning, tokens[1], tokens[2]);
            SetValueForKey(UpClockPll, tokens[1], tokens[2]);
            SetValueForKey(M12VSupply, tokens[1], tokens[2]);
            SetValueForKey(Thermometer, tokens[1], tokens[2]);
            //etc
        }

        private void SetValueForKey(SingleParameter sp, string str1, string str2)
        {
            if (sp.NameIsIn(str1))
            {
                sp.Value = GrabFirstNumber(str2);
            }
        }

        private double GrabFirstNumber(string str)
        {
            string[] tokens = str.Split(
                    new[] { " " },
                    StringSplitOptions.RemoveEmptyEntries);
            double parsedNumber;
            foreach (var s in tokens)
            {
                if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedNumber))
                    return parsedNumber;
            }
            return double.NaN;
        }

        #endregion
    }
}
