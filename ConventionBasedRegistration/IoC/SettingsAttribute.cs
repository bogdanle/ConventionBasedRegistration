namespace ConventionBasedRegistration.IoC;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SettingsAttribute : Attribute
{
    public string? Section { get; set; }
}
