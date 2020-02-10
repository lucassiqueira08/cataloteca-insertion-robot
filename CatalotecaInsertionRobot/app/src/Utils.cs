using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CatalotecaInsertionRobot.app.src;

namespace CatalotecaInsertionRobot.src.Utils
{
    public class Utils
    {
        public static string GetStringConnection(string sgbd, string server, string dbName, string user, string pass)
        {
            string stringConnection;
            user = user == "" ? "root" : user;
            server = server == "" ? "localhost" : server;
            dbName = dbName == "" ? "cataloteca" : dbName;
            if (sgbd == "1") //Mysql
            {
                stringConnection = $"Server={server};Database={dbName};Uid={user};Pwd={pass};";
            }
            else
            { // SQLSERVER
                stringConnection = $"Server={server};Database={dbName};Trusted_Connection=True;";
            }
            return stringConnection;
        }

        public static List<ProductEntity> GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                // Abre arquivo
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }

                // Pega primeira planilha
                var ws = pck.Workbook.Worksheets.First();

                // Define objeto de database
                DataTable database = new DataTable();


                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    database.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }

                // Se tiver Header a primeira linha será a 2
                var startRow = hasHeader ? 2 : 1;

                // For em cada linha da planilha a partir da primeira
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    try
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = database.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro na linha {rowNum}");
                        Console.WriteLine(ex);

                    }

                }
                List<ProductEntity> productList = new List<ProductEntity>();
                productList = (from DataRow dr in database.Rows
                               select new ProductEntity()
                               {
                                   Name = dr["Name"].ToString(),
                                   ShortDescription = dr["Name"].ToString(),
                                   LongDescription = dr["Description"].ToString()
                               }).ToList();
                return productList;
            }
        }

    }
}