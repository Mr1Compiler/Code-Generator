using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessLayer
{

    public class clsTemplets
    {

        public static string PrintColumnsDefintion(List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
";
            string DataType = "";
            string Name = "";

            foreach (var column in ColumnsInfo)
            {
                DataType = clsColumnInfo.MapSqlTypeToCSharpType(column.DataType);
                Name = column.Name;

                str += $@"
public {DataType} {Name} {{set; get;}}";
            }

            return str;
        }
        public static string ColumnTemplete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string Templete = $@"
public class cls{TableName} 
{{
public enum enMode {{AddNew = 0,Update = 1}};
public enMode Mode = enMode.AddNew;
}}";

            // string Definition = PrintColumnsDefintion(ColumnsInfo);

            Templete += PrintColumnsDefintion(ColumnsInfo);

            return Templete;
        }

        public static string PublicConstractersTemplete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {

            string publicConstractor = Environment.NewLine + $@"
public cls{TableName}()
{{";


            foreach (var column in ColumnsInfo)
            {
                publicConstractor += $@"
this.{column.Name} = {clsColumnInfo.AssginValues(column.DataType)};";
            }

            publicConstractor += $@"
Mode = enMode.AddNew;
}}";

            return publicConstractor;
        }

        public static string PrivateConstractorTempelete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string PrivateConstractor = $@"

priavte cls{TableName}(";


            foreach (var column in ColumnsInfo)
            {
                if(column != ColumnsInfo[0])
                {
                    PrivateConstractor += $@" {clsColumnInfo.MapSqlTypeToCSharpType(column.DataType)} {column.Name.ToLower()},";
                }
                else
                {
                    PrivateConstractor += $@"{clsColumnInfo.MapSqlTypeToCSharpType(column.DataType)} {column.Name.ToLower()},";

                }
            }

            PrivateConstractor = PrivateConstractor.Remove(PrivateConstractor.Length - 1);
           
            PrivateConstractor += $@")
{{";

            foreach (var column in ColumnsInfo)
            {
                PrivateConstractor += $@"
this.{column.Name} = {column.Name.ToLower()};";
            }

            PrivateConstractor += $@"
Mode = enMode.Update;
}}
";

                return PrivateConstractor;
        }


        public static string PrivateAddNew(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
cls _AddNew{TableName}()
{{
    this.{ColumnsInfo[0].Name} = cls{TableName}DataAccess.AddNew{TableName}(";

            foreach (var column in ColumnsInfo)
            {
                str += $@" this.{column.Name},";
            }
            str = str.Remove(str.Length - 1);

            str += $@");
    return (this.{ColumnsInfo[0].Name} != -1)
}}
";





                return str;
        }
    }


    public class clsGenerateCode
    {
        public static string GenerateBusinessLayer(string DatabaseName, string TableName)
        {
            string BusinessLayer = "";
            List<clsColumnInfo> Columns = clsDataAccess.GetColumnsInfo(DatabaseName, TableName);

            BusinessLayer += clsTemplets.ColumnTemplete(TableName, Columns);

            BusinessLayer += clsTemplets.PublicConstractersTemplete(TableName, Columns);

            BusinessLayer += clsTemplets.PrivateConstractorTempelete(TableName, Columns);

            BusinessLayer += clsTemplets.PrivateAddNew(TableName, Columns);
            
            return BusinessLayer;
        }
    }
}
