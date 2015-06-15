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
        protected PasswordFileBodyFiltered BodyFiltered = new PasswordFileBodyFiltered(); // This field is kept to maintain original filterOrder information of password file.
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
            MemoryStream bodySteram;
            PasswordFileBody returnVal;

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
                        throw new InvalidMasterPasswordException();
                    }

                    reader.BaseStream.Position = InternalApplicationConfig.HeaderTokenSize + InternalApplicationConfig.Hash.HashSize / InternalApplicationConfig.BitsPerAByte;
                    MemoryStream encryptedFilterData = Utility.ReadBytes(reader);
                    // Do decryption against loaded MemoryStream
                    // something ...

                    encryptedFilterData.Position = 0;
                    this.BodyFiltered = (PasswordFileBodyFiltered)formatter.Deserialize(encryptedFilterData);
                }
            }

            bodySteram = new MemoryStream(this.BodyFiltered.data);

            // Reverse filter list order
            this.BodyFiltered.Filters.Reverse();
            foreach (string filterName in this.BodyFiltered.Filters)
            {
                bool filterFound = false;

                if (IOFilterFactory.Instance.ContainsIOFilter(filterName))
                {
                    IOFilterBase filter = IOFilterFactory.Instance.GetIOFilter(filterName);

                    using (MemoryStream tempStream = new MemoryStream())
                    {
                        filter.InputFilter(bodySteram, tempStream); // Convert input as a filter does and write it to output stream

                        bodySteram.Close(); // Release input stream resources
                        bodySteram = new MemoryStream();

                        tempStream.Position = 0;
                        Utility.CopyStream(tempStream, bodySteram);

                        tempStream.Close(); // Release input stream resources
                    }
                    filterFound = true;
                    break;
                }

                if (!filterFound)
                {
                    throw new NoCorrespondingFilterFoundException();
                }
            }
            // Set back filter order
            this.BodyFiltered.Filters.Reverse();

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

            List<string> appliedFilters = new List<string>();
            foreach (string filterName in this.BodyFiltered.Filters)
            {
                if (IOFilterFactory.Instance.ContainsIOFilter(filterName))
                {
                    IOFilterBase filter = IOFilterFactory.Instance.GetIOFilter(filterName);

                    if (filter.ToString() == filterName)
                    {
                        using (MemoryStream tempStream = new MemoryStream())
                        {
                            filter.OutputFilter(bodySteram, tempStream); // Convert input as a filter does and write it to output stream
                            appliedFilters.Add(filterName);

                            bodySteram.Close(); // Release input stream resources
                            bodySteram = new MemoryStream();

                            tempStream.Position = 0;
                            Utility.CopyStream(tempStream, bodySteram);

                            tempStream.Close(); // Release input stream resources
                        }
                    }
                }
            }
            this.BodyFiltered.Filters = appliedFilters;

            // Construct header
            PasswordHeader header = new PasswordHeader();
            header.Token = DateTime.Now.ToString(CultureInfo.InvariantCulture).ToCharArray();
            header.CombinedMasterPasswordHash = Utility.GetHashCombined(masterPasswordHash, Utility.GetHash(header.Token));

            // Write filtered data to the file
            this.BodyFiltered.data = bodySteram.ToArray();
            using (FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // Write header
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(header.Token); // Write token
                    writer.Write(header.CombinedMasterPasswordHash); // Write hash
                    // Write Filtered data
                    formatter.Serialize(fs, this.BodyFiltered);
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
