using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PeterO.Numbers;
using System.Security.Cryptography;


namespace crypt5
{
    class RSA
    {
        public BigInteger pubKey = 0;
        public BigInteger privKey = 0;       

        private BigInteger q, p, n, phi = 0;
        private const int tokenSize = 8;
        //private EContext ec;
        public RSA()
        {
            GenPrime gp = new GenPrime();
            //q = gp.Generate(1024);
            //p = gp.Generate(1024);
            q = 17; p = 19;
            n = q * p;
            phi = (q - 1) * (p - 1);
            //RSACryptoServiceProvider rp = new RSACryptoServiceProvider();
            BigInteger e = 2;
            while (e < phi)
            {
                if (BigInteger.GreatestCommonDivisor(e, phi) == 1)
                    break;
                else
                    e++;
            }

            pubKey = e;
            privKey = FindD(e, phi);
            
        }

        private EInteger GreatCommDiv(EInteger a, EInteger b)
        {
            EInteger t;
            while (true)
            {
                t = a % b;
                if (t == EInteger.Zero)
                    return b;
                a = b;
                b = t;
            }
        }

        private BigInteger FindD(BigInteger e, BigInteger phi)
        {
            /*            BigInteger i = 1;
                        while (e * i < phi) i++;
                        BigInteger step = i;
                        BigInteger de = e * i;
                        while (de % phi != 1)
                        {
                            Debug.WriteLine(i.ToString());
                            i = i+step-e;
                            de = e * i;
                            if (de % phi == 1) break;
                            else i++;
                        }
                        return de/e;*/
            BigInteger d = 1;
            while (e * d % phi != 1) d++;
            return d;
        }

        // ---------- ENCRYPTION ---------- //
        public string Encrypt(string message, BigInteger publicKey)
        {
            string encryptedText = "";
            byte[] msgbytes = Encoding.UTF8.GetBytes(message);
            BigInteger numToken = 0; BigInteger value = 0;
            for (int i = 0; i < msgbytes.Length; i++)
            {
                numToken = msgbytes[i];
                value = BigInteger.ModPow(numToken, publicKey, n);
                encryptedText += value + " ";
            }

            return encryptedText;
        }

        private string getToken(string text, int index, int length = tokenSize) 
        {
            string token = "";
            for (int i = 0; i < length; i++)
            {
                if (text.Length > index + i)
                    token += text[index + i];
                else
                    break;
            }
            return token;
        }
        // -------------------- //

        // ---------- DECRYPTION ---------- //
        public string Decrypt(string message, BigInteger privateKey)
        {
            string decryptedText = "";
            string[] tokens = message.Split(' ');
            List<byte> bytes = new List<byte> { };
            BigInteger value = 0;
            for (int i = 0; i < tokens.Length-1; i++)
            {
                value = BigInteger.ModPow(BigInteger.Parse(tokens[i]), privateKey, n);
                bytes.Add(Convert.ToByte(value.ToString()));
            }

            decryptedText = Encoding.UTF8.GetString(bytes.ToArray());
            return decryptedText;
        }
        // -------------------- //

/*        private EInteger ModPow(EDecimal value, EDecimal exponent, EInteger modulus)
        {
            EInteger result = (value.Pow(exponent, ec)).ToEInteger().Mod(modulus);
            EDecimal res1 = (value.Pow(exponent, ec));
            EInteger res2 = res1.ToEInteger().Mod(modulus);
            return result;
        }*/
    }
}
