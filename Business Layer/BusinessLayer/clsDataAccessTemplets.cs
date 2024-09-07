using CodeGenerator;
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
     

        public static string AddNew()
        {
            string str = $@"public static int AddNew{TableName}(";

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

            str += $@" SELECT SCOPE_IDENTITY();"";

SqlCommand command = new SqlCommand(query, connection);
";

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    str += $@"
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", {ColumnsInfo[i].Name});";
                }
                else
                {
                    str += $@"

if({ColumnsInfo[i].Name} != {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))} && {ColumnsInfo[i].Name} != null)
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", {ColumnsInfo[i].Name});
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
}}
catch (Exception ex)
{{
//Console.WriteLine(""Error: "" + ex.Message);
               
}}

finally 
{{ 
 connection.Close(); 
}}

return {ColumnsInfo[0].Name};
}}

";
         
            return str;
        }

        public static string Update()
        {
            string str = $@"
public static bool Update{TableName}(";

            foreach(var Column in ColumnsInfo)
            {
                str += $@" {GetDataType(Column.DataType)} {Column.Name},";
            }

            str = RemoveLastLetter(str) + ")";

            str += $@"
{{
int rowsAffected=0;
SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = @""Update {TableName} 
set ";


            for(int i = 1;i < ColumnsInfo.Count; i++)
            {
                str += $@" {ColumnsInfo[i].Name} = @{ColumnsInfo[i].Name},";
            }

            str = RemoveLastLetter(str);

            str += $@"
where {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}"";

            SqlCommand command = new SqlCommand(query, connection);
";


            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    str += $@"
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", {ColumnsInfo[i].Name});";
                }
                else 
                {
                    str += $@"

if({ColumnsInfo[i].Name} != {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))} && {ColumnsInfo[i].Name} != null)
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", {ColumnsInfo[i].Name});
else
command.Parameters.AddWithValue(""@{ColumnsInfo[i].Name}"", System.DBNull.Value);
";
                }
            }

            str += $@"
try
{{
 connection.Open();
 rowsAffected = command.ExecuteNonQuery();

}}
catch (Exception ex)
{{
//Console.WriteLine(""Error: "" + ex.Message);
 return false;
}}

finally 
{{ 
connection.Close(); 
}} 

