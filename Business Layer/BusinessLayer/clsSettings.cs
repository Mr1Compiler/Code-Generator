using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

       public List<string> GetALLColumns(string DatabaseName, string TableName)
        {
            
            var Items = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);

            List<string> Columns = new List<string>();

            foreach (var item in Items)
            {
                Columns.Add(item.Name);
            }

            return Columns;
        }
    }
}
