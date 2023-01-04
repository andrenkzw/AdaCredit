using ConsoleTools;

namespace AdaCredit;
public class Menu
{
    private readonly EmployeeRepository _employeeRepository;
    private readonly ClientRepository _clientRepository;
    private readonly ReportViewer _reportViewer;

    private readonly Employee _user;
    public Menu(EmployeeRepository employeeRepository, ClientRepository clientRepository, ReportViewer reportViewer, Employee user) {
        this._employeeRepository = employeeRepository;
        this._clientRepository = clientRepository;
        this._reportViewer = reportViewer;
        this._user = user;
    }

    public void Show()
    {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 0)
            .Add("Clientes", ShowClients)
            .Add("Funcionários", ShowEmployees)
            .Add("Transações", ShowTransactions)
            .Add("Relatórios", ShowReports)
            .Add("Sair", () => Environment.Exit(0))
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT";
                config.EnableWriteTitle = false;
                config.EnableBreadcrumb = true;
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });
        menu.Show();
    }

    private void ShowClients() {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 1)
            .Add("Cadastrar Novo Cliente", () => CreateClient.Execute(this._clientRepository))
            .Add("Consultar os Dados de um Cliente existente", ShowReadClients)
            .Add("Alterar o Cadastro de um Cliente existente", ShowUpdateClient)
            .Add("Desativar Cadastro de um Cliente existente", () => DeactivateClient.Execute(this._clientRepository))
            .Add("Voltar", ConsoleMenu.Close)
            .Add("Sair", () => Environment.Exit(0))
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT - Clientes";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }

    private void ShowEmployees() {
        string passwordOption = this._user.Username == "user" ? "Alterar Senha de um Funcionário existente" : "Alterar própria senha"; 
        string deactivateOption = this._user.Username == "user" ? "Desativar Cadastro de um Funcionário existente" : "Desativar próprio cadastro";

        var menu = new ConsoleMenu(Array.Empty<string>(), level: 1)
            .Add("Cadastrar Novo Funcionário", () => CreateEmployee.Execute(this._employeeRepository))
            .Add(passwordOption, () => ChangePassword.Execute(this._employeeRepository, this._user, this._user.Username != "user"))
            .Add(deactivateOption, () => DeactivateEmployee.Execute(this._employeeRepository, this._user, this._user.Username != "user"))
            .Add("Voltar", ConsoleMenu.Close)
            .Add("Sair", () => Environment.Exit(0))
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT - Funcionários";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }

    private void ShowTransactions() {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 1)
            .Add("Processar Transações (Reconciliação Bancária)", () => {Console.Clear(); TransactionProcessor.Run(this._clientRepository);})
            .Add("Voltar", ConsoleMenu.Close)
            .Add("Sair", () => Environment.Exit(0))
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT - Transações";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }

    private void ShowReports() {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 1)
            .Add("Exibir Todos os Clientes Ativos com seus Respectivos Saldos", () => this._reportViewer.ActiveClientsTable())
            .Add("Exibir Todos os Clientes Inativos", () => this._reportViewer.InactiveClientsTable())
            .Add("Exibir Todos os Funcionários Ativos e sua Última Data e Hora de Login", () => this._reportViewer.ActiveEmployeesTable())
            .Add("Exibir Transações com Erro (Detalhes da transação e do Erro)", () => this._reportViewer.TransactionErrorsTable())
            .Add("Voltar", ConsoleMenu.Close)
            .Add("Sair", () => Environment.Exit(0))
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT - Relatórios";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }

    private void ShowReadClients() {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 2)
            .AddRange(this._clientRepository.Read().Select(x => new Tuple<string,Action>
                (
                    (x.Active ? " " : "*") + x.Document + " - " + x.FullName,
                    () => ShowClientDetails.Execute(this._clientRepository, x)
                )
            ))
            .Add("Voltar", ConsoleMenu.Close)
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = true;
                config.FilterPrompt = "Filtrar: ";
                config.Title = "ADA CREDIT - Consultar os Dados de um Cliente existente";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }

    private void ShowUpdateClient() {
        var menu = new ConsoleMenu(Array.Empty<string>(), level: 2)
            .Add("Alterar Nome", () => UpdateClientFullName.Execute(this._clientRepository))
            .Add("Alterar Telefone", () => UpdateClientTelephone.Execute(this._clientRepository))
            .Add("Voltar", ConsoleMenu.Close)
            .Configure(config =>
            {
                config.WriteHeaderAction = () => Console.WriteLine("Escolha uma opção:");
                config.Selector = "> ";
                config.EnableFilter = false;
                config.Title = "ADA CREDIT - Alterar o Cadastro de um Cliente existente";
                config.EnableBreadcrumb = true;
                config.WriteBreadcrumbAction = titles => Console.WriteLine(string.Join(" / ", titles));
                config.SelectedItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
            });

        menu.Show();
    }    
}