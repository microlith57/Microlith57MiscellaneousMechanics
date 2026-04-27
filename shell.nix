{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
  packages = with pkgs; [
    luajit
  ];

  DOTNET_ROOT = "${pkgs.dotnet-sdk_9}/share/dotnet";
}
