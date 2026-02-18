using System.Reflection;
using ConventionBasedRegistration.IoC;

namespace ConventionBasedRegistration.Extensions;

/// <summary>
/// Class that automatically registers types with .NET built-in IoC container,
/// either based on convention or using the metadata from custom attribute.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IList<Type> RegisteredTypes { get; } = new List<Type>();

    public static string Environment { get; set; } = "Local";

    public static void DiscoverAndRegisterSettings(this IServiceCollection serviceCollection, string searchPattern = "*.dll")
    {
        var types = DiscoverTypes(searchPattern, false);

        foreach (var type in types.Where(type => type.GetCustomAttribute(typeof(SettingsAttribute)) is SettingsAttribute))
        {
            serviceCollection.AddSingleton(type);
            Console.WriteLine($"Auto-registering settings class {type.Name}");
        }
    }

    public static void BindSettings(this IServiceProvider serviceProvider, IConfiguration configuration, string searchPattern)
    {
        var types = DiscoverTypes(searchPattern, false);

        foreach (var type in types)
        {
            if (type.GetCustomAttribute(typeof(SettingsAttribute)) is SettingsAttribute attr)
            {
                var obj = serviceProvider.GetService(type);
                if (obj != null)
                {
                    configuration.Bind(attr.Section ?? type.Name, obj);
                    Console.WriteLine($"Auto-binding settings class {type.Name}");
                }
            }
        }
    }

    public static void DiscoverAndRegisterTypes(this IServiceCollection serviceCollection, string searchPattern)
    {
        var types = DiscoverTypes(searchPattern);

        foreach (var type in types)
        {
            try
            {
                if (type.GetCustomAttribute(typeof(ContainerRegistrationAttribute)) is ContainerRegistrationAttribute attr)
                {
                    if (attr.Ignore)
                    {
                        continue;
                    }

                    attr.InterfaceType ??= type.GetInterface($"I{type.Name}");

                    if (string.IsNullOrEmpty(attr.Environment) || attr.Environment == Environment)
                    {
                        RegisterType(serviceCollection, type, attr);
                    }
                }
                else if (!type.IsGenericType)
                {
                    // In case the attribute was not specified, check if the type implements interface that matches the convention and register it
                    var interfaceType = type.GetInterface($"I{type.Name}");
                    if (interfaceType != null)
                    {
                        RegisterType(serviceCollection, type, new ContainerRegistrationAttribute { InterfaceType = interfaceType });
                    }
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void RegisterType(IServiceCollection serviceCollection, Type type, ContainerRegistrationAttribute attribute)
    {
        try
        {
            switch (attribute.Lifetime)
            {
                case ObjectLifetime.Default:
                case ObjectLifetime.Transient:
                    serviceCollection.AddTransient(attribute.InterfaceType!, type);
                    break;

                case ObjectLifetime.Singleton:
                    serviceCollection.AddSingleton(attribute.InterfaceType!, type);
                    break;

                case ObjectLifetime.Scoped:
                    serviceCollection.AddScoped(attribute.InterfaceType!, type);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }

        Console.WriteLine($"Auto-registering type {type.Name}, object lifetime: {attribute.Lifetime}");
        RegisteredTypes.Add(type);
    }

    private static List<Type> DiscoverTypes(string searchPattern, bool withInterfaces = true)
    {
        var result = new List<Type>();

        string fullPath = Assembly.GetExecutingAssembly().Location;

        string? directoryName = Path.GetDirectoryName(fullPath);
        if (directoryName != null)
        {
            string[] allFiles = Directory.GetFiles(directoryName, searchPattern);

            var files = allFiles.ToList();

            var assemblies = files.Where(predicate: f => !f.Contains("WebApp.Client.dll", StringComparison.Ordinal)).Select(Assembly.LoadFrom).ToList();

            var types = LoadTypes(assemblies);

            result.AddRange(types.Where(t => t.IsClass && (!withInterfaces || t.GetInterfaces().Length > 0)));
        }

        return result;
    }

    private static IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies, bool skipOnError = true)
    {
        return assemblies.SelectMany(a =>
        {
            IEnumerable<TypeInfo> source;

            try
            {
                source = a.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                if (!skipOnError)
                {
                    throw;
                }

                source = ex.Types.TakeWhile(t => t != null).Select(t => t!.GetTypeInfo());
            }

            return source.Where(ti => ti.IsClass & !ti.IsAbstract && !ti.IsValueType && ti.IsVisible).Select(ti => ti.AsType());
        });
    }
}
