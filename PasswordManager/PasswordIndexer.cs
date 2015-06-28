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
        /// <summary>
        ///  The 1st int represents parent container id and the 2nd List<int> represents list of combined password records
        /// </summary>
        private Dictionary<int, List<int>> RecordIndexes = new Dictionary<int, List<int>>();
        /// <summary>
        /// The 1st int represents parent container id and the 2nd List<int> represents list of combined containers
        /// </summary>
        private Dictionary<int, List<int>> ContainerIndexes = new Dictionary<int, List<int>>();
        /// <summary>
        /// The 1st int represents child record id and the 2nd int represents parent container id
        /// </summary>
        private Dictionary<int, int> RecordReverseIndexes = new Dictionary<int, int>();
        /// <summary>
        /// The 1st int represents child container id and the 2nd int represents parent container id
        /// </summary>
        private Dictionary<int, int> ContainerReverseIndexes = new Dictionary<int, int>();

        public static readonly int RootContainerID = LocalConfig.RootContainerID;
        #endregion

        #region Constructor
        /// <summary>
        /// Construct root container
        /// </summary>
        public PasswordIndexer()
        {
            this.RecordIndexes.Add(LocalConfig.RootContainerID, new List<int>());
            this.ContainerIndexes.Add(LocalConfig.RootContainerID, new List<int>());
        }
        #endregion

        #region Hash algorithm
        /// <summary>
        /// Get hash value representing whole index data
        /// </summary>
        /// <returns></returns>
        public override int GetRepresentingHash()
        {
            int recordsHash = this.GetHash(this.RecordIndexes);
            int containerHash = this.GetHash(this.ContainerIndexes);

            return recordsHash ^ containerHash;
        }

        /// <summary>
        /// Calculate hash from Dictionary<int, List<int>> object
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private int GetHash(Dictionary<int, List<int>> arg)
        {
            int ret = Int32.MaxValue;

            if (arg == null)
            {
                return ret;
            }

            foreach (KeyValuePair<int, List<int>> kvp in arg)
            {
                ret = ret ^ kvp.Key * 3;
                foreach (int list in kvp.Value)
                {
                    ret = ret ^ list * 2;
                }
            }

            return ret;
        }
        #endregion

        #region Setter method
        /// <summary>
        /// Append container to target container. If appending container already exists in target container, it does nothing but returns false.
        /// </summary>
        /// <param name="destContainerID">Destination container id</param>
        /// <param name="containerID">New container id on appending</param>
        /// <returns>The result of process. If false, it means no appending was performed. If true, appending was successful.</returns>
        public override bool AppendContainer(int containerID, int destContainerID)
        {
            if (!this.ContainerIndexes.ContainsKey(destContainerID))
            {
                this.ContainerIndexes.Add(destContainerID, new List<int>() { containerID });
            }
            else
            {
                if (this.ContainerIndexes[destContainerID].Contains(containerID))
                {
                    return false;
                }
                this.ContainerIndexes[destContainerID].Add(containerID);
            }

            this.ContainerReverseIndexes.Remove(containerID);
            this.ContainerReverseIndexes.Add(containerID, destContainerID);

            return true;
        }

        /// <summary>
        /// Cut loose specified container from parent container. If releaing container does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing container</param>
        /// <remarks>This function does not affect sub container of target container to be released</remarks>
        /// <returns>True if it was successfully removed, and false if not.</returns>
        public override bool ReleaseContainer(int containerID, int parentContainerID)
        {
            // Check dest container exists
            if (!this.ContainerIndexes.ContainsKey(parentContainerID))
            {
                return false;
            }

            return this.ContainerIndexes[parentContainerID].Remove(containerID) && this.ContainerReverseIndexes.Remove(containerID);
        }

        /// <summary>
        /// Cut loose specified container from parent container without specifying parent container id.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <remarks>This function does not affect sub container of target container to be released</remarks>
        /// <returns>True if it successfully removed, and false if it did not.</returns>
        public override bool ReleaseContainer(int containerID)
        {
            int parentContainerID;

            try
            {
                parentContainerID = this.GetParentContainerOfContainer(containerID);
            }
            catch
            {
                return false;
            }

            return this.ReleaseContainer(containerID, parentContainerID);
        }

        /// <summary>
        /// Delete specified container and its sub containers/records from parent container in indexer object.
        /// If removing container does not exist in the parent container, it does nothing but returns null.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing container</param>
        /// <remarks>This function removes all child password records/containers in removing container</remarks>
        /// <returns>List of removed containers/records. It returns null if removing was not done successfully</returns>
        public override RemovedObjects RemoveAllContainers(int containerID, int parentContainerID)
        {
            // Check dest container exists
            if (!this.ContainerIndexes.ContainsKey(parentContainerID))
            {
                return null;
            }

            RemovedObjects removedObj = new RemovedObjects();

            // Remove all child password records
            ICollection<int> removingRecordCollection = this.GetChildRecords(containerID);
            if (removingRecordCollection != null)
            {
                // Since removingRecordCollection is a "Reference Type", values within the variable will be deleted at the same time original value is removed
                // Here copies actual value to new variable, which is independent to original reference
                int[] removingRecords = new int[removingRecordCollection.Count];
                for (int i = 0; i < removingRecordCollection.Count; i++)
                {
                    removingRecords[i] = removingRecordCollection.ElementAt(i);
                }

                // Remove records here
                for (int i = 0; i < removingRecords.Length; i++)
                {
                    this.RemoveRecord(removingRecords[i]);
                    removedObj.Removed[typeof(PasswordRecord)].Add(removingRecords[i]);
                }
            }

            // Remove all child containers
            ICollection<int> removingContainerCollection = this.GetChildContainers(containerID);
            if (removingContainerCollection != null)
            {
                // Since removingContainerCollection is a "Reference Type", values within the variable will be deleted at the same time original value is removed
                // Here copies actual value to new variable, which is independent to original reference
                int[] removingContainers = new int[removingContainerCollection.Count];
                for (int i = 0; i < removingContainerCollection.Count; i++)
                {
                    removingContainers[i] = removingContainerCollection.ElementAt(i);
                }

                for (int i = 0; i < removingContainers.Length; i++)
                {
                    RemovedObjects removedChildren = this.RemoveAllContainers(removingContainers[i], containerID); // Recursive call
                    removedObj.Removed[typeof(PasswordContainer)].Add(removingContainers[i]);

                    // Add up removed sub container ids
                    foreach (int removedContainerID in removedChildren.Removed[typeof(PasswordContainer)])
                    {
                        removedObj.Removed[typeof(PasswordContainer)].Add(removedContainerID);
                    }

                    // Add up removed record ids in sub containers
                    foreach (int removedRecordID in removedChildren.Removed[typeof(PasswordRecord)])
                    {
                        removedObj.Removed[typeof(PasswordRecord)].Add(removedRecordID);
                    }
                }
            }

            this.ContainerIndexes[parentContainerID].Remove(containerID);
            this.ContainerReverseIndexes.Remove(containerID);

            return removedObj;
        }

        /// <summary>
        /// Delete specified container and its sub containers/records from parent container in indexer object without specifying parent container id.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <returns>List of removed containers/records. It returns null if removing was not done successfully</returns>
        public override RemovedObjects RemoveAllContainers(int containerID)
        {
            int parentContainerID;

            try
            {
                parentContainerID = this.GetParentContainerOfContainer(containerID);
            }
            catch
            {
                return null;
            }

            return this.RemoveAllContainers(containerID, parentContainerID);
        }

        /// <summary>
        /// Move container from source parent container to destination parent container
        /// </summary>
        /// <param name="containerID">Container id to be moved</param>
        /// <param name="srcContainerID">Original parent container id</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <returns>True if it was successfully moved, and false if not.</returns>
        public override bool MoveContainer(int containerID, int srcContainerID, int destContainerID)
        {
            return this.ReleaseContainer(containerID, srcContainerID) && this.AppendContainer(containerID, destContainerID);
        }

        /// <summary>
        /// Move container from current parent container to another container without specifying current parent container ID.
        /// </summary>
        /// <param name="containerID">Container id to be moved</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <remarks>This method can be executed even if caller do not know whichi is the source parent container</remarks>
        /// <returns>True if it was successfully moved, and false if not.</returns>
        public override bool MoveContainer(int containerID, int destContainerID)
        {
            int srcContainerID;

            try
            {
                srcContainerID = this.GetParentContainerOfContainer(containerID);
            }
            catch
            {
                return false;
            }

            return this.MoveContainer(containerID, srcContainerID, destContainerID);
        }

        /// <summary>
        /// Append record to target container. If the appending record already exists in target container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID">Record id to be appended</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <returns>True if it was successfully appended, and false if not.</returns>
        public override bool AppendRecord(int recordID, int destContainerID)
        {
            if (!this.RecordIndexes.ContainsKey(destContainerID))
            {
                this.RecordIndexes.Add(destContainerID, new List<int>() { recordID });
            }
            else
            {
                if (this.RecordIndexes[destContainerID].Contains(recordID))
                {
                    return false;
                }
                this.RecordIndexes[destContainerID].Add(recordID);
            }

            this.RecordReverseIndexes.Remove(recordID);
            this.RecordReverseIndexes.Add(recordID, destContainerID);

            return true;
        }

        /// <summary>
        /// Remove specified record from parent container. If removing record does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID">Record id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing record</param>
        /// <returns>True if it was removed removed, and false if not.</returns>
        public override bool RemoveRecord(int recordID, int parentContainerID)
        {
            // Check dest container exists
            if (!this.RecordIndexes.ContainsKey(parentContainerID))
            {
                return false;
            }

            return this.RecordIndexes[parentContainerID].Remove(recordID) && this.RecordReverseIndexes.Remove(recordID);
        }

        /// <summary>
        /// Remove specified record from parent container without specifying parent container id.
        /// </summary>
        /// <param name="recordID">Record id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <returns>True if it was removed appended, and false if not.</returns>
        public override bool RemoveRecord(int recordID)
        {
            int parentContainerID;

            try
            {
                parentContainerID = this.GetParentContainerOfRecord(recordID);
            }
            catch
            {
                return false;
            }

            return this.RemoveRecord(recordID, parentContainerID);
        }

        /// <summary>
        /// Move record from current parent container to another container.
        /// </summary>
        /// <param name="recordID">Record id to be moved</param>
        /// <param name="srcContainerID">Original parent container which has moving record</param>
        /// <param name="destContainerID">Destination target container</param>
        /// <returns>True if it was moved appended, and false if not.</returns>
        public override bool MoveRecord(int recordID, int srcContainerID, int destContainerID)
        {
            return this.RemoveRecord(recordID, srcContainerID) && this.AppendRecord(recordID, destContainerID);
        }

        /// <summary>
        /// Move record from current parent container to another container without specifying current parent container ID.
        /// </summary>
        /// <param name="recordID">Record id to be moved</param>
        /// <param name="destContainerID">Destination target container</param>
        /// <remarks>This method can be executed even if caller do not know whichi is the source parent container</remarks>
        /// <returns>True if it was moved appended, and false if not.</returns>
        public override bool MoveRecord(int recordID, int destContainerID)
        {
            int srcContainerID;

            try
            {
                srcContainerID = this.GetParentContainerOfRecord(recordID);
            }
            catch
            {
                return false;
            }

            return this.MoveRecord(recordID, srcContainerID, destContainerID);
        }
        #endregion

        #region Getter method
        /// <summary>
        /// Get associated child containers of specified container. If no child container is found, then it returns null.
        /// </summary>
        /// <param name="containerID">Container id to be inspected</param>
        /// <returns>Collection of child container ids. Null if no children were found</returns>
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
        /// <param name="containerID">Container id to be inspected</param>
        /// <returns>Collection of child record ids. Null if no children were found</returns>
        public override ICollection<int> GetChildRecords(int containerID)
        {
            if (!this.RecordIndexes.ContainsKey(containerID))
            {
                return null;
            }

            return this.RecordIndexes[containerID];
        }

        /// <summary>
        /// Get parent container id for child container. This method might throw exception.
        /// </summary>
        /// <exception cref="KeyNotFoundException">When unknown child container ID is passed to this method, it will throw this exception</exception>
        /// <param name="childContainerID">Container id which you want to examine its parent contaier</param>
        /// <remarks>Since returning type is not reference type but value type, it is not evitable to throw exception when parent container is not found</remarks>
        /// <returns>Parent container id</returns>
        public override int GetParentContainerOfContainer(int childContainerID)
        {
            return this.ContainerReverseIndexes[childContainerID];
        }

        /// <summary>
        /// Get parent container id for child record. This method might throw exception.
        /// </summary>
        /// <exception cref="KeyNotFoundException">When unknown child record ID is passed to this method, it will throw this exception</exception>
        /// <param name="childRecordID">Record id which you want to examine its parent container</param>
        /// <remarks>Since returning type is not reference type but value type, it is not evitable to throw exception when parent container is not found</remarks>
        /// <returns>Parent container id</returns>
        public override int GetParentContainerOfRecord(int childRecordID)
        {
            return this.RecordReverseIndexes[childRecordID];
        }


        /// <summary>
        /// Get container object by container ID
        /// </summary>
        /// <param name="containers">Collection of containers which seems to have target container</param>
        /// <param name="containerID">Target container id</param>
        /// <returns>Container object associated with specified container id</returns>
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
        /// Get record object by record ID
        /// </summary>
        /// <param name="containers">Collection of containers which seems to have target container</param>
        /// <param name="recordID">Target record id</param>
        /// <returns>Record object associated with specified record id</returns>
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

        #region Utility
        /// <summary>
        /// Get unique container ID.
        /// </summary>
        /// <remarks> Unique key would be the maximum number of curret IDs added by 1.</remarks>
        /// <returns>Unique container id which is not overlapped between existing container ids indexer object knows</returns>
        public override int GetUniqueContainerID()
        {
            if (this.ContainerReverseIndexes.Count < 1)
            {
                return PasswordIndexer.RootContainerID + 1;
            }

            return this.ContainerReverseIndexes.Keys.Max() + 1;
        }

        /// <summary>
        /// Get unique record ID.
        /// </summary>
        /// <remarks>Unique key would be the maximum number of curret IDs added by 1.</remarks>
        /// <returns>Unique container id which is not overlapped between existing record ids indexer object knows</returns>
        public override int GetUniqueRecordID()
        {
            if (this.RecordReverseIndexes.Count < 1)
            {
                return 0;
            }

            return this.RecordReverseIndexes.Keys.Max() + 1;
        }
        #endregion
    }
}
