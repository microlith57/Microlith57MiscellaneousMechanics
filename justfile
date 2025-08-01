default: build

build:
    nix-shell --command "dotnet build"
