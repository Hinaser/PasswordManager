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
using System.Runtime.Serialization.Formatters.Binary;
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
                foreach (Type type in asm.GetTypes())
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
    /// Apply or remove filter to/from specified object data
    /// </summary>
    public class IOFilterProcessor
    {
        /// <summary>
        /// This filter change nothing. Only pass exact same content to output stream from input stream.
        /// </summary>
        private sealed class BaseFilter : IOFilterBase
        {
            public override void InputFilter(ref MemoryStream src, ref MemoryStream dest)
            {
                dest = src;
            }

            public override void OutputFilter(ref MemoryStream src, ref MemoryStream dest)
            {
                dest = src;
            }
        }

        /// <summary>
        /// Apply filter to specified object.
        /// </summary>
        /// <param name="obj">Data to be filtered</param>
        /// <param name="filterOrder">Order to apply filter. filterOrder[0] is applied at first and filterOrder[lastindex] is applied at last.</param>
        /// <returns></returns>
        public static FilteredData ApplyFilter(Object obj, List<string> filterOrder)
        {
            MemoryStream[] bodyStream = new MemoryStream[] { new MemoryStream(), new MemoryStream() };
            BinaryFormatter formatter = new BinaryFormatter();

            // Convert password data object to binary data stream
            formatter.Serialize(bodyStream[0], obj);
            bodyStream[0].Position = 0;

            List<string> order = new List<string>();
            order.Add(typeof(BaseFilter).ToString());
            order.AddRange(filterOrder);

            // Apply data filters
            // NoFilter must come first for filtering. This is why do-while statement is using instead of while or for statement
            int i = 0;
            FilteredData filteredData = new FilteredData();
            do
            {
                // Get filter name registered
                string filterName = order[i];

                // Throw exception if associated filter instance does not exist
                if (!IOFilterFactory.Instance.ContainsIOFilter(filterName))
                {
                    bodyStream[0].Close();
                    bodyStream[1].Close();
                    throw new InvalidOperationException();
                }

                // Get target filter instance
                IOFilterBase filter = IOFilterFactory.Instance.GetIOFilter(filterName);

                // Write filter name information
                Utility.GetCharTerminatedByZero(filterName, ref filteredData.Filter);

                // Do apply filter
                filter.OutputFilter(ref bodyStream[i % 2], ref bodyStream[(i + 1) % 2]); // Convert input as a filter does and write it to output stream
                bodyStream[(i + 1) % 2].Position = 0;

                // Set filtered data
                filteredData.data = bodyStream[(i + 1) % 2].ToArray();

                // Set filter object into MemoryStream
                bodyStream[(i + 1) % 2].Close();
                bodyStream[(i + 1) % 2] = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(bodyStream[(i + 1) % 2]);
                writer.Write(filteredData.Filter, 0, filteredData.Filter.Length);
                writer.Write(filteredData.data, 0, filteredData.data.Length);
                // The line below is not nice but in .NET Framework 3.5, when BinaryWriter is Closed(), its BaseStream will be also Closed() so the line below is given in order to preserve original base stream content.
                bodyStream[(i + 1) % 2] = new MemoryStream(bodyStream[(i + 1) % 2].ToArray());
                writer.Close();

                // Reset MemoryStream
                bodyStream[i % 2].Close(); // Release input stream resources
                bodyStream[i % 2] = new MemoryStream();
            } while (++i < order.Count);

            // Convert FilteredData object into MemoryStream

            return filteredData;
        }

        /// <summary>
        /// Remove filter from specified memorystream.
        /// </summary>
        /// <param name="m"></param>
        /// <remarks>Filter data must not be more than 2GB (Signed integer max)</remarks>
        /// <returns></returns>
        public static Object RemoveFilter(FilteredData filteredData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream[] bodyStream = new MemoryStream[2] { new MemoryStream(), new MemoryStream() };
            Object returnVal = null;

            // Parse filter data
            int i = 0;
            for (string filterName = Utility.GetStringByZeroTarminatedChar(filteredData.Filter);
                filterName != typeof(BaseFilter).ToString() && i < InternalApplicationConfig.MaxFilterCount;
                i++, filterName = Utility.GetStringByZeroTarminatedChar(filteredData.Filter))
            {
                if (!IOFilterFactory.Instance.ContainsIOFilter(filterName))
                {
                    bodyStream[0].Close();
                    bodyStream[1].Close();
                    throw new NoCorrespondingFilterFoundException(filterName);
                }

                // Get filter instance
                IOFilterBase filter = IOFilterFactory.Instance.GetIOFilter(filterName);

                // Setup MemoryStream
                bodyStream[i % 2] = new MemoryStream(filteredData.data);

                // Remove filter here
                filter.InputFilter(ref bodyStream[i % 2], ref bodyStream[(i + 1) % 2]);

                // Get filter object
                bodyStream[(i + 1) % 2].Position = 0;
                BinaryReader reader = new BinaryReader(bodyStream[(i + 1) % 2]);
                filteredData.Filter = reader.ReadChars(InternalApplicationConfig.FilterNameFixedLength);
                if (bodyStream[(i + 1) % 2].Length > Int32.MaxValue) throw new OverflowException(); // When byte length is larger than intergar max value, throw exception.
                filteredData.data = reader.ReadBytes((int)bodyStream[(i + 1) % 2].Length);
                // The line below is not nice but in .NET Framework 3.5, when BinaryReader is Closed(), its BaseStream will be also Closed() so the line below is given in order to preserve original base stream content.
                bodyStream[(i + 1) % 2] = new MemoryStream(bodyStream[(i + 1) % 2].ToArray());
                reader.Close();

                // Reset MemoryStream
                bodyStream[i % 2].Close();
                bodyStream[i % 2] = new MemoryStream();
            }

            try
            {
                bodyStream[(i + 1) % 2] = new MemoryStream(filteredData.data);
                returnVal = formatter.Deserialize(bodyStream[(i + 1) % 2]);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                bodyStream[0].Close();
                bodyStream[1].Close();
            }

            return returnVal;
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
        public abstract void InputFilter(ref MemoryStream inStream, ref MemoryStream outSteram);

        /// <summary>
        /// This method should be used before writing data to file.
        /// </summary>
        /// <param name="inStream">Source stream</param>
        /// <param name="outSteram">Destination stream</param>
        public abstract void OutputFilter(ref MemoryStream inStream, ref MemoryStream outSteram);
    }

    /// <summary>
    /// This filter is to check whether filter logic is no problem. So it is not expected to be used for actual service.
    /// </summary>
    public class DebugFilter : IOFilterBase
    {
        public override void InputFilter(ref MemoryStream src, ref MemoryStream dest)
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

        public override void OutputFilter(ref MemoryStream src, ref MemoryStream dest)
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
