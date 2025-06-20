using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public int? ParentAccountId { get; set; }
        public Account ParentAccount { get; set; }
        public int Level { get; set; }
        public ICollection<Account> Children { get; set; } = new List<Account>();
    }
}
