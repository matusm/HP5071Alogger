//******************************************************************************
//
// SingleParameter
// Class to store a parameter of type double and its meta data
// 
// Author: Michael Matus, 2020
//
//
// Usage:
// 1.) instantiate class with two string parameters
//     (the keyname and unit, respectively)
// 2.) set the value of the parameter 
//
// Method: bool NameIsIn(string)
// tests if a string contains the object`s key name
//
//
// Example:
//  var internalTemperature = new SingleParameter("Thermometer", "oC");
//  internalTemperature.Value = 31.45;
// 
//******************************************************************************

using System;

namespace HP5071Alogger
{
    public class SingleParameter
    {

        public string KeyName { get; private set; }
        public string Unit { get; private set; }
        public double Value { get; set; }
        public string Title => $"{KeyName} / {Unit}";

        public SingleParameter(string name, string unit)
        {
            KeyName = name.Trim();
            Unit = unit.Trim();
            Value = double.NaN;
        }

        public bool NameIsIn(string str)
        {
            return str.Contains(KeyName); // case sensitive!
        }

        public override string ToString()
        {
            return $"[{GetType().Name}: Name={KeyName} Value={Value} Unit={Unit}]";
        }

    }
}
