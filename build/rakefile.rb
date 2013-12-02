require "albacore"
require_relative "path"
require_relative "gallio-task"
require_relative "fubu-bottles"

reportsPath = "reports"
version = ENV["BUILD_NUMBER"]

task :build => :pushLocalPackage
task :deploy => :pushPublicPackage

assemblyinfo :assemblyInfo do |asm|
    asm.version = version
    asm.company_name = "Ultraviolet Catastrophe"
    asm.product_name = "FubuMVC Swank"
    asm.title = "FubuMVC Swank"
    asm.description = "FubuMVC Swank."
    asm.copyright = "Copyright (c) #{Time.now.year} Ultraviolet Catastrophe"
    asm.output_file = "src/Swank/Properties/AssemblyInfo.cs"
end

create_fubu_bottle :createBottle do |bottle|
    bottle.package_type = :assembly
    bottle.source_path = 'src/Swank'
    bottle.project_file = 'Swank.csproj'
end

msbuild :buildLibrary => %w{assemblyInfo createBottle} do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/Swank/Swank.csproj"
end

msbuild :buildTestHarness => :buildLibrary do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/TestHarness/TestHarness.csproj"
end

msbuild :buildTests => :buildTestHarness do |msb|
    msb.properties :configuration => :Release
    msb.targets :Clean, :Build
    msb.solution = "src/Tests/Tests.csproj"
end

task :unitTestInit do
	Path.EnsurePath(reportsPath)
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
artifactsPath = 'artifacts'

packagePath = File.join(deployPath, "package")
nuspecFilename = "FubuMVC.Swank.nuspec"
packageContentPath = File.join(packagePath, "content/fubu-content")
packageFilePath = File.join(deployPath, "FubuMVC.Swank.#{version}.nupkg")

task :prepPackage => :unitTests do
	Path.DeleteDirectory(deployPath)
	Path.EnsurePath(packageContentPath)
	Path.DeleteDirectory(artifactsPath)
    Path.EnsurePath(artifactsPath)
    packageLibPath = File.join(packagePath, "lib")
	Path.EnsurePath(packageLibPath)
	Path.CopyFiles("src/Swank/bin/FubuMVC.Swank.*", packageLibPath)
end


nuspec :createSpec => :prepPackage do |nuspec|
   nuspec.id = "FubuMVC.Swank"
   nuspec.version = version
   nuspec.authors = "Mike O'Brien"
   nuspec.owners = "Mike O'Brien"
   nuspec.title = "FubuMVC Swank"
   nuspec.description = "A FubuMVC plugin that allows you to describe and publish documentation for RESTful services."
   nuspec.summary = "A FubuMVC plugin that allows you to describe and publish documentation for RESTful services."
   nuspec.language = "en-US"
   nuspec.licenseUrl = "https://github.com/mikeobrien/FubuMVC.Swank/blob/master/LICENSE"
   nuspec.projectUrl = "https://github.com/mikeobrien/FubuMVC.Swank"
   nuspec.iconUrl = "https://github.com/mikeobrien/FubuMVC.Swank/raw/master/misc/logo.png"
   nuspec.working_directory = packagePath
   nuspec.output_file = nuspecFilename
   nuspec.tags = "fubumvc"
   nuspec.dependency "FubuMVC.Core", "1.0.0.0"
   nuspec.dependency "FubuMVC.Spark", "1.0.0.0"
   nuspec.dependency "FubuMVC.Media", "0.9.5.0"
   nuspec.dependency "MarkdownSharp", "1.0.0.0"
end

nugetpack :createPackage => :createSpec do |nugetpack|
   nugetpack.nuspec = File.join(packagePath, nuspecFilename)
   nugetpack.base_folder = packagePath
   nugetpack.output = deployPath
end

task :pushLocalPackage => :createPackage do
	Path.CopyFiles(packageFilePath, artifactsPath)
end

nugetpush :pushPublicPackage => :createPackage do |nuget|
    nuget.apikey = nugetApiKey
    nuget.package = packageFilePath.gsub('/', '\\')
end
