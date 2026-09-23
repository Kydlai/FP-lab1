﻿using System;
using System.Globalization;

Bank bank = new Bank("Kydlai Bank");
SeedDemoData(bank);

while(true)
{
    Console.WriteLine();
    Console.WriteLine("=== " + bank.Name + " ===");
    Console.WriteLine("1.  List all accounts");
    Console.WriteLine("2.  Create SavingAccount");
    Console.WriteLine("3.  Create CheckingAccount");
    Console.WriteLine("4.  Create FixedDepositAccount");
    Console.WriteLine("5.  Deposit");
    Console.WriteLine("6.  Withdraw");
    Console.WriteLine("7.  Transfer");
    Console.WriteLine("8.  Calculate interest (all)");
    Console.WriteLine("9.  Find account");
    Console.WriteLine("10. Remove account");
    Console.WriteLine("0.  Exit");
    Console.Write("> ");
    var choice = Console.ReadLine();
    if(choice == "0" || choice is null)
        break;
    try
    {
        switch(choice)
        {
            case "1": bank.ProcessAllAccounts(); break;
            case "2": CreateSaving(bank); break;
            case "3": CreateChecking(bank); break;
            case "4": CreateFixed(bank); break;
            case "5": DepositMenu(bank); break;
            case "6": WithdrawMenu(bank); break;
            case "7": TransferMenu(bank); break;
            case "8": CalcInterestAll(bank); break;
            case "9": FindMenu(bank); break;
            case "10": RemoveMenu(bank); break;
            default: Console.WriteLine("unknown command"); break;
        }
    }
    catch(Exception e)
    {
        Console.WriteLine("ERROR: " + e.Message);
        Console.Write("continue? (y/n): ");
        var c = Console.ReadLine();
        if(c is null || (c != "y" && c != "Y" && c != "yes"))
            break;
    }
}
return;

static void SeedDemoData(Bank bank)
{
    bank.CreateSavingAccount(SavingAccountType.AnytimeAvailable, "Alice", 100000m);
    bank.CreateSavingAccount(SavingAccountType.ContiniousUse,  "Bob",    50000m);
    bank.CreateCheckingAccount("Charlie", 25000m);
    bank.CreateFixedDepositAccount("Diana", 200000m, 12, 14.5m);
}

static string ReadString(string prompt)
{
    Console.Write(prompt + ": ");
    var s = Console.ReadLine();
    if(s is null || s.Length == 0)
        throw new ArgumentException(prompt + " must exist");
    return s;
}

static decimal ReadDecimal(string prompt)
{
    var s = ReadString(prompt);
    if(!decimal.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
        throw new ArgumentException("invalid number: " + s);
    return v;
}

static uint ReadUint(string prompt)
{
    var s = ReadString(prompt);
    if(!uint.TryParse(s, out var v))
        throw new ArgumentException("invalid uint: " + s);
    return v;
}

static int ReadAccountNumber()
{
    var s = ReadString("accountNumber");
    if(!int.TryParse(s, out var v))
        throw new ArgumentException("accountNumber must be integer, got: " + s);
    return v;
}

static BankAccount RequireAccount(Bank bank, int number)
{
    return bank.FindAccount(number) ?? throw new ArgumentException($"account #{number} not found");
}

static void CreateSaving(Bank bank)
{
    Console.WriteLine("Types: 0=AnytimeAvailable 1=LongTerm 2=ContiniousUse");
    var t = (SavingAccountType)ReadUint("type");
    var owner = ReadString("ownerName");
    var bal = ReadDecimal("initial balance");
    bank.CreateSavingAccount(t, owner, bal);
}

static void CreateChecking(Bank bank)
{
    var owner = ReadString("ownerName");
    var bal = ReadDecimal("initial balance");
    var od = ReadDecimal("overdraftLimit (default 50000)");
    bank.CreateCheckingAccount(owner, bal, od == 0 ? 50000m : od);
}

static void CreateFixed(Bank bank)
{
    var owner = ReadString("ownerName");
    var bal = ReadDecimal("initial balance");
    var term = ReadUint("termMonths");
    var rate = ReadDecimal("interestRate (e.g. 14.5)");
    bank.CreateFixedDepositAccount(owner, bal, term, rate);
}

static void DepositMenu(Bank bank)
{
    var num = ReadAccountNumber();
    var amt = ReadDecimal("amount");
    RequireAccount(bank, num).Deposit(amt);
}

static void WithdrawMenu(Bank bank)
{
    var num = ReadAccountNumber();
    var amt = ReadDecimal("amount");
    var acc = RequireAccount(bank, num);
    if(acc is SavingAccount sa)
        sa.Withdraw(amt);
    else
        acc.Withdraw(amt);
}

static void TransferMenu(Bank bank)
{
    var from = ReadAccountNumber();
    var to   = ReadAccountNumber();
    var amt  = ReadDecimal("amount");
    bank.Transfer(from, to, amt);
}

static void CalcInterestAll(Bank bank)
{
    Console.WriteLine($"Calculating interest for {bank.GetAllAccounts().Count} account(s) in '{bank.Name}':");
    foreach(var acc in bank.GetAllAccounts())
        Console.WriteLine($"  #{acc.AccountNumber} [{acc.GetType().Name}] balance={acc.Balance} interest={acc.CalculateInterest():F2}");
}

static void FindMenu(Bank bank)
{
    var num = ReadAccountNumber();
    var acc = bank.FindAccount(num);
    if(acc is null)
        Console.WriteLine($"Account #{num} not found");
    else
    {
        Console.WriteLine($"Account #{acc.AccountNumber} found:");
        Console.WriteLine($"  type:    {acc.GetType().Name}");
        Console.WriteLine($"  owner:   {acc.OwnerName}");
        Console.WriteLine($"  balance: {acc.Balance}");
        Console.WriteLine($"  interest (1 year): {acc.CalculateInterest():F2}");
    }
}

static void RemoveMenu(Bank bank)
{
    var num = ReadAccountNumber();
    if(!bank.RemoveAccount(num))
        Console.WriteLine($"Account #{num} not found");
}