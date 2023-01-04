using System.Text.RegularExpressions;

namespace AdaCredit;
public static class UpdateClientFullName
{
    public static void Execute(ClientRepository repository)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Alterar o Nome de um Cliente existente");

        Console.Write("Digite o CPF do cliente  (apenas números, contendo dígitos verificadores): ");
        string documentText = Console.ReadLine() ?? "";
        long.TryParse(documentText, out long document);
        
        var client = repository.GetByDocument(document);
        if (client == null)
        {
            Console.WriteLine("ERRO: Cliente não cadastrado!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }
        else if (!client.Active)
        {
            Console.WriteLine("ERRO: Cliente desativado não pode ter cadastro alterado!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Nome do cliente: " + client.FullName);
        Console.WriteLine("Telefone do cliente: " + client.Telephone);

        var regex = new Regex("^[a-zA-Z ]{1,64}$");
        Console.WriteLine("AVISO: O nome do cliente deverá conter até 64 caracteres sendo formado por letras e espaços apenas.");

        Console.Write("Digite o nome atual do cliente: ");
        string fullname = Console.ReadLine() ?? "";

        if (!regex.IsMatch(fullname))
        {
            Console.WriteLine("ERRO: Nome do cliente não satisfaz regras!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        repository.UpdateFullName(client, fullname);
        Console.WriteLine("Nome alterado com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}