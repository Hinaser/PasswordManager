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
    [Serializable]
    public class PasswordRecord
    {
        #region Member field
        /// <summary>
        /// Header values, which can be seen without master password
        /// </summary>
        private int RecordID; // This represent real record identity. This must be unique all over the Password data
        private string Caption = String.Empty;
        private DateTime CreateDate;
        private DateTime LastUpdateDate;

        /// <summary>
        /// Private values, which cannot be seen without master password
        /// </summary>
        private string ID = String.Empty;
        private string Password = String.Empty;
        private string Descriptioin = String.Empty;
        // Will be implemented in future update
        //private byte[] UnlockToken = null; // This value is required to get actual secret value from this class. If invalid value is passed when secret values are refered, dummy value will be returned.
        #endregion

        #region Constructor
        public PasswordRecord() { }
        public PasswordRecord(int recordID) { this.SetRecordID(recordID); }
        public PasswordRecord(int recordID, string caption, DateTime createDate, DateTime lastUpdateDate, string id, string password, string description)
        {
            this.SetRecordID(recordID);
            this.Caption = caption;
            this.CreateDate = createDate;
            this.LastUpdateDate = lastUpdateDate;
            this.ID = id;
            this.Password = password;
            this.Descriptioin = description;
        }
        #endregion

        #region Setter method
        /// <summary>
        /// Set record ID to instance field
        /// </summary>
        /// <param name="recordID"></param>
        public void SetRecordID(int recordID)
        {
            this.RecordID = recordID;
        }

        /// <summary>
        /// Set caption to instance field
        /// </summary>
        /// <param name="caption"></param>
        public void SetCaption(string caption)
        {
            this.Caption = caption;
        }

        /// <summary>
        /// Set header values to instance
        /// </summary>
        /// <param name="parentID">Parent container id</param>
        /// <param name="caption">Caption</param>
        /// <param name="createDate">Date the record was created</param>
        /// <param name="lastUpdateDate">Date the record was updated most recently</param>
        public void SetHeaderData(string caption, DateTime createDate, DateTime lastUpdateDate)
        {
            this.Caption = caption;
            this.CreateDate = createDate;
            this.LastUpdateDate = lastUpdateDate;
        }

        /// <summary>
        /// Set private values to instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <param name="description"></param>
        /// <param name="complexity"></param>
        public void SetPrivateData(string id, string password, string description)
        {
            this.ID = id;
            this.Password = password;
            this.Descriptioin = description;
        }
        #endregion

        #region Getter method
        // Public data
        public int GetRecordID() { return this.RecordID; }
        public string GetCaption() { return this.Caption; }
        public DateTime GetCreateDate() { return this.CreateDate; }
        public DateTime GetLastUpdateDate() { return this.LastUpdateDate; }

        // Private data
        public string GetID()
        {
            return this.ID;
        }

        public string GetPassword()
        {
            return this.Password;
        }

        public string GetDescription()
        {
            return this.Descriptioin;
        }
        #endregion

        #region Override method
        /// <summary>
        /// Return string of RealID, which is considered to be unique value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.RecordID.ToString();
        }
        #endregion

        #region Utility method
        #endregion
    }
}
