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
    internal class PasswordContainer
    {
        #region Field
        private int UnitID = 0;
        private string Label = String.Empty;
        private List<PasswordRecord> Passwords = new List<PasswordRecord>();
        #endregion

        #region Constructor
        private PasswordContainer() { } // Making default constructor private in order to gurantee all PasswordContainer class exists with its UnitID.
        public PasswordContainer(int unitID) { this.UnitID = unitID; }
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
        public int GetUnitID()
        {
            return this.UnitID;
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

        #region Password records method
        /// <summary>
        /// Add PasswordRecord instance to instance member List object.
        /// </summary>
        /// <param name="p"></param>
        public void AddPasswordRecord(PasswordRecord p)
        {
            this.Passwords.Add(p);
        }

        /// <summary>
        /// Get PasswordRecord instance by index value.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PasswordRecord GetPasswordRecord(int index)
        {
            return this.Passwords[index];
        }

        /// <summary>
        /// Remove PasswordRecode instance from internal List object by index value.
        /// </summary>
        /// <param name="index"></param>
        public void DeletePasswordRecord(int index)
        {
            this.Passwords.RemoveAt(index);
        }

        /// <summary>
        /// Get count of PasswordRecord instaces stored in internal List object.
        /// </summary>
        /// <returns></returns>
        public int GetPasswordCount()
        {
            return this.Passwords.Count;
        }
        #endregion
    }
}
