# Umbraco.Community.FrontDoorCache

A package for sites which are sitting behind Azure Front Door as a CDN

First you, or an Microsoft Entra ID administration will need to create an App Registration in the Azure Portal which will be used to give the site permissions to the Front Door API. Follow [these instructions to setup the new App Registration](EntraIdSetup.md)

To install:

`dotnet add package Umbraco.Community.FrontDoorCache`

To configure add the following section to the root of your appsettings.json file and customise as appropriate
```
"FrontDoor": {
	"Cache": {
		"Enabled": true,
		"Mode": "SelfAndAncestors",
		"SubscriptionId": "",
		"ResourceGroupName": "",
		"FrontDoorName": "",
		"EndpointName": "",
		"TenantId": "",
		"ClientId": "",
		"ClientSecret": "",
		"Domains": [
			"www.sitedomain.com"
		] 
	} 
}, 
```

You'll need to configure these settings based on the values in Azure:

| Setting           | Description                                                           |
| ----------------- | -----------------------------------------------------                 |
| SubscriptionId    | The ID of the Azure subscription that the Front Door belongs to       |
| ResourceGroupName | The name for the Azure resource group that the Front Door belongs to  |
| FrontDoorName     | The name of the Front Door to purge                                   |
| EndpointName      | The name of the endpoint in Front Door to purge                       |
| TenantId          | The value in Directory (tenant) ID on the app registration Overview   |
| ClientId          | The value in Application (Client) ID on the app registration Overview |
| ClientSecret      | The client secret created for the app registration                    |
| Domains           | The client secret created for the app registration                    |

You can also customise the configuration by setting these settings:

| Setting                               | Description                                                                                  |
| ------------------------------------- | -------------------------------------------------------------------------------------------- |
| Enabled                               |  |
| Mode                                  |  |






