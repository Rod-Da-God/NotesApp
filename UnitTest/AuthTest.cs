using AuthLibrary;
using DataAccess;
using DataAccess.Models;



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
                "Test123"
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

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsUser()
        {
            var username = "testuser_" + Guid.NewGuid().ToString()[..8];
            var password = "CorrectPass123";
            await _authService.RegisterAsync(
                new User { Username = username, Role = UserRole.User },
                password
            );
            var user = await _authService.LoginAsync(username, password);
            Assert.IsNotNull(user);
            Assert.AreEqual(username, user.Username);
        }

        [TestMethod]
        public async Task Register_EmptyUsername_ThrowsException()
        {
            var result = await _authService.RegisterAsync(
                    new User { Username = "", Role = UserRole.User },
                    "some"
                );
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Register_EmptyPassword_ThrowsException()
        {
            var username = "testuser_" + Guid.NewGuid().ToString()[..8];

            var result =  await _authService.RegisterAsync(
                    new User { Username = username, Role = UserRole.User },
                    ""
                );
            Assert.IsFalse(result);
           
        }

        [TestMethod]
        public async Task Register_PasswordWithSpacesOnly_ReturnsFalse()
        {
            var username = "testuser_" + Guid.NewGuid().ToString()[..8];
            var result = await _authService.RegisterAsync(
                new User { Username = username, Role = UserRole.User },
                "      "
            );
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Register_DuplicateUsername_ReturnsFalse()
        {
            var username = "testuser_" + Guid.NewGuid().ToString()[..8];

            await _authService.RegisterAsync(
                new User { Username = username, Role = UserRole.User },
                "some1"
            );

            var result = await _authService.RegisterAsync(
                new User { Username = username, Role = UserRole.User },
                "some2"
            );

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task Register_ShortPassword_ReturnsFalse()
        {
            var username = "testuser_" + Guid.NewGuid().ToString()[..8];
            var result = await _authService.RegisterAsync(
                new User { Username = username, Role = UserRole.User },
                "123"
            );
            Assert.IsFalse(result);
        }



    }
}