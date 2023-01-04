using System.Text.RegularExpressions;

namespace AdaCredit;
public static class CreateEmployee
{
    public static void Execute(EmployeeRepository repository)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Cadastrar Novo Funcionário");

        Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9]{3,15}$");
        Console.WriteLine("AVISO: O nome do usuário deverá conter de 4 a 16 caracteres alfanuméricos e começar com uma letra.");

        Console.Write("Digite o nome de usuário do novo funcionário: ");
        string username = Console.ReadLine() ?? "";

        if (!regex.IsMatch(username))
        {
            Console.WriteLine("ERRO: Nome do usuário não satisfaz regras!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        Console.Write("Digite a senha temporária do novo funcionário: ");
        string password = Console.ReadLine() ?? "";

        var employee = new Employee(username, password);

        if (repository.Add(employee))
        {
            Console.WriteLine("Funcionário cadastrado com sucesso!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
        }
    }
}