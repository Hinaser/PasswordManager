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
#endregion

namespace PasswordManager
{
    /// <summary>
    /// Minimal unit for PasswordContainer. This looks like each folder for password records.
    /// </summary>
    [Serializable]
    public class PasswordContainer : IEquatable<PasswordContainer>, IDisposable
    {
        #region Field
        private int ContainerID = 0;
        private string Label = String.Empty;
        #endregion

        #region Constructor
        private PasswordContainer() { } // Making default constructor private in order to gurantee all PasswordContainer class exists with its UnitID.
        public PasswordContainer(int containerID) { this.ContainerID = containerID; }
        public PasswordContainer(int containerID, string label) { this.ContainerID = containerID; this.Label = label; }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Validate equality by their container id
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public bool Equals(PasswordContainer con)
        {
            return con != null && this.ContainerID == con.GetContainerID();
        }

        /// <summary>
        /// Sanitize sensitive password information
        /// </summary>
        unsafe public void Dispose()
        {
            fixed (char* p = this.Label)
            {
                ;
            }
        }
        #endregion

        #region Setter method
        /// <summary>
        /// Set label text to instance member field
        /// </summary>
        /// <param name="label"></param>
        public void SetLabel(string label)
        {
            if (String.IsNullOrEmpty(label))
            {
                return;
            }

            this.Label = label;
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get UnitID of instance
        /// </summary>
        /// <returns></returns>
        public int GetContainerID()
        {
            return this.ContainerID;
        }

        /// <summary>
        /// Get Label text of instance
        /// </summary>
        /// <returns></returns>
        public string GetLabel()
        {
            return String.IsNullOrEmpty(this.Label) ? String.Empty : this.Label;
        }
        #endregion
    }
}
