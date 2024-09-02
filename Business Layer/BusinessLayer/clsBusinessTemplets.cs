using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace CodeGenerator
{
    public class clsBusinessTemplets
    {
        public static string AddNew = "";
        public static string DataAccessLayerName = "";
        public static string ClassName = "";

        public static void Names(string TableName)
        {
            AddNew = $"_AddNew{TableName}";
            DataAccessLayerName = $"cls{TableName}DataAccess";
            ClassName = $"cls{TableName}";
        }


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
                if (column != ColumnsInfo[0])
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


        public static string PrivateAddNewMethod(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
private bool _AddNew{TableName}()
{{
    this.{ColumnsInfo[0].Name} = cls{TableName}DataAccess.AddNew{TableName}(";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" this.{ColumnsInfo[i].Name},";
            }

            //foreach (var column in ColumnsInfo)
            //{
            //    str += $@" this.{column.Name},";
            //}
            str = str.Remove(str.Length - 1);

            str += $@");
    return (this.{ColumnsInfo[0].Name} != -1)
}}
";

            return str;
        }

        public static string PrivateUpdateMethod(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
private bool _Update{TableName}()
{{
    return {DataAccessLayerName}.Update{TableName}(";

            foreach (var column in ColumnsInfo)
            {
                str += $@" this.{column.Name},";
            }
            str = str.Remove(str.Length - 1);

            str += $@");
}}
";

            return str;
        }

        public static string Find(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
public static cls{TableName} Find({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name})
{{
";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@"{clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name}={clsColumnInfo.AssginValues(clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType))}; ";
            }

            str += $@"
if({DataAccessLayerName}.Get{TableName}InfoByID({ColumnsInfo[0].Name},";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" ref {ColumnsInfo[i].Name},";
            }

            str = str.Remove(str.Length - 1);

            str += $@"))

    return new {ClassName}(";

            foreach (var column in ColumnsInfo)
            {
                str += $@" {column.Name},";
            }

            str = str.Remove(str.Length - 1);

            str += $@");

else
    return null;
}}
";


            return str;
        }

        public static string Save(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
public bool Save()
        {{
            

            switch  (Mode)
            {{
                case enMode.AddNew:
                    if (_AddNewContact())
                    {{

                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}

                case enMode.Update:

                    return _UpdateContact();

            }}
        }}
";
            return str;
        }

        public static string GetAll(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
public static DataTable GetAll{TableName}()
{{
    return {DataAccessLayerName}.GetAll{TableName}()
}}
";

            return str;
        }

        public static string Delete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {

            string str = $@"
 public static bool Delete{TableName.Remove(TableName.Length - 1)}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name});
        {{
           return  {DataAccessLayerName}.Delete{TableName.Remove(TableName.Length - 1)}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)});
        }}";

            return str;
        }

        public static string isExist(string TableName, List<clsColumnInfo> ColumnsInfo)
        {

            string str = $@"
 public static bool is{TableName.Remove(TableName.Length - 1)}Exist({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name});
        {{
           return  {DataAccessLayerName}.Is{TableName.Remove(TableName.Length - 1)}Exist({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)});
        }}";

            return str;
        }
    }
}
