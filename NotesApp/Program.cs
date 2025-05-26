using AuthLibrary;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositoies;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotesApp
{
    static class Program
    {
        [STAThread]
        static async Task Main()
        {
            ApplicationConfiguration.Initialize();

            using (var db = new AppDbContext())
            {
                var userRepo = new UserRepo(db);
                var authService = new AuthService(userRepo);


                    Console.WriteLine("zz");
                    try
                    {
                        await authService.RegisterAsync(
                            new User { Username = "admin", Role = UserRole.Admin },
                            "admin123"
                        );
                    await authService.RegisterAsync(
    new User { Username = "Степан", Role = UserRole.User },
    "2525"
);

                }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                
            }

            Application.Run(new LoginForm());
        }
    }
}