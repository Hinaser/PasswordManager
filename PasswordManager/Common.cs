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
#endregion

namespace PasswordManager
{
    public class InternalApplicationConfig
    {
        public static string DefaultLocale = "en-US";

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
    }
}
