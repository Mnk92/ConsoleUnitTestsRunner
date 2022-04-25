@ECHO OFF

SET msbuild="c:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

%msbuild% "%CD%\ConsoleUnitTestsRunner.sln" /m /t:restore /t:pack /p:Configuration="Release" /p:Platform="Any Cpu" /p:VersionPrefix="2.0.0.0"

move bin\Release\*.nupkg ../nuget/
