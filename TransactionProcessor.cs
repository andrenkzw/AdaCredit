using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;

namespace AdaCredit;
public static class TransactionProcessor
{
    public static void Run(ClientRepository repository)
    {
        string transactionsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Transactions");
        if (!Prepare(transactionsPath)) { return; }
        var directoryInfo = new DirectoryInfo(Path.Combine(transactionsPath, "Pending"));
        var files = Utils.GetTransactionFiles(directoryInfo);
        foreach (string file in files)
        {
            ProcessFile(repository, transactionsPath, file);
            File.Delete(Path.Combine(transactionsPath, "Pending", file + ".csv"));
        }
        Console.WriteLine("Fim do processamento.");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }

    private static bool Prepare(string transactionsPath)
    {
        if (!Directory.Exists(Path.Combine(transactionsPath, "Pending")))
        {
            Console.WriteLine($"ERRO: Diretório {Path.Combine(transactionsPath, "Pending")} não existente!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return false;
        }
        if (!Directory.Exists(Path.Combine(transactionsPath, "Completed")))
        {
            Directory.CreateDirectory(Path.Combine(transactionsPath, "Completed"));
        }
        if (!Directory.Exists(Path.Combine(transactionsPath, "Failed")))
        {
            Directory.CreateDirectory(Path.Combine(transactionsPath, "Failed"));
        }
        return true;
    }

    private static void ProcessFile(ClientRepository repository, string transactionsPath, string filename)
    {
        DateTime date = Utils.DateFromTransactionFile(filename);
        Console.Write($"Processando {Path.Combine(transactionsPath, "Pending", filename + ".csv")}...");
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            MissingFieldFound = null
        };
        using (var filein = new StreamReader(Path.Combine(transactionsPath, "Pending", filename + ".csv")))
        using (var csvin = new CsvReader(filein, config))
        using (var fileout = new StreamWriter(Path.Combine(transactionsPath, "Completed", filename + "-completed.csv")))
        using (var csvout = new CsvWriter(fileout, config))
        using (var fileerr = new StreamWriter(Path.Combine(transactionsPath, "Failed", filename + "-failed.csv")))
        using (var csverr = new CsvWriter(fileerr, config))
        {
            while (csvin.Read())
            {
                var transaction = csvin.GetRecord<Transaction>();
                if (Process(repository, transaction!, date))
                {
                    csvout.WriteRecord(transaction);
                    csvout.NextRecord();
                }
                else
                {
                    csverr.WriteRecord(transaction);
                    csverr.NextRecord();
                }
                
            }
        }
        Console.WriteLine(" Completo!");
    }

    private static bool Process(ClientRepository repository, Transaction transaction, DateTime date)
    {
        bool success = true;
        if (!ValidateBank(transaction)) { return false; }
        if (!ValidateBranch(transaction)) { return false; }
        (success, Client? originClient) = ValidateOriginAccount(repository, transaction);
        if (!success) { return false; }
        (success, Client? destinationClient) = ValidateDestinationAccount(repository, transaction);
        if (!success) { return false; }
        if (!ValidateType(transaction, originClient, destinationClient)) { return false; }
        if (!ValidateFlow(transaction)) { return false; }
        (success, decimal destinationValue) = ValidateValue(transaction);
        if (!success) { return false; }
        decimal originValue = ApplyFare(destinationValue, transaction.Type, transaction.Flow, date);        
        success = transaction.Flow switch
        {
            "0" => ProcessTransfer(repository, originClient, originValue, destinationClient, destinationValue),
            "1" => ProcessTransfer(repository, destinationClient, destinationValue, originClient, originValue),
            _ => true
        };
        if (!success)
        {
            transaction.Error = "Saldo Insuficiente";
        }
        return success;
    }

    private static bool ProcessTransfer(ClientRepository repository, Client? from, decimal fromValue, Client? to, decimal toValue)
    {
        bool success = true;
        if (success && from != null)
        {
            success = repository.UpdateBalance(from, -fromValue); 
        }
        if (success && to != null)
        {
            success = repository.UpdateBalance(to, toValue);
        }
        return success;
    }

    private static bool ValidateBank(Transaction transaction)
    {
        var regex = new Regex("[0-9]{3}$");
        if (!regex.IsMatch(transaction.OriginBank))
        {
            transaction.Error = "Banco de Origem Inválido";
            return false;
        }
        if (!regex.IsMatch(transaction.DestinationBank))
        {
            transaction.Error = "Banco de Destino Inválido";
            return false;
        }
        return true;
    }

