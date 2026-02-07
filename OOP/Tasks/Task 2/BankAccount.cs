using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class BankAccount
    {
        /* Fields */
        const string BankCode = "BNK001";
        readonly DateTime CreatedDate;
        private int _accountNumber;
        private string _fullName;
        private string _nationalID;
        private string _phoneNumber;
        private string _address;
        protected decimal _balance;
        /* Constructors */
        public BankAccount()
        {
            CreatedDate = DateTime.Now;
            _accountNumber = 0;
            _fullName = string.Empty;
            _nationalID = string.Empty;
            _phoneNumber = string.Empty;
            _address = string.Empty;
            _balance = 0;
        }
        public BankAccount(string fullName, string nationalID, string phoneNumber, string address, decimal balance)
        {
            CreatedDate = DateTime.Now;
            _accountNumber = 0;
            _fullName = fullName;
            _nationalID = nationalID;
            _phoneNumber = phoneNumber;
            _address = address;
            _balance = balance;
        }
        public BankAccount(string fullName, string nationalID, string phoneNumber, string address)
        {
            CreatedDate = DateTime.Now;
            _accountNumber = 0;
            _fullName = fullName;
            _nationalID = nationalID;
            _phoneNumber = phoneNumber;
            _address = address;
            _balance = 0;
        }
        /* Methods */
        public int AccountNumber
        {
            get { return _accountNumber; }
            set { _accountNumber = value; }
        }
        public virtual void ShowAccountDetails()
        {
            Console.WriteLine("Bank Account Details:");
            Console.WriteLine($"Bank Code: {BankCode}");
            Console.WriteLine($"Account Number: {_accountNumber}");
            Console.WriteLine($"Full Name: {_fullName}");
            Console.WriteLine($"National ID: {_nationalID}");
            Console.WriteLine($"Phone Number: {_phoneNumber}");
            Console.WriteLine($"Address: {_address}");
            Console.WriteLine($"Balance: {_balance:C}");
            Console.WriteLine($"Created Date: {CreatedDate}");
        }
        /* Returns true if the national ID is exactly 14 digits. */
        public bool IsValidNationalID()
        {
            return !string.IsNullOrEmpty(_nationalID) && _nationalID.Length == 14;
        }
        /* Returns true if the phone starts with "01" and is 11 digits. */
        public bool IsValidPhoneNumber()
        {
            return !string.IsNullOrEmpty(_phoneNumber) && _phoneNumber.StartsWith("01") && _phoneNumber.Length == 11;
        }
    }
}