return (rowsAffected > 0);
}}
";

            return str; 
        }

        public static string GetAll()
        {
            string str = $@"public static DataTable GetAll{TableName}()
{{

DataTable dt = new DataTable();
SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = ""SELECT * FROM {TableName}"";

SqlCommand command = new SqlCommand(query, connection);

 try
{{
connection.Open();

 SqlDataReader reader = command.ExecuteReader();

if (reader.HasRows)

 {{
dt.Load(reader);
}}

reader.Close();
               

}}

catch (Exception ex)
{{
// Console.WriteLine(""Error: "" + ex.Message);
}}
finally 
{{ 
    connection.Close(); 
 }}

 return dt;
}}
";


            return str;
        }

        public static string Delete()
        {
            string str = $@"public static bool Delete{TableName}({ColumnsInfo[0].DataType} {ColumnsInfo[0].Name})
{{
int rowsAffected=0;

SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = @""Delete {TableName} 
where {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}"";

SqlCommand command = new SqlCommand(query, connection);

 command.Parameters.AddWithValue(""@{ColumnsInfo[0].Name}"", {ColumnsInfo[0].Name});

 try
{{
 connection.Open();

rowsAffected = command.ExecuteNonQuery();

}}
catch (Exception ex)
{{
   // Console.WriteLine(""Error: "" + ex.Message);
}}
finally 
{{ 
                
 connection.Close(); 

}}

return (rowsAffected > 0);
}}
";
            return str;
        }

        public static string IsExist()
        {
            string str = $@"
public static bool Is{TableName}Exist({ColumnsInfo[0].DataType} {ColumnsInfo[0].Name})
{{
bool isFound = false;

SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = ""SELECT Found=1 FROM {TableName} WHERE {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}"";

SqlCommand command = new SqlCommand(query, connection);

command.Parameters.AddWithValue(""@{ColumnsInfo[0].Name}"", {ColumnsInfo[0].Name});

try
{{
connection.Open();
SqlDataReader reader = command.ExecuteReader();

isFound = reader.HasRows;
                
 reader.Close();
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

}}
";


            return str;
        }

//        public static string FindByName()
//        {
            
//            string str = "";
//            if (clsSettings.NameNumberInColumns != null)
//            {
//                int Number = clsSettings.GetNameNumber();
//                str = $@"public static bool Get{TableName}InfoByName ({GetDataType(ColumnsInfo[Number].DataType)} {ColumnsInfo[Number].Name}, ";

//                for (int i = 0; i < ColumnsInfo.Count; i++)
//                {
//                    if (i == Number)
//                    {
//                        continue;
//                    }

//                    str += $@" ref {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},";
//                }

//                str = RemoveLastLetter(str) + ")";

//                str += $@"
//{{
//bool isFound = false;

//SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

//string query = ""SELECT * FROM {TableName} WHERE {ColumnsInfo[Number].Name} = @{ColumnsInfo[Number].Name}"";

//SqlCommand command = new SqlCommand(query, connection);

//command.Parameters.AddWithValue(""@{ColumnsInfo[Number].Name}"", {ColumnsInfo[Number].Name});

// try
//{{
//connection.Open();
// SqlDataReader reader = command.ExecuteReader();

// if (reader.Read())
// {{

// // The record was found
// isFound = true;
//";

//                for (int i = 0; i < ColumnsInfo.Count; i++)
//                {
//                    if (i == 1)
//                    {
//                        continue;
//                    }

//                    str += $@"
// if (reader[""{ColumnsInfo[i].Name}""] != DBNull.Value)
//{{
//{ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[""{ColumnsInfo[i].Name}""];
//}}
// else
//{{
//    {ColumnsInfo[i].Name} = {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))};
// }}";
//                }

//                str += $@"
//}}
// else
// {{
// // The record was not found
//  isFound = false;
//}}

// reader.Close();


// }}
//  catch (Exception ex)
// {{
//  //Console.WriteLine(""Error: "" + ex.Message);
//  isFound = false;
//}}
//finally
//{{
//   connection.Close();
//}}

// return isFound;
//}}
//";

//            }
//                return str;
//        }

        public static string GeneralFind()
        {
            string str = $@"
";
            for (int k = 0; k < ColumnsInfo.Count; k++)
            {
                int Number = k;
                str += $@"
public static bool Get{TableName}InfoBy{ColumnsInfo[Number].Name}({GetDataType(ColumnsInfo[Number].DataType)} {ColumnsInfo[Number].Name}, ";

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == Number)
                    {
                        continue;
                    }

                    str += $@" ref {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},";
                }

                str = RemoveLastLetter(str) + ")";

                str += $@"
{{
bool isFound = false;

SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

string query = ""SELECT * FROM {TableName} WHERE {ColumnsInfo[Number].Name} = @{ColumnsInfo[Number].Name}"";

SqlCommand command = new SqlCommand(query, connection);

command.Parameters.AddWithValue(""@{ColumnsInfo[Number].Name}"", {ColumnsInfo[Number].Name});

 try
{{
connection.Open();
 SqlDataReader reader = command.ExecuteReader();

 if (reader.Read())
 {{

 // The record was found
 isFound = true;
";

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == 1)
                    {
                        continue;
                    }

                    str += $@"
 if (reader[""{ColumnsInfo[i].Name}""] != DBNull.Value)
{{
{ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[""{ColumnsInfo[i].Name}""];
}}
 else
{{
    {ColumnsInfo[i].Name} = {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))};
 }}";
                }

                str += $@"
}}
 else
 {{
 // The record was not found
  isFound = false;
}}

 reader.Close();


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
}}
";


            }
            return str;
        }
    }
}
