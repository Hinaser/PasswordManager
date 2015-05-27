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
        protected PasswordFileLayout FileContents = new PasswordFileLayout();
        #endregion

        #region Constructor
        private PasswordFile(){} // Making default constructor private in order to gurantee that PasswordFile object is always with the filename.
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
        /// Read raw password data from file without transformation. This method can be overridden by inherited class.
        /// Please override this method if you want to change the way for handling password data file. For example, read encrypted data with decryption.
        /// </summary>
        public virtual void ReadPasswordFromFile()
        {
            if (!File.Exists(this.Filepath))
            {
                this.ResetPasswordFile();
            }

            FileStream fs = new FileStream(this.Filepath, FileMode.Open, FileAccess.Read);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                this.FileContents = (PasswordFileLayout)formatter.Deserialize(fs);
            }
            catch(SerializationException e)
            {
                throw e;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Write row password data to file without transformation. This method can be overridden by inherited class.
        /// Please override this method if you want to change the way for handling password data file. For example, write raw data with converting encrypted data.
        /// </summary>
        public virtual void WritePasswordToFile()
        {
            if (!PasswordFile.CheckDirectoryWritable(Directory.GetParent(this.Filepath).FullName))
            {
                throw new IOException();
            }

            FileStream fs = new FileStream(this.Filepath, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(fs, this.FileContents);
            }
            catch(SerializationException e)
            {
                throw e;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Initialize password data file. Plese be carefull that this method also clear data in this.FileContents member variable.
        /// </summary>
        public virtual void ResetPasswordFile()
        {
            this.FileContents = new PasswordFileLayout();
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
    /// File layout for password objects
    /// </summary>
    [Serializable]
    public class PasswordFileLayout
    {
        public PasswordIndexer Indexer = new PasswordIndexer();
        public List<PasswordContainer> Containers = new List<PasswordContainer>();
        public List<PasswordRecord> Records = new List<PasswordRecord>();
    }
}
