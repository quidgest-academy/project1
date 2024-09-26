using System.IO;
using System.Security.Cryptography;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe com funções utilitárias relativas à criptografia
    /// </summary>
    public class CryptographicFunctions
    {
        /// <summary>
        /// Decifra um byte array cifrado com AES e devolve o conteudo original
        /// </summary>
        /// <param name="key">Key simétrica</param>
        /// <param name="iv">Vector de inicialização</param>
        /// <param name="data">Dados cifrados</param>
        /// <returns>Dados decifrados</returns>
        public static byte[] DecryptData(byte[] key, byte[] iv, byte[] data)
        {
            MemoryStream streamOut = new MemoryStream();
            using (Aes alg = Aes.Create())
            {
                ICryptoTransform decryptor = alg.CreateDecryptor(key, iv);
                using (MemoryStream streamIn = new MemoryStream(data))
                using (CryptoStream cryptoStream = new CryptoStream(streamIn, decryptor, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(streamOut);
                }
            }
            streamOut.Flush();
            return streamOut.ToArray();
        }

        /// <summary>
        /// Cifra um byte array e devolve o conteudo cifrado
        /// </summary>
        /// <param name="key">Key simétrica</param>
        /// <param name="iv">Vector de inicialização</param>
        /// <param name="data">Dados originais</param>
        /// <returns>Dados cifrados</returns>
        public static byte[] EncryptData(byte[] key, byte[] iv, byte[] data)
        {
            using (Aes alg = Aes.Create())
            {
                ICryptoTransform encryptor = alg.CreateEncryptor(key, iv);
                using (MemoryStream stream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    stream.Flush();
                    byte[] dataCrypto = stream.ToArray();
                    return dataCrypto;
                }
            }
        }
    }
}
