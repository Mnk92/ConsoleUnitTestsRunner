@echo off

set msbuild="c:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

%msbuild% Installer\Installer.wixproj /m /t:build /p:Configuration="Release" /p:SolutionDir=%CD%

if %ERRORLEVEL%==0 goto script_end
echo Build failed!

:script_end