{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
  packages = with pkgs; [
    dotnet-sdk_10
    luajit
  ];

  DOTNET_ROOT = "${pkgs.dotnet-sdk_9}/share/dotnet";
}
