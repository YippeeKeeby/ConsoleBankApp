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

        String result = login();

        switch (result)
        {
            case "Admin":
                adminDashboard();
                break;
            case "Customer":
                customerDashboard();
                break;
            default:
                Console.WriteLine(result);
                break;
        }

    }
    private static String login()
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

        return validate(username, password);
    }

    private static String validate(String username, String password)
    {
        if (username.Equals("admin") && password.Equals("admin123")){
            return "Admin";
        }

        foreach (Customer customer in customers)
        {
            if (customer.verify(username, password))
            {
                return "Customer";
            }
        }

        return "None";
    }

    private static void adminDashboard()
    {
        Console.WriteLine("Not Admin implemented yet");
    }

    private static void customerDashboard(){
        Console.WriteLine("Not Customer implemented yet");
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
    protected double balance;
    public abstract string AccountType { get; }

    public virtual void Deposit(double amount)
    {
        balance += amount;
    }

    public abstract void Withdraw(double amount);

    public abstract double AddInterest();
}

class CheckingAccount : Account
{
    public override string AccountType => "CheckingAccount";
    private decimal overdraft_limit = 0.25m;
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
