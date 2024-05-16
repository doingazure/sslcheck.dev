
## Level Set.

This template reflects some opinionated decisions on how to deploy a website. There are many others ways to deploy a website, some or all of which may be better for you. But this is one way. At least some attempt is made to explain some of the reasons behind some of the opinions.

Some of the opinions:

1. Choice: Manage code using GitHub. Reason: Feature-rich with robust integration with Azure.
1. Choice: Deploy to Azure. Reason: DoingAzure.com is an Azure shop.
2. Choice: Deploy to Static Web Sites service (henceforth, "swa" for short). Reason: Inexpensive, while also flexible and easy to work with.
3. Choice: Model the deployment with Infrastructure as Code (IaC). Reason: This is more repeatable and maintainable than doing in the Azure Portal or using only CLI. 
4. Choice: Use Bicep for IaC. Reason: Bicep is very modern and tuned for Azure, even though others like Terraform are cross-platform and more popular.
5. 

## Step 0. Create a repo on GitHub. Include README and LICENSE.

Your README is populated with your project description (if you provided one).

I chose MIT license. 

Clone locally. 

Install needed tools.

## Step 1. Make Deployable

Add enough files to have most trivially deployable Azure Static Web App (henceforth, swa).

Add 

## Step 2. Deploy