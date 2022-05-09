using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting.Services
{
    public class AccountsService
    {
        ApplicationContext db;
        
        public AccountsService(ApplicationContext applicationContext)
        {
            db = applicationContext;
        }
        
        public bool isExist(Account candidate)
        {
            var account = db.Accounts
                   .FirstOrDefault(item => item.Name == candidate.Name);

            if (account == null)
            {
                return false;
            }
            return true;
        }

        public int findIndexByName(string name)
        {
            var account = db.Accounts
                   .FirstOrDefault(item => item.Name == name);

            if (account == null)
            {
                return -1;
            }

            return account.Id;
        }

        public List<Account> getAccountsList()
        {
            var accounts = db.Accounts.ToList();
            return accounts;
        }

        public Account? getAccountById(int id)
        {
            var position = db.Accounts
                   .FirstOrDefault(item => item.Id == id);
            
            return position;
        }

        public void addAccount(Account account)
        {
            db.Accounts.Add(account); 
            db.SaveChanges();
        }

        public void updateAccount(int id, Account account)
        {
            var accountToUpdate = db.Accounts
                .FirstOrDefault(item => item.Id == id);
            
            if (accountToUpdate != null) 
            {
                accountToUpdate.Name = account.Name;
                accountToUpdate.Description = account.Description;
            }

            db.SaveChanges();
        }

        public void deleteAccount(int id)
        {
            db.Accounts.Remove(getAccountById(id));
            db.SaveChanges();
        }
    }
}
