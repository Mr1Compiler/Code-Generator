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
        public static string TableName = "";
        public static string TableNameSingle = "";

        public static void Names(string tableName)
        {
            AddNew = $"_AddNew{tableName}";
            DataAccessLayerName = $"cls{tableName}DataAccess";
            ClassName = $"cls{tableName}";

            TableName = tableName;

            TableNameSingle = tableName.Remove(tableName.Length - 1);


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
";

            // string Definition = PrintColumnsDefintion(ColumnsInfo);

            Templete += PrintColumnsDefintion(ColumnsInfo);

            return Templete;
        }

        public static string GetDataType(string DataType)
        {
            return clsColumnInfo.MapSqlTypeToCSharpType(DataType);
        }

        public static string PublicConstractersTemplete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {

            string publicConstractor = Environment.NewLine + $@"
public cls{TableName}()
{{";


            foreach (var column in ColumnsInfo)
            {
                publicConstractor += $@"
this.{column.Name} = {clsColumnInfo.AssginValues(GetDataType(column.DataType))};";
            }

            publicConstractor += $@"
Mode = enMode.AddNew;
}}";

            return publicConstractor;
        }

        public static string PrivateConstractorTempelete(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string PrivateConstractor = $@"

private cls{TableName}(";


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
private bool _AddNew{TableNameSingle}()
{{
    this.{ColumnsInfo[0].Name} = cls{TableName}DataAccess.AddNew{TableNameSingle}(";

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
    return (this.{ColumnsInfo[0].Name} != -1);
}}
";

            return str;
        }

        public static string PrivateUpdateMethod(string TableName, List<clsColumnInfo> ColumnsInfo)
        {
            string str = $@"
private bool _Update{TableNameSingle}()
{{
    return {DataAccessLayerName}.Update{TableNameSingle}(";

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
if({DataAccessLayerName}.Get{TableName}InfoBy{ColumnsInfo[0].Name}({ColumnsInfo[0].Name},";

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

        public static string Save(string TableName)
        {
            string str = $@"
public bool Save()
        {{
            

            switch  (Mode)
            {{
                case enMode.AddNew:
                    if (_AddNew{TableNameSingle}())
                    {{

                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}

                case enMode.Update:
                    return _Update{TableNameSingle}();

            }}

return false;
        }}
";
            return str;
        }

        public static string GetAll(string TableName)
        {
            string str = $@"
public static DataTable GetAll{TableName}()
{{
    return {DataAccessLayerName}.GetAll{TableName}();
}}
";

            return str;
        }

        public static string Delete(string TableName, clsColumnInfo Column)
        {

            string str = $@"
 public static bool Delete{TableName.Remove(TableName.Length - 1)}({clsColumnInfo.MapSqlTypeToCSharpType(Column.DataType)} {Column.Name})
        {{
           return  {DataAccessLayerName}.Delete{TableName.Remove(TableName.Length - 1)}({Column.Name});
        }}";

            return str;
        }

        public static string isExist(string TableName, clsColumnInfo Column)
        {

            string str = $@"
 public static bool is{TableName.Remove(TableName.Length - 1)}Exist({clsColumnInfo.MapSqlTypeToCSharpType(Column.DataType)} {Column.Name})
        {{
           return  {DataAccessLayerName}.Is{TableName.Remove(TableName.Length - 1)}Exist({Column.Name});
        }}";

            return str;
        }
    }
}
