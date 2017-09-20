﻿using System.Runtime.Serialization;

namespace _11thLauncher.Models
{
    public class TextParameter : LaunchParameter
    {
        private string _value;

        public TextParameter()
        {
            Type = ParameterType.Text;
        }

        #region Properties

        [DataMember(Order = 4)]
        public string Value
        {
            get => _value ?? string.Empty;
            set
            {
                _value = value;
                NotifyOfPropertyChange();
            }
        }

        public override string LaunchString => Value.Trim();

        #endregion

        #region Methods

        public void SetStatus(bool enabled, string value)
        {
            base.SetStatus(enabled);
            _value = value;
        }

        public override void CopyStatus(LaunchParameter parameter)
        {
            base.CopyStatus(parameter);
            _value = ((TextParameter)parameter).Value;
        }

        #endregion
    }
}
