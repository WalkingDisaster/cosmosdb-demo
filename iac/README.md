# Quick Create for Cosmos DB and Azure Functions

## Requirements

[PowerShell 6.x](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-windows?view=powershell-6)

[Azure Powershell](https://docs.microsoft.com/en-us/powershell/azure/overview?view=azps-1.6.0)

[Azure Subscription](https://azure.microsoft.com/en-us/get-started/)

## Running

``` powershell
.\deploy.ps1 `
    -subscriptionId $subscriptionId `
    -resourceGroupName $resourceGroupName `
    -resourceGroupLocation $resourceGroupLocation `
    -deploymentName $deploymentName `
    -parametersFilePath $parametersFilePath `
    -templateFilePath $templateFilePath
```