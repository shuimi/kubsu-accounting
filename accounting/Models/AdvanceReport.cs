using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting
{
    public class AdvanceReport
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public List<AccountingRecord>? AccountingRecords { get; set; } = new List<AccountingRecord>();
        public Worker? Head { get; set; }
        public Worker? AccountablePerson { get; set; }
        public string Appointment { get; set; }
        public Worker? ChiefAccountant { get; set; }
        public Worker? Accountant { get; set; }

        public double getSpent()
        {
            if (AccountingRecords == null) return 0.0;
            return AccountingRecords.Select(record => record.Debit).Sum();
        }

        public double getRecieved()
        {
            if (AccountingRecords == null) return 0.0;
            return AccountingRecords.Select(record => record.Credit).Sum();
        }

        public double getTotal()
        {
            if (AccountingRecords == null) return 0.0;
            return AccountingRecords.Select(record => record.Credit).Sum();
        }
    }
}
