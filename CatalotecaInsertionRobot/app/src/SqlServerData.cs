using System.Data.SqlClient;
using System;
using System.Data;
using CatalotecaInsertionRobot.app.src;
using System.Collections.Generic;

namespace CatalotecaInsertionRobot.src.Data
{
    public class SqlServerData
    {
        public static void InsertSQLServer(string connectionString, List<ProductEntity> dt, string table)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int insertLines = 0;
                    string query = $"INSERT INTO {table} (Id, Name, ShortDescription, LongDescription ) VALUES (@Id, @Name, @ShortDescription, @LongDescription);";

                    connection.Open();
                    
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("SampleTransaction");

                    command.Connection = connection;
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;
                        for (int i = 0; i < dt.ToArray().Length; i++)
                        {
                            var row = dt[i];
                            Console.WriteLine("---------");
                            Console.WriteLine($"Inserindo => {i} => {row.LongDescription}");
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                            command.Parameters.AddWithValue("@Name", row.Name.ToString());
                            command.Parameters.AddWithValue("@ShortDescription", row.ShortDescription.ToString());
                            command.Parameters.AddWithValue("@LongDescription", row.LongDescription.ToString());
                            command.ExecuteNonQuery();
                            insertLines++;
                        }
                        transaction.Commit();
                        Console.WriteLine("-------------------------");
                        Console.WriteLine("Linhas Inseridas:");
                        Console.WriteLine(insertLines + 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                        Console.WriteLine("  Message: {0}", ex.Message);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("  Message: {0}", ex2.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível concluir a tarefa");
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}