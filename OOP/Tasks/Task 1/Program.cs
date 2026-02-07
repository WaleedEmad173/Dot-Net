using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Task_1
{
    public class Program
    {
        static void Main(string[] args)
        {
            /* Create BankAccount using Parametrized Constructor */
            BankAccount bankAccount1 = new BankAccount("Ahmed Ali", "1234567890", "555-1234", "123 Main St", 1000.00m);
            bankAccount1.AccountNumber = 12345;
            bankAccount1.ShowAccountDetails();
            /* Create BankAccount using Overloaded Constructor */
            BankAccount bankAccount2 = new BankAccount("Mohammed Zaki", "0987654321", "555-5678", "456 Elm St");
            bankAccount2.AccountNumber = 54321;
            bankAccount2.ShowAccountDetails();
        }
    }
}
