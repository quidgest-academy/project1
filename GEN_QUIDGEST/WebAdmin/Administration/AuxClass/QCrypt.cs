using System;

namespace Administration.AuxClass
{
    class QCrypt
    {
        // enche uma array de chars com 30 bytes aleatórios
        // copia até 20 chars da string to a array, a partir da posição 4
        // aplica um ou-exclusivo com o byte 0 a partir do byte 1
        // codifica com base64
        // retorna o Qresult
        public static string Encrypt(string sStr)
        {
            int strLength = sStr.Length;
            int strWithSaltLength = strLength + 10;
            //int blockSize = 128;
            //int blocks = Convert.ToInt32(Math.Ceiling(((double)strWithSaltLength) / (double)blockSize));

            int x = 0;
            Random RandomNumber = new Random();
            byte[] buf = new byte[strWithSaltLength];
            for (x = 0; x < buf.Length; x++)
                buf[x] = (byte)RandomNumber.Next(256);

            for (x = 0; x < sStr.Length; x++)
                buf[x + 4] = (byte)sStr[x];

            buf[x + 4] = (byte)0;

            for (x = 1; x < buf.Length - 1; x++)
                buf[x] = (byte)(buf[x] ^ buf[0]);

            return Convert.ToBase64String(buf, 0, strWithSaltLength);
        }

        public static string Decrypt(string sStr)
        {
            try
            {
                byte[] buf = Convert.FromBase64String(sStr);
                for (int x = 1; x < buf.Length; x++)
                    buf[x] = (byte)((int)buf[x] ^ (int)buf[0]);

                string str_output = "";

                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                str_output = enc.GetString(buf);
                str_output = str_output.Substring(4, str_output.IndexOf("\0") - 4);
                return str_output;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}