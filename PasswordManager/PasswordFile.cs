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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Globalization;
#endregion

namespace PasswordManager
{
    /// <summary>
    /// A class which handles input/output PasswordObject to actual file.
    /// </summary>
    public class PasswordFile
    {
        #region Field
        protected string Filepath = InternalApplicationConfig.DefaultPasswordFilePath;
        List<string> FilterOrder = new List<string>();
        #endregion

        #region Constructor
        private PasswordFile() { } // Making default constructor private in order to gurantee that PasswordFile object is always with the filename.
        public PasswordFile(string filepath)
        {
            this.Filepath = filepath;
            this.FilterOrder.Add(typeof(NoFilter).ToString()); // The nearest filter to the data content must be NoFilter
        }
        #endregion

        #region Setter method
        /// <summary>
        /// Set file path to member field.
        /// </summary>
        /// <param name="filename"></param>
        public void SetFilePath(string filepath)
        {
            this.Filepath = filepath;
        }

        /// <summary>
        /// Set filter order hash list. Be attention this clears original list member field.
        /// </summary>
        /// <param name="order"></param>
        public void SetFilterOrder(List<string> order)
        {
            this.FilterOrder = order;
        }

        /// <summary>
        /// Add a filter hash to tail of the list field..
        /// </summary>
        /// <param name="filterHash"></param>
        public void AddFilterOrder(string filterName)
        {
            this.FilterOrder.Add(filterName);
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get file path from member field.
        /// </summary>
        /// <returns></returns>
        public string GetFilename()
        {
            return this.Filepath;
        }
        #endregion

        #region I/O
        /// <summary>
        /// Read password file with removing data filter. Available filters must be added to instance before this method is called.
        /// </summary>
        /// <exception cref="FileNotFoundException">Password file does not exist</exception>
        /// <exception cref="InvalidMasterPasswordException">Master password is invalid</exception>
        public virtual PasswordFileBody ReadPasswordFromFile(byte[] masterPasswordHash, PasswordFileBody defaultPasswordFileBody = null)
        {
            if (masterPasswordHash.Length != InternalApplicationConfig.Hash.HashSize/InternalApplicationConfig.BitsPerAByte)
            {
                throw new ArgumentException();
            }

            // When password file does not exist, throw an exception.
            if (!File.Exists(this.Filepath))
            {
                throw new FileNotFoundException(this.Filepath);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream[] bodyStream = new MemoryStream[2] { new MemoryStream(), new MemoryStream() };
            PasswordFileBody returnVal;
            FilteredData filteredData;

            // Parse file header
            using (FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    PasswordHeader header = new PasswordHeader();
                    header.Token = reader.ReadChars(InternalApplicationConfig.HeaderTokenSize);
                    header.CombinedMasterPasswordHash = reader.ReadBytes(InternalApplicationConfig.Hash.HashSize / InternalApplicationConfig.BitsPerAByte);

                    // Check masterPasswordHash is valid
                    if (!this.CheckMasterPasswordHash(header.CombinedMasterPasswordHash, masterPasswordHash, header.Token))
                    {
                        bodyStream[0].Close();
                        bodyStream[1].Close();
                        throw new InvalidMasterPasswordException();
                    }

                    bodyStream[0] = Utility.GetMemoryStream(reader);
                    bodyStream[0].Position = 0;
                    // Do decryption against loaded MemoryStream
                    // something ...
                    Utility.CopyStream(bodyStream[0], bodyStream[1]);
                    bodyStream[1].Position = 0;

                    filteredData = (FilteredData)formatter.Deserialize(bodyStream[1]);
                    bodyStream[1].Position = 0;
                }
            }

            // Parse filter data
            int i = 0;
            for (string filterName = Utility.GetStringByZeroTarminatedChar(filteredData.Filter);
                filterName != typeof(NoFilter).ToString() && i < InternalApplicationConfig.MaxFilterCount;
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

                // Apply unfilter
                filter.InputFilter(bodyStream[i % 2], bodyStream[(i + 1) % 2]);

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
                bodyStream[(i+1)%2] = new MemoryStream(filteredData.data);
                returnVal = (PasswordFileBody)formatter.Deserialize(bodyStream[(i + 1) % 2]);
            }
            catch(Exception e)
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

        /// <summary>
        /// Write password data with filtering original data. Available filters must be added to instance before this method is called.
        /// </summary>
        public virtual void WritePasswordToFile(byte[] masterPasswordHash, PasswordFileBody passwordData)
        {
            if (!PasswordFile.CheckDirectoryWritable(Directory.GetParent(this.Filepath).FullName))
            {
                throw new IOException();
            }

            MemoryStream[] bodyStream = new MemoryStream[]{new MemoryStream(), new MemoryStream()};
            BinaryFormatter formatter = new BinaryFormatter();

            // Convert password data object to binary data stream
            formatter.Serialize(bodyStream[0], passwordData);
            bodyStream[0].Position = 0;

            // Apply data filters
            // NoFilter must come first for filtering. This is why do-while statement is using instead of while or for statement
            int i = 0;
            FilteredData filteredData = new FilteredData();
            do
            {
                // Get filter name registered
                string filterName = this.FilterOrder[i];

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
                filter.OutputFilter(bodyStream[i % 2], bodyStream[(i + 1) % 2]); // Convert input as a filter does and write it to output stream
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
            } while (++i < this.FilterOrder.Count);

            // Convert FilteredData object into MemoryStream
            formatter.Serialize(bodyStream[(i + 1) % 2], filteredData);

            // Do encryption here
            // .....

            // Construct header
            PasswordHeader header = new PasswordHeader();
            header.Token = DateTime.Now.ToString(CultureInfo.InvariantCulture).ToCharArray();
            header.CombinedMasterPasswordHash = Utility.GetHashCombined(masterPasswordHash, Utility.GetHash(header.Token));

            // Write filtered data to the file
            using (FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // Write header
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(header.Token); // Write token
                    writer.Write(header.CombinedMasterPasswordHash); // Write hash
                    // Write Filtered data which has been encrypted
                    writer.Write(bodyStream[(i + 1) % 2].ToArray());
                }
            }
        }

        /// <summary>
        /// Initialize password data file. Plese be carefull that this method also clear data in this.FileContents member variable.
        /// </summary>
        public virtual void ResetPasswordFile(byte[] masterPasswordHash, PasswordFileBody b = null)
        {
            this.WritePasswordToFile(masterPasswordHash, b != null ? b : new PasswordFileBody());
        }

        /// <summary>
        /// Check whether specified directory is writable.
        /// </summary>
        /// <param name="directoryPath">Directory full path</param>
        /// <returns>true if it is writable, false if it isn't</returns>
        public static bool CheckDirectoryWritable(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return false;
            }

            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(directoryPath);
                return true;
            }
            catch(UnauthorizedAccessException)
            {
                return false;
            }
        }

