namespace TiristorModule.Indicators
{
    public class IndicatorColor
    {
        public static bool? GetTestingStatusLEDColor(ushort statusByte)
        {
            bool? testingStatus = true;
            if (statusByte == 0) return testingStatus;
            else return !testingStatus;
        }
    }
}
