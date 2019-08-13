using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TiristorModule.Validation
{
    class NumberOfTestsRule : ValidationRule
    {
        public byte Min { get; set; } = 1;
        public byte Max { get; set; } = byte.MaxValue;

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo ci)
        {
            int requestInterval = 0;

            try
            {
                requestInterval = int.Parse((string)value);
            }

            catch
            {
                return new ValidationResult(false, "Недопустимые символы.");
            }

            if ((requestInterval < Min) || (requestInterval > Max))
            {
                return new ValidationResult(false,
                  "Количество тестов не входит в диапазон " + Min + " до " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
