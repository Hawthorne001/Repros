using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;

// No web host, no antiforgery — just a DI container with Data Protection. A key repository that
// throws when read makes both *that* and *where* the key store is read obvious. The exception is
// left unhandled on purpose so the stack trace shows the exact call chain:
//   .NET 11 -> throws from Step 1 (CreateProtector, eager)
//   .NET 10 -> reaches Step 2 and throws from Protect (lazy, on first use)
var services = new ServiceCollection();
services
    .AddDataProtection()
    .AddKeyManagementOptions(options => options.XmlRepository = new TripwireXmlRepository());

using var provider = services.BuildServiceProvider();
var dataProtection = provider.GetRequiredService<IDataProtectionProvider>();

Console.WriteLine($"Framework: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
Console.WriteLine();

Console.WriteLine("Step 1: CreateProtector(\"repro\")  — .NET 11 reads the key store HERE (eager)");
var protector = dataProtection.CreateProtector("repro");
Console.WriteLine("        -> returned without reading the key store  [.NET 10 path]");
Console.WriteLine();

Console.WriteLine("Step 2: Protect(\"hello\")  — .NET 10 reads the key store HERE (lazy, on first use)");
protector.Protect("hello");
Console.WriteLine("        -> succeeded");

internal sealed class TripwireXmlRepository : IXmlRepository
{
    IReadOnlyCollection<XElement> IXmlRepository.GetAllElements() =>
        throw new InvalidOperationException("IXmlRepository.GetAllElements() called — the key store was read");

    void IXmlRepository.StoreElement(XElement element, string friendlyName)
    {
        // no-op: this repro never writes keys.
    }
}
