using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TiristorModule.Indicators
{
    public class IndicatorColor
    {
        private static Dictionary<int, Brush> TestingStatus = new Dictionary<int, Brush>(2);
        private static Dictionary<int, Brush> StartStatus = new Dictionary<int, Brush>(2);
        private static Dictionary<int, Brush> StopStatus = new Dictionary<int, Brush>(2);

        public static void InitializeTestingStatusData()
        {
            SetColorToTheIndicator(TestingStatus);
        }

        public static void InitializeStartData()
        {
            SetColorToTheIndicator(StartStatus);
        }

        public static void InitializeStopData()
        {
            SetColorToTheIndicator(StopStatus);
        }

        public static Brush GetStatusFromTestTiristor(ushort statusTest)
        {
            return TestingStatus[statusTest];
        }

        public static Brush GetStartStatus(ushort statusTest)
        {
            return TestingStatus[statusTest];
        }

        public static void SetColorToTheIndicator(Dictionary<int, Brush> dictionary)
        {
            dictionary.Add(0, (Brush)new BrushConverter().ConvertFromString("Green"));
            dictionary.Add(1, (Brush)new BrushConverter().ConvertFromString("Red"));
        }
    }
}
