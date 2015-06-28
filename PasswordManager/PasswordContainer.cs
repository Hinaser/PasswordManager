#region Notice
/*
 * Copyright 2015 Yunoske
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
                for (int i = 0; i < this.Label.Length; i++)
                {
                    p[0] = '\0';
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
            int textInt = this.GetHash(this.Label == null ? String.Empty : this.Label);

            byte[] idBytes = BitConverter.GetBytes(this.ContainerID);
            int idInt = 0;

            if (idBytes.Length > 3) { idInt = (idBytes[3] << 24) + (idBytes[2] << 16) + (idBytes[1] << 8) + idBytes[0]; }
            else if (idBytes.Length == 3) { idInt = (idBytes[2] << 16) + (idBytes[1] << 8) + idBytes[0]; }
            else if (idBytes.Length == 2) { idInt = (idBytes[1] << 8) + idBytes[0]; }
            else if (idBytes.Length == 1) { idInt = idBytes[0]; }
            else { idInt = 0; }

            return textInt*3 ^ idInt*2;
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
            byte[] byteSet = new byte[4] { 255, 255, 255, byteString[0] }; // byteString[0] has value because string length is not less than 1.

            for (int i = 0; i < (byteString.Length - 1) / 4 + 1; i++)
            {
                if (i == (byteString.Length - 1) / 4)
                {
                    mod = (byteString.Length - 1) % 4;
                }

                for (int j = 0; j < mod; j++)
                {
                    byteSet[j] = (byte)(byteSet[(i % 4)]*2 ^ byteString[i * 4 + j]);
                }
            }

            return (byteSet[3] << 24) + (byteSet[2] << 16) + (byteSet[1] << 8) + byteSet[0];
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
