using System.Globalization;
using CsvHelper;

namespace AdaCredit;
public class EmployeeRepository
{
    private string _filePath { get; init; }
    private List<Employee> _employees { get; init; }
    public EmployeeRepository(DirectoryInfo directoryInfo)
    {
        _filePath = Path.Combine(directoryInfo.FullName, "employees.csv");
        _employees = Load();
    }

    private static List<Employee> InitialRepository()
    {
        var employees = new List<Employee>();
        employees.Add(new Employee("user", "pass"));
        return employees;
    }

    private List<Employee> Load()
    {
        if (!File.Exists(this._filePath))
        {
            return InitialRepository();
        }
        using (var reader = new StreamReader(this._filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<Employee>().ToList();
        }
    }

    private void Save()
    {
        using (var writer = new StreamWriter(this._filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(this._employees);
        }
    }

    public Employee? GetByUsername(string username)
        => _employees.FirstOrDefault(x => x.Username.Equals(username));

    public void UpdateLoginTime(Employee employee) {
        employee.Touch();
        Save();
    }

    public void UpdatePassword(Employee employee, string password) {
        employee.ChangePassword(password);
        Save();
    }

    public bool Add(Employee employee)
    {
        if (_employees.Any(x => x.Username.Equals(employee.Username)))
        {
            Console.WriteLine("ERRO: Nome de usuário já cadastrado.");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return false;
        }

        _employees.Add(employee);
        Save();
        return true;
    }

    public void Remove(Employee employee)
    {
        employee.Deactivate();
        Save();
    }

    public IEnumerable<string[]> GetActiveEmployees() {
        return this._employees.Where(x => x.Active).Select(x => new string[] {
            x.Username,
            x.LastLogin.ToString(CultureInfo.CurrentCulture)
        });
    }
}