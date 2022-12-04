using FluentValidation;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shared.Settings;

public interface ISettings
{
    static abstract string SectionName { get; }
}

public abstract class SettingsBase<T> : AbstractValidator<T>, IValidateOptions<T>
    where T : class, ISettings, new()
{
    public ValidateOptionsResult Validate(string? name, T options)
    {
        var result = Validate(options);
        return result.IsValid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(result.Errors.Select(x => x.ErrorMessage));
    }
}
