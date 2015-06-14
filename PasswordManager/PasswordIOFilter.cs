﻿#region Notice
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
    /// Abstract class for handling inputting/outputting data stream.
    /// </summary>
    public abstract class IOFilterBase
    {
        /// <summary>
        /// This method should be used after reading data from file.
        /// </summary>
        /// <param name="inStream">Source stream</param>
        /// <param name="outSteram">Destination stream</param>
        public abstract void InputFilter(MemoryStream inStream, MemoryStream outSteram);

        /// <summary>
        /// This method should be used before writing data to file.
        /// </summary>
        /// <param name="inStream">Source stream</param>
        /// <param name="outSteram">Destination stream</param>
        public abstract void OutputFilter(MemoryStream inStream, MemoryStream outSteram);
    }

    /// <summary>
    /// This filter change nothing. Only pass exact same content to output stream from input stream.
    /// </summary>
    public class NoFilter : IOFilterBase
    {
        public override void InputFilter(MemoryStream src, MemoryStream dest)
        {
            src.Position = 0;
            dest.Position = 0;
            Utility.CopyStream(src, dest);
        }

        public override void OutputFilter(MemoryStream src, MemoryStream dest)
        {
            src.Position = 0;
            dest.Position = 0;
            Utility.CopyStream(src, dest);
        }
    }

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
            Utility.CopyStream(temp, dest);
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
            Utility.CopyStream(temp, dest);
        }
    }
}
