using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class Program
    {
        static void Main(string[] args)
        {
            SavingAccount savingAccount = new SavingAccount("Ahmed Ali", "123456789", "555-1234", "123 Main St", 1000, 5);
            CurrentAccount currentAccount = new CurrentAccount("Mohammed Zaki", "987654321", "555-5678", "456 Elm St", 2000, 500);
            List<BankAccount> Accounts = new List<BankAccount>();
            Accounts.Add(savingAccount);
            Accounts.Add(currentAccount);
            foreach (BankAccount account in Accounts)
            {
                account.ShowAccountDetails();
                if (account is SavingAccount)
                {
                    SavingAccount sa = (SavingAccount)account;
                    Console.WriteLine($"Interest: {sa.CalculateInterest()}");
                }
                Console.WriteLine();
            }
        }
    }
}
