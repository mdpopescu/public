#Requires -Version 7
<#
.SYNOPSIS
    Publishes YouNews.Web and installs it as the automatic "YouNews" Windows service under LocalSystem.

.DESCRIPTION
    Run it from a normal PowerShell window: it asks for elevation (UAC), but for no password.
    LocalSystem has its own profile, so the service is pointed at the current user's AppData folders,
    where yt-dlp and Camoufox are installed.
    Run it again to deploy a new build; it replaces the existing service.
    appsettings.Local.json is copied to the install folder when publishing, so re-run it after editing that file.

.PARAMETER InstallDirectory
    Where the published application goes. Must be on a local disk: a service does not see mapped drives.

.PARAMETER SkipPublish
    Used by the script itself for its elevated half, which installs what the normal half published.
#>
param(
    [string]$InstallDirectory = (Join-Path (Split-Path $PSScriptRoot) 'publish'),
    [switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'

$SERVICE_NAME = 'YouNews'
$SERVICE_URL = 'http://localhost:5080'
$EXECUTABLE_NAME = 'YouNews.Web.exe'
$SERVING_TIMEOUT_SECONDS = 60

$repository = Split-Path $PSScriptRoot
$scratchDirectory = Join-Path $repository '.scratch'
$stagingDirectory = Join-Path $scratchDirectory 'service-publish'
$logPath = Join-Path $scratchDirectory 'install-service.log'

function Test-Administrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    return ([Security.Principal.WindowsPrincipal]$identity).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Publish-Application {
    if (Test-Path $stagingDirectory) { Remove-Item $stagingDirectory -Recurse -Force }
    dotnet publish (Join-Path $repository 'src\YouNews.Web') -c Release -o $stagingDirectory --nologo -v quiet
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed with exit code $LASTEXITCODE." }
}

function Invoke-ElevatedInstall {
    $arguments = @(
        '-NoProfile', '-File', "`"$PSCommandPath`"",
        '-SkipPublish', '-InstallDirectory', "`"$InstallDirectory`""
    )
    $process = Start-Process pwsh -Verb RunAs -Wait -PassThru -ArgumentList $arguments
    if ($process.ExitCode -ne 0) { throw "The elevated install failed. See $logPath." }
}

function Remove-ExistingService {
    if (-not (Get-Service $SERVICE_NAME -ErrorAction SilentlyContinue)) { return }
    Stop-Service $SERVICE_NAME -Force
    Remove-Service $SERVICE_NAME
}

function Copy-Application {
    New-Item -ItemType Directory -Force $InstallDirectory | Out-Null
    Copy-Item (Join-Path $stagingDirectory '*') $InstallDirectory -Recurse -Force
    Remove-Item $stagingDirectory -Recurse -Force
}

function New-ApplicationService {
    New-Service -Name $SERVICE_NAME -DisplayName $SERVICE_NAME -StartupType Automatic `
        -BinaryPathName "`"$(Join-Path $InstallDirectory $EXECUTABLE_NAME)`"" `
        -Description 'Daily AI news briefing made from YouTube channels.' | Out-Null
}

# Python finds its user packages through APPDATA, and Camoufox finds its browser through LOCALAPPDATA.
function Set-ServiceEnvironment {
    New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Services\$SERVICE_NAME" -Name Environment -PropertyType MultiString -Force `
        -Value @("APPDATA=$env:APPDATA", "LOCALAPPDATA=$env:LOCALAPPDATA") | Out-Null
}

function Install-ApplicationService {
    Remove-ExistingService
    Copy-Application
    New-ApplicationService
    Set-ServiceEnvironment
    Start-Service $SERVICE_NAME
    Write-Host "Service '$SERVICE_NAME' is $((Get-Service $SERVICE_NAME).Status), running as LocalSystem."
}

function Invoke-ElevatedHalf {
    Start-Transcript -Path $logPath -Force | Out-Null
    try {
        Install-ApplicationService
    }
    catch {
        Write-Host "FAILED: $($_.Exception.GetBaseException().Message)" -ForegroundColor Red
        Read-Host 'Press Enter to close this window'
        exit 1
    }
    finally {
        Stop-Transcript | Out-Null
    }
}

function Wait-UntilServing {
    $deadline = (Get-Date).AddSeconds($SERVING_TIMEOUT_SECONDS)
    while ((Get-Date) -lt $deadline) {
        try {
            $status = (Invoke-WebRequest $SERVICE_URL -TimeoutSec 5 -SkipHttpErrorCheck).StatusCode
            Write-Host "$SERVICE_URL answered with status $status."
            return
        }
        catch {
            Start-Sleep -Seconds 2
        }
    }
    throw "$SERVICE_URL did not answer within $SERVING_TIMEOUT_SECONDS seconds. Check the Application event log, source '$SERVICE_NAME'."
}

$InstallDirectory = [IO.Path]::GetFullPath($InstallDirectory)

if ($SkipPublish) {
    Invoke-ElevatedHalf
    exit 0
}

Publish-Application
if (Test-Administrator) { Install-ApplicationService } else { Invoke-ElevatedInstall }
Wait-UntilServing