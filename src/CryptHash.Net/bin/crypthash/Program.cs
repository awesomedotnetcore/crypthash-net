﻿/*
 *      Alessandro Cagliostro, 2019
 *      
 *      https://github.com/alecgn
 */

using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using CryptHash.Net.CLI.CommandLineParser;
using CryptHash.Net.CLI.ConsoleUtil;
using CryptHash.Net.Encryption.AES.AE;
using CryptHash.Net.Encryption.AES.EncryptionResults;
using CryptHash.Net.Hash;
using CryptHash.Net.Hash.HashResults;

namespace CryptHash.Net.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProcessArgs(args);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);

                Environment.Exit((int)ExitCode.Error);
            }
        }

        private static void ProcessArgs(string[] args)
        {

            var parserResult = Parser.Default.ParseArguments<CryptOptions, DecryptOptions, HashOptions>(args);

            var exitCode = parserResult.MapResult(
                (CryptOptions opts) => RunCryptOptionsAndReturnExitCode(opts),
                (DecryptOptions opts) => RunDecryptOptionsAndReturnExitCode(opts),
                (HashOptions opts) => RunHashOptionsAndReturnExitCode(opts),
                errors => HandleParseError(errors, parserResult)
            );

            Environment.Exit((int)exitCode);
        }

        private static ExitCode RunCryptOptionsAndReturnExitCode(CryptOptions cryptOptions)
        {
            AesEncryptionResult aesEncryptionResult = null;

            switch (cryptOptions.InputType.ToLower())
            {
                case "string":
                    {
                        switch (cryptOptions.Algorithm.ToLower())
                        {
                            case "aes256":
                                {
                                    aesEncryptionResult = new AE_AES_256_CBC_HMAC_SHA_256().EncryptString(cryptOptions.InputToBeEncrypted, cryptOptions.Password);
                                }
                                break;
                            default:
                                aesEncryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown algorithm \"{cryptOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                case "file":
                    {
                        switch (cryptOptions.Algorithm.ToLower())
                        {
                            case "aes256":
                                {
                                    using (var progressBar = new ProgressBar())
                                    {
                                        var aes = new AE_AES_256_CBC_HMAC_SHA_256();
                                        aes.OnEncryptionProgress += (percentageDone, message) => { progressBar.Report((double)percentageDone / 100); };
                                        aes.OnEncryptionMessage += (msg) => { /*Console.WriteLine(msg);*/ progressBar.WriteLine(msg); };

                                        aesEncryptionResult = aes.EncryptFile(cryptOptions.InputToBeEncrypted, cryptOptions.OutputFilePath, cryptOptions.Password, cryptOptions.DeleteSourceFile);
                                    }
                                }
                                break;
                            default:
                                aesEncryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown algorithm \"{cryptOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                default:
                    aesEncryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown input type \"{cryptOptions.InputType}\"." };
                    break;
            }

            if (aesEncryptionResult.Success)
            {
                Console.WriteLine((cryptOptions.InputType.ToLower().Equals("string") ? aesEncryptionResult.EncryptedDataBase64String : aesEncryptionResult.Message));

                return ExitCode.Sucess;
            }
            else
            {
                Console.WriteLine(aesEncryptionResult.Message);

                return ExitCode.Error;
            }
        }

        private static ExitCode RunDecryptOptionsAndReturnExitCode(DecryptOptions decryptOptions)
        {
            AesEncryptionResult aesDecryptionResult = null;

            switch (decryptOptions.InputType.ToLower())
            {
                case "string":
                    {
                        switch (decryptOptions.Algorithm.ToLower())
                        {
                            case "aes256":
                                {
                                    aesDecryptionResult = new AE_AES_256_CBC_HMAC_SHA_256().DecryptString(decryptOptions.InputToBeDecrypted, decryptOptions.Password);
                                }
                                break;
                            default:
                                aesDecryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown algorithm \"{decryptOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                case "file":
                    {
                        switch (decryptOptions.Algorithm.ToLower())
                        {
                            case "aes256":
                                {
                                    using (var progressBar = new ProgressBar())
                                    {
                                        var aes = new AE_AES_256_CBC_HMAC_SHA_256();
                                        aes.OnEncryptionProgress += (percentageDone, message) => { progressBar.Report((double)percentageDone / 100); };
                                        aes.OnEncryptionMessage += (msg) => { /*Console.WriteLine(msg);*/ progressBar.WriteLine(msg); };

                                        aesDecryptionResult = aes.DecryptFile(decryptOptions.InputToBeDecrypted, decryptOptions.OutputFilePath, decryptOptions.Password, decryptOptions.DeleteEncryptedFile);
                                    }
                                }
                                break;
                            default:
                                aesDecryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown algorithm \"{decryptOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                default:
                    aesDecryptionResult = new AesEncryptionResult() { Success = false, Message = $"Unknown input type \"{decryptOptions.InputType}\"." };
                    break;
            }

            if (aesDecryptionResult.Success)
            {
                Console.WriteLine((decryptOptions.InputType.ToLower().Equals("string") ? aesDecryptionResult.DecryptedDataString : aesDecryptionResult.Message));

                return ExitCode.Sucess;
            }
            else
            {
                Console.WriteLine(aesDecryptionResult.Message);

                return ExitCode.Error;
            }
        }

        private static ExitCode RunHashOptionsAndReturnExitCode(HashOptions hashOptions)
        {
            GenericHashResult hashResult = null;

            switch (hashOptions.InputType.ToLower())
            {
                case "string":
                    {
                        switch (hashOptions.Algorithm.ToLower())
                        {
                            case "md5":
                                hashResult = new MD5().HashString(hashOptions.InputToBeHashed);
                                break;
                            case "sha1":
                                hashResult = new SHA1().HashString(hashOptions.InputToBeHashed);
                                break;
                            case "sha256":
                                hashResult = new SHA256().HashString(hashOptions.InputToBeHashed);
                                break;
                            case "sha384":
                                hashResult = new SHA384().HashString(hashOptions.InputToBeHashed);
                                break;
                            case "sha512":
                                hashResult = new SHA512().HashString(hashOptions.InputToBeHashed);
                                break;
                            case "bcrypt":
                                hashResult = new Hash.BCrypt().HashString(hashOptions.InputToBeHashed);
                                break;
                            default:
                                hashResult = new GenericHashResult() { Success = false, Message = $"Unknown algorithm \"{hashOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                case "file":
                    {
                        switch (hashOptions.Algorithm.ToLower())
                        {
                            case "md5":
                                hashResult = new MD5().HashFile(hashOptions.InputToBeHashed);
                                break;
                            case "sha1":
                                hashResult = new SHA1().HashFile(hashOptions.InputToBeHashed);
                                break;
                            case "sha256":
                                hashResult = new SHA256().HashFile(hashOptions.InputToBeHashed);
                                break;
                            case "sha384":
                                hashResult = new SHA384().HashFile(hashOptions.InputToBeHashed);
                                break;
                            case "sha512":
                                hashResult = new SHA512().HashFile(hashOptions.InputToBeHashed);
                                break;
                            case "bcrypt":
                                hashResult = new GenericHashResult() { Success = false, Message = $"Algorithm \"{hashOptions.Algorithm}\" currently not available for file hashing." };
                                break;
                            default:
                                hashResult = new GenericHashResult() { Success = false, Message = $"Unknown algorithm \"{hashOptions.Algorithm}\"." };
                                break;
                        }
                    }
                    break;
                default:
                    hashResult = new GenericHashResult() { Success = false, Message = $"Unknown input type \"{hashOptions.InputType}\"." };
                    break;
            }

            if (hashResult.Success && !string.IsNullOrWhiteSpace(hashOptions.CompareHash))
            {
                bool hashesMatch = (
                    hashOptions.Algorithm.ToLower() != "bcrypt"
                        ? (hashResult.Hash).Equals(hashOptions.CompareHash, StringComparison.InvariantCultureIgnoreCase)
                        : new Hash.BCrypt().Verify(hashOptions.InputToBeHashed, hashOptions.CompareHash).Success
                );

                var outputMessage = (
                    hashesMatch
                        ? $"Computed hash MATCH with given hash: {(hashOptions.Algorithm.ToLower() != "bcrypt" ? hashResult.Hash : hashOptions.CompareHash)}"
                        : $"Computed hash DOES NOT MATCH with given hash." +
                        (
                            hashOptions.Algorithm.ToLower() != "bcrypt"
                                ? $"\nComputed hash: {hashResult.Hash}\nGiven hash: {hashOptions.CompareHash}"
                                : ""
                        )
                );

                Console.WriteLine(outputMessage);

                return (hashesMatch ? ExitCode.Sucess : ExitCode.Error);
            }
            else if (hashResult.Success && string.IsNullOrWhiteSpace(hashOptions.CompareHash))
            {
                Console.WriteLine(hashResult.Hash);

                return ExitCode.Sucess;
            }
            else
            {
                Console.WriteLine(hashResult.Message);

                return ExitCode.Error;
            }
        }

        private static ExitCode HandleParseError(IEnumerable<Error> errors, ParserResult<object> parserResult)
        {
            HelpText.AutoBuild(parserResult, h =>
            {
                return HelpText.DefaultParsingErrorsHandler(parserResult, h);
            },
            e => { return e; });

            return ExitCode.Error;
        }

        private static void ShowErrorMessage(string errorMessage)
        {
            Console.WriteLine($"An error has occured during processing:\n{errorMessage}");
        }
    }
}
