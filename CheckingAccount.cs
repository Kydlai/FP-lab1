using System;

public class CheckingAccount : BankAccount
{
    /// <summary>
    /// Максимально допустимый отрицательный баланс (овердрафт)
    /// </summary>
    public decimal OverdraftLimit{get; init;}
    /// <summary>
    /// Процентная ставка по текущему счёту (обычно ниже, чем у сберегательного)
    /// </summary>
    public decimal InterestRate{get; init;}

    public CheckingAccount(int accountNumber, string ownerName, decimal? balance, decimal overdraftLimit = 50000m, decimal interestRate = 0.5m)
        : base(accountNumber, ownerName, balance)
    {
        if(overdraftLimit < 0)
            throw new ArgumentException("overdraftLimit can't be negative");
        OverdraftLimit = overdraftLimit;
        InterestRate = interestRate;
        if(balance < -OverdraftLimit)
            throw new ArgumentException($"initial balance can't be less than -overdraftLimit ({-OverdraftLimit})");
    }

    protected override void ValidateBalance(decimal value)
    {
        if(value < -OverdraftLimit)
            throw new ArgumentException($"balance can't be less than -overdraftLimit ({-OverdraftLimit})");
    }

    public override decimal CalculateInterest()
    {
        if(Balance <= 0)
            return 0m;
        return Balance!.Value * InterestRate / 100m;
    }

    public override void Withdraw(decimal? amount)
    {
        if(amount is null || amount <= 0)
            throw new ArgumentException("amount must be more than 0");
        decimal available = Balance!.Value + OverdraftLimit;
        if(amount > available)
            throw new ArgumentException($"insufficient funds: balance = {Balance}, overdraft limit = {OverdraftLimit}, available = {available}, requested = {amount}");
        Balance = Balance - amount;
        Console.WriteLine($"Account #{AccountNumber}: withdrew {amount} roubles" +
            (Balance < 0 ? $" (overdraft used: {-Balance} of {OverdraftLimit})" : "") +
            $", new balance = {Balance}");
    }
}