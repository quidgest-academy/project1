using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace CSGenio.framework
{
    /// <summary>
    /// Classe com funções utilitárias relativas à criptografia
    /// </summary>
    /// <remarks>
    /// @DDT 22/10/2009
    /// </remarks>
    public class CryptographicFunctionsWeb
    {
        /// <summary>
        /// Valida o Qcertificate em vários aspectos: Verifica a existência, a validade e a integridade da informação
        /// </summary>
        /// <param name="httpCertificate"></param>
        /// <returns></returns>
        public static bool validateCertificate(HttpClientCertificate httpCertificate)
        {
            bool isCertSelfValidated = false;
            bool isCertHashValid = false;

            isCertSelfValidated = validaPresenca(httpCertificate);

            if (!isCertSelfValidated)
            {
                return false;
            }
            isCertHashValid = verificaIntegridade(httpCertificate);
            return isCertHashValid;
        }

        /// <summary>
        /// Devolve uma hash SHA256 da key publica do Qcertificate
        /// </summary>
        /// <param name="httpCertificate"></param>
        /// <returns>Hash </returns>
        public static string returnHashPublicKey(HttpClientCertificate httpCertificate)
        {
            byte[] rawcert = httpCertificate.Certificate;
            X509Certificate x509Cert = new X509Certificate(rawcert);
            SHA256 sha = new SHA256Managed();
            byte[] hashvalue = sha.ComputeHash(x509Cert.GetPublicKey());
            return Convert.ToBase64String(hashvalue);
        }

        /// <summary>
        /// Devolve uma hash SHA256 da key publica do Qcertificate
        /// </summary>
        /// <param name="httpCertificate"></param>
        /// <returns></returns>
        public static string GetPublicKeyHash(X509Certificate certificate)
        {
            byte[] rawcert = certificate.GetRawCertData();
            X509Certificate x509Cert = new X509Certificate(rawcert);
            SHA256 sha = new SHA256Managed();
            byte[] hashvalue = sha.ComputeHash(x509Cert.GetPublicKey());
            return Convert.ToBase64String(hashvalue);
        }

        /// <summary>
        /// Verifica a integridade do Qcertificate
        /// </summary>
        /// <param name="httpCertificate"></param>
        /// <returns></returns>
        private static bool verificaIntegridade(HttpClientCertificate httpCertificate)
        {
            byte[] rawcert = httpCertificate.Certificate;
            X509Certificate x509Cert = new X509Certificate(rawcert);
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hashvalue = sha.ComputeHash(rawcert);
            byte[] x509Hash = x509Cert.GetCertHash();
            if (compararArrayBytes(x509Hash, hashvalue))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifica a presenca e a validade do Qcertificate
        /// </summary>
        /// <param name="httpCertificate"></param>
        /// <returns></returns>
        private static bool validaPresenca(HttpClientCertificate httpCertificate)
        {
            return httpCertificate.IsPresent && httpCertificate.IsValid;
        }

        /// <summary>
        /// Compara um array de bytes
        /// </summary>
        /// <param name="firstArray">First array de bytes</param>
        /// <param name="secondArray">Second array de bytes</param>
        /// <returns>TRUE se os arrays forem iguais, caso contrário FALSE</returns>
        private static bool compararArrayBytes(byte[] firstArray, byte[] secondArray)
        {
            if (!(firstArray.Length == secondArray.Length))
            {
                return false;
            }
            Int32 secondByteArrayIndexer = 0;
            foreach (byte leftByte in firstArray)
            {
                if (!(leftByte == secondArray[secondByteArrayIndexer]))
                {
                    return false;
                }
                secondByteArrayIndexer += 1;
            }
            return true;
        }
    }
}
