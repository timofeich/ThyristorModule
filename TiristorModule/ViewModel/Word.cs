using System;
using System.Linq;

namespace TiristorModule
{
    public class Word
    {
        public static UInt16 FromByteArray(byte[] bytes)
        {
            // bytes[0] -> HighByte
            // bytes[1] -> LowByte
            return FromBytes(bytes[1], bytes[0]);
        }

        public static ushort FromBytes(byte LoVal, byte HiVal)
        {
            return (ushort)(HiVal * 256 + LoVal);
        }

        public static UInt16[] ByteToUInt16(byte[] bytes)
        {
            UInt16[] values = new UInt16[bytes.Length / 2];
            int counter = 0;
            for (int cnt = 0; cnt < bytes.Length / 2; cnt++)
                values[cnt] = FromByteArray(new byte[] { bytes[counter++], bytes[counter++] });
            return values;
        }

        public static byte[] ToByteArray(UInt16 value)
        {
            byte[] array = BitConverter.GetBytes(value);
            Array.Reverse(array);
            return array;
        }

        static int[] GetArray(int[] arr)
        {
            return arr.Where((el, ind) => (el % 2 == 0 && ind % 2 != 0)).ToArray();
        }
    }
}
