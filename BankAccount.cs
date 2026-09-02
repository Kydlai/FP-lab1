using System;
using System.Reflection.Metadata;

public abstract class BankAccount
{
    private string? _accountNumber;
    private string? _ownerName;
    private decimal? _balance;

    public string? AccountNumber{get => _accountNumber;}
    public decimal? Balance
    {
        get => _balance; 
        set
        {
            if(_balance is null)
                throw new ArgumentException("balance must exist");
            else if(_balance < 0)
                throw new ArgumentException("balance can't be negative");

            else
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
            else
                _ownerName = value;
        }
    }

    public abstract decimal CalculateInterest();
    public virtual void Deposit(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("deposit must be more than 0");
        _balance += amount;
        Console.WriteLine("A deposit of " + amount + " roubles been made to the account");
    }

    public virtual void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("deposit must be more than 0");
        if(amount > _balance)
            throw new ArgumentException("U can't withdraw more than balance");
        _balance -= amount;
        Console.WriteLine(amount + " rubles were withdrawn from the account");
    }

 
}