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
            clsBusinessLayerGenerator.Load(TableName, Columns);

            BusinessLayer += clsBusinessLayerGenerator.ColumnTemplete();
            BusinessLayer += clsBusinessLayerGenerator.PublicConstractersTemplete();
            BusinessLayer += clsBusinessLayerGenerator.PrivateConstractorTempelete();
            BusinessLayer += clsBusinessLayerGenerator.PrivateAddNewMethod();
            BusinessLayer += clsBusinessLayerGenerator.PrivateUpdateMethod();
            BusinessLayer += clsBusinessLayerGenerator.Save();
            BusinessLayer += clsBusinessLayerGenerator.GetAll();
            BusinessLayer += clsBusinessLayerGenerator.Delete();
            BusinessLayer += clsBusinessLayerGenerator.isExist();
            BusinessLayer += clsBusinessLayerGenerator.Find();

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

            clsDataAccessGenerator.Load(TableName, Columns);

            DataAccessLayer += clsDataAccessGenerator.AddNew();
            DataAccessLayer += clsDataAccessGenerator.Update();
            DataAccessLayer += clsDataAccessGenerator.GetAll();
            DataAccessLayer += clsDataAccessGenerator.Delete();
            DataAccessLayer += clsDataAccessGenerator.IsExist();
            DataAccessLayer += clsDataAccessGenerator.GeneralFind();
            DataAccessLayer += $@"

}}";


            return DataAccessLayer;
        }
    }
}
