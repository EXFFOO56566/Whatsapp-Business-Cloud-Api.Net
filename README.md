# Whatsapp-Business-Cloud-Api.Net

1. Overview
The Cloud API allows you to implement WhatsApp Business APIs without the 
cost of hosting of your own servers and also allows you to more easily scale your 
business messaging. The Cloud API supports up to 80 messages per second of 
combined sending and receiving
2. Requirement
- Visual Studio 2019 version 16.3 or higher.
- Obtain access token for meta developers’ portal: Meta for Developers
3. How to import the project?
- Open visual studio, click “File” → Open → Project/Solution
- The solution contains 2 projects : 
• WhatsappBusiness.CloudApi : the Api integration.
• WhatsappBusinessCloudApi.Web : a sample web application for testing

4. Configuration:
To test the api calls:
- set the project " WhatsappBusinessCloudApi.Web " as startup:right click → Set 
the project as startup
- Open : appsettings.json
- Change json values by your’s.(obtained from your Meta developer account):
- Run the solution.
