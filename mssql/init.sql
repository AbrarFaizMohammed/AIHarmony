/* Create DB (idempotent) */
IF DB_ID(N'AIHarmony') IS NULL
BEGIN
  CREATE DATABASE AIHarmony;
END
GO

USE AIHarmony;
GO

/* Users table (matches AIHarmony.Models.Users) */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.Users (
      usedId       UNIQUEIDENTIFIER NOT NULL,            -- [Key]
      firstName    NVARCHAR(100)    NOT NULL,            -- [Required]
      lastName     NVARCHAR(100)    NOT NULL,            -- [Required]
      emailId      NVARCHAR(256)    NOT NULL,            -- [Required]
      [password]   NVARCHAR(200)    NOT NULL,            -- [Required] (store a hash, not plaintext)

      CONSTRAINT PK_Users PRIMARY KEY (usedId),
      CONSTRAINT UQ_Users_emailId UNIQUE (emailId),
      CONSTRAINT CK_Users_firstName_NotEmpty CHECK (LEN(LTRIM(RTRIM(firstName))) > 0),
      CONSTRAINT CK_Users_lastName_NotEmpty  CHECK (LEN(LTRIM(RTRIM(lastName)))  > 0),
      CONSTRAINT CK_Users_emailId_NotEmpty   CHECK (LEN(LTRIM(RTRIM(emailId)))   > 0),
      CONSTRAINT CK_Users_password_NotEmpty  CHECK (LEN(LTRIM(RTRIM([password])))> 0)
  );
END
GO

/* ConfidentialWords table (matches AIHarmony.Models.ConfidentialWords) */
IF OBJECT_ID(N'dbo.ConfidentialWords', N'U') IS NULL
BEGIN
  CREATE TABLE dbo.ConfidentialWords (
      ConfidentialWordId UNIQUEIDENTIFIER NOT NULL,      -- [Key]
      Word               NVARCHAR(200)    NOT NULL,      -- [Required]
      UserId             UNIQUEIDENTIFIER NOT NULL,      -- [ForeignKey("Users") -> Users.usedId]

      CONSTRAINT PK_ConfidentialWords PRIMARY KEY (ConfidentialWordId),
      CONSTRAINT FK_ConfidentialWords_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(usedId)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,

      CONSTRAINT CK_ConfidentialWords_Word_NotEmpty CHECK (LEN(LTRIM(RTRIM(Word))) > 0)
  );

  /* Optional: prevent duplicate words per user */
  CREATE UNIQUE INDEX UX_ConfidentialWords_User_Word
    ON dbo.ConfidentialWords(UserId, Word);
END
GO
