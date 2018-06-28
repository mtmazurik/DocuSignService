/*
Post-Deployment Script Template							
*/
USE [SignatureSvc]

-- run the permissions script
:r .\Permissions.sql

SET IDENTITY_INSERT [dbo].[Signature] ON 
INSERT [dbo].[Signature] ([SignatureId], [Status], [UTCDateTimeCreated],[UTCDateTimeLastUpdated],[TemplateId], [EnvelopeId], [DocuSignUsername], [DocuSignPassword],[RequestBody], [ResponseBody]) VALUES (1, N'completed', CAST('0001-01-01' AS DATETIME2(7)),CAST('0001-01-01' AS DATETIME2(7)), N'00e571b0-a6e8-4709-aaad-3b50f82bdcbb', N'0cc930d8-9604-45cf-92c4-f8cad93bcca7', N'slewarne@epiqsystems.com',N'P@ssword1',N'''{
  "docuSignTemplateId": "00e571b0-a6e8-4709-aaad-3b50f82bdcbb",
  "emailAddresses": ["mmazurik@epiqglobal.com"],
  "subject": "Signature Service POST called. Document is ready to sign",
  "name" : "Marty Mazurik",
  "fields": [
    { "name": "caseName",
      "fieldType": "Text",
      "dataType" : "string",
      "value" : "Hollywood vs. Oil Claim Jumper"
    },
    { "name": "claimantName",
      "fieldType": "Text",
      "dataType" : "string",
      "value" : "Jed Clampett"
    }
    ]
}''', N'''{
	"meta": {},
	"data": [{
		"email": "mmazurik@epiqglobal.com",
		"envelopeId": "0cc930d8-9604-45cf-92c4-f8cad93bcca7",
		"statusDateTime": "05/08/2018 20:49:20"
	}]
}''')
SET IDENTITY_INSERT [dbo].[Signature] OFF


