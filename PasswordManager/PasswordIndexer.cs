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
#endregion

namespace PasswordManager
{
    /// <summary>
    /// Defining relations between containers and records
    /// </summary>
    [Serializable]
    public class PasswordIndexer : PasswordIndexerBase
    {
        #region Field
        // The 1st int represents parent container id and the 2nd List<int> represents list of combined password records
        private Dictionary<int, List<int>> RecordIndexes = new Dictionary<int, List<int>>();
        // The 1st int represents parent container id and the 2nd List<int> represents list of combined containers
        private Dictionary<int, List<int>> ContainerIndexes = new Dictionary<int, List<int>>();

        public static readonly int RootContainerID = InternalApplicationConfig.RootContainerID;
        #endregion

        #region Constructor
        /// <summary>
        /// Construct root container
        /// </summary>
        public PasswordIndexer()
        {
            this.RecordIndexes.Add(InternalApplicationConfig.RootContainerID, new List<int>());
            this.ContainerIndexes.Add(InternalApplicationConfig.RootContainerID, new List<int>());
        }
        #endregion

        #region Setter method
        /// <summary>
        /// Append container to target container. If appending container already exists in target container, it does not actuall append container and returns false.
        /// </summary>
        /// <param name="destContainerID"></param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public override bool AppendContainer(int containerID, int destContainerID)
        {
            if (!this.ContainerIndexes.ContainsKey(destContainerID))
            {
                this.ContainerIndexes.Add(destContainerID, new List<int>() { containerID });
            }
            else
            {
                this.ContainerIndexes[destContainerID].Add(containerID);
            }

            return true;
        }

        /// <summary>
        /// Remove specified container from parent container. If removing container does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="containerID"></param>
        /// <param name="parentContainerID"></param>
        /// <returns></returns>
        public override bool RemoveContainer(int containerID, int parentContainerID)
        {
            // Check dest container exists
            if (!this.ContainerIndexes.ContainsKey(parentContainerID))
            {
                return false;
            }

            // Check removing container does exist
            if (!this.ContainerIndexes[parentContainerID].Contains(containerID))
            {
                return false;
            }

            return this.ContainerIndexes[parentContainerID].Remove(containerID);
        }

        /// <summary>
        /// Move container from src parent container to dest parent container
        /// </summary>
        /// <param name="containerID"></param>
        /// <param name="srcContainerID"></param>
        /// <param name="destContainerID"></param>
        /// <returns></returns>
        public override bool MoveContainer(int containerID, int srcContainerID, int destContainerID)
        {
            return this.RemoveContainer(containerID, srcContainerID) && this.AppendContainer(containerID, destContainerID);
        }

        /// <summary>
        /// Append record to target container. If appending record already exists in target container, it does not actuall append record and returns false.
        /// </summary>
        /// <param name="destContainerID"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public override bool AppendRecord(int recordID, int destContainerID)
        {
            if (!this.RecordIndexes.ContainsKey(destContainerID))
            {
                this.RecordIndexes.Add(destContainerID, new List<int>() { recordID });
            }
            else
            {
                this.RecordIndexes[destContainerID].Add(recordID);
            }

            return true;
        }

        /// <summary>
        /// Remove specified record from parent container. If removing record does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="parentContainerID"></param>
        /// <returns></returns>
        public override bool RemoveRecord(int recordID, int parentContainerID)
        {
            // Check dest container exists
            if (!this.RecordIndexes.ContainsKey(parentContainerID))
            {
                return false;
            }

            // Check removing container does exist
            if (!this.RecordIndexes[parentContainerID].Contains(recordID))
            {
                return false;
            }

            return this.RecordIndexes[parentContainerID].Remove(recordID);
        }

        /// <summary>
        /// Move container from src parent container to dest parent container
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="srcContainerID"></param>
        /// <param name="destContainerID"></param>
        /// <returns></returns>
        public override bool MoveRecord(int recordID, int srcContainerID, int destContainerID)
        {
            return this.RemoveRecord(recordID, srcContainerID) && this.AppendRecord(recordID, destContainerID);
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get associated child containers of specified container. If no child container is found, then it returns null.
        /// </summary>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public override ICollection<int> GetChildContainers(int containerID)
        {
            if (!this.ContainerIndexes.ContainsKey(containerID))
            {
                return null;
            }

            return this.ContainerIndexes[containerID];
        }

        /// <summary>
        /// Get associated child records of specified container. If no child record is found, then it returns null.
        /// </summary>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public override ICollection<int> GetChildRecords(int containerID)
        {
            if (!this.RecordIndexes.ContainsKey(containerID))
            {
                return null;
            }

            return this.RecordIndexes[containerID];
        }

        /// <summary>
        /// Get container object with the specified by container ID
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public override PasswordContainer GetContainerByID(ICollection<PasswordContainer> containers, int containerID)
        {
            // Pick up child container by its ID. This is a liner search which average search performance would be O(n). 
            // In future data structure will be update to get better performance.
            foreach (PasswordContainer c in containers)
            {
                if (c.GetContainerID() == containerID)
                {
                    return c;
                }
            }

            return null;
        }

        /// <summary>
        /// Get record object with the specified by record ID
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public override PasswordRecord GetRecordByID(ICollection<PasswordRecord> records, int recordID)
        {
            // Pick up child container by its ID. This is a liner search which average search performance would be O(n). 
            // In future data structure will be update to get better performance.
            foreach (PasswordRecord c in records)
            {
                if (c.GetRecordID() == recordID)
                {
                    return c;
                }
            }

            return null;
        }
        #endregion

        #region Integrity check method
        //public bool CheckRecordIndex
        #endregion
    }
}
