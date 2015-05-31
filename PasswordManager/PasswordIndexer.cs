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
    public class PasswordIndexer
    {
        #region Field
        // The 1st int represents parent container id and the 2nd List<int> represents list of combined password records
        private Dictionary<int, List<int>> RecordIndexes = new Dictionary<int, List<int>>();
        // The 1st int represents parent container id and the 2nd List<int> represents list of combined containers
        private Dictionary<int, List<int>> ContainerIndexes = new Dictionary<int, List<int>>();

        public const int RootContainerID = 0;
        #endregion

        #region Constructor
        public PasswordIndexer() { }
        #endregion

        #region Setter method
        /// <summary>
        /// Append container to target container. If appending container already exists in target container, it does not actuall append container and returns false.
        /// </summary>
        /// <param name="destContainerID"></param>
        /// <param name="containerID"></param>
        /// <returns></returns>
        public bool AppendContainer(int containerID, int destContainerID)
        {
            // Check dest container exists
            if (!this.ContainerIndexes.ContainsKey(destContainerID))
            {
                return false;
            }

            // Check appending container is already registered
            if (this.ContainerIndexes[destContainerID].Contains(containerID))
            {
                return false;
            }

            this.ContainerIndexes[destContainerID].Add(containerID);
            return true;
        }

        /// <summary>
        /// Remove specified container from parent container. If removing container does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="containerID"></param>
        /// <param name="parentContainerID"></param>
        /// <returns></returns>
        public bool RemoveContainer(int containerID, int parentContainerID)
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
        public bool MoveContainer(int containerID, int srcContainerID, int destContainerID)
        {
            return this.RemoveContainer(containerID, srcContainerID) && this.AppendContainer(containerID, destContainerID);
        }

        /// <summary>
        /// Append record to target container. If appending record already exists in target container, it does not actuall append record and returns false.
        /// </summary>
        /// <param name="destContainerID"></param>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public bool AppendRecord(int recordID, int destContainerID)
        {
            // Check dest container exists
            if (!this.RecordIndexes.ContainsKey(destContainerID))
            {
                return false;
            }

            // Check appending record is already registered
            if (this.RecordIndexes[destContainerID].Contains(recordID))
            {
                return false;
            }

            this.RecordIndexes[destContainerID].Add(recordID);
            return true;
        }

        /// <summary>
        /// Remove specified record from parent container. If removing record does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="parentContainerID"></param>
        /// <returns></returns>
        public bool RemoveRecord(int recordID, int parentContainerID)
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
        public bool MoveRecord(int recordID, int srcContainerID, int destContainerID)
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
        public List<int> GetChildContainers(int containerID)
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
        public List<int> GetChildRecords(int containerID)
        {
            if (!this.RecordIndexes.ContainsKey(containerID))
            {
                return null;
            }

            return this.RecordIndexes[containerID];
        }
        #endregion
    }
}
