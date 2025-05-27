using DataAccess.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using DataAccess.Repositoies;

namespace AuthLibrary
{
    public class AuthService
    {
        private readonly UserRepo _userRepo;
        
        public AuthService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _userRepo.GetUserByUsername(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return false;
            }
            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                await _userRepo.AddUserAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                string details = ex.Message;
                if (ex.InnerException != null)
                    details += "\n" + ex.InnerException.Message;
                Console.WriteLine($"Ошибка регистрации: {details}");
                return false;
            }
        }
    }
}