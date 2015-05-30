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
        protected string Filepath = Environment.CurrentDirectory + InternalApplicationConfig.DefaultPasswordFilename;
        protected PasswordHeader Header = new PasswordHeader();
        protected PasswordFileBodyFiltered BodyFiltered = new PasswordFileBodyFiltered();
        protected List<IOFilterBase> AvailableFilters = new List<IOFilterBase>();
        #endregion

        #region Constructor
        private PasswordFile() { } // Making default constructor private in order to gurantee that PasswordFile object is always with the filename.
        public PasswordFile(string filepath)
        {
            this.Filepath = filepath;
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
        /// Append I/O filter object to member field.
        /// </summary>
        /// <param name="filter"></param>
        public void AddIOFilter(IOFilterBase filter)
        {
            this.AvailableFilters.Add(filter);
        }

        /// <summary>
        /// Set filter order hash list. Be attention this clears original list member field.
        /// </summary>
        /// <param name="order"></param>
        public void SetFilterOrder(List<string> order)
        {
            this.BodyFiltered.Filters = order;
        }

        /// <summary>
        /// Add a filter hash to tail of the list field..
        /// </summary>
        /// <param name="filterHash"></param>
        public void AddFilterOrder(string filterName)
        {
            this.BodyFiltered.Filters.Add(filterName);
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
        public virtual PasswordFileBody ReadPasswordFromFile(byte[] masterPasswordHash)
        {
            if (masterPasswordHash.Length != InternalApplicationConfig.Hash.HashSize/8)
            {
                throw new ArgumentException();
            }

            if (!File.Exists(this.Filepath))
            {
                this.ResetPasswordFile(masterPasswordHash);
            }

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream bodySteram;
            PasswordFileBody returnVal;

            using (FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    this.Header.Token = reader.ReadChars(InternalApplicationConfig.HeaderTokenSize);
                    this.Header.CombinedMasterPasswordHash = reader.ReadBytes(InternalApplicationConfig.Hash.HashSize / 8);

                    // Check masterPasswordHash is valid
                    if (!this.CheckMasterPasswordHash(this.Header.CombinedMasterPasswordHash, masterPasswordHash, this.Header.Token))
                    {
                        throw new UnauthorizedAccessException();
                    }

                    reader.BaseStream.Position = InternalApplicationConfig.HeaderTokenSize + InternalApplicationConfig.Hash.HashSize/8;
                    this.BodyFiltered = (PasswordFileBodyFiltered)formatter.Deserialize(reader.BaseStream);
                }
            }

            bodySteram = new MemoryStream(this.BodyFiltered.data);

            foreach (string filterName in this.BodyFiltered.Filters)
            {
                bool filterFound = false;
                foreach (IOFilterBase filter in this.AvailableFilters)
                {
                    if (filter.ToString() == filterName)
                    {
                        using (MemoryStream tempStream = new MemoryStream())
                        {
                            filter.InputFilter(bodySteram, tempStream); // Convert input as a filter does and write it to output stream

                            bodySteram.Close(); // Release input stream resources
                            bodySteram = new MemoryStream();

                            tempStream.Position = 0;
                            PrivateUtility.CopyStream(tempStream, bodySteram);

                            tempStream.Close(); // Release input stream resources
                        }
                        filterFound = true;
                        break;
                    }
                }

                if (!filterFound)
                {
                    throw new InvalidDataException();
                }
            }

            try
            {
                bodySteram.Position = 0;
                returnVal = (PasswordFileBody)formatter.Deserialize(bodySteram);
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                bodySteram.Close();
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

            MemoryStream bodySteram = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(bodySteram, passwordData);

            foreach (string filterName in this.BodyFiltered.Filters)
            {
                foreach (IOFilterBase filter in this.AvailableFilters)
                {
                    if (filter.ToString() == filterName)
                    {
                        using (MemoryStream tempStream = new MemoryStream())
                        {
                            filter.OutputFilter(bodySteram, tempStream); // Convert input as a filter does and write it to output stream

                            bodySteram.Close(); // Release input stream resources
                            bodySteram = new MemoryStream();

                            tempStream.Position = 0;
                            PrivateUtility.CopyStream(tempStream, bodySteram);

                            tempStream.Close(); // Release input stream resources
                        }
                    }
                }
            }

            // Construct header
            this.Header.Token = DateTime.Now.ToString(CultureInfo.InvariantCulture).ToCharArray();
            this.Header.CombinedMasterPasswordHash = PrivateUtility.GetHashCombined(masterPasswordHash, PrivateUtility.GetHash(this.Header.Token));

            // Construct filter informatoin
            foreach (IOFilterBase filter in this.AvailableFilters)
            {
                this.BodyFiltered.Filters.Add(filter.ToString());
            }
            this.BodyFiltered.data = bodySteram.ToArray();

            using (FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // Write header
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(this.Header.Token); // Write token
                    writer.Write(this.Header.CombinedMasterPasswordHash); // Write hash
                    // Write Filtered data
                    formatter.Serialize(fs, this.BodyFiltered);
                }
            }
        }

        /// <summary>
        /// Initialize password data file. Plese be carefull that this method also clear data in this.FileContents member variable.
        /// </summary>
        public virtual void ResetPasswordFile(byte[] masterPasswordHash)
        {
            this.WritePasswordToFile(masterPasswordHash, new PasswordFileBody());
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
            return CompareHashes(expectedCombinedHash, PrivateUtility.GetHashCombined(challengingMasterPasswordHash, PrivateUtility.GetHash(token)));
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
        public byte[] CombinedMasterPasswordHash = new byte[InternalApplicationConfig.Hash.HashSize/8];
    }

    /// <summary>
    /// Password file body which is filtered
    /// </summary>
    [Serializable]
    public class PasswordFileBodyFiltered
    {
        public List<string> Filters = new List<string>();
        public byte[] data;
    }

    /// <summary>
    /// Raw(unfiltered) contents of password file
    /// </summary>
    [Serializable]
    public class PasswordFileBody
    {
        public PasswordIndexer Indexer = new PasswordIndexer();
        public List<PasswordContainer> Containers = new List<PasswordContainer>();
        public List<PasswordRecord> Records = new List<PasswordRecord>();
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
    public class NoFilter : IOFilterBase
    {
        public override void InputFilter(MemoryStream src, MemoryStream dest)
        {
            src.Position = 0;
            dest.Position = 0;
            PrivateUtility.CopyStream(src, dest);
        }

        public override void OutputFilter(MemoryStream src, MemoryStream dest)
        {
            src.Position = 0;
            dest.Position = 0;
            PrivateUtility.CopyStream(src, dest);
        }
    }
}
