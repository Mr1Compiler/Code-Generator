using BusinessLayer;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsBusinessLayerGenerator
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
            StringBuilder sb = new StringBuilder();

            foreach (var column in ColumnsInfo)
            {
                string dataType = clsColumnInfo.MapSqlTypeToCSharpType(column.DataType);
                string name = column.Name;
                sb.AppendLine($"public {dataType} {name} {{set; get;}}");
            }

            sb.AppendLine();

            return sb.ToString();
        }

        public static string ColumnTemplete()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public class cls{TableName}");
            sb.AppendLine("{");
            sb.AppendLine("public enum enMode { AddNew = 0, Update = 1 };");
            sb.AppendLine("public enMode Mode = enMode.AddNew;");
            sb.AppendLine();
            sb.Append(PrintColumnsDefintion());

            sb.AppendLine();

            return sb.ToString();
        }

        public static string GetDataType(string dataType)
        {
            return clsColumnInfo.MapSqlTypeToCSharpType(dataType);
        }

        public static string PublicConstractersTemplete()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public cls{TableName}()");
            sb.AppendLine("{");

            foreach (var column in ColumnsInfo)
            {
                sb.AppendLine($"this.{column.Name} = {clsColumnInfo.AssginValues(GetDataType(column.DataType))};");
            }

            sb.AppendLine("Mode = enMode.AddNew;");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string PrivateConstractorTempelete()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"private cls{TableName}(");

            foreach (var column in ColumnsInfo)
            {
                if (column != ColumnsInfo[0])
                {
                    sb.Append($" {clsColumnInfo.MapSqlTypeToCSharpType(column.DataType)} {column.Name.ToLower()},");
                }
                else
                {
                    sb.Append($"{clsColumnInfo.MapSqlTypeToCSharpType(column.DataType)} {column.Name.ToLower()},");
                }
            }

            if (sb.Length > 0)
            {
                sb.Length--; 
            }

            sb.AppendLine(")");
            sb.AppendLine("{");

            foreach (var column in ColumnsInfo)
            {
                sb.AppendLine($"this.{column.Name} = {column.Name.ToLower()};");
            }

            sb.AppendLine("Mode = enMode.Update;");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string PrivateAddNewMethod()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"private bool _AddNew{TableName}()");
            sb.AppendLine("{");
            sb.AppendLine($"    this.{ColumnsInfo[0].Name} = {DataAccessLayerName}.AddNew{TableName}(");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                sb.Append($" this.{ColumnsInfo[i].Name},");
            }

            if (sb.Length > 0)
            {
                sb.Length--;
            }

            sb.AppendLine(");");
            sb.AppendLine($"    return (this.{ColumnsInfo[0].Name} != -1);");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string PrivateUpdateMethod()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"private bool _Update{TableName}()");
            sb.AppendLine("{");
            sb.Append($"    return {DataAccessLayerName}.Update{TableName}(");

            foreach (var column in ColumnsInfo)
            {
                sb.Append($" this.{column.Name},");
            }

            if (sb.Length > 0)
            {
                sb.Length--; 
            }

            sb.AppendLine(");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static int NameNumberInColumn()
        {
            return Convert.ToInt16(clsSettings.NameNumberInColumns);
        }

        public static string Save()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public bool Save()");
            sb.AppendLine("{");
            sb.AppendLine("    switch (Mode)");
            sb.AppendLine("    {");
            sb.AppendLine("        case enMode.AddNew:");
            sb.AppendLine($"            if (_AddNew{TableName}())");
            sb.AppendLine("            {");
            sb.AppendLine("                Mode = enMode.Update;");
            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("            else");
            sb.AppendLine("            {");
            sb.AppendLine("                return false;");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("        case enMode.Update:");
            sb.AppendLine($"            return _Update{TableName}();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    return false;");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string GetAll()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static DataTable GetAll{TableName}()");
            sb.AppendLine("{");
            sb.AppendLine($"    return {DataAccessLayerName}.GetAll{TableName}();");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string Delete()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static bool Delete{TableName}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name})");
            sb.AppendLine("{");
            sb.AppendLine($"    return {DataAccessLayerName}.Delete{TableName}({ColumnsInfo[0].Name});");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string isExist()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static bool is{TableName}Exist({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name})");
            sb.AppendLine("{");
            sb.AppendLine($"    return {DataAccessLayerName}.Is{TableName}Exist({ColumnsInfo[0].Name});");
            sb.AppendLine("}");

            sb.AppendLine();

            return sb.ToString();
        }

        public static string Find()
        {
            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < ColumnsInfo.Count; k++)
            {
                int number = k;
                sb.AppendLine($"public static cls{TableName} FindWith{ColumnsInfo[number].Name}({clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[number].DataType)} {ColumnsInfo[number].Name})");
                sb.AppendLine("{");

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == number)
                    {
                        continue;
                    }
                    sb.AppendLine($"{clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name} = {clsColumnInfo.AssginValues(clsColumnInfo.MapSqlTypeToCSharpType(ColumnsInfo[i].DataType))};");
                }

                sb.AppendLine($"if({DataAccessLayerName}.Get{TableName}InfoBy{ColumnsInfo[number].Name}({ColumnsInfo[number].Name},");

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == number)
                    {
                        continue;
                    }
                    sb.Append($" ref {ColumnsInfo[i].Name},");
                }

                if (sb.Length > 0)
                {
                    sb.Length--; 
                }

                sb.AppendLine("))");
                sb.AppendLine();
                sb.AppendLine($"    return new {ClassName}(");

                foreach (var column in ColumnsInfo)
                {
                    sb.Append($" {column.Name},");
                }

                if (sb.Length > 0)
                {
                    sb.Length--; 
                }

                sb.AppendLine(");");
                sb.AppendLine();
                sb.AppendLine("else");
                sb.AppendLine("    return null;");
                sb.AppendLine("}");
                sb.AppendLine(); 
            }

            return sb.ToString();
        }
    }
}
