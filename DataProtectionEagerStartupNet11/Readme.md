# DataProtectionEagerStartupNet11

Minimal repro of an **undocumented .NET 11 behavior change**: `IDataProtectionProvider.CreateProtector`
now **eagerly loads the Data Protection key ring** (reads the key store) instead of deferring it to the
first `Protect`/`Unprotect`. On **.NET 10** the read is **lazy** — `CreateProtector` returns without
touching the store, and the read happens on first use.

The repro is a plain console app — no web host, no antiforgery — just a DI container:
`AddDataProtection()` → resolve `IDataProtectionProvider` → `CreateProtector("repro")` → `Protect("hello")`.
An `IXmlRepository` that throws when read makes both *that* and *where* the key store is read obvious.

**Why it matters:** real apps create a protector during startup (e.g. `app.UseAntiforgery()` resolves
`IAntiforgery`, cookie auth, etc.). With keys persisted to an external store (`PersistKeysToDbContext`,
Redis, Azure Blob), the app now does a key-store round-trip **at boot** and **fails to start** if the
store is unreachable — previously it started and only touched the store on the first request that used
data protection.

`FrameworkReference Microsoft.AspNetCore.App` (not a NuGet package) is used so Data Protection comes
from the shared framework and its behavior is driven by the **runtime** version — which is the point.

## Run

### One command (Nix) — recommended

Installs .NET 10 + the .NET 11 preview SDK and runs the project on **both** runtimes, printing the
contrast in one go:

```sh
nix run .#repro
```

### Interactive (Nix dev shell)

```sh
nix develop          # puts dotnet (10 + 11) on PATH
dotnet run -f net11.0
dotnet run -f net10.0
```

### Without Nix

Requires the .NET 10 SDK and the .NET 11 preview SDK from `global.json` installed, then
`dotnet run -f net11.0` / `dotnet run -f net10.0`.

## Expected output

Same code, same SDK — only the target framework (and thus the runtime) differs. The exception is left
unhandled so the stack trace pinpoints where the read happens.

**`net11.0`** — read is **eager**: Step 1 throws, stack ends at `CreateProtector`:

```
Step 1: CreateProtector("repro")  — .NET 11 reads the key store HERE (eager)
Unhandled exception. System.InvalidOperationException: IXmlRepository.GetAllElements() called — the key store was read
   at TripwireXmlRepository.Microsoft.AspNetCore.DataProtection.Repositories.IXmlRepository.GetAllElements()
   at Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager.GetAllKeys()
   at Microsoft.AspNetCore.DataProtection.KeyManagement.KeyRingProvider.GetCurrentKeyRingCoreNew(DateTime utcNow, Boolean forceRefresh)
   at Microsoft.AspNetCore.DataProtection.KeyManagement.KeyRingBasedDataProtectionProvider.CreateProtector(String purpose)
   at Program.<Main>$(String[] args)
```

**`net10.0`** — read is **lazy**: Step 1 returns, Step 2 (`Protect`) throws (how it used to work):

```
Step 1: CreateProtector("repro")  — .NET 11 reads the key store HERE (eager)
        -> returned without reading the key store  [.NET 10 path]
Step 2: Protect("hello")  — .NET 10 reads the key store HERE (lazy, on first use)
Unhandled exception. System.Security.Cryptography.CryptographicException: An error occurred while trying to encrypt the provided data.
 ---> System.InvalidOperationException: IXmlRepository.GetAllElements() called — the key store was read
   at TripwireXmlRepository.Microsoft.AspNetCore.DataProtection.Repositories.IXmlRepository.GetAllElements()
   at Microsoft.AspNetCore.DataProtection.KeyManagement.KeyRingBasedDataProtector.Protect(Byte[] plaintext)
   at Microsoft.AspNetCore.DataProtection.DataProtectionCommonExtensions.Protect(IDataProtector protector, String plaintext)
   at Program.<Main>$(String[] args)
```

## Root cause

`KeyRingBasedDataProtectionProvider.CreateProtector` changed from lazy to eager between
[release/10.0](https://github.com/dotnet/aspnetcore/blob/release/10.0/src/DataProtection/DataProtection/src/KeyManagement/KeyRingBasedDataProtectionProvider.cs)
(returns the protector without reading the ring) and
[main](https://github.com/dotnet/aspnetcore/blob/main/src/DataProtection/DataProtection/src/KeyManagement/KeyRingBasedDataProtectionProvider.cs)
(calls `_keyRingProvider.GetCurrentKeyRing()` eagerly). Not listed in the .NET 11 / ASP.NET Core 11
breaking-change docs as of 2026-06-26.
