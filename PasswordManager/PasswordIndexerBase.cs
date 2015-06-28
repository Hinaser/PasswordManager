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
    [Serializable]
    public abstract class PasswordIndexerBase
    {
        /// <summary>
        /// Get hash value calculated from instance itself. It should be an unique representing value of instance.
        /// </summary>
        /// <returns></returns>
        public abstract int GetRepresentingHash();

        /// <summary>
        /// This might store Password container/record object indexes deleted by indexer method
        /// </summary>
        public class RemovedObjects
        {
            public Dictionary<Type, List<int>> Removed = new Dictionary<Type, List<int>>();

            public RemovedObjects()
            {
                this.Removed.Add(typeof(PasswordContainer), new List<int>());
                this.Removed.Add(typeof(PasswordRecord), new List<int>());
            }
        }

        /// <summary>
        /// Append container to target container. If appending container already exists in target container, it does nothing but returns false.
        /// </summary>
        /// <param name="destContainerID">Destination container id</param>
        /// <param name="containerID">New container id on appending</param>
        /// <returns>The result of process. If false, it means no appending was performed. If true, appending was successful.</returns>
        public abstract bool AppendContainer(int containerID, int destContainerID);

        /// <summary>
        /// Cut loose specified container from parent container. If releaing container does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing container</param>
        /// <remarks>This function does not affect sub container of target container to be released</remarks>
        /// <returns>True if it was successfully removed, and false if not.</returns>
        public abstract bool ReleaseContainer(int containerID, int parentContainerID);

        /// <summary>
        /// Cut loose specified container from parent container without specifying parent container id.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <remarks>This function does not affect sub container of target container to be released</remarks>
        /// <returns>True if it successfully removed, and false if it did not.</returns>
        public abstract bool ReleaseContainer(int containerID);

        /// <summary>
        /// Delete specified container and its sub containers/records from parent container in indexer object.
        /// If removing container does not exist in the parent container, it does nothing but returns null.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing container</param>
        /// <remarks>This function removes all child password records/containers in removing container</remarks>
        /// <returns>List of removed containers/records. It returns null if removing was not done successfully</returns>
        public abstract RemovedObjects RemoveAllContainers(int containerID, int parentContainerID);

        /// <summary>
        /// Delete specified container and its sub containers/records from parent container in indexer object without specifying parent container id.
        /// </summary>
        /// <param name="containerID">Container id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <returns>List of removed containers/records. It returns null if removing was not done successfully</returns>
        public abstract RemovedObjects RemoveAllContainers(int containerID);

        /// <summary>
        /// Move container from source parent container to destination parent container
        /// </summary>
        /// <param name="containerID">Container id to be moved</param>
        /// <param name="srcContainerID">Original parent container id</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <returns>True if it was successfully moved, and false if not.</returns>
        public abstract bool MoveContainer(int containerID, int srcContainerID, int destContainerID);

        /// <summary>
        /// Move container from current parent container to another container without specifying current parent container ID.
        /// </summary>
        /// <param name="containerID">Container id to be moved</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <remarks>This method can be executed even if caller do not know whichi is the source parent container</remarks>
        /// <returns>True if it was successfully moved, and false if not.</returns>
        public abstract bool MoveContainer(int containerID, int destContainerID);

        /// <summary>
        /// Append record to target container. If the appending record already exists in target container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID">Record id to be appended</param>
        /// <param name="destContainerID">Destination target container id</param>
        /// <returns>True if it was successfully appended, and false if not.</returns>
        public abstract bool AppendRecord(int recordID, int destContainerID);

        /// <summary>
        /// Remove specified record from parent container. If removing record does not exist in the parent container, it does nothing but returns false.
        /// </summary>
        /// <param name="recordID">Record id to be removed</param>
        /// <param name="parentContainerID">Parent container id which has removing record</param>
        /// <returns>True if it was removed removed, and false if not.</returns>
        public abstract bool RemoveRecord(int recordID, int parentContainerID);

        /// <summary>
        /// Remove specified record from parent container without specifying parent container id.
        /// </summary>
        /// <param name="recordID">Record id to be removed</param>
        /// <remarks>This method can be executed even if caller do not know which is the source parent container</remarks>
        /// <returns>True if it was removed appended, and false if not.</returns>
        public abstract bool RemoveRecord(int recordID);

        /// <summary>
        /// Move record from current parent container to another container.
        /// </summary>
        /// <param name="recordID">Record id to be moved</param>
        /// <param name="srcContainerID">Original parent container which has moving record</param>
        /// <param name="destContainerID">Destination target container</param>
        /// <returns>True if it was moved appended, and false if not.</returns>
        public abstract bool MoveRecord(int recordID, int srcContainerID, int destContainerID);

        /// <summary>
        /// Move record from current parent container to another container without specifying current parent container ID.
        /// </summary>
        /// <param name="recordID">Record id to be moved</param>
        /// <param name="destContainerID">Destination target container</param>
        /// <remarks>This method can be executed even if caller do not know whichi is the source parent container</remarks>
        /// <returns>True if it was moved appended, and false if not.</returns>
        public abstract bool MoveRecord(int recordID, int destContainerID);

        /// <summary>
        /// Get associated child containers of specified container. If no child container is found, then it returns null.
        /// </summary>
        /// <param name="containerID">Container id to be inspected</param>
        /// <returns>Collection of child container ids. Null if no children were found</returns>
        public abstract ICollection<int> GetChildContainers(int containerID);

        /// <summary>
        /// Get associated child records of specified container. If no child record is found, then it returns null.
        /// </summary>
        /// <param name="containerID">Container id to be inspected</param>
        /// <returns>Collection of child record ids. Null if no children were found</returns>
        public abstract ICollection<int> GetChildRecords(int containerID);

        /// <summary>
        /// Get parent container id for child container. This method might throw exception.
        /// </summary>
        /// <exception cref="KeyNotFoundException">When unknown child container ID is passed to this method, it will throw this exception</exception>
        /// <param name="childContainerID">Container id which you want to examine its parent contaier</param>
        /// <remarks>Since returning type is not reference type but value type, it is not evitable to throw exception when parent container is not found</remarks>
        /// <returns>Parent container id</returns>
        public abstract int GetParentContainerOfContainer(int childContainerID);

        /// <summary>
        /// Get parent container id for child record. This method might throw exception.
        /// </summary>
        /// <exception cref="KeyNotFoundException">When unknown child record ID is passed to this method, it will throw this exception</exception>
        /// <param name="childRecordID">Record id which you want to examine its parent container</param>
        /// <remarks>Since returning type is not reference type but value type, it is not evitable to throw exception when parent container is not found</remarks>
        /// <returns>Parent container id</returns>
        public abstract int GetParentContainerOfRecord(int childRecordID);

        /// <summary>
        /// Get container object by container ID
        /// </summary>
        /// <param name="containers">Collection of containers which seems to have target container</param>
        /// <param name="containerID">Target container id</param>
        /// <returns>Container object associated with specified container id</returns>
        public abstract PasswordContainer GetContainerByID(ICollection<PasswordContainer> containers, int containerID);

        /// <summary>
        /// Get record object by record ID
        /// </summary>
        /// <param name="containers">Collection of containers which seems to have target container</param>
        /// <param name="recordID">Target record id</param>
        /// <returns>Record object associated with specified record id</returns>
        public abstract PasswordRecord GetRecordByID(ICollection<PasswordRecord> records, int recordID);

        /// <summary>
        /// Get unique container ID.
        /// </summary>
        /// <returns>Unique container id which is not overlapped between existing container ids indexer object knows</returns>
        public abstract int GetUniqueContainerID();

        /// <summary>
        /// Get unique record ID.
        /// </summary>
        /// <returns>Unique container id which is not overlapped between existing record ids indexer object knows</returns>
        public abstract int GetUniqueRecordID();
    }
}
