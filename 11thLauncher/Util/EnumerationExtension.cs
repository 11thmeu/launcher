﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace _11thLauncher.Util
{
    /// <summary>
    /// Extension for handling enums in views.
    /// </summary>
    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumerationExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException(nameof(enumType));

            EnumType = enumType;
        }

        public Type EnumType
        {
            get => _enumType;
            private set
            {
                if (_enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember(enumValue, GetDescription(enumValue))).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = EnumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;


            return descriptionAttribute != null
              ? Resources.Strings.ResourceManager.GetString(descriptionAttribute.Description)
              : enumValue.ToString();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; }

            public EnumerationMember(object value, string description)
            {
                Value = value;
                Description = description;
            }

            public override bool Equals(object obj)
            {
                var item = obj as EnumerationMember;

                return item != null && Value.Equals(item.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }
    }
}
