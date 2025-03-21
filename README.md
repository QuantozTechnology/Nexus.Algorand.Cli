# Algorand Account and Asset, Creation and Management CLI
The Nexus platform is provided as a SaaS solution by the company Quantoz Blockchain Services to partners that issue and manage tokens on the Algorand blockchain. New standard asset tokens (ASA’s) on the Algorand blockchain can be defined and created by a token issuer using the Nexus platform. For more information on ASA's see [here](https://developer.algorand.org/docs/get-details/asa/).

Quantoz Blockchain Services provides a separate command-line tool to assist with token creation independent of the Nexus platform. The intent of this tool is to support the separation of token ownership (to sit with our partners) and the operational token activities (to sit within Nexus), by creating the token outside of Nexus and then importing it into Nexus. Alternatively to this tool, ASA’s on the Algorand blockchain can be defined and created using third party Algorand tools, or even direct API calls on the blockchain nodes.  

This tool can be used by our partners (in consultation with us if needed) to create new accounts and take ownership of the private key of the Creator account. The private key of this Creator account is only shared once with the operator of the tool, which can be run offline (on a computer without internet connection) to generate and save the private key.

For more info on secure management of external created Algorand tokens in Nexus reach out to us [here](https://quantoz.com/contact/).

## Requirements
The Algorand Account and Asset, Creation and Management CLI tool is a C# written tool and requires [.NET SDK 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installed.

## Algorand Test and Main Net
Algorand has three public networks: MainNet, TestNet, and BetaNet. The Algorand Account and Asset, Creation and Management CLI tool is hardcoded to use the MainNet, but could easily be updated to use TestNet to play around with.

## Configuration
The Algorand Account and Asset, Creation and Management CLI tool is already configured to immediately be ready to be used and connect to MainNet. 

It is hardcoded to use [Nodely](https://nodely.io/docs/free/start)'s free credentials as a node provider, but can be updated in the appsettings to use another provider.

## Algorand Fees
Submitting signed transactions through this tool requires the tool to be online (with internet connection) and the payment of a fee on the Algorand blockchain to confirm the action. For this reason, you will have to fund your newly created account a small amount of ALGO.

## Management of private keys
After creating your account and token, your creator/manager key needs to be stored by the token issuer in a secure manner. When this key would get lost, it becomes impossible to take control over the token if needed.  

A storing mechanism that is advised is a 2 out of 3, or a 3 out of 5 method where the private key is split in three or five partial keys. The partial keys are physically separate stored in different locations. At least 2 or 3 of the parts are needed to restore a full private key, preventing anybody with access to only one part to take control. This also allows for 1 or 2 keys to get lost and still be able to restore the full key.  

The physical backup of the three or five partial keys can be by writing down the codes, or by using a set of 12 or 24 key words. To make it fully safe (and for the paranoid), these words could be stored on special metal plates that can even survive fire.

## Examples
Below are a few examples for what this tool could be used for in combination with Nexus. 
Examples may contain dummy values for display purposes.
Note for the submit action, you are required to be online.

### 1. Create and import an EMT into Nexus
It is especially relevant in regulated environments where ownership should be connected to the license holder, for example electronic money tokens (EMTs) under MiCAR regulations, to support the separation of token ownership and the operational token activities. 

By importing a token, ownership still sits with the client due to the creator address' private key not being stored in Nexus. The creator's address can at any stage be used to change the token's reserve account, to remove any access which Nexus had to the token.

Follow the below steps to import an EMT created independent of Nexus, into Nexus:
- Create an account by using the tool:  
  ```bash
  dotnet run generateaccount
  ```
- Fund account with algo
- Create token using your new account and by using the tool:
  ```bash
  dotnet run createasset --defaultfrozen true --creator U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --manager U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --reserve U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --round 49800622 --decimals 7 --overalllimit 10000 --unit TEMT001 --name TestEMTCreation1
  ```
  ```bash
  dotnet run sign --privatekey XmtrNdFBhIK2VQxNFzsgAC6Q/5qgMdGFgDfqNXYa34U= --unsignedtx iKRhcGFyiaJhbrBUZXN0RU1UQ3JlYXRpb24xoWPEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGomRjB6JkZsOhZsQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahbcQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahcsQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahdM8AAAAXSHboAKJ1bqdURU1UMDAxo2ZlZc0D6KJmds4C9+Wuo2dlbqx0ZXN0bmV0LXYxLjCiZ2jEIEhjtRiks8hOyBDyLU8QgcsPcfBZp6wg3sYvf3DlCToiomx2zgL36Zajc25kxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqR0eXBlpGFjZmc=
  ```
  ```bash
  dotnet run submit --signedtx gqNzaWfEQMTnlS1BddkBc2GFizI1R8uDO+Gvrkn0s6+89bvakfdhoI1p0Hk+RQg+7khHnVxTMCxuvN8ndR3X9rwlydAOZQ6jdHhuiKRhcGFyiaJhbrBUZXN0RU1UQ3JlYXRpb24xoWPEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGomRjB6JkZsOhZsQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahbcQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahcsQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8ahdM8AAAAXSHboAKJ1bqdURU1UMDAxo2ZlZc0D6KJmds4C9+Wuo2dlbqx0ZXN0bmV0LXYxLjCiZ2jEIEhjtRiks8hOyBDyLU8QgcsPcfBZp6wg3sYvf3DlCToiomx2zgL36Zajc25kxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqR0eXBlpGFjZmc=
  ```
- Start the import process in Nexus by starting with step 1 and providing your new token's asset Id in Nexus to validate and select _Generate Reserve Address_
- In Nexus' step 2 of the import process, copy the provided reserve address to use in the next step
- Update token using the reserve address in the previous step and by using the tool:
  ```bash
  dotnet run manageasset --asset 736070675 --from U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --creator U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --manager U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --reserve CKTANMKUIJAGTBCTD4PSNGPSXKQB3ETMX7FL2C23GKZIUIUAPAVF2TKS3I --round 49800994 --decimals 7 --overalllimit 10000 --unit TEMT001 --name TestEMTCreation1
  ```
  ```bash
  dotnet run sign --privatekey XmtrNdFBhIK2VQxNFzsgAC6Q/5qgMdGFgDfqNXYa34U= --unsignedtx iaRhcGFyhKFjxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqFmxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqFtxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFyxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqRjYWlkzivfjBOjZmVlzQPoomZ2zgL35yKjZ2VurHRlc3RuZXQtdjEuMKJnaMQgSGO1GKSzyE7IEPItTxCByw9x8FmnrCDexi9/cOUJOiKibHbOAvfrCqNzbmTEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpHR5cGWkYWNmZw==
  ```
  ```bash
  dotnet run submit --signedtx gqNzaWfEQPpg3zkYhmXjW96BLG/mXxtz8nYrwtzB9BD3chWUpZZs8086vpSwV9ieQtn8r9gnU9hOFaXWB9owRqQMtDH0/AijdHhuiaRhcGFyhKFjxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqFmxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqFtxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFyxCASpgaxVEJAaYRTHx8mmfK6oB2SbL/KvQtbMrKKIoB4KqRjYWlkzivfjBOjZmVlzQPoomZ2zgL35yKjZ2VurHRlc3RuZXQtdjEuMKJnaMQgSGO1GKSzyE7IEPItTxCByw9x8FmnrCDexi9/cOUJOiKibHbOAvfrCqNzbmTEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpHR5cGWkYWNmZw==
  ```
- In Nexus' step 2 of the import process, select _Validate Reserve Address_ to validate that the previous step has been done successfully
- Transfer the token's total supply to the new reserve address by using the tool (note the amount should be in smallest unit (so * 10000000 for 7 decimals)):
  ```bash
  dotnet run payment --from U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --to CKTANMKUIJAGTBCTD4PSNGPSXKQB3ETMX7FL2C23GKZIUIUAPAVF2TKS3I --asset 736070675 --amount 100000000000 --round 49801200
  ```
  ```bash
  dotnet run sign --privatekey XmtrNdFBhIK2VQxNFzsgAC6Q/5qgMdGFgDfqNXYa34U= --unsignedtx iaRhYW10zwAAABdIdugApGFyY3bEIBKmBrFUQkBphFMfHyaZ8rqgHZJsv8q9C1sysooigHgqo2ZlZc0D6KJmds4C9+fwomdoxCBIY7UYpLPITsgQ8i1PEIHLD3HwWaesIN7GL39w5Qk6IqJsds4C9+vYo3NuZMQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8akdHlwZaVheGZlcqR4YWlkzivfjBM=
  ```
  ```bash
  dotnet run submit --signedtx gqNzaWfEQAeCfciovOskLyNHeDK6NDWg/fh3pDbLSVW9KE0WmZJBIoMQOzqYeV//hfwM34q5HhCbRHu6aPSYZuOJetWZjw6jdHhuiaRhYW10zwAAABdIdugApGFyY3bEIBKmBrFUQkBphFMfHyaZ8rqgHZJsv8q9C1sysooigHgqo2ZlZc0D6KJmds4C9+fwomdoxCBIY7UYpLPITsgQ8i1PEIHLD3HwWaesIN7GL39w5Qk6IqJsds4C9+vYo3NuZMQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8akdHlwZaVheGZlcqR4YWlkzivfjBM=
  ```
- In Nexus' step 3 of the import process, select _Validate Transfer_ to validate that the previous step has been done successfully
- In Nexus' step 4 of the import process, enter the missing data needed and select _Import_

### 2. Remove Nexus access from your imported EMT  
After creating and importing an EMT into Nexus, the token's creator and manager address will differ to that of the token's reserve, freeze and clawback, which means ownership of the token sit with you, while the operational token activities address can still be done within Nexus. Ownership means that the token's creator and manager address' private key is not stored within Nexus, and that the owner has the power to at any stage, clawback all tokens, and change the token's reserve, freeze and clawback address to remove any access which Nexus had to the token.

Follow the below steps to remove any access Nexus has to your EMT:
 - Update token by setting the reserve account back to the creator account or a new different account and by using the tool:
  ```bash
  dotnet run manageasset --asset 736070675 --from U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --creator U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --manager U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --reserve U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --round 49801250 --decimals 7 --overalllimit 10000 --unit TEMT001 --name TestEMTCreation1
  ```
  ```bash
  dotnet run sign --privatekey XmtrNdFBhIK2VQxNFzsgAC6Q/5qgMdGFgDfqNXYa34U= --unsignedtx iaRhcGFyhKFjxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFmxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFtxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFyxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqRjYWlkzivfjBOjZmVlzQPoomZ2zgL36CKjZ2VurHRlc3RuZXQtdjEuMKJnaMQgSGO1GKSzyE7IEPItTxCByw9x8FmnrCDexi9/cOUJOiKibHbOAvfsCqNzbmTEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpHR5cGWkYWNmZw==
  ```
  ```bash
  dotnet run submit --signedtx gqNzaWfEQHc6cI/606+92kwd1f1o08ml7yDnOzEFn4Rlvqjx2kcmy3YGr5WFYzEQiwjD76/1Gy10w+cZUnck1u6IqLrLHw+jdHhuiaRhcGFyhKFjxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFmxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFtxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqFyxCCni+PPT3R28d0lkPpIYLhboFoqhubLw2aTQAGqSs8jxqRjYWlkzivfjBOjZmVlzQPoomZ2zgL36CKjZ2VurHRlc3RuZXQtdjEuMKJnaMQgSGO1GKSzyE7IEPItTxCByw9x8FmnrCDexi9/cOUJOiKibHbOAvfsCqNzbmTEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpHR5cGWkYWNmZw==
  ```
 - Clawback tokens by using the tool (note the amount should be in smallest unit (so * 10000000 for 7 decimals)):
  ```bash
  dotnet run clawback --from CKTANMKUIJAGTBCTD4PSNGPSXKQB3ETMX7FL2C23GKZIUIUAPAVF2TKS3I --to U6F6HT2POR3PDXJFSD5EQYFYLOQFUKUG43F4GZUTIAA2USWPEPDIL2X52I --asset 736070675 --amount 10000 --round 49807326
  ```
  ```bash
  dotnet run sign --privatekey XmtrNdFBhIK2VQxNFzsgAC6Q/5qgMdGFgDfqNXYa34U= --unsignedtx iqRhYW10zScQpGFyY3bEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpGFzbmTEIBKmBrFUQkBphFMfHyaZ8rqgHZJsv8q9C1sysooigHgqo2ZlZc0D6KJmds4C9//eomdoxCBIY7UYpLPITsgQ8i1PEIHLD3HwWaesIN7GL39w5Qk6IqJsds4C+APGo3NuZMQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8akdHlwZaVheGZlcqR4YWlkzivfjBM=
  ```
  ```bash
  dotnet run submit --signedtx gqNzaWfEQBfr3qPUWkxI3yrRIltb294ddzfvuJWfZWyH3Mt0y/I7eDb/+Oo0rVuimHmm0LgoS0PCLkgV15OopxxgpjxlJQyjdHhuiqRhYW10zScQpGFyY3bEIKeL489PdHbx3SWQ+khguFugWiqG5svDZpNAAapKzyPGpGFzbmTEIBKmBrFUQkBphFMfHyaZ8rqgHZJsv8q9C1sysooigHgqo2ZlZc0D6KJmds4C9//eomdoxCBIY7UYpLPITsgQ8i1PEIHLD3HwWaesIN7GL39w5Qk6IqJsds4C+APGo3NuZMQgp4vjz090dvHdJZD6SGC4W6BaKobmy8Nmk0ABqkrPI8akdHlwZaVheGZlcqR4YWlkzivfjBM=
  ```

## Frequently Received Errors
See below a list of some frequently received errors and an explanation on each. Errors may contain dummy values for display purposes.
- **txn dead: round 49732660 outside of 48168757--48169757** - In order to keep your private key secure, this tool has the ability to run offline for most of its actions, but also mean that the current MainNet round/block needs to be retrieved and fed to the tool in the form of the 'round' option. On top of this you are able to specify the 'validrounds' option (which defaults to a 1000 if not specified). These two options creates the set in which you need to submit your request in order to not receive this error.
- **overspend** - Certain actions possible through this tool token requires the payment of a fee on the Algorand blockchain to confirm the action. Your account does not have any or enough ALGO.
- **must optin** - The account and token you are referring to in your action, does not have a trustline set up between them, and the account first needs to optin to that token.
- **asset frozen in recipient** - A token is able to be created with the option "defaultfrozen" set as true. This will mean any account that optedin to that token first needs to be unfrozen in order to transact with that token.
- **underflow on subtracting 10 from sender amount 9** - The amount of tokens you are trying to send from the sender to the receiver account is more than the available balance on the sender account.

## Licence
The Algorand Account and Asset, Creation and Management CLI tool is licensed under an MIT license. See the License file for details.
