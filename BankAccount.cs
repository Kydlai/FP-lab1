using System;
using System.Reflection.Metadata;

public abstract class BankAccount
{
    private string? _accountNumber;
    private string? _ownerName;
    private decimal? _balance;

    private string AccountNumber{get;}
    private decimal? balance
    {
        get; 
        set
        {
            if(balance is null)
                throw new ArgumentException("balance must exist");
            else if(balance < 0)
                throw new ArgumentException("balance can't be negative");

            else
                _balance = value;

        }
    }
    private string OwnerName
    {
        get; 
        set{
            if(value is null || value.Length == 0)
                throw new ArgumentException("ownerName must exist");
            else
                _ownerName = value;
        }
    }

    public abstract decimal calculateInterest();
    public virtual void deposit(decimal amount)
    {
        
    }

 
}