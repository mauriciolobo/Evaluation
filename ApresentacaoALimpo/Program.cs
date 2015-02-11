using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ApresentacaoALimpo.Model;

namespace ApresentacaoALimpo
{
    class Program
    {        
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

            foreach (DataRow row in GetDataSet(pendingAssetsStudentsQuery, conn).Tables[0].Rows)
            {
                var sb = new StringBuilder();
                
                sb.AppendLine("---------- this is an automatic email -------------");
                foreach (DataRow r in GetDataSet(pendingAssetsByStudentId, conn, new SqlParameter("student_id", row["Id"])).Tables[0].Rows)
                    sb.AppendLine(string.Format("Need to return object: {0}({1})", r["Name"], r["Id"]));
                sb.AppendLine("---------------------------------------------------");
                
                SendEmailTo((string)row["Email"], sb.ToString());
            }
        }        

        #region Helper Methods

        private static DataSet GetDataSet(string query, SqlConnection conn, params SqlParameter[] parameters)
        {
            var comm = new SqlCommand(query, conn);
            comm.Parameters.AddRange(parameters);
            var ds = new DataSet();
            new SqlDataAdapter(comm).Fill(ds);
            return ds;
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
