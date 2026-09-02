using System;
using SavingAccountTypesDictionary = Dictionary<SavingAccountType, (decimal withdrawLimit, uint operationLimit, decimal overLimitWithdrawPenalty, decimal lowerInterestRate, uint lowerInterestDelay)>;

public enum SavingAccountType
{
    AnytimeAvailable,
    LongTerm,
    ContiniousUse
}

public class SavingAccount : BankAccount
{
    private decimal _withdrawLimit; // не более withdrawLimit рублей снятие за операцию
    private uint _operationLimit; // не более operationLimit снятий за месяц. Превышение
    private uint _operationsCounter = 0; // количество произведенных операций за текущий месяц
    private DateOnly? _lastOperationDate = null, _lastOverlimitOperationDate = null;
    private decimal _overlimitWithdrawPenalty; // Штраф за снятие вне лимита
    private decimal _lowerInterestRate; // Процентная ставка, которая будет применена при большом количестве снятий вне лимита
    private uint _lowerInterestDelay; // Количество снятий за год до применения пониженной ставки
    

    public uint WithdrawLimit{get;}
    public uint OperationLimit{get;}

    private static readonly SavingAccountTypesDictionary _accountTypes = new SavingAccountTypesDictionary
        {
            {SavingAccountType.AnytimeAvailable, ()}
        };

    public SavingAccount(SavingAccountType accountType)
    {
        
    }

    public override decimal CalculateInterest()
    {
        throw new NotImplementedException();
    }
}