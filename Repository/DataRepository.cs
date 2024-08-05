

using CombustiblesrdBack.Interface;
using CombustiblesrdBack.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CombustiblesrdBack.Repository
{
    public class DataRepository : IDataRepository, IDisposable
    {
        private readonly IDbConnection _dbConnection;
        private readonly IConfiguration configx;

        public DataRepository(IDbConnection dbConnection,IConfiguration config)
        {
            
            this._dbConnection = dbConnection;
            this.configx = config;
        }
        

        public void Dispose()
        {
            _dbConnection.Close();
        }

        public async Task<IEnumerable<Combustible>> GetAllAsync()
        {
            string query = "SELECT top (6) * FROM combustibleRD;";
            string cs = configx.GetConnectionString("DefaultConnection");
            List<Combustible> data = new List<Combustible>();
            using (SqlConnection connection = new SqlConnection(cs))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Ejecutar la consulta y obtener el lector de datos
                    SqlDataReader reader = command.ExecuteReader();

                    // Leer los datos
                    while (reader.Read())
                    {
                        Combustible com = new Combustible() {
                            Nombre = reader["name"].ToString(),
                            Precio = reader["price"].ToString(),
                            updateDate = DateTime.Parse(reader["updateDate"].ToString())
                        };
                        data.Add(com);
                    }

                    // Cerrar el lector de datos
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            Console.WriteLine("Consulta completada.");
            return data.AsEnumerable();
        }

        public async Task<IEnumerable<List<Combustible>>> GetHistory()
        {
            List<List<Combustible>> history = new List<List<Combustible>>();
            string query = "select updateDate from combustibleRD group by updateDate";
            string query2 = "select * from combustibleRD where updateDate = ";
            string cs = configx.GetConnectionString("DefaultConnection");
            List<string> dates= new List<string>();
            using (SqlConnection connection = new SqlConnection(cs))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    // Abrir la conexión
                    connection.Open();

                    // Ejecutar la consulta y obtener el lector de datos
                    SqlDataReader reader = command.ExecuteReader();

                    // Leer los datos
                    while (reader.Read())
                    {
                        string date = reader["updateDate"].ToString();
                        dates.Add(date);
                    }

                    // Cerrar el lector de datos
                    reader.Close();


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            foreach (string datex in dates)
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    SqlCommand command = new SqlCommand(query2+"'"+datex+"'", connection);

                    try
                    {
                        // Abrir la conexión
                        connection.Open();

                        // Ejecutar la consulta y obtener el lector de datos
                        SqlDataReader reader = command.ExecuteReader();
                        List<Combustible>element = new List<Combustible>();
                        while (reader.Read())
                        {
                            Combustible com2 = new Combustible()
                            {
                                Nombre = reader["name"].ToString(),
                                Precio = reader["price"].ToString(),
                                updateDate = DateTime.Parse(reader["updateDate"].ToString())
                            };
                            element.Add(com2);
                        }

                        // Cerrar el lector de datos
                        reader.Close();
                        history.Add(element);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }

            Console.WriteLine("Consulta completada.");
            return history;
        }

    }
}
