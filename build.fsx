#I "packages/FAKE/tools"
#r "Nuget.Core.dll"
#r "FakeLib.dll"
open System
open System.IO
open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper

let project = "Climax.Web.Http"
let summary = "A collection of add-ons for ASP.NET Web API"
let description = """
  A collection of add-ons for ASP.NET Web API."""
// List of author names (for NuGet package)
let authors = [ "filipw"; "climax-media" ]
let tags = "webapi climax aspnet aspnetwebapi"
let solutionFile = "Climax.Web.Http.sln"
let outputDir = "bin"

// Read additional information from the release notes document
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let (!!) includes = (!! includes).SetBaseDirectory __SOURCE_DIRECTORY__
let release = parseReleaseNotes (IO.File.ReadAllLines "RELEASE_NOTES.md")
let nugetVersion = release.NugetVersion

Target "AssemblyInfo" (fun _ ->
  let fileName = "./src/Climax.Web.Http/Properties/AssemblyInfo.cs"
  CreateCSharpAssemblyInfo fileName
      [ Attribute.Title project
        Attribute.Product project
        Attribute.Description summary
        Attribute.Version release.AssemblyVersion
        Attribute.FileVersion release.AssemblyVersion ] )

Target "RestorePackages" RestorePackages

Target "Clean" (fun _ ->
    CleanDirs [outputDir]
)

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuildRelease outputDir "Rebuild"
    |> ignore
)

Target "CopyFiles" (fun _ ->
    [ "LICENSE.txt" ] |> CopyTo outputDir
)

Target "NuGet" (fun _ ->
    let webApiVersion = GetPackageVersion "packages" "Microsoft.AspNet.WebApi.Core"
    NuGet (fun p -> 
        { p with   
            Authors = authors
            WorkingDir = "./"
            Project = project
            Summary = summary
            Description = description
            Version = release.NugetVersion
            ReleaseNotes = String.Join(Environment.NewLine, release.Notes)
            Tags = tags
            OutputPath = outputDir
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey"
            Dependencies = [ "Microsoft.AspNet.WebApi.Core", webApiVersion ]
            Files = [ (@"bin\Climax.Web.Http.dll", Some "lib/net45", None)
                      (@"bin\Climax.Web.Http.pdb", Some "lib/net45", None) ] })
        ("Climax.Web.Http.nuspec")
)

Target "All" DoNothing

Target "BuildPackage" DoNothing

"Clean"
  ==> "RestorePackages"
  ==> "AssemblyInfo"
  ==> "Build"
  ==> "CopyFiles"
  ==> "NuGet"
  ==> "All"

RunTargetOrDefault "All"