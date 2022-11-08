using FluentValidation;
using IGroceryStore.Shared.Settings;

namespace IGroceryStore.Shops.Settings;

internal class DynamoDbSettings : SettingsBase<DynamoDbSettings>, ISettings
{
    public static string SectionName => "Shops:DynamoDb";
    public string LocalServiceUrl { get; set; }
    public bool LocalMode { get; set; }
    public double MaxDelayForTableCreationInSeconds { get; set; }

    public DynamoDbSettings()
    {
        RuleFor(x => x.LocalServiceUrl)
            .NotEmpty()
            .Custom((issuer, context) =>
            {
                if (!Uri.TryCreate(issuer, UriKind.Absolute, out var uri))
                {
                    context.AddFailure($"Issuer must be a valid URI but found {uri}");
                }
            });
    }
}
