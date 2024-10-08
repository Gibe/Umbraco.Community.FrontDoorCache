# Umbraco.Community.FrontDoorCache

A package for sites which are sitting behind Azure Front Door using it as a CDN and a cache for both media and content. 
Typically they'll be a delay between publishing content and the cache updating at Front Door, this will mean your sites will be serving older versions of the content
for a time following an update. This package will trigger a purge of the modified content forcing Front Door to update its cache and serve the latest version of the content.
This will allow you to specify longer cache times in Front Door and reduce the load on your site.

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
| ----------------- | --------------------------------------------------------------------- |
| Enabled           | Set to true to enable the module, or false to disable                 |
| Mode              | Can be one of: All, Self or SelfAndAncestors (See below)              |
| SubscriptionId    | The ID of the Azure subscription that the Front Door belongs to       |
| ResourceGroupName | The name for the Azure resource group that the Front Door belongs to  |
| FrontDoorName     | The name of the Front Door to purge                                   |
| EndpointName      | The name of the endpoint in Front Door to purge                       |
| TenantId          | The value in Directory (tenant) ID on the app registration Overview   |
| ClientId          | The value in Application (Client) ID on the app registration Overview |
| ClientSecret      | The client secret created for the app registration                    |
| Domains           | The domains configured in Front Door that you want to purge           |

# Modes

| Name             | Description                                                                                                                         |
| ---------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| All              | Will completely purge the cache when either media or content is published                                                           |
| Self             | Purges just the media or content that is published                                                                                  |
| SelfAndAncestors | Purges the published media and content. Additionally, for content only, will purge all of its ancestors working up the content tree | 

# License

Copyright © 2024 Gibe Digital Ltd.

All source code is licensed under the MIT License

# Acknowledgements

This package takes heavy inspiration from [CloudflareMediaCache](https://github.com/jcdcdev/jcdcdev.Umbraco.CloudflareMediaCache) thanks @jcdcdev

