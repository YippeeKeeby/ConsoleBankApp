/*
BankApp: Single Flow Console Applicaiton
Main -> Welcome -> Authentication & Authorization
Stakeholders: Admin and Customer
2dashboard/menus: Admin & Customer Menu
Operations: CRUD(Add Customer, Delete, Update, View All Accounts)
*/

#region Main
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

Main.main();

public class Main{
    static List<Customer> customers = new List<Customer>();
    static String state = "";
    static Admin admin = new Admin();

    public static void main()
    {
        makeCustomers(3);
        welcome();

        while (state != "Exit")
        {
            Console.WriteLine("Type Exit to quit or press Enter to continue.");
            state = Console.ReadLine() ?? string.Empty;
        }
    }
    public static void welcome()
    {
        Console.WriteLine("Welcome to ABC Digital Bank!");

        User result = login();

       if (result is Admin){
            adminDashboard();
       }
       else if (result is Customer)
        {
            customerDashboard(result);
        }
        else{
            Console.WriteLine("None");
        }

    }
    private static User login()
    {
        Console.WriteLine("Please Enter username & password (seperate with a space)");
        //take in input
        String all = Console.ReadLine();
        int split_index = all.IndexOf(" ");

        if (split_index == -1)
        {
            return "Invalid input format. Please enter username and password separated by a space.";
        }

        String username = all.Substring(0, split_index);
        String password = all.Substring(split_index + 1);

        if (username.Equals("admin") && password.Equals("admin123")){
            return admin;
        }

        foreach (Customer customer in customers)
        {
            if (customer.verify(username, password))
            {
                return customer;
            }
        }

        return null;
    }

    private static void adminDashboard()
    {
        Console.WriteLine("Not Admin implemented yet");
    }

    private static void customerDashboard(Customer c){
        String choice = "";

        while(choice != "7"){
            Console.WriteLine("1) Create Account\n2) View All Accounts\n3) Deposit\n4) Withdraw\n5) Transfer\n6) Close Account\n7) Exit.");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Will this be a checking or saving account? (c for checking, s for savings)");
                    String which = Console.ReadLine();

                    if (which == "c"){
                        c.AddAccount("Checking");
                    }
                    else if (which == "s")
                    {
                        c.AddAccount("Saving");
                    }

                case "2":
                    foreach (Account account in c.accounts){
                        account.PrintAccount();
                    }
                case "3":
                    Console.WriteLine("Where are you Depositing from?");

                    Console.WriteLine("Where will you Deposit from?");
                case "4":
                    Console.WriteLine("Where are you Withdrawing from?");
                case "5":
                    Console.WriteLine("Where are you Transfering from?");
                case "6":
                    Console.WriteLine("Which account will you close?");
            }
        }
        


    }

    private static void makeCustomers(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Customer c = makeSingleCustomer(i + 1);
            customers.Add(c);
        }
    }

    private static Customer makeSingleCustomer(int index)
    {
        return new Customer(index, "Jane", $"Doe {index}", $"jane_doe{index}", "1234");
    }
}
#endregion

#region Account
abstract class Account
{
    protected int id;
    protected double balance;
    public abstract string AccountType { get; }

    public Account(int id, double balance){
        this.id = id;
        this.balance = 0;
    }
    public virtual void Deposit(double amount)
    {
        balance += amount;
    }

    public abstract void Withdraw(double amount);

    public abstract double AddInterest();

    public virtual void PrintAccount()
    {
        Console.WriteLine($"{AccountType}\t|\t{id}\t|\t{balance}");
    }
}

class CheckingAccount : Account
{
    public override string AccountType => "CheckingAccount";
    private decimal overdraft_limit = 0.25m;

    public CheckingAccount(int id){
        this.id = id;
        this.balance = 0;
    }
    public override void Withdraw(double amount)
    {
        double new_balance = balance - amount;

        if (new_balance < 0 && Math.Abs(new_balance) > (double)overdraft_limit)
        {
            Console.WriteLine("Withdrawing too much");
        }
        else
        {
            balance = new_balance;
        }
    }

    public override double AddInterest()
    {
        return 0.0;
    }
}

class SavingsAccount : Account
{
    public override string AccountType => "SavingsAccount";
    private double interest_rate = 0.02;


    public SavingsAccount(int id){
        this.id = id;
        this.balance = 0;
    }
    public override void Withdraw(double amount)
    {
        double new_balance = balance - amount;

        if (new_balance >= 100)
        {
            balance = new_balance;
        }
        else
        {
            Console.WriteLine("Can't withdraw that much from account");
        }
    }

    public override double AddInterest()
    {
        return balance * interest_rate;
    }
}
#endregion

#region User
abstract class User
{
    protected String first_name = "";
    protected String last_name = "";
    protected String username = "";
    protected String password = "";
}

class Customer : User
{
    private int id;
    private List<Account> accounts;

    public Customer(int id, String first_name, String last_name, String username, String password, List<Account> accounts)
    {
        this.id = id;
        this.first_name = first_name;
        this.last_name = last_name;
        this.accounts = accounts;
        this.username = username;
        this.password = password;
    }

    public Customer(int id, String first_name, String last_name, String username, String password){
        this.id = id;
        this.first_name = first_name;
        this.last_name = last_name;
        this.username = username;
        this.password = password;
        this.accounts = new List<Account> { new CheckingAccount(), new SavingsAccount() };
    }

    public bool verify(String username, String password)
    {
        return (this.username.Equals(username) && this.password.Equals(password));
    }

    public void AddAccount(String type)
    {
        Account new_account;

        if (type == "Checking"){
            new_account = new CheckingAccount(accounts.Count + 1);
        }
        else if (type == "Saving"){
            new_account = new SavingsAccount(accounts.Count + 1);
        }
        else{
            return;
        }

        this.accounts.Add(new_account);

    }

    private Account getAccount(int id){
        foreach (Account account in this.accounts)
        {
            if (account.id == id){
                return account;
            }
        }

        return null;
    }

    public Account getCandidateAccounts(String action)
    {
        Console.WriteLine($"Where are you {action}ing from?");
        int first_id = Console.ReadLine();
        Account first = getAccount(Regex.Match(first_id, @"\d+").Value);
        if (first == null){
            return;
        }

        Console.WriteLine($"Where will you {action} to?");
        int second_id = Console.ReadLine();
        Account second = getAccount(Regex.Match(second_id, @"\d+").Value);
        if (second == null){
            return;
        }

        return [first, second];
    }
}

class Admin : User
{
    public Admin()
    {
        username = "admin";
        password = "admin123";
    }
}
#endregion
