using AuthLibrary;
using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace UnitTest
{
    [TestClass]
    public class TestAuthLibrary
    {
        private AuthService? _authService;
        private AppDbContext? _db;
        [TestInitialize]
        public void Init() {
            _db = new AppDbContext();
            _authService = new AuthService(new DataAccess.Repositoies.UserRepo(_db));
            CleanupTestData();
        }

        private void CleanupTestData()
        {
            var testUsers = _db.Users.Where(u => u.Username.StartsWith("testuser_")).ToList();
            _db.Users.RemoveRange(testUsers);
            _db.SaveChanges();
        }

        [TestMethod]
        public async Task Register_NewUser_ReturnsTrue()
        {
            
            var result = await _authService.RegisterAsync(
                new User { Username = "testuser_" + System.Guid.NewGuid().ToString()[..8], Role = UserRole.User },
                "Test"
            );
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task Login_Invalid_Password()
        {
            var username = "testuser_" + System.Guid.NewGuid().ToString()[..8];
             await _authService.RegisterAsync(
    new User { Username = username, Role = UserRole.User },
    "1234"
);
            var result = await _authService.LoginAsync(username, "WrongPass");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Login_NonExistenUser_ReturnsNull()
        {
            var result = await _authService.LoginAsync("none", "none");

            Assert.IsNull(result);
        }

    }
}