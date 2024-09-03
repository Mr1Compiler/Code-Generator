using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsDataAccessTemplets
    {
        public static string TableNameSingle = "";
        public static string TableName = "";
        public static List<clsColumnInfo> ColumnsInfo = null;
        public static void Load(string tableName, List<clsColumnInfo> columns)
        {
            ColumnsInfo = columns;
            TableName = tableName;
            TableNameSingle = RemoveLastLetter(tableName);

        }

        public static string GetDataType(string DataType)
        {
            return clsColumnInfo.MapSqlTypeToCSharpType(DataType);
        }

        public static string RemoveLastLetter(string str)
        {
            return str.Remove(str.Length - 1);
        }
        public static string GetAll()
        {

            string str = $@"
public static bool Get{TableName}InfoBy{GetDataType(ColumnsInfo[0].DataType)}({GetDataType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name}, ";



            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" ref {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},";
            }

            str = RemoveLastLetter(str) + $@")
{{";

            str += $@"
bool isFound = false;

SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = ""SELECT * FROM {TableName} WHERE {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}"";

SqlCommand command = new SqlCommand(query, connection);

command.Parameters.AddWithValue(""{ColumnsInfo[0].Name}"", {ColumnsInfo[0].Name.ToLower()});

 try
{{
connection.Open();
SqlDataReader reader = command.ExecuteReader();

if (reader.Read())
{{
isFound = true;
";

            for(int i = 1; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    str += $@"{ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[""{ColumnsInfo[i].Name}""];
";
                }
                else
                {
                    str += $@"
if(reader[""{ColumnsInfo[i].Name}""] != DBNull.Value)
{{
{ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[""{ColumnsInfo[i].Name}""];
}}
else
{{
{ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[""{ColumnsInfo[i].Name}""];
}}
";
                }
            }

            str += $@"
else
{{
    isFound = false;
}}

catch (Exception ex)
            {{
                //Console.WriteLine(""Error: "" + ex.Message);
                isFound = false;
            }}
            finally
            {{ 
                connection.Close(); 
            }}

            return isFound;
}}";

            return str;
        }
    }
}