        /// <summary>
        /// Challenge master password hash with token.
        /// </summary>
        /// <param name="expectedCombinedHash">This should be combining result of master password hash and token, which might be described in Password file header</param>
        /// <param name="challengingMasterPasswordHash">Challenging password hash</param>
        /// <param name="token">Token value, which might be described in Password file header</param>
        /// <returns></returns>
        public bool CheckMasterPasswordHash(byte[] expectedCombinedHash, byte[] challengingMasterPasswordHash, char[] token)
        {
            return CompareHashes(expectedCombinedHash, Utility.GetHashCombined(challengingMasterPasswordHash, Utility.GetHash(token)));
        }

        /// <summary>
        /// Compare two argument hashes one by one.
        /// </summary>
        /// <param name="b1">A hash value to compare</param>
        /// <param name="b2">A hash value to compare</param>
        /// <returns></returns>
        public bool CompareHashes(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
            {
                return false;
            }

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }

    /// <summary>
    /// Header layout for password file
    /// </summary>
    public class PasswordHeader
    {
        public char[] Token = new char[InternalApplicationConfig.HeaderTokenSize];
        public byte[] CombinedMasterPasswordHash = new byte[InternalApplicationConfig.Hash.HashSize / InternalApplicationConfig.BitsPerAByte];
    }

    /// <summary>
    /// Data which is filtered
    /// </summary>
    [Serializable]
    public class FilteredData
    {
        public char[] Filter = new char[InternalApplicationConfig.FilterNameFixedLength];
        public byte[] data;
    }

    /// <summary>
    /// Raw(unfiltered) contents of password file
    /// </summary>
    [Serializable]
    public class PasswordFileBody : IDisposable
    {
        public PasswordIndexerBase Indexer;
        public ICollection<PasswordContainer> Containers;
        public ICollection<PasswordRecord> Records;

        public PasswordFileBody() : this(new PasswordIndexer(), new List<PasswordContainer>(), new List<PasswordRecord>()) { }
        public PasswordFileBody(PasswordIndexerBase i, ICollection<PasswordContainer> c, ICollection<PasswordRecord> r)
        {
            this.Indexer = i;
            this.Containers = c;
            this.Records = r;
        }

        /// <summary>
        /// Sanitize all stored password data
        /// </summary>
        public void Dispose()
        {
            if (this.Containers!= null)
            {
                foreach (PasswordContainer c in this.Containers)
                {
                    c.Dispose();
                }
            }

            if (this.Records != null)
            {
                foreach (PasswordRecord r in this.Records)
                {
                    r.Dispose();
                }
            }
        }
    }
}
