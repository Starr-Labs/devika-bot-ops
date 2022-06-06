# Devika Azure DevOps Bot
## Inspiration
We see the Responsible AI movement as a call for us to be deliberate about the use of AI to create bridges, points of access and empowerment for the whole world. When we started to think about how this thinking could be applied to Azure DevOps, the idea of Devika, the DevOps bot popped out. 
## What it does
Devika let's you use Teams or Alexa to manage workitems in Azure DevOps. It is built on top of Azure Bot Framework Composer and uses a gaggle of Azure resources to achieve its goals. One of those goals is to allow persons with visual impairments to manage workitems in Azure using voice. We're well on our way to achieving this.
## How we built it
The bot was built in Bot Framework Composer. It uses cards, responses and variety of input approaches to receive input. It leverages the Language Understanding Intelligence Service, LUIS to enable a variety of inputs to resolve to clear intents. 
Azure Functions act like the connective tissue between user intents and Azure DevOps or an Azure SQL  instance.
Finally, Alexa was added as a channel via the Azure Bot service so user can use either Microsoft Teams or Amazon Alexa to interact with Azure DevOps.
## Challenges we ran into
Working with LUIS was both exciting but challenging. Finding the right way to get conversational flow took quite a bit of trial and error.
## Accomplishments that we're proud of
Fairness - This solution attempts to use AI to make it easier for both sighted and visually-impaired persons to use Azure DevOps
Reliability & Safety – By using LUIS, training for reliable conversational engagement allows for accurate determination of intents. By limiting what intents the app supports, users should come away with a high sense of confidence that the app does what it says it will do. 
Privacy & Security – No private data about our users are collected while engaging with AI for this solution. 
Inclusiveness – Devika, the Azure DevOps bot has a core goal to empower visually-impaired developers to do more with Azure DevOps
Transparency – By using LUIS, the developers have been clear in terms of what inputs produce what outputs. 
Accountability - Activity on the bot is tracked via Azure Application Insights, as well as LUIS to ensure that we are able to monitor that the bot is accomplishing the stated goals.
## What we learned
Responsible AI principles are great points of guidance. We're happy to explore them as we build out Devika.
## What's next for Devika - The Azure DevOps virtual assistant
Devika opened our eyes to the need for approaches like this to empower other developers. We'd like to build this out some more.
