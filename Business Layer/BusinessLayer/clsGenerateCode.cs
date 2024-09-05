using CodeGenerator;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessLayer
{
    public class clsGenerateCode
    {
        public static string FillBusinessLayer(string DatabaseName, string TableName)
        {
            string BusinessLayer = $@"using System;
using System.Data;
using {TableName}BusinessLayer;

namespace {TableName}BusinessLayer
{{
";
            List<clsColumnInfo> Columns = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);
            clsBusinessTemplets.Names(TableName);

            BusinessLayer += clsBusinessTemplets.ColumnTemplete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PublicConstractersTemplete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateConstractorTempelete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateAddNewMethod(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateUpdateMethod(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.Find(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.Save(TableName);
            BusinessLayer += clsBusinessTemplets.GetAll(TableName);
            BusinessLayer += clsBusinessTemplets.Delete(TableName, Columns[0]);
            BusinessLayer += clsBusinessTemplets.isExist(TableName, Columns[0]);

            BusinessLayer += $@"
}}
}}
";

            return BusinessLayer;
        }
        public static string FillDataAccessLayer(string DatabaseName, string TableName)
        {
            string DataAccessLayer = $@"using System;
using System.Data;
using System.Data.SqlClient;

namespace {TableName}DataAccessLayer
{{
public class cls{TableName} 
{{
";
            List<clsColumnInfo> Columns = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);

            clsDataAccessTemplets.Load(TableName, Columns);

            DataAccessLayer += clsDataAccessTemplets.GetInfoByID();

            DataAccessLayer += clsDataAccessTemplets.AddNew();

            DataAccessLayer += clsDataAccessTemplets.Update();

            DataAccessLayer += clsDataAccessTemplets.GetAll();

            DataAccessLayer += clsDataAccessTemplets.Delete();

            DataAccessLayer += clsDataAccessTemplets.IsExist();

            DataAccessLayer += $@"
}}

}}";


            return DataAccessLayer;
        }
    }
}
