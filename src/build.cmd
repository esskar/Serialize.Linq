@echo off
".nuget\NuGet.exe" install -OutputDirectory packages .\packages.config
"packages\Sake.0.2\tools\sake.exe" %*