using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class SavingAccount : BankAccount
    {
        public decimal  InterestRate { get; set; }
        /* Constructors */
        public SavingAccount() : base()
        {
            InterestRate = 0;
        }
        public SavingAccount(string fullName, string nationalID, string phoneNumber, string address, decimal balance, decimal interestRate)
            : base(fullName, nationalID, phoneNumber, address, balance)
        {
            InterestRate = interestRate;
        }
        public SavingAccount(string fullName, string nationalID, string phoneNumber, string address, decimal interestRate)
            : base(fullName, nationalID, phoneNumber, address)
        {
            InterestRate = interestRate;
        }
        public override void ShowAccountDetails()
        {
            base.ShowAccountDetails();
            Console.WriteLine($"Interest Rate: {InterestRate}");
        }
        public decimal CalculateInterest()
        {
            return _balance * InterestRate / 100;
        }
    }
}
