using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class CurrentAccount : BankAccount
    {
        public decimal OverdraftLimit { get; set; }

        /* Constructors */
        /* Default constructor */
        public CurrentAccount() : base()
        {
            OverdraftLimit = 0;
        }
        /* Parametrized With Balance */
        /* public BankAccount(string fullName, string nationalID, string phoneNumber, string address, decimal balance) */
        public CurrentAccount(string fullName, string nationalID, string phoneNumber, string address, decimal balance, decimal overdraftLimit)
            : base(fullName, nationalID, phoneNumber, address, balance)
        {
            OverdraftLimit = overdraftLimit;
        }
        /* Overloaded Constructor Without */
        /* public BankAccount(string fullName, string nationalID, string phoneNumber, string address) */
        public CurrentAccount(string fullName, string nationalID, string phoneNumber, string address, decimal overdraftLimit)
            : base(fullName, nationalID, phoneNumber, address)
        {
            OverdraftLimit = overdraftLimit;
        }
        public override void ShowAccountDetails()
        {
            base.ShowAccountDetails();
            Console.WriteLine($"Overdraft Limit: {OverdraftLimit}");
        }
    }
}
