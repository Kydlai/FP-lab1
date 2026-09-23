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
    private uint _overlimitCounterThisYear;
    private uint _totalOverlimitOperations;
    private DateOnly? _overlimitYearStart;
    private static readonly SavingAccountTypesDictionary _accountTypes = new SavingAccountTypesDictionary
        {
            {SavingAccountType.AnytimeAvailable, (500000m, null, null, 7.2m, null, null)},
            {SavingAccountType.ContiniousUse, (100000m, 20, 0.05m, 8.9m, 6.9m, 20)},
            {SavingAccountType.LongTerm, (50000m, 3, 0.25m, 11.6m, 7.2m, 5)}
        };

    public SavingAccount(SavingAccountType accountType, int accountNumber, string ownerName, decimal? balance)
        : base(accountNumber, ownerName, balance)
    {
        if(balance < 0)
            throw new ArgumentException("balance can't be negative");
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
        if(LowerInterestRate is null || LowerInterestDelay is null)
            return Balance!.Value * InterestRate / 100m;
        if(_totalOverlimitOperations >= LowerInterestDelay.Value)
            return Balance!.Value * LowerInterestRate.Value / 100m;
        return Balance!.Value * InterestRate / 100m;
    }

    public new void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("amount must be more than 0");
        if(amount > Balance)
            throw new ArgumentException($"insufficient funds: balance = {Balance}, requested = {amount}");

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        ResetMonthlyCounterIfNeeded(today);
        ResetYearlyOverlimitIfNeeded(today);

        bool overLimitByAmount = WithdrawLimit is not null && amount > WithdrawLimit;
        bool overLimitByCount  = OperationLimit is not null && OperationCounter >= OperationLimit;

        if(overLimitByAmount)
            Console.WriteLine($"  WARNING: requested amount {amount} exceeds WithdrawLimit {WithdrawLimit}");
        if(overLimitByCount)
            Console.WriteLine($"  WARNING: monthly OperationLimit {OperationLimit} reached ({OperationCounter} operations)");

        decimal penalty = 0m;
        if(overLimitByAmount || overLimitByCount)
        {
            if(OverlimitWithdrawPenalty is not null)
                penalty = amount.Value * OverlimitWithdrawPenalty.Value;
            _overlimitCounterThisYear++;
            _totalOverlimitOperations++;
            LastOverlimitOperationDate = today;
            if(_overlimitYearStart is null)
                _overlimitYearStart = today;
            Console.WriteLine($"  Penalty applied: {penalty} roubles ({OverlimitWithdrawPenalty:P0} of {amount})");
            Console.WriteLine($"  Total over-limit operations this year: {_overlimitCounterThisYear}, total: {_totalOverlimitOperations}");
            if(LowerInterestDelay is not null && _totalOverlimitOperations >= LowerInterestDelay.Value)
                Console.WriteLine($"  NOTE: interest rate lowered to {LowerInterestRate}% (after {LowerInterestDelay} over-limit ops)");
        }

        Balance = Balance - amount - penalty;
        OperationCounter++;
        LastOperationDate = today;
        Console.WriteLine($"Account #{AccountNumber}: withdrew {amount} roubles" +
            (penalty > 0 ? " (+penalty " + penalty + ")" : "") +
            $", new balance = {Balance}, monthly ops = {OperationCounter}");
    }

    private void ResetMonthlyCounterIfNeeded(DateOnly today)
    {
        if(LastOperationDate is null)
            return;
        if(LastOperationDate.Value.Year != today.Year || LastOperationDate.Value.Month != today.Month)
        {
            Console.WriteLine($"  New month: monthly OperationCounter reset from {OperationCounter} to 0");
            OperationCounter = 0;
        }
    }

    private void ResetYearlyOverlimitIfNeeded(DateOnly today)
    {
        if(_overlimitYearStart is null)
            return;
        if(_overlimitYearStart.Value.Year != today.Year)
        {
            Console.WriteLine($"  New year: yearly over-limit counter reset from {_overlimitCounterThisYear} to 0");
            _overlimitCounterThisYear = 0;
            _overlimitYearStart = today;
        }
    }
}