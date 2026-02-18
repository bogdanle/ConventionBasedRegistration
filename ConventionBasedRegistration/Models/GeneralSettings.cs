using System.ComponentModel.DataAnnotations;
using ConventionBasedRegistration.IoC;

namespace ConventionBasedRegistration.Models;

[Settings(Section = "General")]
public class GeneralSettings : SettingsBase
{
    [Required]
    [MinLength(3)]
    public static string Environment => System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
    
    [Required]
    public string Configuration { get; set; } = "Default";

    [Required]
    [MinLength(10)]
    public string BaseUrl { get; set; } = string.Empty;
    
    public string BuildNotificationsHookUrl { get; set; } = string.Empty;
}
