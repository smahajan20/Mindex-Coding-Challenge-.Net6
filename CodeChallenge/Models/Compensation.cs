using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        public String CompensationId { get; set; }

        [ForeignKey("Employee")]
        public String EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public double Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}