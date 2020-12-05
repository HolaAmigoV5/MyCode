REM Disable "!Foo!" expansions because they break the filter syntax
setlocal disableextensions

set target=%1
set sdkVersion=%2
set runtimeVersion=%3
set helixQueue=%4

set DOTNET_HOME=%HELIX_CORRELATION_PAYLOAD%\sdk
set DOTNET_ROOT=%DOTNET_HOME%\x64
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
set DOTNET_MULTILEVEL_LOOKUP=0
set DOTNET_CLI_HOME=%HELIX_CORRELATION_PAYLOAD%\home

set PATH=%DOTNET_ROOT%;%PATH%

powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; &([scriptblock]::Create((Invoke-WebRequest -useb 'https://dot.net/v1/dotnet-install.ps1'))) -Architecture x64 -Version %sdkVersion% -InstallDir %DOTNET_ROOT%"
powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; &([scriptblock]::Create((Invoke-WebRequest -useb 'https://dot.net/v1/dotnet-install.ps1'))) -Architecture x64 -Runtime dotnet -Version %runtimeVersion% -InstallDir %DOTNET_ROOT%"

set HELIX=%helixQueue%

%DOTNET_ROOT%\dotnet vstest %target% -lt >discovered.txt
find /c "Exception thrown" discovered.txt
REM "ERRORLEVEL is not %ERRORLEVEL%" https://blogs.msdn.microsoft.com/oldnewthing/20080926-00/?p=20743/
if not errorlevel 1 (
    echo Exception thrown during test discovery. 1>&2
    type discovered.txt 1>&2
    exit 1
)

set exit_code=0

REM Run non-flaky tests first
REM We need to specify all possible Flaky filters that apply to this environment, because the flaky attribute
REM only puts the explicit filter traits the user provided in
REM Filter syntax: https://github.com/Microsoft/vstest-docs/blob/master/docs/filter.md
set NONFLAKY_FILTER="Flaky:All!=true&Flaky:Helix:All!=true&Flaky:Helix:Queue:All!=true&Flaky:Helix:Queue:%HELIX%!=true"
echo Running non-flaky tests.
%DOTNET_ROOT%\dotnet vstest %target% --logger:trx --logger:console;verbosity=normal --TestCaseFilter:%NONFLAKY_FILTER%
if errorlevel 1 (
    echo Failure in non-flaky test 1>&2
    set exit_code=1
    REM DO NOT EXIT
)

set FLAKY_FILTER="Flaky:All=true|Flaky:Helix:All=true|Flaky:Helix:Queue:All=true|Flaky:Helix:Queue:%HELIX%=true"
echo Running known-flaky tests.
%DOTNET_ROOT%\dotnet vstest %target% --logger:trx --logger:console;verbosity=normal --TestCaseFilter:%FLAKY_FILTER%
if errorlevel 1 (
    echo Failure in flaky test 1>&2
    REM DO NOT EXIT and DO NOT SET EXIT_CODE to 1
)

exit %exit_code%

