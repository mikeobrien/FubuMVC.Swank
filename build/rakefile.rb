require "albacore"
require_relative "filesystem"
require_relative "gallio-task"
require_relative "fubu-bottles"

reportsPath = "reports"
version = ENV["BUILD_NUMBER"]

task :build => :createPackage
task :deploy => :pushPackage

assemblyinfo :assemblyInfo do |asm|
    asm.version = version
    asm.company_name = "Ultraviolet Catastrophe"
    asm.product_name = "FubuMVC Swank"
    asm.title = "FubuMVC Swank"
    asm.description = "FubuMVC Swank."
    asm.copyright = "Copyright (c) #{Time.now.year} Ultraviolet Catastrophe"
    asm.output_file = "src/Swank/Properties/AssemblyInfo.cs"
end

msbuild :buildLibrary => :assemblyInfo do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/Swank/Swank.csproj"
end

msbuild :buildTests => :buildLibrary do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/Tests/Tests.csproj"
end

task :unitTestInit do
	FileSystem.EnsurePath(reportsPath)
end

gallio :unitTests => [:buildTests, :unitTestInit] do |runner|
	runner.echo_command_line = true
	runner.add_test_assembly("src/Tests/bin/Release/Tests.dll")
	runner.verbosity = 'Normal'
	runner.report_directory = reportsPath
	runner.report_name_format = 'tests'
	runner.add_report_type('Html')
end

nugetApiKey = ENV["NUGET_API_KEY"]
deployPath = "deploy"

packagePath = File.join(deployPath, "package")
nuspecFilename = "FubuMVC.Swank.nuspec"
contentPath = File.join(packagePath, "content/fubu-content")

task :prepPackage => :unitTests do
	FileSystem.DeleteDirectory(deployPath)
	FileSystem.EnsurePath(contentPath)
end

create_fubu_bottle :createBottle => :prepPackage do |bottle|
    bottle.project_path = 'src/Swank/Swank.csproj'
    bottle.output_path = contentPath
end

nuspec :createSpec => :createBottle do |nuspec|
   nuspec.id = "FubuMVC.Swank"
   nuspec.version = version
   nuspec.authors = "Mike O'Brien"
   nuspec.owners = "Mike O'Brien"
   nuspec.title = "FubuMVC Swank"
   nuspec.description = "Generates swank api specification and documentation."
   nuspec.summary = "Generates swank api specification and documentation."
   nuspec.language = "en-US"
   nuspec.licenseUrl = "https://github.com/mikeobrien/FubuMVC.Swank/blob/master/LICENSE"
   nuspec.projectUrl = "https://github.com/mikeobrien/FubuMVC.Swank"
   nuspec.iconUrl = "https://github.com/mikeobrien/FubuMVC.Swank/raw/master/misc/logo.png"
   nuspec.working_directory = packagePath
   nuspec.output_file = nuspecFilename
   nuspec.tags = "fubumvc"
   nuspec.dependency "FubuMVC.References", "0.9.0.0"
end

nugetpack :createPackage => :createSpec do |nugetpack|
   nugetpack.nuspec = File.join(packagePath, nuspecFilename)
   nugetpack.base_folder = packagePath
   nugetpack.output = deployPath
end

nugetpush :pushPackage => :createPackage do |nuget|
    nuget.apikey = nugetApiKey
    nuget.package = File.join(deployPath, "FubuMVC.Swank.#{version}.nupkg").gsub('/', '\\')
end