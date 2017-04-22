﻿namespace _11thLauncher.Model.Parameter
{
    public class ParameterValueItem
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }

        public ParameterValueItem(string value, string displayValue)
        {
            Value = value;
            DisplayName = displayValue;
        }
    }
}
