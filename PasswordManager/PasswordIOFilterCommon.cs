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
using System.Reflection;
#endregion

namespace PasswordManager
{
    /// <summary>
    /// Storing all available IOFilter objects saved in assembly
    /// </summary>
    public class IOFilterFactory
    {
        // Singleton related objects
        private static IOFilterFactory InternalInstance = null;
        private static Object syncRoot = new Object();

        private Dictionary<string, IOFilterBase> IOFilters = new Dictionary<string, IOFilterBase>();

        // Private default constructor to achieve singleton model
        private IOFilterFactory() { }

        /// <summary>
        /// Only 1 instance for this class
        /// </summary>
        public static IOFilterFactory Instance
        {
            get
            {
                if (InternalInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (IOFilterFactory.InternalInstance == null)
                        {
                            IOFilterFactory.InternalInstance = new IOFilterFactory();
                            IOFilterFactory.InternalInstance.GatherIOFilterFromAssemly(Assembly.GetExecutingAssembly(), false);
                        }
                    }
                }

                return IOFilterFactory.InternalInstance;
            }
        }

        /// <summary>
        /// Inquire specified assembly to see if there are any IOFilterBase class defined. If there are, it adds those instances to own collection object.
        /// </summary>
        /// <param name="asm"></param>
        public void GatherIOFilterFromAssemly(Assembly asm, bool preserve = false)
        {
            if (asm == null)
            {
                throw new ArgumentNullException();
            }

            // To enable to keep current registered IOFIlterBase class
            HashSet<string> hash = new HashSet<string>();

            // Clear current registered IOFilters
            if (!preserve)
            {
                this.IOFilters.Clear();
            }
            else
            {
                foreach (var kvp in this.IOFilters)
                {
                    // Enroll current registered IOFilter class names
                    hash.Add(kvp.Key);
                }
            }

            // Create Instance from assembly
            try
            {
                foreach (Type type in asm.GetExportedTypes())
                {
                    if (type.IsSubclassOf(typeof(IOFilterBase)))
                    {
                        // When the type is already stored, skip the type
                        if (!hash.Add(type.ToString()))
                        {
                            continue;
                        }

                        IOFilterBase filter = (IOFilterBase)Activator.CreateInstance(type);
                        this.IOFilters.Add(type.ToString(), filter);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Load IOFilter class from specified dll file path
        /// </summary>
        /// <param name="dllPath"></param>
        public void GatherIOFilterFromAssemly(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException(dllPath);
            }

            Assembly asm;

            // Load assembly from specified dll file path
            try
            {
                asm = Assembly.LoadFrom(dllPath);
            }
            catch (FileLoadException e) { throw e; }
            catch (BadImageFormatException e) { throw e; }
            catch (Exception e) { throw e; }

            this.GatherIOFilterFromAssemly(asm, true);
        }

        /// <summary>
        /// Get specified IO filter class
        /// </summary>
        /// <param name="typeName">String representing the name of the class derived from IOFilterBase</param>
        /// <returns></returns>
        public IOFilterBase GetIOFilter(string typeName)
        {
            if (!this.IOFilters.ContainsKey(typeName))
            {
                return null;
            }

            return this.IOFilters[typeName];
        }

        /// <summary>
        /// Clear all registered IOFilter classes
        /// </summary>
        public void ClearIOFilters()
        {
            this.IOFilters.Clear();
        }

        /// <summary>
        /// Check whether an instance of specified type name is registered into the factory
        /// </summary>
        /// <param name="typeName">String representing the name of the class derived from IOFilterBase</param>
        /// <returns></returns>
        public bool ContainsIOFilter(string typeName)
        {
            return this.IOFilters.ContainsKey(typeName);
        }

        /// <summary>
        /// Get enumrator of registered IOFilterBase
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IOFilterBase>.Enumerator GetEnumrator()
        {
            return this.IOFilters.GetEnumerator();
        }
    }

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
    public sealed class NoFilter : IOFilterBase
    {
        public override void InputFilter(MemoryStream src, MemoryStream dest)
        {
            Utility.CopyStream(src, dest);
        }

        public override void OutputFilter(MemoryStream src, MemoryStream dest)
        {
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
            Utility.CopyStream(temp, dest);
        }
    }
}
