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
            string BusinessLayer = "";
            List<clsColumnInfo> Columns = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);

            clsSettings.GetALLColumns(Columns);
            clsBusinessTemplets.Load(TableName, Columns);

            BusinessLayer += clsBusinessTemplets.ColumnTemplete();
            BusinessLayer += clsBusinessTemplets.PublicConstractersTemplete();
            BusinessLayer += clsBusinessTemplets.PrivateConstractorTempelete();
            BusinessLayer += clsBusinessTemplets.PrivateAddNewMethod();
            BusinessLayer += clsBusinessTemplets.PrivateUpdateMethod();
            BusinessLayer += clsBusinessTemplets.Save();
            BusinessLayer += clsBusinessTemplets.GetAll();
            BusinessLayer += clsBusinessTemplets.Delete();
            BusinessLayer += clsBusinessTemplets.isExist();
            BusinessLayer += clsBusinessTemplets.Find();

            BusinessLayer += $@"
}}
";

            return BusinessLayer;
        }
        public static string FillDataAccessLayer(string DatabaseName, string TableName)
        {
            string DataAccessLayer = $@"public class cls{TableName}DataAccess
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
            DataAccessLayer += clsDataAccessTemplets.GeneralFind();
            DataAccessLayer += $@"

}}";


            return DataAccessLayer;
        }
    }
}
