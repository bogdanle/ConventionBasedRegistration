using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace ConventionBasedRegistration.Models;

public abstract class SettingsBase
{
    private readonly Dictionary<string, string> _errors = new();

    public virtual bool IsValid()
    {
        Validate(true);
        return _errors.Count == 0;
    }

    public virtual void Validate(bool silent = false)
    {
        _errors.Clear();

        var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var prop in props)
        {
            ValidateProperty(prop);
        }

        if (_errors.Count > 0 && !silent)
        {
            throw new ValidationException(PrintErrors());
        }
    }

    public string PrintErrors()
    {
        var sb = new StringBuilder();

        foreach (var kvp in _errors)
        {
            sb.AppendLine($"{kvp.Key} : {kvp.Value}");
        }

        return sb.ToString();
    }

    private void ValidateProperty(PropertyInfo pi)
    {
        var results = new List<ValidationResult>(1);

        var context = new ValidationContext(this, null, null) { MemberName = pi.Name };

        bool result = Validator.TryValidateProperty(pi.GetValue(this), context, results);
        if (!result)
        {
            var validationResult = results.First();
            _errors[pi.Name] = validationResult.ErrorMessage!;
        }
        else
        {
            _errors.Remove(pi.Name);
        }
    }
}
