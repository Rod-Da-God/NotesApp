using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositoies
{
    public class TaskRepository
    {
        private readonly AppDbContext dbContext;

        public TaskRepository(AppDbContext db)
        {
            dbContext = db;
        }

        public async Task<List<Tasks>> GetAllTasksAsync()
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUser)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<List<Tasks>> GetUserTasksAsync(int userId)
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUser)
                .Where(t => t.AssignedUserId == userId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task AssignTaskAsync(Tasks task, int userId)
        {
            task.AssignedUserId = userId;
            task.Status = Models.TaskStatus.New; 
            dbContext.Tasks.Add(task);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateTaskStatusAsync(int taskId, Models.TaskStatus status)
        {
            var task = await dbContext.Tasks.FindAsync(taskId);
            if (task == null)
                throw new Exception("Задача не найдена");

            task.Status = status;
            await dbContext.SaveChangesAsync();
        }

        public async Task<Tasks> GetTaskByIdAsync(int taskId)
        {
            return await dbContext.Tasks
                .Include(t => t.AssignedUser)
                .FirstOrDefaultAsync(t => t.Id == taskId);
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            using (var dbContext = new AppDbContext())
            {
                var task = await dbContext.Tasks.FindAsync(taskId);
                if (task != null)
                {
                    dbContext.Tasks.Remove(task);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
