using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace AzureVMSet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var servicePrincipal = new ServicePrincipalLoginInformation { ClientId = "a72b5fad-ea6d-4dc6-ad29-a61ec5764996", ClientSecret = "r=gVw3Z7E=AvZxegH_-paS2B8HOJNw-4" };
            var creds = new AzureCredentials(servicePrincipal, tenantId: "865a8d92-047d-4700-bdd5-1642317b02fb", AzureEnvironment.AzureGlobalCloud);
            var restClient = Microsoft.Azure.Management.ResourceManager.Fluent.Core.RestClient
                .Configure()
                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                .WithCredentials(creds)
                .Build();
            var listResources = new ResourceManagementClient(restClient);
            listResources.SubscriptionId = "6fca20d4-4d6e-48ff-94ef-53850004a3dd";
            // var rgs = await resourceManagementClient.ResourceGroups.ListAsync();
            // var test =  new ResourceManagementClient(restClient).Resources.ListAsync()
            var resourcegrouplist = "ARCONPAMAZURE";
            // var rgs =  azure.ResourceGroups.GetByName(resourcegrouplist);
            var id = new String[2];
            var i = 0;
            //  var resourceList = listResources.Resources.ListByResourceGroup(resourcegrouplist[i]);
            var resourceList = await listResources.Resources.ListByResourceGroupAsync(resourcegrouplist);
            foreach (var resource in resourceList)
            {
                if (resource.Type.Equals("Microsoft.Compute/virtualMachineScaleSets"))
                {
                    id[i] = resource.Id;
                    i++;
                    Console.WriteLine(resource.Name.ToString());
                }
            }
            var listvms1 = await listResources.Resources.GetByIdAsync(id[0], "2020-06-01");
            var listvms2 = await listResources.Resources.GetByIdAsync(id[1], "2020-06-01");

            var tokenclient = new RestSharp.RestClient("https://login.microsoftonline.com/865a8d92-047d-4700-bdd5-1642317b02fb/oauth2/v2.0/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("scope", "https://management.azure.com/.default");
            request.AddParameter("client_secret", "r=gVw3Z7E=AvZxegH_-paS2B8HOJNw-4");
            request.AddParameter("client_id", "a72b5fad-ea6d-4dc6-ad29-a61ec5764996");
            request.AddParameter("grant_type", "client_credentials");
            IRestResponse response = tokenclient.Execute(request);
            Console.WriteLine(response.Content);
            JObject tokenjson = JObject.Parse(response.Content);
            var token = tokenjson.GetValue("access_token").ToString();
            var VMclient = new RestSharp.RestClient("https://management.azure.com/subscriptions/6fca20d4-4d6e-48ff-94ef-53850004a3dd/resourceGroups/"+ resourcegrouplist + "/providers/Microsoft.Compute/virtualMachineScaleSets/"+ listvms1.Name + "/virtualmachines/0?api-version=2020-06-01");
            var VMrequest = new RestRequest(Method.GET);
            VMrequest.AddHeader("Authorization", "Bearer " + token);
            IRestResponse passchangeclientresponse = VMclient.Execute(VMrequest);
            Console.WriteLine(response.Content);
        }
    }
}
