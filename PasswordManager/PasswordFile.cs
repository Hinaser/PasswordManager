﻿#region Notice
/*
 * Copyright 2015 Hinaser
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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
    /// Password check algorithm for lazy implementation
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="masterPasswordhash"></param>
    /// <returns></returns>
    public delegate bool PasswordHashChecker(string filePath, byte[] masterPasswordhash);

    /// <summary>
    /// A class which handles input/output PasswordObject to actual file.
    /// </summary>
    public class PasswordFile
    {
        #region Field
        protected string Filepath = LocalConfig.DefaultPasswordFilePath;
        List<string> FilterOrder = new List<string>();
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
            foreach (string filterName in order)
            {
                if (!IOFilterFactory.Instance.ContainsIOFilter(filterName))
                {
                    throw new ArgumentException();
                }
            }

            this.FilterOrder = order;
        }

        /// <summary>
        /// Add a filter hash to tail of the list field..
        /// </summary>
        /// <param name="filterHash"></param>
        public void AddFilterOrder(string filterName)
        {
            if (!IOFilterFactory.Instance.ContainsIOFilter(filterName))
            {
                throw new ArgumentException();
            }

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
        /// Check whether password hash is valid for specified password file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool ChallengeHashedMasterPassword(string filePath, byte[] challengingPasswordHash)
        {
            // When password file does not exist, throw an exception.
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            // Parse file header
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    PasswordHeader header = new PasswordHeader();
                    header.Token = reader.ReadBytes(LocalConfig.HeaderTokenSize);
                    header.CombinedMasterPasswordHash = reader.ReadBytes(LocalConfig.MasterPasswordHashedKeySize);

                    return CompareHashes(header.CombinedMasterPasswordHash, Utility.Scramble(challengingPasswordHash, header.Token, LocalConfig.MasterPasswordHashedKeySize));
                }
            }
        }

        /// <summary>
        /// Read password file with removing data filter. Available filters must be added to instance before this method is called.
        /// </summary>
        /// <exception cref="FileNotFoundException">Password file does not exist</exception>
        /// <exception cref="InvalidMasterPasswordException">Master password is invalid</exception>
        public virtual PasswordFileBody ReadPasswordFromFile(byte[] masterPasswordHash, PasswordFileBody defaultPasswordFileBody = null)
        {
            if (masterPasswordHash.Length != LocalConfig.MasterPasswordHashedKeySize)
            {
                throw new ArgumentException();
            }

            // When password file does not exist, throw an exception.
            if (!File.Exists(this.Filepath))
            {
                throw new FileNotFoundException(this.Filepath);
            }

            FilteredData filteredData;

            // Parse file header
            using (FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    PasswordHeader header = new PasswordHeader();
                    header.Token = reader.ReadBytes(LocalConfig.HeaderTokenSize);
                    header.CombinedMasterPasswordHash = reader.ReadBytes(LocalConfig.MasterPasswordHashedKeySize);

                    // Check masterPasswordHash is valid
                    if (!CheckMasterPasswordHash(header.CombinedMasterPasswordHash, masterPasswordHash, header.Token))
                    {
                        throw new InvalidMasterPasswordException();
                    }

                    MemoryStream encryptedData = new MemoryStream();
                    BinaryFormatter formatter = new BinaryFormatter();

                    encryptedData = Utility.GetMemoryStream(reader);
                    encryptedData.Position = 0;
                    // Do decryption against loaded MemoryStream
                    byte[] decryptedBytes = Utility.Decrypt(encryptedData.ToArray(), masterPasswordHash, header.Token);
                    MemoryStream decryptedData = new MemoryStream(decryptedBytes);
                    decryptedData.Position = 0;

                    // Get filtered data
                    filteredData = (FilteredData)formatter.Deserialize(decryptedData);
                }
            }

            // Parse filter data
            return (PasswordFileBody)IOFilterProcessor.RemoveFilter(filteredData);
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

            MemoryStream decryptedData = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            // Apply data filters
            FilteredData filteredData = IOFilterProcessor.ApplyFilter(passwordData, this.FilterOrder);

            // Convert FilteredData object into MemoryStream
            formatter.Serialize(decryptedData, filteredData);
            decryptedData.Position = 0;

            // Get token
            byte[] token = Utility.Get16BytesHash(DateTime.Now);

            // Do encryption here
            byte[] encryptedBytes = Utility.Encrypt(decryptedData.ToArray(), masterPasswordHash, token);

            // Construct header
            PasswordHeader header = new PasswordHeader();
            header.Token = token;
            header.CombinedMasterPasswordHash = Utility.Scramble(masterPasswordHash, token, LocalConfig.MasterPasswordHashedKeySize);

            // Write filtered data to the file
            using (FileStream fs = new FileStream(this.Filepath, FileMode.Create, FileAccess.Write))
            {
                // Write header
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(header.Token); // Write token
                    writer.Write(header.CombinedMasterPasswordHash); // Write hash
                    // Write Filtered data which has been encrypted
                    writer.Write(encryptedBytes);
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
        public static bool CheckMasterPasswordHash(byte[] expectedCombinedHash, byte[] challengingMasterPasswordHash, byte[] token)
        {
            return CompareHashes(expectedCombinedHash, Utility.Scramble(challengingMasterPasswordHash, token, LocalConfig.MasterPasswordHashedKeySize));
        }

        /// <summary>
        /// Compare two argument hashes one by one.
        /// </summary>
        /// <param name="b1">A hash value to compare</param>
        /// <param name="b2">A hash value to compare</param>
        /// <returns></returns>
        public static bool CompareHashes(byte[] b1, byte[] b2)
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
        public byte[] Token = new byte[LocalConfig.HeaderTokenSize];
        public byte[] CombinedMasterPasswordHash = new byte[LocalConfig.MasterPasswordHashedKeySize];
    }

    /// <summary>
    /// Data which is filtered
    /// </summary>
    [Serializable]
    public class FilteredData
    {
        public char[] Filter = new char[LocalConfig.FilterNameFixedLength];
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

        /// <summary>
        /// Get hash value representing instance data
        /// </summary>
        /// <returns></returns>
        public int GetRepresentingHash()
        {
            return this.GetRepresentingHashIndexer()
                ^ this.GetRepresentingHashContainers()
                ^ this.GetRepresentingHashRecords();
        }

        /// <summary>
        /// Get indexer hash
        /// </summary>
        /// <returns></returns>
        public int GetRepresentingHashIndexer()
        {
            return this.Indexer.GetRepresentingHash();
        }

        /// <summary>
        /// Get container collection hash
        /// </summary>
        /// <returns></returns>
        public int GetRepresentingHashContainers()
        {
            int containersHash = 0;

            foreach (var c in this.Containers)
            {
                containersHash ^= c.GetRepresentingHash();
            }

            return containersHash;
        }

        /// <summary>
        /// Get record collection hash
        /// </summary>
        /// <returns></returns>
        public int GetRepresentingHashRecords()
        {
            int recordsHash = 0;

            foreach (var r in this.Records)
            {
                recordsHash ^= r.GetRepresentingHash();
            }

            return recordsHash;
        }
    }
}
