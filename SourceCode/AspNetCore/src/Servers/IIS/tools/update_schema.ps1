<#
.DESCRIPTION
Updates aspnetcore_schema.xml to the latest version.
Updates aspnetcore_schema.xml to the latest version.
Requires admin privileges.
#>
[cmdletbinding(SupportsShouldProcess = $true)]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 1

$ancmSchemaFileLocation = Join-Path $PSScriptRoot "aspnetcore_schema_v2.xml";
if (!(Test-Path $ancmSchemaFileLocation))
{
    $ancmSchemaFileLocation = Resolve-Path "$PSScriptRoot\..\AspNetCoreModuleV2\AspNetCore\aspnetcore_schema_v2.xml";
}

[bool]$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")

if (-not $isAdmin -and -not $WhatIfPreference) {
    if ($PSCmdlet.ShouldContinue("Continue as an admin?", "This script needs admin privileges to update IIS Express and IIS.")) {
        $thisFile = Join-Path $PSScriptRoot $MyInvocation.MyCommand.Name

        Start-Process `
            -Verb runas `
            -FilePath "powershell.exe" `
            -ArgumentList $thisFile `
            -Wait `
            | Out-Null

        if (-not $?) {
            throw 'Update failed'
        }
        exit
    }
    else {
        throw 'Requires admin privileges'
    }
}

$schemaFile = "aspnetcore_schema.xml"
$schemaSource = $ancmSchemaFileLocation

$destinations = @(
    "${env:ProgramFiles(x86)}\IIS Express\config\schema\",
    "${env:ProgramFiles}\IIS Express\config\schema\",
    "${env:ProgramW6432}\IIS Express\config\schema\",
    "${env:windir}\system32\inetsrv\config\schema\"
)

foreach ($destPath in $destinations) {
    $dest = "$destPath\${schemaFile}";

    if (!(Test-Path $destPath))
    {
        Write-Host "$destPath doesn't exist"
        continue;
    }

    if ($PSCmdlet.ShouldProcess($dest, "Replace file")) {
        Write-Host "Updated $dest"
        Move-Item $dest "${dest}.bak" -ErrorAction Ignore
        Copy-Item $schemaSource $dest
    }
}
