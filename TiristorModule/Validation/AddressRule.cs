using System;
using System.Windows.Controls;

namespace TiristorModule.Validation
{
    public class AddressRule : ValidationRule
    {
        public byte Min { get; set; } = 0;
        public byte Max { get; set; } = byte.MaxValue;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
             int requestInterval = 0;

            try
            {
               requestInterval = byte.Parse((string)value, System.Globalization.NumberStyles.HexNumber);
            }

            catch
            {
                return new ValidationResult(false, "Недопустимые символы.");
            }

            if ((requestInterval < Min) || (requestInterval > Max))
            {
                return new ValidationResult(false,
                  "Значение адреса не входит в диапазон " + Min + " до " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
