{
  description = "Repro: .NET 11 Data Protection eagerly reads the key store at startup via UseAntiforgery (works on .NET 10)";

  # Pinned to a nixpkgs rev that ships the .NET 11 preview SDK matching global.json.
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/567a49d1913ce81ac6e9582e3553dd90a955875f";

  outputs = {
    self,
    nixpkgs,
  }: let
    systems = ["x86_64-linux" "aarch64-linux" "x86_64-darwin" "aarch64-darwin"];
    forAllSystems = f: nixpkgs.lib.genAttrs systems (system: f system nixpkgs.legacyPackages.${system});
    # One combined install exposes both runtimes so the same project can run on net10 and net11.
    dotnetFor = pkgs: with pkgs.dotnetCorePackages; combinePackages [sdk_10_0 sdk_11_0];
  in {
    devShells = forAllSystems (_system: pkgs: let
      dotnet = dotnetFor pkgs;
    in {
      default = pkgs.mkShell {
        packages = [dotnet];
        DOTNET_ROOT = "${dotnet}/share/dotnet";
        DOTNET_CLI_TELEMETRY_OPTOUT = "1";
        DOTNET_NOLOGO = "1";
      };
    });

    # `nix run .#repro` (or just `nix run`) installs .NET and runs the project on both runtimes,
    # printing the contrast: .NET 10 starts fine, .NET 11 reads the key store at startup and throws.
    packages = forAllSystems (_system: pkgs: let
      dotnet = dotnetFor pkgs;
    in {
      default = pkgs.writeShellApplication {
        name = "repro";
        runtimeInputs = [dotnet];
        text = ''
          work="$(mktemp -d)"
          trap 'rm -rf "$work"' EXIT
          cp -R ${self}/. "$work"/
          chmod -R u+w "$work"
          cd "$work"

          export DOTNET_ROOT="${dotnet}/share/dotnet"
          export DOTNET_CLI_TELEMETRY_OPTOUT=1
          export DOTNET_NOLOGO=1
          export DOTNET_CLI_HOME="$work"
          export HOME="$work"

          echo "######################################################################"
          echo "#  .NET 10  -> key store read LAZILY at first Protect (CreateProtector returns first)"
          echo "######################################################################"
          dotnet run -f net10.0 || true

          echo
          echo "######################################################################"
          echo "#  .NET 11  -> key store read EAGERLY at CreateProtector"
          echo "######################################################################"
          dotnet run -f net11.0 || true
        '';
      };
    });

    apps = forAllSystems (system: _pkgs: let
      prog = "${self.packages.${system}.default}/bin/repro";
    in {
      default = {
        type = "app";
        program = prog;
      };
      repro = {
        type = "app";
        program = prog;
      };
    });
  };
}
