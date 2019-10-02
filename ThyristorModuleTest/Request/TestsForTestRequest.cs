using NUnit.Framework;
using TiristorModule.Request;

namespace ThyristorModuleTest
{
    class TestsForTestRequest
    {
        private static byte[] times = new byte[] { 0, 5, 7, 9, 11, 13, 15, 17, 19 };
        private static byte[] capasities = new byte[] { 40, 30, 40, 50, 60, 70, 80, 90, 100 };

        private static byte[] testRequest = new byte[] { 0x67, 0x91, 7, 15, 54 / 10, 10, 0x01, 0x2C, 0x01, 0x2C };

        TestRequest TestRequest = new TestRequest( 0x67, 0x91, 7, 15, 54/10, 10, 300, 300 );

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CheckCorrectnessOfStartRequest()
        {
            Assert.AreEqual(testRequest, TestRequest.GetRequestPackage());
        }
    }
}
