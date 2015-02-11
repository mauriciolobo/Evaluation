using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ApresentacaoALimpo.Model
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(string currentDb)
            : base(currentDb)
        {
            Database.SetInitializer(new DatabaseCreator());
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Section> Sections { get; set; }
    }

    public class Enrollment
    {
        public int Id { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? ExitDate { get; set; }
        public Section Section { get; set; }
        public Student Student { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? Checkin { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Checkout { get; set; }
        public Student WithStudent { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }
    }

    class DatabaseCreator : DropCreateDatabaseIfModelChanges<MyDbContext>
    {
        private Section math = new Section { Name = "Math" };
        private Section english = new Section { Name = "English" };
        private Student mauricio = new Student { FirstName = "Mauricio", LastName = "Lobo", Email = "mau@lobo.com" };
        private Student mayara = new Student { FirstName = "Mayara", LastName = "Sa", Email = "mayara@sa.com" };
        private Student oldStudent = new Student { FirstName = "Old", LastName = "Student", Email = "old@student.com" };

        protected override void Seed(MyDbContext context)
        {
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Book", WithStudent = mauricio });
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Pencil", WithStudent = mauricio });
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Tablet", WithStudent = mauricio });
            context.Enrollments.Add(new Enrollment { Section = math, StartDate = DateTime.Now.AddMonths(-3), Student = mauricio });
            context.Enrollments.Add(new Enrollment { Section = english, StartDate = DateTime.Now.AddMonths(-3), Student = mauricio });

            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Book", WithStudent = mayara });
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Pen", WithStudent = mayara, Checkin = DateTime.Now.AddDays(5) });
            context.Enrollments.Add(new Enrollment { Section = math, StartDate = DateTime.Now.AddMonths(-3), Student = mayara, ExitDate = DateTime.Now.AddDays(-15) });
            context.Enrollments.Add(new Enrollment { Section = english, StartDate = DateTime.Now.AddMonths(-3), Student = mayara });

            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Pen", WithStudent = oldStudent });
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Book", WithStudent = oldStudent });
            context.Assets.Add(new Asset { Checkout = DateTime.Now.AddDays(-30), Name = "Tablet", WithStudent = oldStudent, Checkin = DateTime.Now.AddDays(-2) });
            context.Enrollments.Add(new Enrollment { Section = math, StartDate = DateTime.Now.AddMonths(-3), Student = oldStudent, ExitDate = DateTime.Now.AddDays(-18) });
            context.Enrollments.Add(new Enrollment { Section = english, StartDate = DateTime.Now.AddMonths(-3), Student = oldStudent, ExitDate = DateTime.Now.AddDays(-13) });

            base.Seed(context);
        }
    }
}