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
        #endregion

        #region Constructor
        #endregion
    }
}
