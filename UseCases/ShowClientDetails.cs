using System.Globalization;

namespace AdaCredit;
public static class ShowClientDetails
{
    public static void Execute(ClientRepository repository, Client client)
    {
        Console.Clear();
        Console.WriteLine("ADA CREDIT - Consultar os Dados de um Cliente existente");

        Console.WriteLine("CPF: " + client.PrettyDocument());
        Console.WriteLine("Nome completo: " + client.FullName);
        Console.WriteLine("Telefone: " + client.Telephone);
        Console.WriteLine("Data de nascimento: " + client.BirthDate.ToString("yyyy-MM-dd"));
        Console.WriteLine("Conta principal: " + client.Account.PrettyIdentifier());
        Console.WriteLine("Saldo da conta: " + client.Account.Balance.ToString("C", CultureInfo.CurrentCulture));
        Console.WriteLine("<pressione qualquer tecla para voltar>");
        Console.ReadKey();
    }
}