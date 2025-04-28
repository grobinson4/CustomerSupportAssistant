using CustomerSupportAssistant.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CustomerSupportAssistant.Persistence.Seed;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Apply any pending migrations first
        await context.Database.MigrateAsync();

        // Seed Users
        if (!context.Users.Any())
        {
            var user = new User
            {
                Email = "testuser@example.com",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("Test1234!"),
                Name = "Test User",
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Seed Projects
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "testuser@example.com");
        if (existingUser != null && !context.Projects.Any())
        {
            var project = new Project
            {
                Title = "First Project",
                Description = "This is your first auto-seeded project.",
                UserId = existingUser.Id
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            // Seed Tasks
            var task = new TaskItem
            {
                ProjectId = project.Id,
                Title = "First Task",
                Description = "Complete the first auto-seeded task.",
                Status = "Pending",
                DueDate = DateTime.UtcNow.AddDays(7)
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();
        }
    }
}
