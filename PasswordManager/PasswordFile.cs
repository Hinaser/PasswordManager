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
        protected List<IOFilterBase> AvailableFilters = new List<IOFilterBase>();
        protected List<string> FilterOrder = new List<string>();
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
        public virtual PasswordFileBody ReadPasswordFromFile(byte[] masterPasswordHash)
        {
            if (!File.Exists(this.Filepath))
            {
                this.ResetPasswordFile(masterPasswordHash);
            }

            BinaryFormatter formatter = new BinaryFormatter(); ;
            MemoryStream bodySteram;
            PasswordFileLayout header;
            PasswordFileBody returnVal;

            using (FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read))
            {
                header = (PasswordFileLayout)formatter.Deserialize(fs);
            }

            bodySteram = new MemoryStream(header.Data);

            foreach (string filterName in header.FilterOrder)
            {
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
                    }
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

            foreach (string filterName in this.FilterOrder)
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
            PasswordFileLayout header = new PasswordFileLayout();
            header.TimeToken = DateTime.Now;
            header.HashedHashOfMasterPassword = PrivateUtility.GetHashCombined(PrivateUtility.GetHash(masterPasswordHash), PrivateUtility.GetHash(header.TimeToken));
            foreach (IOFilterBase filter in this.AvailableFilters)
            {
                header.FilterOrder.Add(filter.ToString());
            }
            header.Data = bodySteram.ToArray();

            using (FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                formatter.Serialize(fs, header);
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
        #endregion
    }

    /// <summary>
    /// Header data and filtered body data for password file
    /// </summary>
    [Serializable]
    public class PasswordFileLayout
    {
        public DateTime TimeToken = DateTime.UtcNow;
        public byte[] HashedHashOfMasterPassword;
        public List<string> FilterOrder = new List<string>();

        public byte[] Data; // This should be filtered PasswordFileBody
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
