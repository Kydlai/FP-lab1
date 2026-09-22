using System;
public class CheckingAccount : BankAccount
{
    public override decimal CalculateInterest()
    {
        throw new NotImplementedException();
    }
}