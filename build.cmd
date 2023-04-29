@echo off

set corflags="c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\corflags.exe"
set msbuild="c:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe"

%msbuild% "%CD%\ConsoleUnitTestsRunner.sln" /m /t:rebuild /p:Configuration="Release" /p:Platform="Any Cpu"
copy /Y "%CD%\bin\Release\net6.0\ConsoleUnitTestsRunner.exe" "%CD%\bin\Release\net6.0\ConsoleUnitTestsRunner32.exe" 
: #rem %corflags% /32bit+ "%CD%\bin\Release\net6.0\ConsoleUnitTestsRunner32.exe"

if %ERRORLEVEL%==0 goto script_end
echo Build failed!

:script_end