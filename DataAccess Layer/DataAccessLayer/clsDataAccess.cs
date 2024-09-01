using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class clsDataAccess
    {

        public static List<string> GetDataBases()
        {
            List<string> list = new List<string>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);


            string query = "select name from sys.databases";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // pass
            }

            finally
            {
                connection.Close();
            }

            return list;
        }

        public static List<string> GetTables(string DataBaseName)
        {
            List<string> list = new List<string>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);


            string query = $@"
                             USE [{DataBaseName}];
                             SELECT TABLE_NAME
                             FROM INFORMATION_SCHEMA.TABLES
                             WHERE TABLE_TYPE = 'BASE TABLE';";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DataBaseName", DataBaseName);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // pass
            }

            finally
            {
                connection.Close();
            }

            return list;

        }
    }
}
