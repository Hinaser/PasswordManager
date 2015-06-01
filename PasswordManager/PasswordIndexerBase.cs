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
    [Serializable]
    public abstract class PasswordIndexerBase
    {
        public abstract bool AppendContainer(int containerID, int destContainerID);
        public abstract bool RemoveContainer(int containerID);
        public abstract bool RemoveContainer(int containerID, int parentContainerID);
        public abstract bool MoveContainer(int containerID, int srcContainerID, int destContainerID);
        public abstract bool MoveContainer(int containerID, int destContainerID);
        public abstract bool AppendRecord(int recordID, int destContainerID);
        public abstract bool RemoveRecord(int recordID, int parentContainerID);
        public abstract bool RemoveRecord(int recordID);
        public abstract bool MoveRecord(int recordID, int srcContainerID, int destContainerID);
        public abstract bool MoveRecord(int recordID, int destContainerID);
        public abstract ICollection<int> GetChildContainers(int containerID);
        public abstract ICollection<int> GetChildRecords(int containerID);
        public abstract int GetParentContainerOfContainer(int childContainerID);
        public abstract int GetParentContainerOfRecord(int childRecordID);
        public abstract PasswordContainer GetContainerByID(ICollection<PasswordContainer> containers, int containerID);
        public abstract PasswordRecord GetRecordByID(ICollection<PasswordRecord> records, int recordID);
    }
}
