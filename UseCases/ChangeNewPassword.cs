using System.Text.RegularExpressions;

namespace AdaCredit;
public static class ChangeNewPassword
{
    public static void Execute(EmployeeRepository repository, Employee employee)
    {
        bool match = false;
        Regex regex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,16}$");

        do
        {
            Console.Clear();
            Console.WriteLine("ADA CREDIT - ALTERAÇÃO DA SENHA TEMPORÁRIA");
            Console.WriteLine("AVISO: A senha deverá conter de 8 a 16 alfanuméricos e caracteres especiais (#?!@$%^&*-) com pelo menos uma maiúscula, uma minúscula, um digito e um caractere especial.");

            Console.Write("Digite a nova senha: ");
            string password1 = Utils.ReadPassword();

            if (!regex.IsMatch(password1))
            {
                Console.WriteLine("ERRO: Senha não satisfaz regras!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                continue;
            }

            Console.Write("Digite mais uma vez a nova senha: ");
            string password2 = Utils.ReadPassword();

            match = password1.Equals(password2);
            if (!match)
            {
                Console.WriteLine("ERRO: As senhas digitadas não coincidem!");
                Console.WriteLine("<pressione qualquer tecla para voltar>");
                Console.ReadKey();
                continue;
            }
            repository.UpdatePassword(employee, password1);
        } while (!match);

        Console.Clear();
        Console.WriteLine("Senha alterada com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para continuar>");
        Console.ReadKey();
        Console.Clear();
    }
}