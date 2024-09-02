using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class clsColumnInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }


        public static string MapSqlTypeToCSharpType(string sqlType)
        {
            sqlType = sqlType.ToUpper();

            switch (sqlType)
            {
                case "INT":
                    return "int";
                case "BIGINT":
                    return "long";
                case "SMALLINT":
                    return "short";
                case "TINYINT":
                    return "byte";
                case "FLOAT":
                    return "float";
                case "REAL":
                    return "float";
                case "NUMERIC":
                    return "decimal";
                case "DECIMAL":
                    return "decimal";
                case "MONEY":
                    return "decimal";
                case "SMALLMONEY":
                    return "decimal";
                case "CHAR":
                    return "string";
                case "VARCHAR":
                    return "string";
                case "TEXT":
                    return "string";
                case "NCHAR":
                    return "string";
                case "NVARCHAR":
                    return "string";
                case "NTEXT":
                    return "string";
                case "DATETIME":
                    return "DateTime";
                case "SMALLDATETIME":
                    return "DateTime";
                case "DATE":
                    return "DateTime";
                case "TIME":
                    return "TimeSpan";
                case "BIT":
                    return "bool";
                case "UNIQUEIDENTIFIER":
                    return "Guid";
                case "BINARY":
                case "VARBINARY":
                case "IMAGE":
                    return "byte[]";
                default:
                    return "object";
            }
        }

        public static string AssginValues(string DataType)
        {
            switch (DataType)
            {
                case "int":
                    return "-1";
                case "string":
                    return "\"\"";
                case "DateTime":
                    return "DateTime.Now";
                default:
                    return "\"\"";
            }
        }
    }
}
