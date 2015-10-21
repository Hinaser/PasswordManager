#region Notice
/*
 * Copyright 2015 Hinaser
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

#region include
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; // For DllImport attribute
using System.IO;
using System.Security.Cryptography;
using System.Globalization;
#endregion

namespace PasswordManager
{
    public class LocalConfig
    {
        public static string LocaleEnUS = "en-US";
        public static string LocaleJaJP = "ja-JP";
        public static string DefaultLocale = LocaleEnUS;
        public static string DefaultPasswordFileName = @"password.dat";
        public static string DefaultPasswordFilePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + DefaultPasswordFileName;
        public static string DefaultMasterPassword = "";
        public static string DefaultSalt = "salt";
        public static int RootContainerID = 0;
        public static string RootContainerLabel = "All";
        public static string NewUnnamedContainerLabel = "New folder";
        public static int HeaderTokenSize = 16; // in bytes
        public static int MasterPasswordHashedKeySize = 32; // in bytes. Must be either 16/24/32.
        public static int CaptionMaxLength = 128;
        public static int IDMaxLength = 128;
        public static decimal PasswordMinLength = 1;
        public static decimal PasswordMaxLength = 128;
        public static int DescriptionMaxLength = 1024;
        public static int ContainerNameMax = 64;
        public static double MaxWeakPasswordStrength = FormCreatePassword.CalculatePasswordStrength("aaAA00", 6);
        public static double MaxNormalPasswordStrength = FormCreatePassword.CalculatePasswordStrength("aaAA00Abc1", 10);
        public static int PasswordStrengthNoticeHeaderSize = 8;
        public static string PasswordStrengthNoticeFormat = String.Format("{0}{1}{2}{3}", "{0, -", PasswordStrengthNoticeHeaderSize, "} {1}", Environment.NewLine);
        public static string[] DangerousPasswordList = new String[] {
            "password", "Password","Pass","pass","pswd","Pswd",
            "a","aa","aaa","aaaa","aaaaa",
            "0","1","2","3","4","5","6","7","8","9",
            "00","11","22","33","44","55","66","77","88","99",
            "000","111","222","333","444","555","666","777","888","999",
            "0000","1111","2222","3333","4444","5555","6666","7777","8888","9999"
            ,"1234","2345","3456","4567","5678","6789",
            "9876","8765","7654","6543","5432","4321",
        };
        public static string Separater1 = ",";
        public static int FilterNameFixedLength = 128;
        public static int MaxFilterCount = 10;
        public static int InitialWaitMiliSecondsWhenPasswordIsInvalid = 1000;
        public static int RetryTickIntervalMiliSec = 200;
        public static string OpeningPasswordFileFilter = "Data files (*.dat)|*.dat|All files (*.*)|*.*";
        public static int StatusStripFilePathMaxLength = 40;
        public static int StatusStripPasswordLabelMaxLength = 10;
        public static string DefaultFileExt = ".dat";
        public static CipherMode CipherMode = CipherMode.CBC;
        public static PaddingMode PaddingMode = PaddingMode.PKCS7;
        public static int CryptBlockSize = 128;
        public static int Rfc2898DeriveBytesIterationCount = 3000;
        public enum FolderState { CLOSING = 0, OPENNING = 1 };
    }

    public static class Utility
    {
        // Parameters related to Windows Handle
        public const UInt32 WsHScroll = 0x100000;
        public const UInt32 WsVScroll = 0x200000;
        public const int GwlStyle = (-16);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Password character resource
        public static char[] ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static char[] Alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        public static char[] Numeric = "0123456789".ToCharArray();
        public static char[] Symbol = "!\"#$%&'()-=^~\\|@`[{;+:*]},<.>/?_ ".ToCharArray();

        /// <summary>
        /// This method is imitating Stream.CopyTo because .NET 3.5 does not implement the method.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyStream(Stream src, Stream dest)
        {
            byte[] buffer = new byte[16 * 1024];
            int bytesRead;

            while ((bytesRead = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dest.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Get MemorySteram from BinaryReader. The binary data is read from the original position.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static MemoryStream GetMemoryStream(BinaryReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException();
            }

            MemoryStream ms = new MemoryStream();

            const int bufferSize = 16 * 1024;
            byte[] buffer = new byte[bufferSize];
            int count;

            while ((count = reader.Read(buffer, 0, bufferSize)) != 0)
            {
                ms.Write(buffer, 0, count);
            }

            return ms;
        }

        /// <summary>
        /// Get char array terminated by NUL.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        public static char[] GetCharTerminatedByZero(string text, int maxLen)
        {
            maxLen = maxLen > text.Length ? maxLen : text.Length + 1;

            char[] retValue = new char[maxLen];

            for (int i = text.Length; i < maxLen; i++)
            {
                retValue[i] = '\0';
            }

            for (int i = 0; i < text.Length; i++)
            {
                retValue[i] = text[i];
            }

            return retValue;
        }

        /// <summary>
        /// Fill target char[] with string text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <remarks>When length of string is larger than length of char array, extra characters will be cut but the last index of char array is always NUL.</remarks>
        /// <returns></returns>
        public static void GetCharTerminatedByZero(string text, ref char[] target)
        {
            int maxLen = target.Length;

            target[maxLen - 1] = '\0';

            for (int i = text.Length; i < maxLen; i++)
            {
                target[i] = '\0';
            }

            for (int i = 0; i < text.Length && i < maxLen - 1; i++)
            {
                target[i] = text[i];
            }
        }

        /// <summary>
        /// Convert 0 terminated char array into string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetStringByZeroTarminatedChar(char[] text)
        {
            int positionFirstNUL;

            for (positionFirstNUL = 0; positionFirstNUL < text.Length; positionFirstNUL++)
            {
                if (text[positionFirstNUL] == '\0')
                {
                    break;
                }
            }

            if (positionFirstNUL == 0)
            {
                return String.Empty;
            }

            return new String(text, 0, positionFirstNUL);
        }

        /// <summary>
        /// Get shortened text for specified text and maximum length.
        /// Midst text will be replaced by short word (replacer).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <param name="replacer"></param>
        /// <example>
        /// "C:\test1\test2\test3\test4\test5\somefile.txt" can be "C:\test1\~\somefile.txt"
        /// </example>
        /// <returns></returns>
        public static string GetShorterTextMiddle(string text, int maxLen, string replacer = null)
        {
            const string replacerDefault = "...";
            if (String.IsNullOrEmpty(replacer))
            {
                replacer = replacerDefault;
            }

            if (text.Length <= maxLen || text.Length < replacer.Length + 2 || maxLen < replacer.Length + 2) // +2 represents head char(+1) and tail char(+1)
            {
                return text;
            }

            // Pick up just a center index of maxLen value.
            int replacePosition = (maxLen - 1 - (replacer.Length - 1)) / 2;

            StringBuilder retVal = new StringBuilder();
            StringBuilder head = new StringBuilder();
            StringBuilder tail = new StringBuilder();
            for (int i = 0; i < replacePosition; i++)
            {
                head.Append(text[i]);
                tail.Append(text[text.Length - replacePosition + (i +1) - 1]); // when i = replacePosition -1, this is tail.Append(text[text.Length - 1]);
            }

            if ((maxLen - replacer.Length) % 2 != 0)
            {
                return retVal
                    .Append(head.ToString())
                    .Append(replacer)
                    .Append(text[text.Length - replacePosition - 1])
                    .Append(tail.ToString())
                    .ToString();
            }

            return retVal
                .Append(head.ToString())
                .Append(replacer)
                .Append(tail.ToString())
                .ToString(); ;
        }

        /// <summary>
        /// Get crypt algorithm
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static SymmetricAlgorithm GetCryptAlgorithm(byte[] key, byte[] iv)
        {
            return new RijndaelManaged
            {
                Mode = LocalConfig.CipherMode,
                Padding = LocalConfig.PaddingMode,
                BlockSize = LocalConfig.CryptBlockSize,
                Key = key,
                IV = iv
            };
        }

        /// <summary>
        /// Get shortened text for specified text and maximum length.
        /// Right edge text will be replaced by short word (replacer).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <param name="replacer"></param>
        /// <returns></returns>
        public static string GetShorterTextRight(string text, int maxLen, string replacer = null)
        {
            const string replacerDefault = "...";
            if (String.IsNullOrEmpty(replacer))
            {
                replacer = replacerDefault;
            }

            if (text.Length <= maxLen || text.Length < replacer.Length + 1 || maxLen < replacer.Length + 1) // +1 represents original head char(+1)
            {
                return text;
            }

            // Pick up just a center index of maxLen value.
            int replacePosition = (maxLen - 1 - (replacer.Length - 1));

            StringBuilder retVal = new StringBuilder();
            StringBuilder head = new StringBuilder();
            for (int i = 0; i < replacePosition; i++)
            {
                head.Append(text[i]);
            }

            return retVal
                .Append(head.ToString())
                .Append(replacer)
                .ToString(); ;
        }

        /// <summary>
        /// Get encrypted MemoryStream using Rijndael(AES) 256it
        /// </summary>
        /// <param name="m">MemoryStream to encrypt</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null)
            {
                throw new ArgumentNullException();
            }

            SymmetricAlgorithm algorithm = Utility.GetCryptAlgorithm(key, iv);
            ICryptoTransform encryptor = algorithm.CreateEncryptor();

            return Utility.PerformCrypt(encryptor, data);
        }

        /// <summary>
        /// Get decrypted MemoryStream using Rijndael(AES) 256it
        /// </summary>
        /// <param name="m">MemoryStream to decrypt</param>
        /// <param name="iv">Initializing vector</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data == null || key == null || iv == null)
            {
                throw new ArgumentNullException();
            }

            SymmetricAlgorithm algorithm = Utility.GetCryptAlgorithm(key, iv);
            ICryptoTransform decryptor = algorithm.CreateDecryptor();

            return Utility.PerformCrypt(decryptor, data);
        }

        /// <summary>
        /// Encrypt or Decrypt passwd memorystream
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="m"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] PerformCrypt(ICryptoTransform transform, byte[] data)
        {
            if (transform == null || data == null)
            {
                throw new ArgumentNullException();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Get scrambled value with speficified byte length from data and salt.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="salt"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static byte[] Scramble(byte[] data, byte[] salt, int byteCount)
        {
            Rfc2898DeriveBytes val = new Rfc2898DeriveBytes(data, salt, LocalConfig.Rfc2898DeriveBytesIterationCount);
            return val.GetBytes(byteCount);
        }

        /// <summary>
        /// Get scrambled value with speficified byte length from data and salt.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="salt"></param>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static byte[] Scramble(string data, string salt, int byteCount)
        {
            return Utility.Scramble(Encoding.UTF8.GetBytes(data), Utility.Get16bytesHash(salt), byteCount);
        }

        /// <summary>
        /// Get hash from specified string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] Get16bytesHash(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                s = "0";
            }

            HashAlgorithm hash = new SHA256Managed();
            byte[] b256 = hash.ComputeHash(Encoding.UTF8.GetBytes(s));
            byte[] ret = new byte[128/8];

            for (int i = 0; i < 128/8; i++)
            {
                ret[i] = (byte)(b256[i] ^ b256[i + 128/8]);
            }

            return ret;
        }

        /// <summary>
        /// Get hash from specified datetime
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static byte[] Get16BytesHash(DateTime d)
        {
            if (d == null)
            {
                d = DateTime.Now;
            }

            HashAlgorithm hash = new SHA256Managed();
            byte[] b256 = hash.ComputeHash(Encoding.ASCII.GetBytes(d.Ticks.ToString()));
            byte[] ret = new byte[128/8];

            for (int i = 0; i < 128/8; i++)
            {
                ret[i] = (byte)(b256[i] ^ b256[i+128/8]);
            }

            return ret;
        }
    }

    #region Exception
    public class InvalidMasterPasswordException : Exception
    {
        public InvalidMasterPasswordException() { }
        public InvalidMasterPasswordException(string msg) : base(msg) { }
        public InvalidMasterPasswordException(string msg, Exception innerException) : base(msg, innerException) { }
    }

    public class NoCorrespondingFilterFoundException : Exception
    {
        public NoCorrespondingFilterFoundException() { }
        public NoCorrespondingFilterFoundException(string msg) : base(msg) { }
        public NoCorrespondingFilterFoundException(string msg, Exception innterException) : base(msg, innterException) { }
    }
    #endregion
}
