namespace AdaCredit;
public static class DeactivateEmployee
{
    public static void Execute(EmployeeRepository repository, Employee user, bool changeOwn = false)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Desativar Cadastro");

        var employee = user;
        if (!changeOwn)
        {
            Console.Write("Digite o nome de usuário do funcionário a ser desativado: ");
            string username = Console.ReadLine() ?? "";
            if (username == "user") {
                Console.WriteLine("ERRO: Funcionário não pode ser desativado!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                return;
            }
            employee = repository.GetByUsername(username);
            if (employee == null)
            {
                Console.WriteLine("ERRO: Funcionário não existente!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                return;
            }
        }

        Console.Write("Confirme o nome de usuário do funcionário a ser desativado: ");
        string usernameConfirm = Console.ReadLine() ?? "";

        if (!usernameConfirm.Equals(employee.Username))
        {
            Console.WriteLine("ERRO: Nome de usuário não coincide!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        repository.Remove(employee);
        Console.WriteLine("Funcionário desativado com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();

        if (changeOwn)
        {
            Environment.Exit(0);
        }
    }
}