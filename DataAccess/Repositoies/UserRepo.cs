using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositoies
{
    public class UserRepo
    {
        private readonly AppDbContext dbContext;

        public UserRepo(AppDbContext db)
        {
            dbContext = db;
        }

        public async Task AddUserAsync(User user)
        {
            if (await dbContext.Users.AnyAsync(u => u.Username == user.Username))
                throw new Exception("Пользователь с таким именем уже существует");

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();
        }


        public async Task<User> GetUserByIdAsync(int id)
        {
            return await dbContext.Users.FindAsync(id);
        }


        public async Task<User> GetUserByUsername(string username)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int> GetAdminCount()
        {
            var users = await dbContext.Users.ToListAsync();
            return users.Count(u => u.Role == UserRole.Admin);
        }

        public AppDbContext GetDbContext()
        {
            return dbContext;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await dbContext.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }
    }

}
