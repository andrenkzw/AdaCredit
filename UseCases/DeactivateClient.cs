namespace AdaCredit;
public static class DeactivateClient
{
    public static void Execute(ClientRepository repository)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Desativar Cadastro de um Cliente existente");

        Console.Write("Digite o CPF do cliente a ser desativado  (apenas números, contendo dígitos verificadores): ");
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

        repository.Remove(client);
        Console.WriteLine("Cliente desativado com sucesso!");
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}