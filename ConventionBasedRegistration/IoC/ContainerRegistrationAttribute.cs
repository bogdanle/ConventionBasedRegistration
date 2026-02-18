namespace ConventionBasedRegistration.IoC;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ContainerRegistrationAttribute : Attribute
{
    public ContainerRegistrationAttribute(ObjectLifetime lifetime = ObjectLifetime.Default)
    {
        Lifetime = lifetime;
    }

    /// <summary>
    /// Gets or sets the interface type. If left blank it will be deducted from class name, assuming standard naming convention is used.
    /// </summary>
    public Type? InterfaceType { get; set; }

    /// <summary>
    /// Gets or sets the object lifetime, like Transient or Singleton.
    /// </summary>
    public ObjectLifetime Lifetime { get; set; }

    /// <summary>
    /// Gets or sets the environment.
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    public string? Name { get; set; }

    public bool Ignore { get; set; }
}
