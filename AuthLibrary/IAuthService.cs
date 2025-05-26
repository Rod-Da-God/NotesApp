using DataAccess.Models;
using System.Threading.Tasks;

namespace AuthLibrary
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }
        User CurrentUser { get; }

        Task<User> Login(string username, string password);
        Task<bool> Register(User user, string password);
    }
}