namespace AdaCredit;
class Program
{
    static void Main(string[] args)
    {
        var directoryInfo = new DirectoryInfo("Database");
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        var employeeRepository = new EmployeeRepository(directoryInfo);
        var clientRepository = new ClientRepository(directoryInfo);
        var reportViewer = new ReportViewer(employeeRepository, clientRepository);
        var login = new Login(employeeRepository);
        var employee = login.Authenticate();
        var menu = new Menu(employeeRepository, clientRepository, reportViewer, employee);
        menu.Show();
    }
}