    private static bool ValidateBranch(Transaction transaction)
    {
        var regex = new Regex("[0-9]{4}$");
        if (
            !regex.IsMatch(transaction.OriginBranch) ||
            (transaction.OriginBank == "777" && transaction.OriginBranch != "0001")
        )
        {
            transaction.Error = "Agência de Origem Inválida";
            return false;
        }
        if (
            !regex.IsMatch(transaction.DestinationBranch) ||
            (transaction.DestinationBank == "777" && transaction.DestinationBranch != "0001")
        )
        {
            transaction.Error = "Agência de Destino Inválida";
            return false;
        }
        return true;
    }

    private static (bool, Client?) ValidateOriginAccount(ClientRepository repository, Transaction transaction)
    {
        var regex = new Regex("[0-9]{6}$");
        Client? client = null;
        if (!regex.IsMatch(transaction.OriginAccount))
        {
            transaction.Error = "Conta de Origem Inválida";
            return (false, client);
        }
        if (transaction.OriginBank == "777" && transaction.OriginBranch == "0001")
        {
            int account = int.Parse(transaction.OriginAccount);
            client = repository.GetByAccountIdentifier(account);
            if (client == null) {
                transaction.Error = "Conta de Origem Inválida";
                return (false, client);
            }
            else if (!client.Active)
            {
                transaction.Error = "Conta de Origem Desativada";
                return (false, client);
            }
        }
        return (true, client);
    }

    private static (bool, Client?) ValidateDestinationAccount(ClientRepository repository, Transaction transaction)
    {
        var regex = new Regex("[0-9]{6}$");
        Client? client = null;
        if (!regex.IsMatch(transaction.DestinationAccount))
        {
            transaction.Error = "Conta de Destino Inválida";
            return (false, client);
        }
        if (transaction.DestinationBank == "777" && transaction.DestinationBranch == "0001")
        {
            int account = int.Parse(transaction.DestinationAccount);
            client = repository.GetByAccountIdentifier(account);
            if (client == null) {
                transaction.Error = "Conta de Destino Inválida";
                return (false, client);
            }
            else if (!client.Active)
            {
                transaction.Error = "Conta de Destino Desativada";
                return (false, client);
            }
        }
        return (true, client);
    }

    private static bool ValidateType(Transaction transaction, Client? origin, Client? destination)
    {
        switch (transaction.Type)
        {
            case "TED":
            case "DOC":
                return true;
            case "TEF":
                if (origin == null || destination == null)
                {
                    transaction.Error = "TEF Incompatível";
                    return false;
                }
                return true;
            default:
                transaction.Error = "Tipo de Transação Inválido";
                return false;
        }
    }

    private static bool ValidateFlow(Transaction transaction)
    {
        if (transaction.Flow != "0" && transaction.Flow != "1")
        {
            transaction.Error = "Sentido da Transação Inválido";
            return false;
        }
        return true;
    }

    private static (bool, decimal) ValidateValue(Transaction transaction)
    {
        bool success = decimal.TryParse(transaction.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value);
        return (success, value);
    }

    private static decimal ApplyFare(decimal value, string type, string flow, DateTime date)
    {
        if (flow == "1" || date < new DateTime(2022, 12, 1)) { return value; }
        switch (type)
        {
            case "TED":
                value += 5;
                break;
            case "DOC":
                decimal partial = value * (decimal)0.01;
                value += 1 + (partial <= 5 ? partial : 5); 
                break;
        }
        return value;
    }

    public static IEnumerable<string[]> GetErrors() {
        var errors = Enumerable.Empty<string[]>();
        string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Transactions", "Failed");
        if (!Directory.Exists(directoryPath)) { return errors; }
        var directoryInfo = new DirectoryInfo(directoryPath);
        foreach (string filename in directoryInfo.GetFiles().Select(x => x.Name))
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                MissingFieldFound = null
            };
            using (var reader = new StreamReader(Path.Combine(directoryPath, filename)))
            using (var csv = new CsvReader(reader, config))
            {
                string nameWithoutPreffix = filename.Substring(0, filename.Length - 11);
                var records = csv.GetRecords<Transaction>().Select(x => new string[] {
                    nameWithoutPreffix,
                    x.Error,
                    x.OriginBank,
                    x.OriginBranch,
                    x.OriginAccount,
                    x.DestinationBank,
                    x.DestinationBranch,
                    x.DestinationAccount,
                    x.Type,
                    x.Flow,
                    x.Value
                }).ToArray();
                errors = errors.Concat(records);
            }
        }
        return errors;
    }
}
