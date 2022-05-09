using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting
{
    public class AccountingRecord
    {
        public int Id { get; set; }
        public Account? DebitAccount { get; set; }
        public double Debit { get; set; }
        public Account? CreditAccount { get; set; }
        public double Credit { get; set; }
    }
}
