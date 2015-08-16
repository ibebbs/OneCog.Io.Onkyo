#r "./src/packages/FAKE.4.1.3/tools/FakeLib.dll"

open Fake
open System.IO;

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

let portableAssemblies = [ "OneCog.Io.Onkyo.dll"; "OneCog.Io.Onkyo.pdb"; ]

let libDir = "lib"
let target = "portable-win81+wpa81+net45+uap10.0"
 
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
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = buildDir + "TestResults.xml" })
)

Target "Package" (fun _ ->

    portableAssemblies |> List.map(fun a -> buildDir @@ a) |> CopyFiles deployDir  

    let targetFiles = portableAssemblies |> List.map(fun a -> (a, Some(Path.Combine(libDir, target)), None))

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
            Version = version
            Files = targetFiles
            Publish = false }) 
            "./OneCog.Io.Onkyo.nuspec"
)

Target "Run" (fun _ -> 
    trace "FAKE build complete"
)
  
// Dependencies
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Package"
  ==> "Run"
 
// start build
RunTargetOrDefault "Run"