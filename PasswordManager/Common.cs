#region Notice
/*
 * Author: Yunoske
 * Create Date: May 27, 2015
 * Description :
 * 
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
    public class InternalApplicationConfig
    {
        public static string LocaleEnUS = "en-US";
        public static string LocaleJaJP = "ja-JP";
        public static string DefaultLocale = LocaleEnUS;
        public static Dictionary<int, string> Locale = new Dictionary<int, string>();
        public static string DefaultPasswordFilePath = Environment.CurrentDirectory + @"\password.dat";
        public static string DefaultMasterPassword = "";
        public static int RootContainerID = 0;
        public static string RootContainerLabel = "All";
        public static string NewUnnamedContainerLabel = "New folder";
        public static HashAlgorithm Hash = new SHA512Managed();
        public static int HeaderTokenSize = DateTime.Now.ToString(CultureInfo.InvariantCulture).ToCharArray().Length;
        public static int BitsPerAByte = 8;
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
        /// Combine 2 hash value
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static byte[] GetHashCombined(byte[] h1, byte[] h2)
        {
            if (h1.Length != h2.Length)
            {
                throw new ArgumentException();
            }

            byte[] ret = new byte[h1.Length];
            for (int i = 0; i < h1.Length; i++)
            {
                ret[i] = (byte)((int)h1[i]*3 ^ (int)h2[i]);
            }

            return ret;
        }

        /// <summary>
        /// Get hash value as byte array
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static byte[] GetHash(char[] c)
        {
            if (c == null || c.Length < 1)
            {
                c = new char[] { '\0' };
            }

            return InternalApplicationConfig.Hash.ComputeHash(Encoding.Unicode.GetBytes(c));
        }

        /// <summary>
        /// Get hash value as byte array
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] GetHash(string b)
        {
            if (b == null)
            {
                b = String.Empty;
            }

            return Utility.GetHash(b.ToCharArray());
        }

        /// <summary>
        /// Get hash value as byte array for string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] GetHash(byte[] b)
        {
            return InternalApplicationConfig.Hash.ComputeHash(b);
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
