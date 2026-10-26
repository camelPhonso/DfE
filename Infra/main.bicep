// ******************************************************
// Parameters
// ******************************************************

param location string = resourceGroup().location

@description('Unique token, used to define resource names.')
param resourceToken string = toLower(uniqueString(subscription().id, location))

param appName string = 'func-${resourceToken}'

// ******************************************************
// Variables
// ******************************************************

var deploymentContainerName = 'app-storage-${take(appName, 32)}-${take(resourceToken, 7)}'

// ******************************************************
// Analytics
// ******************************************************

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2025-07-01' = {
  name: 'log-${resourceToken}'
  location: location
  properties: any({
    retentionInDays: 30
    features: {
      searchVersion: 1
    }
    sku: {
      name: 'PerGB2018'
    }
  })
}

// ******************************************************
//  Application Insights
// ******************************************************

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'insight-${resourceToken}'
  location: location
  kind: 'web'
  properties: {
    Application_Type:'web'
    WorkspaceResourceId: logAnalytics.id
    DisableLocalAuth: true
  }
}

// ******************************************************
// Storage
// ******************************************************

resource storage 'Microsoft.Storage/storageAccounts@2026-04-01' = {
  name: 'storage-${resourceToken}'
  location: location
  kind: 'BlobStorage'
  sku: {
    name: 'Standard_GRS'
  }
  properties: {
    accessTier: 'Cool'
    allowBlobPublicAccess: true
    allowSharedKeyAccess: false
    dnsEndpointType: 'Standard'
    minimumTlsVersion: 'TLS1_3'
    publicNetworkAccess: 'Enabled'
  }
  resource blobServices 'blobServices' = {
    name: 'default'
    properties: {
      deleteRetentionPolicy: {
        allowPermanentDelete: true
      }
    }
    resource deploymentContainer 'containers' = {
      name: deploymentContainerName
      properties: {
        publicAccess: 'None'
      }
    }
  }
}

resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2025-05-31-preview' = {
  name: 'data-owner-${resourceToken}'
  location: location
}

// ******************************************************
// Function App
// ******************************************************
resource functionApp 'Microsoft.Web/sites@2025-03-01' = {
  name: 'DfE_time_trigger'
  location: 'uk south'
  kind: 'functionapp,linux'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentity.id}': {}
    }
  }
  properties: {
    functionAppConfig: {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${storage.properties.primaryEndpoints.blob}${deploymentContainerName}'
          authentication: {
            type: 'UserAssignedIdentity'
            userAssignedIdentityResourceId: userAssignedIdentity.id
          }
        }
      }
    }
  }
}
