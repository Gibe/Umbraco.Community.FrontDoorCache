# Setting up Microsoft Entra ID to work with the module

## Creating an App Registration

In the Azure portal under your Azure Active Directory tenant, select App Registrations

![App Registrations Portal View](https://user-images.githubusercontent.com/113788/228666546-633e434a-4466-4f7c-9a6b-666751aae7bc.png)

Click "New registration"
Give that application a meaningful name
Press "Register"

In your new application select "Certificates & secrets" and then "New client secret"

Give the secret a meaningful description and set the expiry date 

Copy that secret and use that for the ClientSecret item in the configuration, in the overview the Application (client) ID is the ClientId and the Directory (tenant) ID is the TenantId

## Giving your new App registration access to Front Door

Go to your Front Door resource in the Azure Portal

Select "Access Control (IAM)", then "Add a role assignment"

Select the role "CDN Profile Contributor" then "Next"

Click "+ Select members", search for your new App Registration and select it

Click "Review + assign"

