using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class clsDataAccessGenerator
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
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static int AddNew{TableName}(");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                sb.Append($" {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},");
            }

            sb.Length--; // Remove last comma
            sb.AppendLine(")");

            sb.AppendLine("{");
            sb.AppendLine($"    {GetDataType(ColumnsInfo[0].DataType)} {ColumnsInfo[0].Name} = {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[0].DataType))};");
            sb.AppendLine();
            sb.AppendLine($"    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
            sb.AppendLine();
            sb.AppendLine($"    string query = @\"insert into {TableName}(");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                sb.Append($" {ColumnsInfo[i].Name},");
            }

            sb.Length--; // Remove last comma
            sb.AppendLine(")");
            sb.AppendLine($"    Values (");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                sb.Append($" @{ColumnsInfo[i].Name},");
            }

            sb.Length--; // Remove last comma
            sb.AppendLine(");");
            sb.AppendLine($"    SELECT SCOPE_IDENTITY();\";");

            sb.AppendLine();
            sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    sb.AppendLine($"    command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", {ColumnsInfo[i].Name});");
                }
                else
                {
                    sb.AppendLine($"    if({ColumnsInfo[i].Name} != {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))} && {ColumnsInfo[i].Name} != null)");
                    sb.AppendLine($"        command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", {ColumnsInfo[i].Name});");
                    sb.AppendLine($"    else");
                    sb.AppendLine($"        command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", System.DBNull.Value);");
                }
            }

            sb.AppendLine();
            sb.AppendLine("    try");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Open();");
            sb.AppendLine();
            sb.AppendLine("        object result = command.ExecuteScalar();");
            sb.AppendLine();
            sb.AppendLine("        if (result != null && int.TryParse(result.ToString(), out int insertedID))");
            sb.AppendLine("        {");
            sb.AppendLine($"            {ColumnsInfo[0].Name} = insertedID;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
            sb.AppendLine("    }");
            sb.AppendLine("    finally");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Close();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine($"    return {ColumnsInfo[0].Name};");
            sb.AppendLine("}");

            sb.AppendLine(); // Two line breaks after the method content
            sb.AppendLine();

            return sb.ToString();
        }

        public static string Update()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static bool Update{TableName}(");

            foreach (var column in ColumnsInfo)
            {
                sb.Append($" {GetDataType(column.DataType)} {column.Name},");
            }

            sb.Length--; // Remove last comma
            sb.AppendLine(")");

            sb.AppendLine("{");
            sb.AppendLine("    int rowsAffected = 0;");
            sb.AppendLine("    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
            sb.AppendLine();
            sb.AppendLine($"    string query = @\"Update {TableName}");
            sb.AppendLine("    set");

            for (int i = 1; i < ColumnsInfo.Count; i++)
            {
                sb.Append($" {ColumnsInfo[i].Name} = @{ColumnsInfo[i].Name},");
            }

            sb.Length--; // Remove last comma
            sb.AppendLine();
            sb.AppendLine($"    where {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}\";");
            sb.AppendLine();
            sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");

            for (int i = 0; i < ColumnsInfo.Count; i++)
            {
                if (ColumnsInfo[i].AllowNull == false)
                {
                    sb.AppendLine($"    command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", {ColumnsInfo[i].Name});");
                }
                else
                {
                    sb.AppendLine($"    if({ColumnsInfo[i].Name} != {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))} && {ColumnsInfo[i].Name} != null)");
                    sb.AppendLine($"        command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", {ColumnsInfo[i].Name});");
                    sb.AppendLine($"    else");
                    sb.AppendLine($"        command.Parameters.AddWithValue(\"@{ColumnsInfo[i].Name}\", System.DBNull.Value);");
                }
            }

            sb.AppendLine();
            sb.AppendLine("    try");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Open();");
            sb.AppendLine("        rowsAffected = command.ExecuteNonQuery();");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
            sb.AppendLine("        return false;");
            sb.AppendLine("    }");
            sb.AppendLine("    finally");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Close();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    return (rowsAffected > 0);");
            sb.AppendLine("}");

            sb.AppendLine(); // Two line breaks after the method content
            sb.AppendLine();

            return sb.ToString();
        }

        public static string GetAll()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static DataTable GetAll{TableName}()");
            sb.AppendLine("{");
            sb.AppendLine("    DataTable dt = new DataTable();");
            sb.AppendLine("    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
            sb.AppendLine();
            sb.AppendLine($"    string query = \"SELECT * FROM {TableName}\";");
            sb.AppendLine();
            sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");
            sb.AppendLine();
            sb.AppendLine("    try");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Open();");
            sb.AppendLine();
            sb.AppendLine("        SqlDataReader reader = command.ExecuteReader();");
            sb.AppendLine();
            sb.AppendLine("        if (reader.HasRows)");
            sb.AppendLine("        {");
            sb.AppendLine("            dt.Load(reader);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        reader.Close();");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
            sb.AppendLine("    }");
            sb.AppendLine("    finally");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Close();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    return dt;");
            sb.AppendLine("}");

            sb.AppendLine(); // Two line breaks after the method content
            sb.AppendLine();

            return sb.ToString();
        }

        public static string Delete()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static bool Delete{TableName}({ColumnsInfo[0].DataType} {ColumnsInfo[0].Name})");
            sb.AppendLine("{");
            sb.AppendLine("    int rowsAffected = 0;");
            sb.AppendLine("    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
            sb.AppendLine();
            sb.AppendLine($"    string query = @\"Delete {TableName}");
            sb.AppendLine($"    where {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}\";");
            sb.AppendLine();
            sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");
            sb.AppendLine();
            sb.AppendLine($"    command.Parameters.AddWithValue(\"@{ColumnsInfo[0].Name}\", {ColumnsInfo[0].Name});");
            sb.AppendLine();
            sb.AppendLine("    try");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Open();");
            sb.AppendLine("        rowsAffected = command.ExecuteNonQuery();");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
            sb.AppendLine("    }");
            sb.AppendLine("    finally");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Close();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    return (rowsAffected > 0);");
            sb.AppendLine("}");

            sb.AppendLine(); // Two line breaks after the method content
            sb.AppendLine();

            return sb.ToString();
        }

        public static string IsExist()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"public static bool Is{TableName}Exist({ColumnsInfo[0].DataType} {ColumnsInfo[0].Name})");
            sb.AppendLine("{");
            sb.AppendLine("    bool isFound = false;");
            sb.AppendLine("    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
            sb.AppendLine();
            sb.AppendLine($"    string query = \"SELECT Found=1 FROM {TableName} WHERE {ColumnsInfo[0].Name} = @{ColumnsInfo[0].Name}\";");
            sb.AppendLine();
            sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");
            sb.AppendLine();
            sb.AppendLine($"    command.Parameters.AddWithValue(\"@{ColumnsInfo[0].Name}\", {ColumnsInfo[0].Name});");
            sb.AppendLine();
            sb.AppendLine("    try");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Open();");
            sb.AppendLine();
            sb.AppendLine("        SqlDataReader reader = command.ExecuteReader();");
            sb.AppendLine();
            sb.AppendLine("        isFound = reader.HasRows;");
            sb.AppendLine();
            sb.AppendLine("        reader.Close();");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
            sb.AppendLine("        isFound = false;");
            sb.AppendLine("    }");
            sb.AppendLine("    finally");
            sb.AppendLine("    {");
            sb.AppendLine("        connection.Close();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    return isFound;");
            sb.AppendLine("}");

            sb.AppendLine(); // Two line breaks after the method content
            sb.AppendLine();

            return sb.ToString();
        }

        public static string GeneralFind()
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < ColumnsInfo.Count; k++)
            {
                int number = k;
                sb.AppendLine($"public static bool Get{TableName}InfoBy{ColumnsInfo[number].Name}({GetDataType(ColumnsInfo[number].DataType)} {ColumnsInfo[number].Name}, ");

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == number)
                    {
                        continue;
                    }

                    sb.Append($" ref {GetDataType(ColumnsInfo[i].DataType)} {ColumnsInfo[i].Name},");
                }

                sb.Length--; // Remove last comma
                sb.AppendLine(")");

                sb.AppendLine("{");
                sb.AppendLine("    bool isFound = false;");
                sb.AppendLine("    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);");
                sb.AppendLine();
                sb.AppendLine($"    string query = \"SELECT * FROM {TableName} WHERE {ColumnsInfo[number].Name} = @{ColumnsInfo[number].Name}\";");
                sb.AppendLine();
                sb.AppendLine("    SqlCommand command = new SqlCommand(query, connection);");
                sb.AppendLine();
                sb.AppendLine($"    command.Parameters.AddWithValue(\"@{ColumnsInfo[number].Name}\", {ColumnsInfo[number].Name});");
                sb.AppendLine();
                sb.AppendLine("    try");
                sb.AppendLine("    {");
                sb.AppendLine("        connection.Open();");
                sb.AppendLine();
                sb.AppendLine("        SqlDataReader reader = command.ExecuteReader();");
                sb.AppendLine();
                sb.AppendLine("        if (reader.Read())");
                sb.AppendLine("        {");
                sb.AppendLine("            isFound = true;");
                sb.AppendLine();

                for (int i = 0; i < ColumnsInfo.Count; i++)
                {
                    if (i == number)
                    {
                        continue;
                    }

                    sb.AppendLine($"            if (reader[\"{ColumnsInfo[i].Name}\"] != DBNull.Value)");
                    sb.AppendLine($"            {{");
                    sb.AppendLine($"                {ColumnsInfo[i].Name} = ({GetDataType(ColumnsInfo[i].DataType)})reader[\"{ColumnsInfo[i].Name}\"];");
                    sb.AppendLine("            }");
                    sb.AppendLine($"            else");
                    sb.AppendLine($"            {{");
                    sb.AppendLine($"                {ColumnsInfo[i].Name} = {clsColumnInfo.AssginValues(GetDataType(ColumnsInfo[i].DataType))};");
                    sb.AppendLine("            }");
                }

                sb.AppendLine("        }");
                sb.AppendLine("        else");
                sb.AppendLine("        {");
                sb.AppendLine("            isFound = false;");
                sb.AppendLine("        }");
                sb.AppendLine();
                sb.AppendLine("        reader.Close();");
                sb.AppendLine("    }");
                sb.AppendLine("    catch (Exception ex)");
                sb.AppendLine("    {");
                sb.AppendLine("        // Console.WriteLine(\"Error: \" + ex.Message);");
                sb.AppendLine("        isFound = false;");
                sb.AppendLine("    }");
                sb.AppendLine("    finally");
                sb.AppendLine("    {");
                sb.AppendLine("        connection.Close();");
                sb.AppendLine("    }");
                sb.AppendLine();
                sb.AppendLine("    return isFound;");
                sb.AppendLine("}");

                sb.AppendLine(); // Two line breaks after each method content
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
