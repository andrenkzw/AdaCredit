using System.Globalization;
using System.Text.RegularExpressions;

namespace AdaCredit;
public static class CreateClient
{
    public static void Execute(ClientRepository repository)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Cadastrar Novo Cliente");

        Regex regex = new Regex("^[a-zA-Z ]{1,64}$");
        Console.WriteLine("AVISO: O nome do cliente deverá conter até 64 caracteres sendo formado por letras e espaços apenas.");

        Console.Write("Digite o nome completo do cliente: ");
        string fullname = Console.ReadLine() ?? "";

        if (!regex.IsMatch(fullname))
        {
            Console.WriteLine("ERRO: Nome do cliente não satisfaz regras!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        regex = new Regex("^[0-9]{11}$");
        Console.Write("Digite o CPF do cliente (apenas números, contendo dígitos verificadores): ");
        string documentText = (Console.ReadLine() ?? "");

        if (!regex.IsMatch(documentText))
        {
            Console.WriteLine("ERRO: CPF fora do padrão!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }
        long document = long.Parse(documentText);

        regex = new Regex("^\\+?[1-9][0-9]{7,14}$");
        Console.WriteLine("AVISO: O telefone deverá estar em padrão internacional sem espaços (p. ex. +5511912345678).");

        Console.Write("Digite o telefone do cliente: ");
        string telephone = Console.ReadLine() ?? "";

        if (!regex.IsMatch(telephone))
        {
            Console.WriteLine("ERRO: Telefone fora do padrão!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        Console.Write("Digite a data de nascimento do cliente (aaaa-mm-dd): ");
        string birthdateText = Console.ReadLine() ?? "";

        if (!DateTime.TryParseExact(birthdateText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
            out DateTime birthdate))
        {
            Console.WriteLine("ERRO: Data fora de padrão!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        var client = new Client(document, fullname, telephone, birthdate, repository.GetUniqueAccountIdentifier());

        if (repository.Add(client))
        {
            Console.WriteLine("Cliente cadastrado com sucesso!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
        }
    }
}