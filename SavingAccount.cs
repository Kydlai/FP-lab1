using System;
using System.Dynamic;
using SavingAccountTypesDictionary = System.Collections.Generic.Dictionary<SavingAccountType, (decimal? withdrawLimit, uint? operationLimit, decimal? overLimitWithdrawPenalty, decimal interestRate, decimal? lowerInterestRate, uint? lowerInterestDelay)>;

public enum SavingAccountType
{
    AnytimeAvailable,
    LongTerm,
    ContiniousUse
}

public class SavingAccount : BankAccount
{
    /// <summary>
    /// не более withdrawLimit рублей снятие за операцию
    /// </summary>
    public decimal? WithdrawLimit{get; init;} 
    /// <summary>
    /// не более operationLimit снятий за месяц.
    /// </summary>
    public uint? OperationLimit{get; init;}
    /// <summary>
    /// количество произведенных операций за текущий месяц
    /// </summary>
    public uint OperationCounter{get; private set;}
    public DateOnly? LastOperationDate{get; private set;} 
    public DateOnly? LastOverlimitOperationDate{get; private set;}
    /// <summary>
    /// Доля штрафа от суммы за снятие вне лимита, от 0 до 1
    /// </summary>
    public decimal? OverlimitWithdrawPenalty{get; init;}
    public decimal InterestRate{get; init;}
    /// <summary>
    /// Процентная ставка, которая будет применена при большом количестве снятий вне лимита
    /// </summary>
    public decimal? LowerInterestRate{get; init;}
    /// <summary>
    /// Количество превышенных снятий за год до применения пониженной ставки
    /// </summary>
    private uint? LowerInterestDelay{get; init;}
    private static readonly SavingAccountTypesDictionary _accountTypes = new SavingAccountTypesDictionary
        {
            {SavingAccountType.AnytimeAvailable, (500000m, null, null, 7.2m, null, null)},
            {SavingAccountType.ContiniousUse, (100000m, 20, 0.05m, 8.9m, 6.9m, 20)},
            {SavingAccountType.LongTerm, (50000m, 3, 0.25m, 11.6m, 7.2m, 5)}  
        };

    public SavingAccount(SavingAccountType accountType)
    {
        var typeData = _accountTypes[accountType];
        WithdrawLimit = typeData.withdrawLimit;
        OperationLimit = typeData.operationLimit;
        OverlimitWithdrawPenalty = typeData.overLimitWithdrawPenalty;
        InterestRate = typeData.interestRate;
        LowerInterestRate = typeData.lowerInterestRate;
        LowerInterestDelay = typeData.lowerInterestDelay;
    }

    public override decimal CalculateInterest()
    {
        if(OperationCounter > )
    }

    public void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("deposit must be more than 0");
        if(amount > _balance)
            throw new ArgumentException("U can't withdraw more than balance");
        // _balance -= amount;
        Console.WriteLine(amount + " rubles were withdrawn from the account");
    }
}