using System;
using System.Text;

namespace RobotKarel.Jadro.Packet
{
    public static class Helper
    {
        public static string ByteArrayToHexaString(byte[] array)
        {
            return BitConverter.ToString(array).Replace("-", "");
        }

        public static byte[] HexaStringToByteArray(string text)
        {
            return new ASCIIEncoding().GetBytes(text);
        }

        public static int ByteArrayToInt(byte[] array)
        {
            var help = new byte[4];
            Array.Reverse(array);
            Array.Copy(array, 0, help, 0, array.Length);
            return BitConverter.ToInt32(help, 0);
        }
        
        public static byte[] IntToByteArray(int value)
        {
            var help = BitConverter.GetBytes(value);
            Array.Reverse(help);
            var result = new byte[2];
            Array.Copy(help, 2, result, 0, 2);
            return result;
        }

        public static byte[] SubArray(byte[] data, int index, int length = 0)
        {       
            int trueLength = length;
            if (trueLength == 0)
                trueLength = data.Length - index;

            byte[] result = new byte[trueLength];
            Array.Copy(data, index, result, 0, trueLength);
            return result;
        }
    }
}
