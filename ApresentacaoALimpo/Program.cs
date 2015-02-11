using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        private static string pendingAssetsStudentsQuery = @"
select distinct s.Id, s.Email
from Assets a
	inner join Students s on s.Id = a.WithStudent_Id
where Checkin is null and 
	s.Id NOT IN (select distinct e.Student_Id 
				from Enrollments e
				where e.ExitDate is null);";

        private static string pendingAssetsByStudentId = @"
select a.Id, a.Name
from Assets a
where a.WithStudent_Id = @student_id and a.Checkin is null
";

        private static void ExecuteExamQuestion(string currentDb)
        {
            //Send email for all students who have any asset and fisished all courses with more than 10 days
            var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[currentDb].ConnectionString);
            var comm = new SqlCommand(pendingAssetsStudentsQuery, conn);
            var ds = new DataSet();            
            new SqlDataAdapter(comm).Fill(ds);            

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comm = new SqlCommand(pendingAssetsByStudentId, conn);
                comm.Parameters.AddWithValue("student_id", (int)row["id"]);
                ds = new DataSet();
                new SqlDataAdapter(comm).Fill(ds);
                var sb = new StringBuilder();
                sb.AppendLine("---------- this is an automatic email -------------");
                foreach (DataRow r in ds.Tables[0].Rows)
                    sb.AppendLine(string.Format("Need to return object: {0}({1})", r["Name"], r["Id"]));
                sb.AppendLine("---------------------------------------------------");
                SendEmailTo((string)row["Email"], sb.ToString());
            }
        }        

        private static void SendEmailTo(string email, string body)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Sending email to: " + email);
            Console.WriteLine(body);
            Console.WriteLine();
            Console.WriteLine();
        }

        #region Helper Methods
        private static void CreateDatabase(string currentDb)
        {
            Console.WriteLine("Creating database...");
            var db = new MyDbContext(currentDb);

            ShowTableData(db.Sections.ToList());
            ShowTableData(db.Students.ToList());
            ShowTableData(db.Assets.ToList());
            ShowTableData(db.Enrollments.ToList());
        }

        private static void ShowTableData<T>(List<T> list)
        {
            var t = typeof(T);
            var tProperties = t.GetProperties();
            Console.WriteLine();
            Console.WriteLine("------- Table: " + t.Name + " -------");
            Console.WriteLine(string.Join(";", tProperties.Select(p => p.Name)));
            list.ForEach(l => Console.WriteLine(string.Join(";", t.GetProperties().Select(p => p.GetValue(l, null)))));
            Console.WriteLine(new string('-', 40));
        }
        #endregion
    }
}
