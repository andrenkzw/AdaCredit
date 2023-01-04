using CsvHelper.Configuration;

namespace AdaCredit;
public sealed class Transaction
{
    public string OriginBank { get; init; }
    public string OriginBranch { get; init; }
    public string OriginAccount { get; init; }
    public string DestinationBank { get; init; }
    public string DestinationBranch { get; init; }
    public string DestinationAccount { get; init; }
    public string Type { get; init; }
    public string Flow { get; init; }
    public string Value { get; init; }
    public string? Error { get; set; }

    public Transaction
    (
        string OriginBank,
        string OriginBranch,
        string OriginAccount,
        string DestinationBank,
        string DestinationBranch,
        string DestinationAccount,
        string Type,
        string Flow,
        string Value,
        string Error
    )
    {
        this.OriginBank = OriginBank;
        this.OriginBranch = OriginBranch;
        this.OriginAccount = OriginAccount;
        this.DestinationBank = DestinationBank;
        this.DestinationBranch = DestinationBranch;
        this.DestinationAccount = DestinationAccount;
        this.Type = Type;
        this.Flow = Flow;
        this.Value = Value;
        this.Error = Error;
    }
}

public class TransactionMap : ClassMap<Transaction>
{
    public TransactionMap()
    {
        Map(m => m.OriginBank).Index(0);
        Map(m => m.OriginBranch).Index(1);
        Map(m => m.OriginAccount).Index(2);
        Map(m => m.DestinationBank).Index(3);
        Map(m => m.DestinationBranch).Index(4);
        Map(m => m.DestinationAccount).Index(5);
        Map(m => m.Type).Index(6);
        Map(m => m.Flow).Index(7);
        Map(m => m.Value).Index(8);
        Map(m => m.Error).Index(9).Optional();
    }
}