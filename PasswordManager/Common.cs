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
#endregion

namespace PasswordManager
{
    public class InternalApplicationConfig
    {
        public static string DefaultLocale = "en-US";
        public static string DefaultPasswordFilename = "data";
        public static int RootContainerID = 0;
    }

    public static class PrivateUtility
    {
        // Parameters related to Windows Handle
        public const UInt32 WsHScroll = 0x100000;
        public const UInt32 WsVScroll = 0x200000;
        public const int GwlStyle = (-16);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// This method is imitating Stream.CopyTo because .NET 3.5 does not implement the method.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
