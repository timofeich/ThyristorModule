using System.Windows.Controls;

namespace TiristorModule.Validation
{
    public class TimeRule : ValidationRule
    {
        public int Min { get; set; } = 1;
        public int Max { get; set; } = int.MaxValue;

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
                  "Интервал м-ду запросами не входит в диапазон " + Min + " до " + Max + ".");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
