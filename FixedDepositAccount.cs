using System;

public class FixedDepositAccount : BankAccount
{
    /// <summary>
    /// Срок вклада в месяцах
    /// </summary>
    public uint TermMonths{get; init;}
    /// <summary>
    /// Дата открытия вклада
    /// </summary>
    public DateOnly OpenDate{get; init;}
    /// <summary>
    /// Дата окончания вклада (после неё можно снимать без штрафа)
    /// </summary>
    public DateOnly MaturityDate{get; init;}
    /// <summary>
    /// Фиксированная годовая процентная ставка
    /// </summary>
    public decimal InterestRate{get; init;}
    /// <summary>
    /// Доля штрафа от суммы при досрочном снятии (от 0 до 1)
    /// </summary>
    public decimal EarlyWithdrawPenalty{get; init;}

    public FixedDepositAccount(int accountNumber, string ownerName, decimal? balance, uint termMonths, decimal interestRate, decimal earlyWithdrawPenalty = 0.1m)
        : base(accountNumber, ownerName, balance)
    {
        if(balance < 0)
            throw new ArgumentException("balance can't be negative");
        if(termMonths == 0)
            throw new ArgumentException("termMonths must be more than 0");
        if(interestRate < 0)
            throw new ArgumentException("interestRate can't be negative");
        if(earlyWithdrawPenalty < 0 || earlyWithdrawPenalty > 1)
            throw new ArgumentException("earlyWithdrawPenalty must be between 0 and 1");
        TermMonths = termMonths;
        InterestRate = interestRate;
        EarlyWithdrawPenalty = earlyWithdrawPenalty;
        OpenDate = DateOnly.FromDateTime(DateTime.Now);
        MaturityDate = OpenDate.AddMonths((int)termMonths);
    }

    public bool IsMatured => DateOnly.FromDateTime(DateTime.Now) >= MaturityDate;

    public override decimal CalculateInterest()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        if(today <= OpenDate)
            return 0m;
        int monthsElapsed = (today.Year - OpenDate.Year) * 12 + (today.Month - OpenDate.Month);
        if(today.Day < OpenDate.Day)
            monthsElapsed--;
        if(monthsElapsed <= 0)
            return 0m;
        decimal rate = IsMatured ? InterestRate : InterestRate / 2m;
        return Balance!.Value * rate / 100m * monthsElapsed / 12m;
    }

    public override void Deposit(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("deposit must be more than 0");
        if(IsMatured)
            throw new InvalidOperationException($"can't deposit into a matured fixed deposit (maturity date {MaturityDate})");
        Balance = Balance + amount;
        Console.WriteLine($"Account #{AccountNumber}: deposited {amount} roubles, new balance = {Balance}");
        Console.WriteLine($"  Fixed deposit: opened {OpenDate}, matures {MaturityDate} ({TermMonths} months)");
    }

    public override void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("amount must be more than 0");
        decimal penalty = 0m;
        if(!IsMatured)
        {
            penalty = amount.Value * EarlyWithdrawPenalty;
            Console.WriteLine($"  WARNING: early withdrawal before maturity date {MaturityDate}");
            Console.WriteLine($"  Penalty: {penalty} roubles ({EarlyWithdrawPenalty:P0} of {amount})");
        }
        decimal total = amount.Value + penalty;
        if(total > Balance)
            throw new ArgumentException($"insufficient funds: balance = {Balance}, requested = {amount}, total with penalty = {total}");
        Balance = Balance - total;
        Console.WriteLine($"Account #{AccountNumber}: withdrew {amount} roubles" +
            (penalty > 0 ? $" (+penalty {penalty})" : "") +
            $", new balance = {Balance}");
    }
}