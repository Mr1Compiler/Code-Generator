using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
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
        public static List<clsColumnInfo> ColumnsInfo;
        public static void Load(string tableName, List<clsColumnInfo> columnsInfo)
        {
            AddNew = $"_AddNew{tableName}";
            DataAccessLayerName = $"cls{tableName}DataAccess";
            ClassName = $"cls{tableName}";
            ColumnsInfo = columnsInfo;

            TableName = tableName;

            TableNameSingle = tableName.Remove(tableName.Length - 1);


        }


        public static string PrintColumnsDefintion()
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
        public static string ColumnTemplete()
        {
            string Templete = $@"
public class cls{TableName} 
{{
public enum enMode {{AddNew = 0,Update = 1}};
public enMode Mode = enMode.AddNew;
";

            // string Definition = PrintColumnsDefintion(ColumnsInfo);

            Templete += PrintColumnsDefintion();

            return Templete;
        }

        public static string GetDataType(string DataType)
        {
            return clsColumnInfo.MapSqlTypeToCSharpType(DataType);
        }

        public static string PublicConstractersTemplete()
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

        public static string PrivateConstractorTempelete()
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


        public static string PrivateAddNewMethod()
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
    return (this.{ColumnsInfo[0].Name} != -1);
}}
";

            return str;
        }

        public static string PrivateUpdateMethod()
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



        public static int NameNumberInColumn()
        {
            return Convert.ToInt16(clsSettings.NameNumberInColumns);
        }


        public static string Save()
        {
            string str = $@"
public bool Save()
        {{
            

            switch  (Mode)
            {{
                case enMode.AddNew:
                    if (_AddNew{TableName}())
                    {{

                        Mode = enMode.Update;
                        return true;
                    }}
                    else
                    {{
                        return false;
                    }}

                case enMode.Update:
                    return _Update{TableName}();

            }}

return false;
        }}
";
            return str;
        }

        public static string GetAll()
        {
            string str = $@"
public static DataTable GetAll{TableName}()
{{
    return {DataAccessLayerName}.GetAll{TableName}();
}}
";

            return str;
        }

        public static string Delete()
        {

            string str = $@"
 public static bool Delete{TableName}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name})
        {{
           return  {DataAccessLayerName}.Delete{TableName}({ColumnsInfo[0].Name});
        }}";

            return str;
        }

        public static string isExist()
        {

            string str = $@"
 public static bool is{TableName}Exist({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name})
        {{
           return  {DataAccessLayerName}.Is{TableName}Exist({ColumnsInfo[0].Name});
        }}";

            return str;
        }

        //        public static void Find()
        //        {
        //            for (int k = 0; k < ColumnsInfo.Count; k++)
        //            {
        //                string str = $@"
        //public static cls{TableName} Find({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[k].DataType)} {ColumnsInfo[k].Name})
        //{{
        //";

        //                for (int i = 1; i < ColumnsInfo.Count; i++)
        //                {
        //                    str += $@"{clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name}={clsColumnInfo.AssginValues(clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType))}; ";
        //                }

        //                str += $@"
        //if({DataAccessLayerName}.Get{TableName}InfoBy{ColumnsInfo[k].Name}({ColumnsInfo[k].Name},";

        //                for (int i = 1; i < ColumnsInfo.Count; i++)
        //                {
        //                    str += $@" ref {ColumnsInfo[i].Name},";
        //                }

        //                str = str.Remove(str.Length - 1);

        //                str += $@"))

        //    return new {ClassName}(";

        //                foreach (var column in ColumnsInfo)
        //                {
        //                    str += $@" {column.Name},";
        //                }

        //                str = str.Remove(str.Length - 1);

        //                str += $@");

        //else
        //    return null;
        //}}
        //";


        //            }

        //        }
        public static string Find()
        {
            string str = $@"
";
            for (int k = 0; k < ColumnsInfo.Count; k++)
            {
                int Number = k;
                str += $@"
public static cls{TableName} FindWith{ColumnsInfo[Number].Name}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[Number].DataType)} {ColumnsInfo[Number].Name})
{{
";

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == Number)
                    {
                        continue;
                    }
                    str += $@"{clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name}={clsColumnInfo.AssginValues(clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType))}; ";
                }

                str += $@"
if({DataAccessLayerName}.Get{TableName}InfoBy{ColumnsInfo[Number].Name}({ColumnsInfo[Number].Name},";

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == Number)
                    {
                        continue;
                    }
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


            }

            
            return str;
        }
    }
}
