using System;
using System.Collections.Generic;

namespace DCIT318_Q1
{
    // record representing a transaction (immutable)
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // processors interface
    public interface ITransactionProcessor { void Process(Transaction transaction); }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:d}.");
        }
    }

    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Transferring {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Broadcasting {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}.");
        }
    }

    // Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // base behaviour: deduct amount
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Applied {transaction.Amount:C} for {transaction.Category}. New balance: {Balance:C}");
        }
    }

    // Sealed specialized account - savings
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                base.ApplyTransaction(transaction);
            }
        }
    }

    // App that runs the scenario
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== FinanceApp Demo ===");
            var account = new SavingsAccount("SA-1001", 1000m);

            var t1 = new Transaction(1, DateTime.Now, 120m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 900m, "Entertainment");

            ITransactionProcessor p1 = new MobileMoneyProcessor();
            ITransactionProcessor p2 = new BankTransferProcessor();
            ITransactionProcessor p3 = new CryptoWalletProcessor();

            p1.Process(t1);
            account.ApplyTransaction(t1);
            _transactions.Add(t1);

            p2.Process(t2);
            account.ApplyTransaction(t2);
            _transactions.Add(t2);

            p3.Process(t3);
            account.ApplyTransaction(t3); // may print Insufficient funds if not enough
            _transactions.Add(t3);

            Console.WriteLine("\nTransactions recorded:");
            foreach (var tx in _transactions)
                Console.WriteLine(tx);

            Console.WriteLine("=== End FinanceApp Demo ===");
        }
    }

    class Program
    {
        static void Main()
        {
            new FinanceApp().Run();
        }
    }
}
