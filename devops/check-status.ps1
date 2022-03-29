#!/usr/bin/env pwsh

# Set parameters
param ($DockerFolderPath = ".", $LogsPath = ".")

# Functions
function Check-DockerContainer {
    param ([string]$ContainerName, [string]$ExpectedStatus, [int]$ExpectedExitCode = 0)

    $ContainerName = "vshop_${ContainerName}_1"
    $ContainerState = $(docker container inspect -f '{{json .State}}' $ContainerName) | ConvertFrom-Json

    if ($ContainerState.Status -ne $ExpectedStatus -or $ContainerState.ExitCode -ne $ExpectedExitCode)
    {
        Write-Error "Error: $ContainerName is not assigned the expected status or exit code."
        Write-Output "Expected status: $ExpectedStatus"
        Write-Output "Expected exit code: $ExpectedExitCode"
    }
}

# Set variables
Set-Variable -Name "DockerComposeUpFilePath" -Value "$LogsPath/docker-compose-up.txt"
Set-Variable -Name "DockerComposeBuildFilePath" -Value "$LogsPath/docker-compose-build.txt"

# Check id docker-compose output contains errors

Write-Output "Checking log output..."

$composeUpSel = Select-String -Path $DockerComposeUpFilePath -Pattern "Error", "error"
$composeBuildSel = Select-String -Path $DockerComposeBuildFilePath -Pattern "Error", "error"

if ($composeUpSel -ne $null -or $composeBuildSel -ne $null)
{
    Write-Error "Error: detected docker-compose errors! Check output logs for more information (Store > Docker)."
}

# Check containers status

Write-Output "Checking containers status..."

$RunningContainers = 
@(
    'eventstore.db',
    'eventstore.db.tests',
    'postgres.db.billing',
    'postgres.db.billing.tests', 
    'postgres.db.process_manager',
    'postgres.db.process_manager.tests',
    'postgres.db.identity',
    'postgres.db.identity.tests',
    'postgres.db.sales', 
    'postgres.db.sales.tests',
    'postgres.db.catalog', 
    'postgres.db.catalog.tests'
)

cd $DockerFolderPath
foreach ($Container in $RunningContainers)
{
    Check-DockerContainer -ContainerName $Container -ExpectedStatus "running"
}