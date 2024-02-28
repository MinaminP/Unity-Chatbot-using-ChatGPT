# Azure (Bing Speech)
https://docs.microsoft.com/en-us/azure/cognitive-services/Speech/Home


For more information on authentication and how to get your api key, please visit:
https://docs.microsoft.com/en-us/azure/cognitive-services/speech/how-to/how-to-authentication

Based on https://github.com/Microsoft/mixedreality-azure-samples


## NOTES: 
* Azure needs an API-level of .NET 4.x or .NET Standard 2.0
* Unity 2017.x doesn't recognize the namespace "System.Net.Http" by default. You have to create a file named "mcs.rsp" and "csc.rsp" in the "Assets"-folder and add the line "-r:System.Net.Http.dll".