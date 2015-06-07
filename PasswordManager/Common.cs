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
        public static string DefaultMasterPassword = "password";
        public static string DefaultPasswordFilename = "data";
        public static int RootContainerID = 0;
        public static string RootContainerLabel = "All";
        public static string NewUnnamedContainerLabel = "New folder";
        public static HashAlgorithm Hash = new SHA512Managed();
        public static int MaxFilter = 1000;
        public static int HeaderTokenSize = DateTime.Now.ToString(CultureInfo.InvariantCulture).ToCharArray().Length;
        public static int BitsPerAByte = 8;
        public static decimal PasswordMinLength = 1;
        public static decimal passwordMaxLength = 128;
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
            return InternalApplicationConfig.Hash.ComputeHash(Encoding.Unicode.GetBytes(c));
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
    }

    #region Exception
    public class InvalidMasterPasswordException : Exception { }
    public class NoCorrespondingFilterFoundException : Exception { }
    #endregion
}
