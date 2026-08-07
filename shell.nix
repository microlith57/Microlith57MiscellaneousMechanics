{ pkgs ? import <nixpkgs> {} }:
pkgs.mkShell {
  packages = with pkgs; [
    luajit

    (writeScriptBin "cake" ''
      #!${bash}/bin/bash
      cd "''${DIRENV_DIR#-}"
      dotnet cake.cs -- $@
    '')
  ];

  DOTNET_ROOT = "${pkgs.dotnet-sdk_9}/share/dotnet";
}
