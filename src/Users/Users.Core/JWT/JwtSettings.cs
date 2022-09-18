using System.Text;
using FluentValidation;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IGroceryStore.Users.Core.JWT;

internal class JwtSettings : SettingsBase<JwtSettings>, ISettings
{
    public static string SectionName => "Users:JwtSettings";
    public string Key { get; set; }
    public int ExpireSeconds { get; set; }
    public string Issuer { get; set; }
    public int ClockSkew { get; set; }
    public long TicksPerSecond = 10_000 * 1_000;
    
    public static void Configure(JwtBearerOptions jwtBearerOptions, string audience, JwtSettings? jwtSettings)
    {
        jwtBearerOptions.RequireHttpsMetadata = false;
        jwtBearerOptions.SaveToken = true;
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true, 
            ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew)
        };
        if (audience == Tokens.Audience.Access)
        {
            jwtBearerOptions.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        }
    }

    public JwtSettings()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MinimumLength(16);
        
        RuleFor(x => x.Issuer)
            .NotEmpty()
            .Custom((issuer, context) =>
            {
                if (!Uri.TryCreate(issuer, UriKind.Absolute, out var uri))
                {
                    context.AddFailure("Issuer must be a valid URI");
                }
            });
    }
}
