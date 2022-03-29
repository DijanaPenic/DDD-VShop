#!/usr/bin/env pwsh

# Set parameters
param ([string]$DockerFolderPath=".")

# Set variables
Set-Variable -Name "DockerComposeUpFilePath" -Value "docker-compose-up.txt"
Set-Variable -Name "DockerComposeBuildFilePath" -Value "docker-compose-build.txt"

# Check id docker-compose output contains errors

Write-Output "Checking log output..."

$composeUpSel = Select-String -Path $DockerComposeUpFilePath -Pattern "Error", "error"
$composeBuildSel = Select-String -Path $DockerComposeBuildFilePath -Pattern "Error", "error"

if ($composeUpSel -ne $null -or $composeBuildSel -ne $null)
{
    Write-Error "Error: detected docker-compose errors! Check output logs for more information (Store > Docker)."
}