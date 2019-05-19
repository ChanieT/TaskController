using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;


namespace Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Assignment
    {
        public int Id { get; set; }
        public string Task { get; set; }
        //public bool InProgress { get; set; }
        //public bool Completed { get; set; }
        public Status Status { get; set; }
        public User User { get; set; }
    }

    public enum Status
    {
        incomplete,
        complete,
        inProgress
    }


    public class TasksContext : DbContext
    {
        private string _conn;
        public TasksContext(string conn)
        {
            _conn = conn;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conn);
        }
    }

    public class TasksContextFactory : IDesignTimeDbContextFactory<TasksContext>
    {
        public TasksContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), $"..{Path.DirectorySeparatorChar}5.15.19"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true).Build();

            return new TasksContext(config.GetConnectionString("ConStr"));
        }
    }

    public class TasksRepository
    {
        private string _conn;
        public TasksRepository(string conn)
        {
            _conn = conn;
        }

        public void AddUser(User u)
        {
            using (var context = new TasksContext(_conn))
            {
                var user = new User
                {
                    Name = u.Name,
                    Email = u.Email,
                    Password = HashPassword(u.Password)
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Match(string input, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(input, passwordHash);
        }

        public User GetUserByEmail(string email)
        {
            using (var context = new TasksContext(_conn))
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public IEnumerable<Assignment> GetAssignments()
        {
            using (var context = new TasksContext(_conn))
            {
                return context.Assignments.ToList();
            }
        }

        public IEnumerable<Assignment> GetIncompletedAssignments()
        {
            using (var context = new TasksContext(_conn))
            {
                return context.Assignments.Include(a => a.User).Where(a => a.Status != Status.complete).ToList();
            }
        }

        public Assignment AddTask(Assignment assignment)
        {
            using (var context = new TasksContext(_conn))
            {
                assignment.Status = Status.incomplete;
                context.Assignments.Add(assignment);
                context.SaveChanges();
                return assignment;
            }
        }

        public Assignment AssignTaskToUser(Assignment assignment)
        {
            using (var context = new TasksContext(_conn))
            {
                assignment.User = GetUserByEmail(assignment.User.Email);
                //assignment.Status = Status.inProgress;
                //context.Assignments.Add(assignment);
                //context.SaveChanges();
                context.Database.ExecuteSqlCommand(
                    "UPDATE Assignments SET UserId=@userId, Status=@status WHERE Id=@taskId",
                    new SqlParameter("@userId", assignment.User.Id),
                    new SqlParameter("@status", Status.inProgress),
                    new SqlParameter("@taskId", assignment.Id));
                return assignment;
            }
        }

        public void FinishedAssignment(Assignment assignment)
        {
            using (var context = new TasksContext(_conn))
            {
                //assignment.Status = Status.complete;
                //context.Assignments.Add(assignment);
                //context.SaveChanges();
                context.Database.ExecuteSqlCommand(
                    "UPDATE Assignments SET Status=@status WHERE Id=@taskId",
                    new SqlParameter("@status", Status.complete),
                    new SqlParameter("@taskId", assignment.Id));
            }
        }
    }
}
