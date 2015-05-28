#region Notice
/*
 * Author: Yunoske
 * Create Date: May 29, 2015
 * Description :
 * 
 */
#endregion

#region include
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#endregion

namespace PasswordManager
{
    /// <summary>
    /// This filter is to check whether filter logic is no problem. So it is not expected to be used for actual service.
    /// </summary>
    public class DebugFilter : IOFilterBase
    {
        public override void InputFilter(MemoryStream src, MemoryStream dest)
        {
            byte[] debug = src.ToArray();

            // Modify any byte value to whatever you want to debug
            for(int i=0;i<debug.Length;i++)
            {
                 debug[i] = (byte)(~(int)debug[i]);
            }
            //////////////////////////////////////////////////////////////////

            MemoryStream temp = new MemoryStream(debug);
            dest.Position = 0;
            PrivateUtility.CopyStream(temp, dest);
        }

        public override void OutputFilter(MemoryStream src, MemoryStream dest)
        {
            byte[] debug = src.ToArray();

            // Modify any byte value to whatever you want to debug
            for (int i = 0; i < debug.Length; i++)
            {
                debug[i] = (byte)(~(int)debug[i]);
            }
            //////////////////////////////////////////////////////////////////

            MemoryStream temp = new MemoryStream(debug);
            dest.Position = 0;
            PrivateUtility.CopyStream(temp, dest);
        }
    }
}
