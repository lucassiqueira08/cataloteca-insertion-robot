using System;
using System.Diagnostics;
using CatalotecaInsertionRobot.src.Utils;
using System.Collections.Generic;
using CatalotecaInsertionRobot.app.src;
using System.Linq;
using CatalotecaInsertionRobot.src.Data;

namespace CatalotecaInsertionRobot
{
    class Program
    {
        private const string tablename = "product";

        public static void Main()
        {
            Console.WriteLine("Robô de inserção Cataloteca");
            Console.WriteLine("-------------------------");

            // Input de dados
            Console.Write("Digite qual o gerenciador de banco de dados (1-MySQL, 2-SQLServer):");
            string sgbd = Console.ReadLine();

            Console.Write("Digite host do banco de dados (Default => localhost):");
            string server = Console.ReadLine();

            Console.Write("Digite usuario (Default => root):");
            string user = Console.ReadLine();

            Console.Write("Digite a senha: ");
            string password = CustomConsole.ReadPassword();

            Console.Write("Digite o nome do banco de dados (Default => cataloteca):");
            string dbName = Console.ReadLine();

            Console.WriteLine("Digite o caminho completo para o arquivo:");
            string filePath = @Console.ReadLine();

            string stringConnection = Utils.GetStringConnection(sgbd, server, dbName, user, password);

            // Iniciando count de tempo de processamento
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("-------------------------");
            Console.WriteLine("Iniciando leitura da planilha...");
            List<ProductEntity> dt = Utils.GetDataTableFromExcel(filePath);

            Console.WriteLine("Iniciando Inserção...");
            ForwardsInsertion(stringConnection, sgbd, dt, tablename);


            Console.WriteLine("-------------------------");
            // Terminando count de tempo de processamento
            Console.WriteLine($"Tempo de processamento (Em segundos): {stopwatch.Elapsed.TotalSeconds}");
            stopwatch.Stop();
            Console.WriteLine("Fim!");

        }
        private static void ForwardsInsertion(string stringConnection, string sgbd, List<ProductEntity> content, string table)
        {
            if (sgbd == "1")
            {
                MysqlData.InsertMySQL(stringConnection, content, table);
            } else
            {
                SqlServerData.InsertSQLServer(stringConnection, content, table);
            }
        }

        public class CustomConsole
        {
            public static string ReadPassword(char mask)
            {
                const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
                int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

                var pass = new Stack<char>();
                char chr = (char)0;

                while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
                {
                    if (chr == BACKSP)
                    {
                        if (pass.Count > 0)
                        {
                            System.Console.Write("\b \b");
                            pass.Pop();
                        }
                    }
                    else if (chr == CTRLBACKSP)
                    {
                        while (pass.Count > 0)
                        {
                            System.Console.Write("\b \b");
                            pass.Pop();
                        }
                    }
                    else if (FILTERED.Count(x => chr == x) > 0) { }
                    else
                    {
                        pass.Push((char)chr);
                        System.Console.Write(mask);
                    }
                }

                System.Console.WriteLine();

                return new string(pass.Reverse().ToArray());
            }
            public static string ReadPassword()
            {
                return ReadPassword('*');
            }
        }
    }
}
