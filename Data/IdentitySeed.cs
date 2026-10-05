using Microsoft.AspNetCore.Identity;

namespace ClinicManagement.Data;

public static class IdentitySeed
{
    public static async Task SeedRolesAsync(
        IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        [
            "Admin",
            "Doctor",
            "Receptionist",
            "Patient"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Could not create role '{role}': {errors}");
                }
            }
        }
    }
    public static async Task RemoveAdminFromUserAsync(
    IServiceProvider services,
    string email)
{
    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    var user = await userManager.FindByEmailAsync(email);

    if (user is null)
    {
        return;
    }

    if (await userManager.IsInRoleAsync(user, "Admin"))
    {
        await userManager.RemoveFromRoleAsync(user, "Admin");
    }
}
}