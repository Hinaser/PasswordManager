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
        protected PasswordFileBody FileContents = new PasswordFileBody();
        protected List<IOFilterBase> Filters = new List<IOFilterBase>();
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
        /// Read raw password data from file without transformation.
        /// </summary>
        public virtual void ReadPasswordFromFile()
        {
            if (!File.Exists(this.Filepath))
            {
                this.ResetPasswordFile();
            }

            MemoryStream istream;

            using (FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                istream = (MemoryStream)formatter.Deserialize(fs);
            }

            foreach (IOFilterBase f in this.Filters)
            {
                using (MemoryStream ostream = new MemoryStream())
                {
                    f.InputFilter(istream, ostream); // Convert input as a filter does and write it to output stream

                    istream.Close(); // Release input stream resources
                    istream = new MemoryStream();

                    PrivateUtility.CopyStream(ostream, istream);

                    ostream.Close(); // Release input stream resources
                }
            }

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                this.FileContents = (PasswordFileBody)formatter.Deserialize(istream);
            }
            catch { throw; }
            finally
            {
                istream.Close();
            }
        }

        /// <summary>
        /// Write row password data to file without transformation.
        /// </summary>
        public virtual void WritePasswordToFile()
        {
            if (!PasswordFile.CheckDirectoryWritable(Directory.GetParent(this.Filepath).FullName))
            {
                throw new IOException();
            }

            MemoryStream istream = new MemoryStream();

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(istream, this.FileContents);

            foreach (IOFilterBase f in this.Filters)
            {
                using (MemoryStream ostream = new MemoryStream())
                {
                    f.OutputFilter(istream, ostream); // Convert input as a filter does and write it to output stream

                    istream.Close(); // Release input stream resources
                    istream = new MemoryStream();

                    PrivateUtility.CopyStream(ostream, istream);

                    ostream.Close(); // Release input stream resources
                }
            }

            using (FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                formatter.Serialize(fs, istream);
            }
        }

        /// <summary>
        /// Initialize password data file. Plese be carefull that this method also clear data in this.FileContents member variable.
        /// </summary>
        public virtual void ResetPasswordFile()
        {
            this.FileContents = new PasswordFileBody();
            this.WritePasswordToFile();
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
    /// Header data for password file
    /// </summary>
    [Serializable]
    public class PasswordFileHeader
    {
        public DateTime LastUpdate = DateTime.UtcNow;
        public string HashMasterPassword = String.Empty;
        public List<string> FilterInformation = new List<string>();
    }

    /// <summary>
    /// Main contents of password file
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
        public override void InputFilter(MemoryStream inStream, MemoryStream outSteram)
        {
            PrivateUtility.CopyStream(inStream, outSteram);
        }

        public override void OutputFilter(MemoryStream inStream, MemoryStream outSteram)
        {
            PrivateUtility.CopyStream(inStream, outSteram);
        }
    }
}
