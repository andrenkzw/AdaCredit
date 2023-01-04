using ConsoleTables;

namespace AdaCredit;
public class ReportViewer
{
    private readonly EmployeeRepository _employeeRepository;
    private readonly ClientRepository _clientRepository;
    public ReportViewer(EmployeeRepository employeeRepository, ClientRepository clientRepository) {
        this._employeeRepository = employeeRepository;
        this._clientRepository = clientRepository;
    }

    public void ActiveClientsTable()
    {
        Console.Clear();
        var table = new ConsoleTable("CPF", "Nome", "Saldo");
        var rows = this._clientRepository.GetActiveClients();
        foreach (var row in rows) { table.AddRow(row); }
        table.Write(Format.Minimal);
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }

    public void InactiveClientsTable()
    {
        Console.Clear();
        var table = new ConsoleTable("CPF", "Nome", "Telefone");
        var rows = this._clientRepository.GetInactiveClients();
        foreach (var row in rows) { table.AddRow(row); }
        table.Write(Format.Minimal);
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }

    public void ActiveEmployeesTable()
    {
        Console.Clear();
        var table = new ConsoleTable("Nome de usuário", "Última Data e Hora de Login");
        var rows = this._employeeRepository.GetActiveEmployees();
        foreach (var row in rows) { table.AddRow(row); }
        table.Write(Format.Minimal);
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }

    public void TransactionErrorsTable()
    {
        Console.Clear();
        var table = new ConsoleTable(
            "Nome do Arquivo",
            "Erro",
            "Banco de Origem",
            "Agência de Origem",
            "Conta de Origem",
            "Banco de Destino",
            "Agência de Destino",
            "Conta de Destino",
            "Tipo",
            "Fluxo",
            "Valor"
        );
        var rows = TransactionProcessor.GetErrors();
        foreach (var row in rows) { table.AddRow(row); }
        table.Write(Format.Minimal);
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}
