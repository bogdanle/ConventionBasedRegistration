# ConventionBasedRegistration
This is simple project that adds convention-based registration support for built-in .NET IoC container. 

# How to use it
Add this line in your Program.cs:<br>
```csharp
string searchPattern = "Prefix*.dll";  
builder.Services.DiscoverAndRegisterTypes(searchPattern);

Replace 'Prefix' with your company/namespace naming convention to avoid importing types from 3rd party libraries.
