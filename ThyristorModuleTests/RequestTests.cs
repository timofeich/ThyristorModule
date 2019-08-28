using NUnit.Framework;
using System.Collections.Generic;
using TiristorModule.Request;
using TiristorModule.Response;

namespace Tests
{
    public class Tests
    {

        CurrentVoltageResponse CurrentVoltageResponse = new CurrentVoltageResponse();
        private byte[] StartRequest = { 0x67, 0x90, 0x00, 0x9C };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SendCorrectStartRequest()
        {
            Assert.AreEqual(StartRequest, CurrentVoltageResponse.);
        }
    }
}