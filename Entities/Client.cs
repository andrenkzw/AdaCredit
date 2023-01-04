using CsvHelper.Configuration;

namespace AdaCredit;
public sealed class Client
{
    public long Document { get; init; }
    public string FullName { get; set; }
    public string Telephone { get; set; }
    public DateTime BirthDate { get; set; }
    public bool Active { get; private set; }
    public Account Account { get; init; }

    public Client(long Document, string FullName, string Telephone, DateTime BirthDate, bool Active,
        int AccountIdentifier, decimal AccountBalance)
    {
        this.Document = Document;
        this.FullName = FullName;
        this.Telephone = Telephone;
        this.BirthDate = BirthDate;
        this.Active = Active;
        this.Account = new Account(AccountIdentifier, AccountBalance);
    }

    public Client(long Document, string FullName, string Telephone, DateTime BirthDate,
        int AccountIdentifier)
    {
        this.Document = Document;
        this.FullName = FullName;
        this.Telephone = Telephone;
        this.BirthDate = BirthDate;
        this.Active = true;
        this.Account = new Account(AccountIdentifier);
    }

    public string PrettyDocument()
        =>  (Document / 100000000).ToString().PadLeft(3, '0') + "." +
            (Document / 100000 % 1000).ToString().PadLeft(3, '0') + "." +
            (Document / 100 % 1000).ToString().PadLeft(3, '0') + "-" +
            (Document % 100).ToString().PadLeft(2, '0');

    public void Deactivate()
    {
        this.Active = false;
    }
}

public sealed class Account
{
    public int Identifier { get; init; }
    private decimal balance;
    public decimal Balance
    {
        get
        {
            return balance;
        }
        set
        {
            if (value < 0) { throw new InvalidOperationException("Saldo não pode ser negativo."); }
            else { balance = value; }
        }
    }

    public Account(int Identifier, decimal Balance)
    {
        this.Identifier = Identifier;
        this.Balance = Balance;
    }

    public Account(int Identifier)
    {
        this.Identifier = Identifier;
        this.Balance = 0;
    }

    public string PrettyIdentifier()
        =>  (Identifier / 10).ToString().PadLeft(5, '0') + "-" + (Identifier % 10);
}

public class ClientLoadMap : ClassMap<Client>
{
    public ClientLoadMap()
    {
        Parameter("Document").Name("Document");
        Parameter("FullName").Name("FullName");
        Parameter("Telephone").Name("Telephone");
        Parameter("BirthDate").Name("BirthDate");
        Parameter("Active").Name("Active");
        Parameter("AccountIdentifier").Name("AccountIdentifier");
        Parameter("AccountBalance").Name("AccountBalance");
    }
}

public class ClientSaveMap : ClassMap<Client>
{
    public ClientSaveMap()
    {
        Map(m => m.Document).Name("Document");
        Map(m => m.FullName).Name("FullName");
        Map(m => m.Telephone).Name("Telephone");
        Map(m => m.BirthDate).Name("BirthDate");
        Map(m => m.Active).Name("Active");
        Map(m => m.Account.Identifier).Name("AccountIdentifier");
        Map(m => m.Account.Balance).Name("AccountBalance");
    }
}