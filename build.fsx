#r "./src/packages/FAKE.4.64.12/tools/FakeLib.dll"

open System.IO
open Fake
open Fake.Testing

//RestorePackages()
 
// Properties
let buildDir = "./build/"
let deployDir = "./deploy/"
 
// version info
let version = environVarOrDefault "PackageVersion" "1.0.0.0"  // or retrieve from CI server
let summary = "Open source, portable library from interacting with Onkyo home cinema devices via eICSP protocol."
let copyright = "Ian Bebbington, 2014"
let tags = "Onkyo eISCP ISCP portable"
let description = "Open source, portable library from interacting with Onkyo home cinema devices via eICSP protocol."

let nunitRunnerPath = "tools/NUnit.ConsoleRunner.3.8.0/tools/nunit3-console.exe"

let portableAssemblies = [ "OneCog.Io.Onkyo.dll"; "OneCog.Io.Onkyo.pdb"; ]

let libDir = "lib"
let srcDir = "src"
let netStandardTarget = "netstandard1.6"
let srcTarget = "src"
 
// Targets
Target "Clean" (fun _ ->
    CleanDirs [ deployDir ]
)
 
Target "Build" (fun _ ->
   !! "./src/**/*.csproj"
     |> MSBuildRelease buildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (buildDir + "*.Test.dll") 
    ++ (buildDir + "*.Tests.dll")
      |> NUnit3 (fun p -> {p with ToolPath = nunitRunnerPath })
)

Target "Package" (fun _ ->

    // Copy to deployment folder
    CopyWithSubfoldersTo deployDir [ !! "./src/**/*.cs" ]
    portableAssemblies |> List.map(fun a -> buildDir @@ a) |> CopyFiles deployDir  

    // Setup files to include in package
    let netStandardFiles = portableAssemblies |> List.map(fun a -> (a, Some(Path.Combine(libDir, netStandardTarget)), None))
    let srcFiles = [ (@"src\**\*.*", Some "src", None) ]

    let dependencies = getDependencies "./src/OneCog.Io.Onkyo/OneCog.Io.Onkyo.csproj" |> List.filter (fun (name, version) -> name <> "FAKE")
    
    NuGet (fun p -> 
        {p with
            Authors = [ "Ian Bebbington" ]
            Project = "OneCog.Io.Onkyo"
            Description = description
            Summary = summary
            Copyright = copyright
            Tags = tags
            OutputPath = deployDir
            WorkingDir = deployDir
            SymbolPackage = NugetSymbolPackage.Nuspec
            Version = version
            Dependencies = dependencies
            Files = netStandardFiles @ srcFiles
            Publish = false }) 
            "./OneCog.Io.Onkyo.nuspec"
)

Target "Run" (fun _ -> 
    trace "FAKE build complete"
)
  
// Dependencies
"Clean"
  ==> "Build"
//  ==> "Test"
  ==> "Package"
  ==> "Run"
 
// start build
RunTargetOrDefault "Run"