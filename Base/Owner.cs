using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PaymentV.Base
{
    public static class Owner
    {
        static string keyfile = "keyfile.tmp";

        public static string GenerateLink()
        {
            string key = GenerateKey(30);

            File.WriteAllText(keyfile, key);

            return $"https://t.me/NI_PaymentV_Bot?start=key-{key}";
        }

        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


        public static bool IsValidKey(string key)
        {
            if(key.Contains(File.ReadAllText(keyfile))) return true;
            return false;
        }

        public static string GenerateKey(int length)
        {
            var stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(chars[random.Next(chars.Length)]);
            }
            return stringBuilder.ToString();
        }



        public static string GetKey()
        {
            if(!File.Exists(keyfile)) return null;

            return System.IO.File.ReadAllText(keyfile);
        }
    }
}
