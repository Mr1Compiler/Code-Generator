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
        public static string GenerateBusinessLayer(string DatabaseName, string TableName)
        {
            string BusinessLayer = "";
            List<clsColumnInfo> Columns = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);

            clsBusinessTemplets.Names(TableName);

            BusinessLayer += clsBusinessTemplets.ColumnTemplete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PublicConstractersTemplete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateConstractorTempelete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateAddNewMethod(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.PrivateUpdateMethod(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.Find(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.Save(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.GetAll(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.Delete(TableName, Columns);
            BusinessLayer += clsBusinessTemplets.isExist(TableName, Columns);

            return BusinessLayer;
        }
    }
}
