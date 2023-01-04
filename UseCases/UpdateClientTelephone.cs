using System.Text.RegularExpressions;

namespace AdaCredit;
public static class UpdateClientTelephone
{
    public static void Execute(ClientRepository repository)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Alterar o Telefone de um Cliente existente");

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

        var regex = new Regex("^\\+?[1-9][0-9]{7,14}$");
        Console.WriteLine("AVISO: O telefone deverá estar em padrão internacional sem espaços (p. ex. +5511912345678).");

        Console.Write("Digite o novo telefone do cliente: ");
        string telephone = Console.ReadLine() ?? "";

        if (!regex.IsMatch(telephone))
        {
            Console.WriteLine("ERRO: Telefone fora do padrão!");
            Console.WriteLine("<pressione qualquer tecla para voltar>");
            Console.ReadKey();
            return;
        }

        repository.UpdateTelephone(client, telephone);
        Console.WriteLine("Telefone alterado com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}