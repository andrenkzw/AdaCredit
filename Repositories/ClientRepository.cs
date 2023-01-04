using System.Globalization;
using Bogus;
using CsvHelper;

namespace AdaCredit;
public class ClientRepository
{
    private string _filePath { get; init; }
    private List<Client> _clients { get; init; }
    private Faker _faker { get; init; }
    public ClientRepository(DirectoryInfo directoryInfo)
    {
        _filePath = Path.Combine(directoryInfo.FullName, "clients.csv");
        _clients = Load();
        _faker = new Faker(locale: "pt_BR");
    }

    private List<Client> Load()
    {
        if (!File.Exists(this._filePath))
        {
            return new List<Client>();
        }
        using (var reader = new StreamReader(this._filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<ClientLoadMap>();
            return csv.GetRecords<Client>().ToList();
        }
    }

    private void Save()
    {
        using (var writer = new StreamWriter(this._filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<ClientSaveMap>();
            csv.WriteRecords(this._clients);
        }
    }

    public Client? GetByDocument(long document)
        => _clients.FirstOrDefault(x => x.Document.Equals(document));

    public int GetUniqueAccountIdentifier() {
        bool exists = true;
        int identifier = 0;
        do
        {
            int randomNumber = this._faker.Random.Int(0,99999);
            identifier = 10 * randomNumber + randomNumber % 10;
            exists = _clients.Any(x => x.Account.Identifier.Equals(identifier));
        } while (exists);
        return identifier;
    }

    public IEnumerable<Client> Read() {
        return (IEnumerable<Client>)this._clients;
    }

    public bool Add(Client client)
    {
        if (_clients.Any(x => x.Document.Equals(client.Document)))
        {
            Console.WriteLine("ERRO: Cliente já cadastrado.");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return false;
        }

        _clients.Add(client);
        Save();
        return true;
    }

    public void UpdateTelephone(Client client, string telephone) {
        client.Telephone = telephone;
        Save();
    }

    public void UpdateFullName(Client client, string fullName) {
        client.FullName = fullName;
        Save();
    }

    public void Remove(Client client)
    {
        client.Deactivate();
        Save();
    }

    public Client? GetByAccountIdentifier(int identifier)
        => _clients.FirstOrDefault(x => x.Account.Identifier.Equals(identifier));
    
    public bool UpdateBalance(Client client, decimal value) {
        try { client.Account.Balance += value; }
        catch { return false; }
        Save();
        return true;
    }

    public IEnumerable<string[]> GetActiveClients() {
        return this._clients.Where(x => x.Active).Select(x => new string[] {
            x.PrettyDocument(), 
            x.FullName, 
            x.Account.Balance.ToString("C", CultureInfo.CurrentCulture)
        });
    }

    public IEnumerable<string[]> GetInactiveClients() {
        return this._clients.Where(x => !x.Active).Select(x => new string[] {
            x.PrettyDocument(), 
            x.FullName, 
            x.Telephone
        });
    }
}