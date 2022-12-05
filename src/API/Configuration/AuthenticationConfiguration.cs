using System.Security.Claims;
using FluentValidation;
using IGroceryStore.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace IGroceryStore.API.Configuration;

public class Auth0Settings : SettingsBase<Auth0Settings>, ISettings
{
    public static string SectionName => "Authentication:Schemes:Bearer";
    
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
    
    public Auth0Settings()
    {
        RuleFor(x => ValidIssuer).NotEmpty();
        RuleFor(x => ValidAudience).NotEmpty();
    }
}

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetOptions<Auth0Settings>();
        
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.Authority = options.ValidIssuer;
            o.Audience = options.ValidAudience;
        });
        
        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy("read:messages", policy 
                => policy.Requirements.Add(new HasScopeRequirement("read:messages"))
            );
        });
    }
}

public class HasScopeRequirement : IAuthorizationRequirement
{
    public string Scope { get; }

    public HasScopeRequirement(string scope)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
    }
}
    
public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement) 
    {
        if (!context.User.HasClaim(c => c.Type == "scope")) return Task.CompletedTask;

        var scopes = context.User.FindFirstValue("scope")?
            .Split(' ');
        if (scopes == null) return Task.CompletedTask;

        if (scopes.Any(s => s == requirement.Scope)) context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
