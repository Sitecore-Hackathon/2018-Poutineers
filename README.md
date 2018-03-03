![Hackathon Logo](documentation/images/hackathon.png?raw=true "Hackathon Logo")

#Poutineers Project
##Purpose

This project will allow users to visit our site and get a personalized list of articles based on the slack messages they have posted in our slack channel.

##Installation
###Configuration Files

Configuration settings can be found in the app.config for the console application and the Sitecore web application. Check these values and change them for your environment:

-Change the guid to match the one in your xConnect Service app.config.

        <add key="xConnectCertificateConnectionString" value="StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=3DE93B08A1C99FDC874B5A035EF02C66F3CB77E0" />

-Change this root path to match the root of your xConnect Service
        
<add key="xConnectRootUrl" value="https://xp0.xconnect" />

-These two stay the same for the demo however if you want to work with your Slack then you would need to register for api and then put in your oauth and channel name:

        <add key="SlackOAuthToken" value="xoxp-323366903376-323999081732-323928621059-c1a9b65a4714421b2c9655852e26a440" />
        <add key="SlackChannelName" value="C9J451PC5" />

-These need to match the lists in your sitecore instance.  The package would create them but if you manually create them the guids will be different.

        <add key="CommerceContactList" value="38e9c0b0-ed57-49c6-9714-79f1b09159c2" />
        <add key="PowershellContactList" value="d2c719e6-d9ac-42ef-e2b6-263137430fe0"/>

##Installing the Sitecore Package

How it works

1.A console application starts up, connects to xDB via xConnect.
2.The end user writes a message on slack.
3.This console application connects to Slack and looks for new messages in the Random Channel.
4.This console application creates a new contact for the user of each message.
5.This console application adds the contact to the proper lists depending on their slack messages.
6.This is where a "list determination" provider will be loaded to determine the proper list(s))
7.The end user visits our website.
8.The end user types in their slack id and clicks search.
9.The controller loads the proper "article" provider and calls getarticles passing in the slack id.
10.The provider will use XConnect to get the contact and lists from xDB. 
11.The provider will use this information to determine the criteria for articles.
12.The provider will use that to get the list of articles to display.
13.The articles are displayed to the user.
14.As the user clicks on articles, interactions are created regarding the articles. 

More advanced providers can use these interactions to learn which articles are best to show and which ones not.

##Extending with Providers and Reflection

The initial project has 2 spots where providers are loaded via reflection so you can continuously improve the provider in a few ways:

Article Providers determine which articles get returned. The simple article provider looks at the list and uses the list name to search for articles.  However additional providers can be created to watch other interactions and use those to improve accuracy. Additional contact information could be pulled in such as twitter interactions crm data and the more data used the provider could apply it.
The console application uses a simple list determination provider. This on looks for specific keywords and uses their existence to determine the list.  Additional providers could use natural language processing to determine relevance.

##Executables

PoutineersWebsite: This is the Sitecore 9 website that will consist of 2 views. The first is a form where you enter your slack id. The second is a page with a list of articles based on your slack id.  There will be a refresh button so you can get an update on your article list.

PoutineersSlackToXConnectConsole: This console application will connect to the poutineers slack and get any messages from the Random channel.
It will add any slack users as contacts and add the contacts to the proper lists based on the list determination provider.


