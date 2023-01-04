using System.Text.RegularExpressions;

namespace AdaCredit;
public static class ChangePassword
{
    public static void Execute(EmployeeRepository repository, Employee user, bool changeOwn = false)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Alterar Senha");

        var employee = user;
        if (!changeOwn)
        {
            Console.Write("Digite o nome de usuário do funcionário: ");
            string username = Console.ReadLine() ?? "";
            employee = repository.GetByUsername(username);
            if (employee == null)
            {
                Console.WriteLine("ERRO: Funcionário não existente!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                return;
            }
        }

        Regex regex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,16}$");
        Console.WriteLine("AVISO: A senha deverá conter de 8 a 16 alfanuméricos e caracteres especiais (#?!@$%^&*-) com pelo menos uma maiúscula, uma minúscula, um digito e um caractere especial.");

        Console.Write("Digite a nova senha: ");
        string password1 = Utils.ReadPassword();

        if (!regex.IsMatch(password1))
        {
            Console.WriteLine("ERRO: Senha não satisfaz regras!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        Console.Write("Digite mais uma vez a nova senha: ");
        string password2 = Utils.ReadPassword();

        if (!password1.Equals(password2))
        {
            Console.WriteLine("ERRO: As senhas digitadas não coincidem!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }
        repository.UpdatePassword(employee, password1);
        Console.WriteLine("Senha alterada com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}