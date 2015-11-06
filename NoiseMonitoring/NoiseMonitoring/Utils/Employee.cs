using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseMonitoring.Utils
{
    [Table("employee")]
    class Employee
    {
        public Employee()
        {

        }
        [Key]
        [Column("id_employee")]
        public int id { get; set; }
        public string Name { get; set; }
    }
}
