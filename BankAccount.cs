using System;
using System.Reflection.Metadata;

public abstract class BankAccount
{
    private int _accountNumber;
    private string? _ownerName;
    private decimal? _balance;

    public int AccountNumber{get => _accountNumber;}
    public decimal? Balance
    {
        get => _balance;
        protected set
        {
            if(value is null)
                throw new ArgumentException("balance must exist");
            ValidateBalance(value.Value);
            _balance = value;
        }
    }
    public string? OwnerName
    {
        get => _ownerName;
        set
        {
            if(value is null || value.Length == 0)
                throw new ArgumentException("ownerName must exist");
            _ownerName = value;
        }
    }

    protected virtual void ValidateBalance(decimal value)
    {
        if(value < 0)
            throw new ArgumentException("balance can't be negative");
    }

    protected BankAccount(int accountNumber, string ownerName, decimal? balance)
    {
        if(accountNumber <= 0)
            throw new ArgumentException("accountNumber must be positive");
        if(ownerName is null || ownerName.Length == 0)
            throw new ArgumentException("ownerName must exist");
        if(balance is null)
            throw new ArgumentException("balance must exist");
        _accountNumber = accountNumber;
        _ownerName = ownerName;
        _balance = balance;
    }

    public abstract decimal CalculateInterest();

    public virtual void Deposit(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("deposit must be more than 0");
        _balance += amount;
        Console.WriteLine($"Account #{_accountNumber}: deposited {amount} roubles, new balance = {_balance}");
    }

    public virtual void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("amount must be more than 0");
        if(amount > _balance)
            throw new ArgumentException($"insufficient funds: balance = {_balance}, requested = {amount}");
        _balance -= amount;
        Console.WriteLine($"Account #{_accountNumber}: withdrew {amount} roubles, new balance = {_balance}");
    }
}