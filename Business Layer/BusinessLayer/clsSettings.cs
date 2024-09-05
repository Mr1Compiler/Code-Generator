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
        public static byte ?NameNumberInColumns = 0;
        public List<string> GetAllLocalDatabases()
        {
            return clsDataAccess.GetDataBases();
        }

        public List<string> GetAllTables(string DatabaseName)
        {
            return clsDataAccess.GetTables(DatabaseName);
        }

       public static List<string> GetALLColumns(List<clsColumnInfo> Items)
        {
            

            List<string> Columns = new List<string>();

            foreach (var item in Items)
            {
                Columns.Add(item.Name);
            }

            NameNumberInColumns = GetNameNumberInColumns(Columns);

            return Columns;
        }

        static public int GetNameNumber()
        {
            return Convert.ToInt16(NameNumberInColumns);
        }
        public static byte? GetNameNumberInColumns(List<string> Columns)
        {
            byte Number = 0;


            foreach (string Column in Columns)
            {
                if(Column.ToLower().Contains("first"))
                {
                    return Number;
                }

                if(Column.ToLower().Contains("name"))
                {
                    return Number;
                }

                Number++;
            }

            return null;
        }
    }
}
