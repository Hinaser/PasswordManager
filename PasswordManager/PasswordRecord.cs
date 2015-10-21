#region Notice
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
#endregion

namespace PasswordManager
{
    /// <summary>
    /// This represents one password item.
    /// </summary>
    [Serializable]
    public class PasswordRecord : IEquatable<PasswordRecord>, IDisposable
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
        private string Description = String.Empty;
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
            this.Description = description;
        }
        #endregion

        #region Virtual methods
        /// <summary>
        /// Check equality by their recordIDs
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public bool Equals(PasswordRecord rec)
        {
            return rec != null && this.RecordID == rec.GetRecordID();
        }

        /// <summary>
        /// Sanitize sensitive password information
        /// </summary>
        unsafe public void Dispose()
        {
            // Sanitize Record id
            this.RecordID = -1;

            // Sanitize Caption
            fixed (char* p = this.Caption)
            {
                for (int i = 0; i < this.Caption.Length; i++)
                {
                    p[i] = '\0';
                }
            }

            // Sanitize ID
            fixed (char* p = this.ID)
            {
                for (int i = 0; i < this.ID.Length; i++)
                {
                    p[i] = '\0';
                }
            }

            // Sanitize Password
            fixed (char* p = this.Password)
            {
                for (int i = 0; i < this.Password.Length; i++)
                {
                    p[i] = '\0';
                }
            }

            // Sanitize Description
            fixed (char* p = this.Description)
            {
                for (int i = 0; i < this.Description.Length; i++)
                {
                    p[i] = '\0';
                }
            }
        }
        #endregion

        #region Hash algorithm for holding data
        /// <summary>
        /// Get hash value representing instance data
        /// </summary>
        /// <returns></returns>
        public int GetRepresentingHash()
        {
            int recordIDHash = this.RecordID;
            int captionHash = this.GetHash(this.Caption);
            int cdateHash = this.GetHash(this.CreateDate);
            int udateHash = this.GetHash(this.LastUpdateDate);
            int idHash = this.GetHash(this.ID);
            int passwordHash = this.GetHash(this.Password);
            int descHash = this.GetHash(this.Description);

            return recordIDHash ^ captionHash * 2 ^ cdateHash * 3 ^ udateHash * 4 ^ idHash * 5 ^ passwordHash * 6 ^ descHash * 7;
        }

        /// <summary>
        /// Get hashed int value from string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int GetHash(string s)
        {
            if (String.IsNullOrEmpty(s) || s.Length < 1)
            {
                return 0;
            }

            byte[] byteString = Encoding.UTF8.GetBytes(s);

            int mod = 4;
            byte[] byteSet = new byte[4] { 255, 255, 255, 255 }; // byteString[0] has value because string length is not less than 1.

            for (int i = 0; i < (byteString.Length - 1) / 4 + 1; i++)
            {
                if (i == (byteString.Length - 1) / 4)
                {
                    mod = (byteString.Length - 1) % 4;
                }

                for (int j = 0; j < mod; j++)
                {
                    byteSet[j] = (byte)(byteSet[(i % 4)] * 2 ^ byteString[i * 4 + j]);
                }
            }

            return (byteSet[3] << 24) + (byteSet[2] << 16) + (byteSet[1] << 8) + byteSet[0];
        }

        /// <summary>
        /// Get hashed int value from DateTime
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private int GetHash(DateTime d)
        {
            if (d == null)
            {
                return this.GetHash(String.Empty);
            }

            return this.GetHash(d.Ticks.ToString());
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
            this.Description = description;
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
            return this.Description;
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
