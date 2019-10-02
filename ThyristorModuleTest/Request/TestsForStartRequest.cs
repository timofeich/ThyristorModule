using NUnit.Framework;
using TiristorModule.Request;

namespace Tests
{
    public class TestForStartRequest
    {
        private static byte[] times = new byte[] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] capasities = new byte[] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };

        private static byte[] correctStartRequest = new byte[] { 0x67, 0x87, 0x1C, 0x00, 0x05, 0x07, 0x09, 0x0B, 0x0D,
            0x0F, 0x11, 0x13, 0x28, 0x1E, 0x28, 0x32, 0x3C, 0x46, 0x50, 0x5A, 0x64, 0x01, 0x2C, 0x0A, 0x00, 0x01,
            0x2C, 0x0A, 0x00, 0x55, 0x00, 0x0E };

        StartRequest StartRequest = new StartRequest(0x67, 0x87, 28, times, capasities, 300, 10, 0,
            300, 10, 0, 85, 0);

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CheckCorrectnessOfStartRequest()
        {
            Assert.AreEqual(correctStartRequest, StartRequest.GetRequestPackage());
        }
    }
}