using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
namespace NoiseMonitoring.Utils
{
    class Context:DbContext
    {
        public Context():base(nameOrConnectionString: "MyDbContextConnectionString")
        {
            Database.SetInitializer<Context>(new MyDbInitializer());
        }
        public DbSet<Employee> Employees { get; set; }
    }

    class MyDbInitializer: CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            context.Employees.Add(new Employee() { id = 1, Name = "Trung" });
            base.Seed(context);
        }
    }
}
