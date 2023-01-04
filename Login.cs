namespace AdaCredit;
class Login
{
    private readonly EmployeeRepository _employeeRepository;

    public Login(EmployeeRepository employeeRepository)
    {
        this._employeeRepository = employeeRepository;
    }

    public Employee Authenticate()
    {
        bool loggedIn = false;
        Employee? employee = null;

        do
        {
            Console.Clear();

            Console.WriteLine("ADA CREDIT - LOGIN");

            Console.Write("Digite o nome de usuário: ");
            string username = Console.ReadLine() ?? "";
            employee = this._employeeRepository.GetByUsername(username);
            if (employee == null)
            {
                Console.WriteLine("ERRO: Usuário não existente!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                continue;
            }
            else if (!employee.Active)
            {
                Console.WriteLine("ERRO: Usuário desativado!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                continue;
            }

            Console.Write("Digite a senha: ");
            string password = BCrypt.Net.BCrypt.HashPassword(Utils.ReadPassword(), employee.Salt);
            loggedIn = password.Equals(employee.Password);
            if (!loggedIn)
            {
                Console.WriteLine("ERRO: Senha não coincide com a cadastrada!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
            }
        } while (!loggedIn);

        Console.Clear();
        Console.WriteLine("ADA CREDIT - LOGIN");
        Console.WriteLine("Login efetuado com sucesso!");
        if (employee!.NeverLoggedIn())
        {
            Console.WriteLine("AVISO: Login pela primeira vez. Senha deverá ser trocada.");
        }
        Console.WriteLine("<pressione qualquer tecla para continuar>");
        Console.ReadKey();
        Console.Clear();
        if (employee.NeverLoggedIn())
        {
            ChangeNewPassword.Execute(this._employeeRepository, employee);
        }
        Console.Clear();
        this._employeeRepository.UpdateLoginTime(employee);
        return employee;
    }
}
