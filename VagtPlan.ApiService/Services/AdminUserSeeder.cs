using ApiService.DBContext;
using ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Services;

public static class AdminUserSeeder
{
    public const string AdminUsername = "Admin";
    public const string AdminPassword = "Password1!";
    public const string AdminRoleName = "Admin";
    public const string DefaultDepartmentName = "Administration";

    public static async Task SeedAsync(AppDBContext context, CancellationToken cancellationToken = default)
    {
        await MigratePlainTextPasswordsAsync(context, cancellationToken);

        var adminRole = await context.UserRoles
            .FirstOrDefaultAsync(r => r.Name == AdminRoleName, cancellationToken);

        if (adminRole is null)
        {
            adminRole = new UserRole { Name = AdminRoleName };
            context.UserRoles.Add(adminRole);
            await context.SaveChangesAsync(cancellationToken);
        }

        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Name == DefaultDepartmentName, cancellationToken);

        if (department is null)
        {
            department = new Department { Name = DefaultDepartmentName };
            context.Departments.Add(department);
            await context.SaveChangesAsync(cancellationToken);
        }

        var adminUser = await context.Users
            .FirstOrDefaultAsync(u => u.Name.ToLower() == AdminUsername, cancellationToken);

        if (adminUser is not null)
        {
            var shouldSave = false;

            if (!PasswordService.LooksLikeBcryptHash(adminUser.Password))
            {
                var sourcePassword = string.IsNullOrWhiteSpace(adminUser.Password)
                    ? AdminPassword
                    : adminUser.Password;
                adminUser.Password = PasswordService.HashPassword(sourcePassword);
                shouldSave = true;
            }

            if (adminUser.UserRoleId != adminRole.Id)
            {
                adminUser.UserRoleId = adminRole.Id;
                shouldSave = true;
            }

            if (adminUser.DepartmentId != department.Id)
            {
                adminUser.DepartmentId = department.Id;
                shouldSave = true;
            }

            if (shouldSave)
            {
                await context.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        context.Users.Add(new User
        {
            Name = AdminUsername,
            Password = PasswordService.HashPassword(AdminPassword),
            VactionDays = 0,
            DepartmentId = department.Id,
            UserRoleId = adminRole.Id
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task MigratePlainTextPasswordsAsync(AppDBContext context, CancellationToken cancellationToken)
    {
        var usersWithPlainTextPasswords = (await context.Users
            .Where(u => u.Password != null && u.Password != "")
            .ToListAsync(cancellationToken))
            .Where(u => !PasswordService.LooksLikeBcryptHash(u.Password))
            .ToList();

        if (usersWithPlainTextPasswords.Count == 0)
        {
            return;
        }

        foreach (var user in usersWithPlainTextPasswords)
        {
            user.Password = PasswordService.HashPassword(user.Password!);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
