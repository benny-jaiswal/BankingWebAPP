namespace BankingWebApp.Services
{
    public interface IAuthService
    {
        Task<bool> Login(string username, string password);
        Task Logout();
        Task<string> GetToken();
        Task<bool> IsAuthenticated();
    }

}
