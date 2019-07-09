﻿using CryptHash.Net.Encryption.AES.AE;
using System;
using System.Text;

namespace crypthash
{
    class Program
    {
        private static AE_AES_256_CBC_HMAC_SHA_256 _aes = null;

        static void Main(string[] args)
        {
            _aes = new AE_AES_256_CBC_HMAC_SHA_256();
            _aes.OnEncryptionProgress += (percentage, msg) => { Console.WriteLine(msg); };
            _aes.OnEncryptionMessage += (msg) => { Console.WriteLine(msg); };

            TestStringEncryption();
            TestStringDecryption();
            TestFileEncryption();
            TestFileDecryption();

            Console.WriteLine("\nPress any key to finish...");
            Console.ReadKey();
        }

        private static void TestStringEncryption()
        {
            Console.WriteLine("\nPress any key to start string encryption test...");
            Console.ReadKey(true);

            Console.WriteLine("\nEnter the source string: ");
            var sourceString = Console.ReadLine();

            Console.WriteLine("Enter encryption password: ");
            var password = Console.ReadLine();

            var result = _aes.EncryptString(sourceString, password);

            if (result.Success)
                Console.WriteLine($"Encrypted string: {result.EncryptedDataBase64String}");
            else
                Console.WriteLine(result.Message);

            Console.WriteLine("\nString encryption test done.");
        }

        private static void TestStringDecryption()
        {
            Console.WriteLine("\nPress any key to start string decryption test...");
            Console.ReadKey(true);

            Console.WriteLine("\nEnter the encrypted base64 string: ");
            var encryptedString = Console.ReadLine();

            Console.WriteLine("Enter decryption password: ");
            var password = Console.ReadLine();

            var result = _aes.DecryptString(encryptedString, password);

            if (result.Success)
                Console.WriteLine($"Decrypted string: {result.DecryptedDataString}");
            else
                Console.WriteLine(result.Message);

            Console.WriteLine("\nString decryption test done.");
        }

        private static void TestFileEncryption()
        {
            Console.WriteLine("\nPress any key to start file encryption test...");
            Console.ReadKey(true);

            Console.WriteLine("\nEnter the source file path: ");
            var sourceFile = Console.ReadLine();

            Console.WriteLine("Enter encryption password: ");
            var password = Console.ReadLine();

            var result = _aes.EncryptFile(sourceFile, sourceFile, password);

            Console.WriteLine(result.Message);

            Console.WriteLine("\nFile encryption test done.");
        }

        private static void TestFileDecryption()
        {
            Console.WriteLine("\nPress any key to start file decryption test...");
            Console.ReadKey(true);

            Console.WriteLine("\nEnter the encrypted file path: ");
            var encryptedFile = Console.ReadLine();

            Console.WriteLine("Enter decryption password: ");
            var password = Console.ReadLine();

            var result = _aes.DecryptFile(encryptedFile, encryptedFile, password);

            Console.WriteLine(result.Message);

            Console.WriteLine("\nFile decryption test done.");
        }
    }
}
