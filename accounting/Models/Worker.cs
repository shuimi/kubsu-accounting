using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting
{
    public class Worker
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Middlename { get; set; }
        public string EmploymentDate { get; set; }
        public string ContractId { get; set; }
        public string Status { get; set; }
        public List<Position> Positions { get; set; } = new List<Position>();
    }
}
