using ApiService.DBContext;
using ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiService.Services;

public static class AdminUserSeeder
{
    private const string AdminUsernameKey = "ADMIN_AUTH_USERNAME";
    private const string AdminPasswordKey = "ADMIN_AUTH_PASSWORD";
    private const string AdminRoleNameKey = "ADMIN_AUTH_ROLE_NAME";
    private const string DefaultDepartmentNameKey = "ADMIN_AUTH_DEPARTMENT_NAME";

    public static async Task SeedAsync(AppDBContext context, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        var adminUsername = GetRequiredConfigurationValue(configuration, AdminUsernameKey);
        var adminPassword = GetRequiredConfigurationValue(configuration, AdminPasswordKey);
        var adminRoleName = GetRequiredConfigurationValue(configuration, AdminRoleNameKey);
        var defaultDepartmentName = GetRequiredConfigurationValue(configuration, DefaultDepartmentNameKey);

        await MigratePlainTextPasswordsAsync(context, cancellationToken);

        var adminRole = await context.UserRoles
            .FirstOrDefaultAsync(r => r.Name == adminRoleName, cancellationToken);

        if (adminRole is null)
        {
            adminRole = new UserRole { Name = adminRoleName };
            context.UserRoles.Add(adminRole);
            await context.SaveChangesAsync(cancellationToken);
        }

        var department = await context.Departments
            .FirstOrDefaultAsync(d => d.Name == defaultDepartmentName, cancellationToken);

        if (department is null)
        {
            department = new Department { Name = defaultDepartmentName };
            context.Departments.Add(department);
            await context.SaveChangesAsync(cancellationToken);
        }

        var normalizedAdminUsername = adminUsername.Trim();
        var adminUser = await context.Users
            .FirstOrDefaultAsync(u => u.Name.ToLower() == normalizedAdminUsername.ToLower(), cancellationToken);

        if (adminUser is not null)
        {
            var shouldSave = false;

                if (!PasswordService.LooksLikeBcryptHash(adminUser.Password))
            {
                var sourcePassword = string.IsNullOrWhiteSpace(adminUser.Password)
                    ? adminPassword
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
            Name = adminUsername,
            Password = PasswordService.HashPassword(adminPassword),
            VactionDays = 0,
            DepartmentId = department.Id,
            UserRoleId = adminRole.Id
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    private static string GetRequiredConfigurationValue(IConfiguration configuration, string key)
    {
        var value = configuration[key] ?? Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Required environment variable '{key}' is not configured.");
        }

        return value;
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
