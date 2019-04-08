param(
    [Parameter(Mandatory = $True)]
    [string]
    $subscriptionId,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupName,

    [string]
    $resourceGroupLocation,

    [Parameter(Mandatory = $True)]
    [string]
    $deploymentName,

    [string]
    $nameBase,

    [string]
    $templateFilePath = "template.json",

    [string]
    $parametersFilePath = "parameters.json"
)

Function RegisterRP {
    Param(
        [string]$ResourceProviderNamespace
    )

    Write-Host "Registering resource provider '$ResourceProviderNamespace'"
    Register-AzResourceProvider -ProviderNamespace $ResourceProviderNamespace
}

#******************************************************************************
# Script body
# Execution begins here
#******************************************************************************
$ErrorActionPreference = "Stop"

# sign in
$ctx = Get-AzContext -ErrorAction SilentlyContinue
$promptForLogin = $true

if ($ctx) {
    Write-Host "You are currently logged in"

    foreach ($sub in Get-AzSubscription) {
        if ($sub.Id = $subscriptionId) {
            $promptForLogin = $false
        }
    }

    if ($promptForLogin) {
        Write-Host "Your current credentials do not give you access to subscription $subscriptionId. You must log in to the correct account."
    }
}

if ($promptForLogin) {
    Write-Host "Logging in..."
    $result = Login-AzAccount
    Write-Verbose $result
    $ctx = Get-AzContext
}

# select subscription
Write-Host "Selecting subscription '$subscriptionId'"
$result = Set-AzContext -Subscription $subscriptionId
Write-Verbose $result

# Register RPs
$resourceProviders = (Get-Content .\providerNamespaces.json | ConvertFrom-Json)
if ($resourceProviders.length) {
    Write-Host "Registering resource providers"
    foreach ($resourceProvider in $resourceProviders) {
        $result = RegisterRP($resourceProvider)
        Write-Verbose $result
    }
}

#Create or check for existing resource group
$resourceGroup = Get-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if (!$resourceGroup) {
    Write-Host "Resource group '$resourceGroupName' does not exist. To create a new resource group, please enter a location."
    if (!$resourceGroupLocation) {
        $resourceGroupLocation = Read-Host "resourceGroupLocation"
    }
    Write-Host "Creating resource group '$resourceGroupName' in location '$resourceGroupLocation'"
    $result = New-AzResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation
    Write-Verbose $result
}
else {
    Write-Host "Using existing resource group '$resourceGroupName'"
}

# Start the deployment
Write-Host "Starting deployment..."
if (Test-Path $parametersFilePath) {
    $result = New-AzResourceGroupDeployment `
        -ResourceGroupName $resourceGroupName `
        -Name $deploymentName `
        -TemplateFile $templateFilePath `
        -TemplateParameterFile $parametersFilePath `
        -nameBase $nameBase
}
else {
    $result = New-AzResourceGroupDeployment `
        -ResourceGroupName $resourceGroupName `
        -Name $deploymentName `
        -TemplateFile $templateFilePath `
        -nameBase $nameBase
}

Write-Verbose $result -Verbose

Write-Output $result.Outputs['comsosDbResourceId'].Value