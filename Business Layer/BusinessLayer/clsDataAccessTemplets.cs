using DataAccessLayer;
using System;
using System.CodeDom;
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
        public static string GetInfoByID()
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

command.Parameters.AddWithValue(""@{ColumnsInfo[0].Name}"", {ColumnsInfo[0].Name.ToLower()});

 try
{{
connection.Open();
SqlDataReader reader = command.ExecuteReader();

if (reader.Read())
{{
isFound = true;
";

            for (int i = 1; i < ColumnsInfo.Count; i++)
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


        public static string AddNew()
        {
            string str = $@"public static int AddNew{TableNameSingle}(";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},";
            }

            str = RemoveLastLetter(str) + ")";

            str += $@"
{{
{GetDataType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name} = {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[0].DataType))};

 SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = @""insert into {TableName}(
";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" {ColumnsInfo[i].Name},";
            }

            str = RemoveLastLetter(str) + ")";

            str += $@" Values (";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                str += $@" @{ColumnsInfo[i].Name},";
            }

            str = RemoveLastLetter(str) + ");";

            str += $@" Select SELECT SCOPE_IDENTITY();"";

SqlCommand command = new SqlCommand(query, connection);
";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    str += $@"command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}
                "", {ColumnsInfo[i].Name})";
                }
                else
                {
                    str += $@"if({ColumnsInfo[i].Name} != {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))} && {ColumnsInfo[i].Name} != null
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", {ColumnsInfo[i].Name})

else
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", System.DBNull.Value);
";
                }
            }

                str += $@"
try
 {{
connection.Open();

object result = command.ExecuteScalar();

if (result != null && int.TryParse(result.ToString(), out int insertedID))
{{
{ColumnsInfo[0].Name} = insertedID;
}}

catch (Exception ex)
{{
//Console.WriteLine(""Error: "" + ex.Message);
               
}}

finally 
{{ 
 connection.Close(); 
}}

return {ColumnsInfo[0].Name};";
         
            return str;
        }
    }
}
