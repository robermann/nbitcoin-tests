using NBitcoin.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoinFirstProject
{
    class Hasher
    {
        public static void Run()
        {
            
            byte[] bytes = File.ReadAllBytes(@"C:\Users\roberto.mannai\Downloads\bitcoin_2NA5L1cYrMRQEUYisWCK7bjfM1UFdxumYps.pdf"); ;
            
            byte[] hash1 = hash256(bytes);
            Console.WriteLine(toString(hash1));      //e0d67bebf79a1bcc179c6fe687003fbb56bd08dbd9c7c59bebf9560a8691af48

            byte[] hash2 = hash256Ripemd160(bytes);
            Console.WriteLine(toString(hash2));      //4565629a158493bb5e9f9319491e3724b5be3445

            Console.ReadLine();
        }

        public static byte[] hash256(byte[] rawData)
        {
            byte[] hash = Hashes.SHA256(rawData);
            return hash;
        }

        public static byte[] hash256Ripemd160(byte[] rawData)
        {
            byte[] sha256 = hash256(rawData);
            byte[] hash = Hashes.RIPEMD160(sha256, sha256.Length); 
            return hash;
        }

        private static string toString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
