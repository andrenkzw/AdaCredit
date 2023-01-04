namespace AdaCredit;
public sealed class Employee
{
    public string Username { get; init; }
    public string Salt { get; private set; }
    public string Password { get; private set; }
    public bool Active { get; private set; }
    public DateTime LastLogin { get; private set; }

    public Employee(string Username, string Password, string Salt, bool Active, DateTime LastLogin)
    {
        this.Username = Username;
        this.Salt = Salt;
        this.Password = Password;
        this.Active = Active;
        this.LastLogin = LastLogin;
    }

    public Employee(string username, string password)
    {
        this.Username = username;
        this.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        this.Password = BCrypt.Net.BCrypt.HashPassword(password, this.Salt);
        this.Active = true;
        this.LastLogin = DateTime.UnixEpoch;
    }

    public bool NeverLoggedIn()
        => this.LastLogin == DateTime.UnixEpoch;

    public void Touch()
    {
        this.LastLogin = DateTime.Now;
    }

    public void ChangePassword(string password)
    {
        this.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        this.Password = BCrypt.Net.BCrypt.HashPassword(password, this.Salt);
    }

    public void Deactivate()
    {
        this.Active = false;
    }
}