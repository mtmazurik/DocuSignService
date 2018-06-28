CREATE TABLE [dbo].[Signature] (
    [SignatureId]  INT              IDENTITY (1, 1) NOT NULL,
    [Status]       NVARCHAR (50)    NULL,
	[UTCDateTimeCreated]  DATETIME2(7)		NULL,
	[UTCDateTimeLastUpdated]  DATETIME2(7)		NULL,
    [TemplateId]   UNIQUEIDENTIFIER NULL,
    [EnvelopeId]   UNIQUEIDENTIFIER NULL,
	[DocuSignUsername] NVARCHAR (50) NULL,
	[DocuSignPassword] NVARCHAR (50) NULL,
    [RequestBody]  NVARCHAR (MAX)   NULL,
    [ResponseBody] NVARCHAR (MAX)   NULL
);

