using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsSettings
    {

        public List<string> GetAllLocalDatabases()
        {
            return clsDataAccess.GetDataBases();
        }

        public List<string> GetAllTables(string DatabaseName)
        {
            return clsDataAccess.GetTables(DatabaseName);
        }


    }
}
