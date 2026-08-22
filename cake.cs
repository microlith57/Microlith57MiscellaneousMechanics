#!/usr/bin/env dotnet
#:sdk Cake.Sdk@6.2.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

Task("CleanCode")
    .Does(() => {
        CleanDirectories("./{Code,Source,Rysy}/{bin,obj}");
    });

Task("CleanAssets")
    .Does(() => {
        CleanDirectory("./Loenn");
        CleanDirectory("./Graphics");
    });

Task("Build")
    .IsDependentOn("CleanCode")
    .Does(() => {
        DotNetBuild("./Microlith57Misc.slnx", new() {
            Configuration = configuration,
        });
    });

Task("Preprocess")
    .IsDependentOn("CleanAssets")
    .Does(() => {
        var luajit = new CommandSettings()
            .WithToolName("LuaJIT")
            .WithExecutableNames(new string[] {"luajit", "luajit.exe" })
            .WithWorkingDirectory("./preprocess")
            .WithEnvironmentVariable("MICROLITH57_MISC_CONFIGURATION", configuration.ToLower());
        var arg = new ProcessArgumentBuilder().Append("preprocess.lua");

        Command(luajit, arg);
    });

static FilePathCollection ToPackage()
    => GetFiles("./Code/bin/Microlith57MiscellaneousMechanics.{dll,pdb}")
     + GetFiles("./Rysy/bin/Microlith57Misc.Rysy.{dll,pdb}")
     + GetFiles("./everest.yaml")
     + GetFiles("./.everestignore")
     + GetFiles("./Loenn/**/*.{lua,lang}")
     + GetFiles("./Graphics/**/*.{png,xml}");

Task("Zip")
    .WithCriteria(c => target == "Zip" || configuration == "Release")
    .IsDependentOn("Build")
    .IsDependentOn("Preprocess")
    .Does(() => Zip("./", "Microlith57MiscellaneousMechanics.zip", ToPackage()));

Task("Copy")
    .IsDependentOn("Build")
    .IsDependentOn("Preprocess")
    .Does(() => {
        CleanDirectory("./artifact");
        CopyFiles(ToPackage(), "./artifact", preserveFolderStructure: true);
    });

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Preprocess")
    .IsDependentOn("Zip");

RunTarget(target);
