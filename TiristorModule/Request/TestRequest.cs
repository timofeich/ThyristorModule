using System;
using System.Collections.Generic;

namespace TiristorModule.Request
{
    public class TestRequest : BaseRequest
    {
        private byte PersentOfTestPower;
        private byte NominalVoltage;

        private ushort CurrentKz1;
        private ushort CurrentKz2;

        private byte TestsQuantity;

        private byte CRC8
        {
            get { return CalculateCRC8(GetRequestWithoutCRC8()); }
        }

        public TestRequest(byte AddressSlave, byte Command, byte TotalBytes, byte PersentOfTestPower, byte NominalVoltage,
                            byte TestsQuantity, ushort CurrentKz1, ushort CurrentKz2)
        {
            this.AddressSlave = AddressSlave;
            this.Command = Command;
            this.TotalBytes = TotalBytes;

            this.PersentOfTestPower = PersentOfTestPower;
            this.NominalVoltage = NominalVoltage;
            this.TestsQuantity = TestsQuantity;

            this.CurrentKz1 = CurrentKz1;
            this.CurrentKz2 = CurrentKz2;
        }

        public byte[] GetRequestPackage()
        {
            List<byte> RequestWithoutCRC8 = GetRequestWithoutCRC8();
            RequestWithoutCRC8.Add(CRC8);
            return RequestWithoutCRC8.ToArray();
        }

        private List<byte> GetRequestWithoutCRC8()
        {
            List<byte> Request = new List<byte>() { AddressSlave, Command, TotalBytes };

            Request.Add(PersentOfTestPower);
            Request.Add(NominalVoltage);

            Request.Add(TestsQuantity);

            Request.Add(Convert.ToByte(CurrentKz1 >> 8));
            Request.Add(Convert.ToByte(CurrentKz1 ^ 0x100));

            Request.Add(Convert.ToByte(CurrentKz2 >> 8));
            Request.Add(Convert.ToByte(CurrentKz2 ^ 0x100));

            return Request;

        }
    }
}
