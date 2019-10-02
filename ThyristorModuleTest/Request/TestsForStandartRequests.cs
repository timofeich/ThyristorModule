using NUnit.Framework;
using TiristorModule.Request;

namespace ThyristorModuleTest
{
    class TestsForStandartRequests
    {
        private static byte[] times = new byte[] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] capasities = new byte[] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };

        private static byte[] correctCurrentValueRequest = new byte[] { 0x67, 0x90, 0x00, 0x6E };
        private static byte[] correctAlarmStopRequest = new byte[] { 0x67, 0x99, 0x00, 0xAD };
        private static byte[] correctStopRequest = new byte[] { 0x67, 0x88, 0x00, 0x37 };

        private StandartRequest CurrentValueRequest = new StandartRequest( 0x67, 0x90, 0x00 );
        private StandartRequest AlarmStopRequest = new StandartRequest( 0x67, 0x99, 0x00 );
        private StandartRequest StopRequest = new StandartRequest( 0x67, 0x88, 0x00 );

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CheckCorrectnessOfCurrentValueRequest()
        {
            Assert.AreEqual(correctCurrentValueRequest, CurrentValueRequest.GetRequestPackage());
        }

        [Test]
        public void CheckCorrectnessOfAlarmStopRequest()
        {
            Assert.AreEqual(correctAlarmStopRequest, AlarmStopRequest.GetRequestPackage());
        }

        [Test]
        public void CheckCorrectnessOfStopRequest()
        {
            Assert.AreEqual(correctStopRequest, StopRequest.GetRequestPackage());
        }
    }
}
