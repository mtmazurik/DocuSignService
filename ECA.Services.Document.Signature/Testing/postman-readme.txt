Postman is a REST API tool, google: postman
It is relatively easy to use

This link is a shared link to import: https://www.getpostman.com/collections/e5e4ee889be5d7684d09

To import:
1) Install/open Postman
2) Hit 'Import' button
3) Select 'Import from Link'
4) past above URL


To test Container and DocuSign
1) Container is running: have IP address and Port
   a) if running locally, use http://localhost 
   b) port can be determined at command prompt by:     > docker ps -a -s
   c) note: the port number of the running local container   PORT: ______  (example: 1463)

2) Ping (should return: 200 OK)  http://localhost:1463/signature/ping

3) Swagger:   a description of API   http://localhost:1463/swagger

4) Send Test:    POST http://localhost:57279/signature  will have this JSON body:

{
  "docuSignUsername": "slewarne@epiqsystems.com",
  "docuSignPassword": "P@ssword1",	
  "docuSignTemplateId": "00e571b0-a6e8-4709-aaad-3b50f82bdcbb",
  "emailAddress": "mmazurik@epiqglobal.com",
  "subject": "Your DocuSign document is ready to sign",
  "name": "Jed Clampett",
  "fields": {
  	"caseName": "Hollywood vs. Oil Claim Jumper",
  	"claimantName": "Jed Clampett"
  }
}

Replace emailAddress: with your own and press Send

5) docuSignTemplateId can be played with, document can be modified 
	a) use docuSignUsername/docuSignPassword with login from https://appdemo.docusign.com/home
	b) edit document and add more text fields with DataLabel to match what is in above "fields", add to the caseName, claimantName

	NOTE:  The json has initial cap lower, the DataLabel has capitalized first character which is automatically mapped correclty to the l.c. version

6) can experiment with another docuSignTemplate and docuSignTemplateId with POSTMAN, too.


