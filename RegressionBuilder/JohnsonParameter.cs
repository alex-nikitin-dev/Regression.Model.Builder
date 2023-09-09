using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;

namespace RegressionBuilder
{
    public class JohnsonParameter
    {
        public decimal Max { get; private set; }
        public decimal Min { get; private set; }
        private decimal _value;
        public decimal Precision;
        public bool NeedVerifying { get; set; }

        public decimal Value
        {
            set
            {
                if (IsValueOutOfRage(value))
                    ThrowValueIncorrectException("value", value);
                
                _value = value;
            }
            get => _value;
        }

        private decimal _initialValue;
        public decimal InitialValue
        {
            private set
            {
                if (IsValueOutOfRage(value))
                    ThrowValueIncorrectException("initial value", value);
                
                _initialValue = value;
            }
            get => _initialValue;
        }

        public decimal StepsCount => Precision <= 0
            ? throw new ArgumentException("Cannot calculate steps count: precision is <= zero")
            : (Max - Min) / Precision;

        public JohnsonParameterNames Name { get; }

        public JohnsonParameter(JohnsonParameterNames name, decimal min, decimal max, decimal initialValue, decimal precision, bool needVerifying = true)
        {
            Name = name;
            NeedVerifying = needVerifying;
            Set(min, max, initialValue, initialValue, precision);
        }
        public JohnsonParameter(JohnsonParameterNames name, decimal min, decimal max, decimal initialValue, decimal val, decimal precision, bool needVerifying = true)
        {
            Name = name;
            NeedVerifying = needVerifying;
            Set(min, max, initialValue, val, precision);
        }

        public void Set(decimal min, decimal max, decimal initialValue, decimal value, decimal precision)
        {
            Min = min;
            Max = max;
            InitialValue = initialValue;
            Value = value;
            Precision = precision;
        }

        public enum PropertyName
        {
            [Display(Name = "Max")]
            Max,
            [Display(Name = "Min")]
            Min,
            [Display(Name = "Value")]
            Value,
            [Display(Name = "Initial Value")]
            InitialValue,
            [Display(Name = "Precision")]
            Precision,
            [Display(Name= "Steps count")]
            StepsCount
        }

        public List<(PropertyName name, decimal val)> GetProperties()
        {
            return new List<(PropertyName name, decimal val)>()
            {
                (PropertyName.Max, Max),
                (PropertyName.Min, Min),
                (PropertyName.Value, Value),
                (PropertyName.InitialValue, InitialValue),
                (PropertyName.Precision, Precision),
                (PropertyName.StepsCount,StepsCount)
            };
        }

        private void ThrowValueIncorrectException(string name, decimal val)
        {
            if (NeedVerifying)
                throw new ArgumentException(
                    $"the {name} ({val}) of johnson parameter {Name} is out of range {Min} - {Max}");
        }

        private bool IsValueOutOfRage(decimal val)
        {
            return val < Min || val > Max;
        }

        public JohnsonParameter Clone()
        {
            return new JohnsonParameter(Name, Min, Max, InitialValue, Value, Precision);
        }
    }
}
