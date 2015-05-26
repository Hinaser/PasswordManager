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
    /// This represents one password item.
    /// </summary>
    internal class PasswordRecord
    {
        #region Member field
        /// <summary>
        /// Header values, which can be seen without master password
        /// </summary>
        private int RealID; // This represent real record identity. This must be unique all over Password data
        private int ParentID = InternalApplicationConfig.RootContainerID; // This indicates Parent record container ID.
        private string Caption = String.Empty;
        private DateTime CreateDate;
        private DateTime LastUpdateDate;

        /// <summary>
        /// Private values, which cannot be seen without master password
        /// </summary>
        private string ID = String.Empty;
        private string Password = String.Empty;
        private string Descriptioin = String.Empty;
        private int Complexity;
        // Will be implemented in future update
        // private byte[] UnlockToken = null; // This value is required to get actual secret value from this class. If invalid value is passed when secret values are refered, dummy value will be returned.
        #endregion

        #region Constructor
        private PasswordRecord() { } // Disable non-argument constructor.
        public PasswordRecord(int realID) { this.RealID = realID; } // Instance must be with Real ID
        #endregion

        #region Setter method
        /// <summary>
        /// Set header values
        /// </summary>
        /// <param name="parentID">Parent container id</param>
        /// <param name="caption">Caption</param>
        /// <param name="createDate">Date the record was created</param>
        /// <param name="lastUpdateDate">Date the record was updated most recently</param>
        public void SetHeaderData(int parentID, string caption, DateTime createDate, DateTime lastUpdateDate)
        {
            this.ParentID = parentID;
            this.Caption = caption;
            this.CreateDate = createDate;
            this.LastUpdateDate = lastUpdateDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="description"></param>
        /// <param name="complexity"></param>
        public void SetPrivateData(string id, string password, string description, int complexity)
        {
            this.ID = id;
            this.Password = password;
            this.Descriptioin = description;
            this.Complexity = complexity;
        }
        #endregion

        #region Getter method
        #endregion

        #region Override method
        public override string ToString()
        {
            return this.RealID.ToString();
        }
        #endregion
    }
}
