using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApresentacaoALimpo.Model;

namespace ApresentacaoALimpo
{
    class Program
    {
        private static string embeddedSql = "mylocaldb";
        private static string sqlServer = "mysqldb";

        static void Main(string[] args)
        {
            var currentDb = sqlServer;
            CreateDatabase(currentDb);

            ExecuteExamQuestion(currentDb);

            Console.WriteLine();
            Console.WriteLine("Application finished! Press <any key> to quit.");
            Console.ReadKey();
        }

        private static void ExecuteExamQuestion(string currentDb)
        {
            //Send email for all students who have any asset and fisished all courses with more than 10 days
            var conn = new SqlConnection(ConfigurationManager.AppSettings[currentDb]);
        }

        #region Helper Methods
        private static void CreateDatabase(string currentDb)
        {
            Console.WriteLine("Creating database...");
            var db = new MyDbContext(currentDb);
            db.Database.CreateIfNotExists();

            ShowTableData(db.Students.ToList());
            ShowTableData(db.Assets.ToList());
            ShowTableData(db.Enrollments.ToList());
        }

        private static void ShowTableData<T>(List<T> list)
        {
            var t = typeof (T);
            var tProperties = t.GetProperties();
            Console.WriteLine();
            Console.WriteLine("------- Table: " + t.Name + " -------");
            Console.WriteLine(string.Join(";", tProperties.Select(p => p.Name)));
            list.ForEach(l =>Console.WriteLine(string.Join(";", t.GetProperties().Select(p => p.GetValue(l, null)))));
            Console.WriteLine(new string('-', 40));
        }
        #endregion
    }
}
