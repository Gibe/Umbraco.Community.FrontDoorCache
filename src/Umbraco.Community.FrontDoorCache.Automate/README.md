# Umbraco.Community.FrontDoorCache.Automate

Azure Front Door cache purging for [Umbraco Automate](https://umbraco.com/products/add-ons/automate/) (Umbraco 17.4+, per the `Umbraco.Automate.Core` package's own minimum CMS version).

Exposes an "Azure Front Door" Connection and three Actions you can use in any Automate automation:

- **Purge Front Door Path(s)** - purges one or more specific paths. The Paths field supports `${...}` runtime bindings, so it can be wired to a trigger's output (e.g. bind to a Content Published trigger's URL, or a webhook payload field).
- **Purge Front Door (All)** - wildcard-purges the entire endpoint (`/*`).
- **Get Content or Media URL** - resolves the absolute URL (and just the path) for a content or media item by key. Doesn't call Front Door and needs no connection - use it to turn a trigger's `content.key`/`media.key` output into a path you can feed straight into **Purge Front Door Path(s)**.

## Install

`dotnet add package Umbraco.Community.FrontDoorCache.Automate`

## Setup

First you, or a Microsoft Entra ID administrator, will need to create an App Registration in the Azure Portal which will be used to give the site permissions to the Front Door API. Follow [these instructions to setup the new App Registration](https://github.com/Gibe/Umbraco.Community.FrontDoorCache/blob/main/EntraIdSetup.md) (same steps as the classic package).

The Entra ID credentials are **not** entered in the backoffice - they're read from configuration, so secrets never live in the Automate database. Add the following to the root of your `appsettings.json`:

```json
"FrontDoor": {
	"Cache": {
		"Credentials": {
			"TenantId": "",
			"ClientId": "",
			"ClientSecret": ""
		}
	}
}
```

| Setting      | Description                                                            |
| ------------ | ------------------------------------------------------------------------ |
| TenantId     | The value in Directory (tenant) ID on the app registration Overview      |
| ClientId     | The value in Application (Client) ID on the app registration Overview    |
| ClientSecret | The client secret created for the app registration                       |

As with any secret, prefer environment variables, User Secrets, or Azure Key Vault over committing `ClientSecret` to `appsettings.json` directly.

1. In the Umbraco backoffice, go to **Automate > Connections > Create**, choose **Azure Front Door**, and fill in Subscription ID, Resource Group Name, Front Door Profile Name and Endpoint Name. Use **Test Connection** to confirm - this also verifies the credentials configured above.
2. Create or edit an automation, add a **Purge Front Door Path(s)** and/or **Purge Front Door (All)** action step, select the connection created above, and (for Paths) either enter static paths or bind the field to a trigger output.

## Relationship to Umbraco.Community.FrontDoorCache

This package replaces the automatic content/media-publish-triggered purging of the classic `Umbraco.Community.FrontDoorCache` package with an explicit, Automate-driven model: you decide which triggers should purge which paths, rather than every publish purging automatically. If you are not running Automate, keep using the classic package instead.

# License

Copyright © 2026 Gibe Digital Ltd.

All source code is licensed under the MIT License
