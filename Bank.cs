using System;
using System.Collections.Generic;
using System.Linq;

public class Bank
{
    private readonly Dictionary<int, BankAccount> _accounts = new Dictionary<int, BankAccount>();
    private int _nextAccountNumber = 1;
    public string Name{get; init;}

    public Bank(string name)
    {
        if(name is null || name.Length == 0)
            throw new ArgumentException("bank name must exist");
        Name = name;
    }

    public SavingAccount CreateSavingAccount(SavingAccountType type, string ownerName, decimal? balance)
    {
        var acc = new SavingAccount(type, _nextAccountNumber, ownerName, balance);
        AddAccount(acc);
        return acc;
    }

    public CheckingAccount CreateCheckingAccount(string ownerName, decimal? balance, decimal overdraftLimit = 50000m, decimal interestRate = 0.5m)
    {
        var acc = new CheckingAccount(_nextAccountNumber, ownerName, balance, overdraftLimit, interestRate);
        AddAccount(acc);
        return acc;
    }

    public FixedDepositAccount CreateFixedDepositAccount(string ownerName, decimal? balance, uint termMonths, decimal interestRate, decimal earlyWithdrawPenalty = 0.1m)
    {
        var acc = new FixedDepositAccount(_nextAccountNumber, ownerName, balance, termMonths, interestRate, earlyWithdrawPenalty);
        AddAccount(acc);
        return acc;
    }

    private void AddAccount(BankAccount account)
    {
        if(_accounts.ContainsKey(account.AccountNumber))
            throw new ArgumentException($"account #{account.AccountNumber} already exists");
        _accounts[account.AccountNumber] = account;
        _nextAccountNumber++;
        Console.WriteLine($"Bank '{Name}': created account #{account.AccountNumber} ({account.GetType().Name}) for '{account.OwnerName}' with initial balance {account.Balance}");
    }

    public BankAccount? FindAccount(int accountNumber)
    {
        if(accountNumber <= 0)
            return null;
        return _accounts.TryGetValue(accountNumber, out var acc) ? acc : null;
    }

    public bool RemoveAccount(int accountNumber)
    {
        if(accountNumber <= 0)
            return false;
        if(_accounts.TryGetValue(accountNumber, out var acc))
        {
            _accounts.Remove(accountNumber);
            Console.WriteLine($"Bank '{Name}': closed account #{accountNumber} ({acc.GetType().Name}) for '{acc.OwnerName}', final balance = {acc.Balance}");
            return true;
        }
        return false;
    }

    public void Transfer(int fromNumber, int toNumber, decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("transfer amount must be more than 0");
        var from = FindAccount(fromNumber) ?? throw new ArgumentException($"source account #{fromNumber} not found");
        var to   = FindAccount(toNumber)   ?? throw new ArgumentException($"target account #{toNumber} not found");
        Console.WriteLine($"Transfer: {amount} roubles from #{fromNumber} to #{toNumber}");
        if(from is SavingAccount sa)
            sa.Withdraw(amount);
        else
            from.Withdraw(amount);
        to.Deposit(amount);
        Console.WriteLine($"Transfer completed: #{fromNumber} balance = {from.Balance}, #{toNumber} balance = {to.Balance}");
    }

    public IReadOnlyCollection<BankAccount> GetAllAccounts() => _accounts.Values;

    public void ProcessAllAccounts()
    {
        Console.WriteLine($"Bank '{Name}': {_accounts.Count} account(s)");
        foreach(var acc in _accounts.Values)
        {
            Console.WriteLine($"  #{acc.AccountNumber} [{acc.GetType().Name}] owner='{acc.OwnerName}' balance={acc.Balance} interest={acc.CalculateInterest():F2}");
        }
    }
}