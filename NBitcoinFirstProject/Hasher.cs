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
            
            byte[] bytes = File.ReadAllBytes(@"C:\Users\roberto.mannai\Downloads\shotcut-win64-180811.exe"); ;
            
            byte[] hash1 = hash256(bytes);
            Console.WriteLine(toString(hash1));      //265ff794aedcce3e35e23d8c868975d53b92dcd8d55f811c5c54c4bff81a15cb

            byte[] hash2 = hash256Ripemd160(bytes);
            Console.WriteLine(toString(hash2));      //fdd9427038374267ca2cc750b81620755b301b7c

            Console.ReadLine();
        }

        static byte[] hash256(byte[] rawData)
        {
            byte[] hash = Hashes.SHA256(rawData);
            return hash;
        }

        static byte[] hash256Ripemd160(byte[] rawData)
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
