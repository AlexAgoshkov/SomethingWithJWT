IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [ActiveKeys] (
        [Id] int NOT NULL IDENTITY,
        [Key] nvarchar(50) NULL,
        [Created] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_ActiveKeys] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Authentications] (
        [Id] int NOT NULL IDENTITY,
        [AccessToken] nvarchar(max) NULL,
        [RefreshToken] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [Expires] datetime2 NOT NULL,
        CONSTRAINT [PK_Authentications] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Images] (
        [Id] int NOT NULL IDENTITY,
        [ImagePath] nvarchar(max) NULL,
        [CroppedImagePath] nvarchar(max) NULL,
        CONSTRAINT [PK_Images] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [LogData] (
        [Id] int NOT NULL IDENTITY,
        [Category] nvarchar(max) NULL,
        [Message] nvarchar(max) NULL,
        [User] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        [Chat] nvarchar(max) NULL,
        [ChatId] int NOT NULL,
        CONSTRAINT [PK_LogData] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [UserName] varchar(25) NOT NULL,
        [FirstName] varchar(25) NULL,
        [SurName] varchar(25) NULL,
        [Email] varchar(50) NULL,
        [Password] nvarchar(max) NOT NULL,
        [UserRole] nvarchar(max) NULL,
        [ImageId] int NULL,
        [ActiveKeyId] int NULL,
        [AuthenticationId] int NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_ActiveKeys_ActiveKeyId] FOREIGN KEY ([ActiveKeyId]) REFERENCES [ActiveKeys] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Users_Authentications_AuthenticationId] FOREIGN KEY ([AuthenticationId]) REFERENCES [Authentications] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Users_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [Images] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Chats] (
        [Id] int NOT NULL IDENTITY,
        [IsPrivate] bit NOT NULL,
        [IsOnlyJoin] bit NOT NULL,
        [IsReadOnly] bit NOT NULL,
        [ChatType] int NOT NULL,
        [ImageId] int NULL,
        [ChatName] nvarchar(max) NULL,
        [ChatOwnerId] int NULL,
        CONSTRAINT [PK_Chats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Chats_Users_ChatOwnerId] FOREIGN KEY ([ChatOwnerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Chats_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [Images] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Friends] (
        [Id] int NOT NULL IDENTITY,
        [UserAddedId] int NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Friends] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Friends_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [LastChatDatas] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(max) NULL,
        [Text] nvarchar(max) NULL,
        [ChatId] int NOT NULL,
        CONSTRAINT [PK_LastChatDatas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LastChatDatas_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [Messages] (
        [Id] int NOT NULL IDENTITY,
        [Text] nvarchar(max) NULL,
        [ImageId] int NULL,
        [ChatId] int NOT NULL,
        [SenderId] int NOT NULL,
        [Time] datetime2 NOT NULL,
        [OriginalMessageId] int NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Messages_Images_ImageId] FOREIGN KEY ([ImageId]) REFERENCES [Images] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_Messages_OriginalMessageId] FOREIGN KEY ([OriginalMessageId]) REFERENCES [Messages] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [UserChatReads] (
        [UserId] int NOT NULL,
        [ChatId] int NOT NULL,
        [IsRead] bit NOT NULL,
        CONSTRAINT [PK_UserChatReads] PRIMARY KEY ([ChatId], [UserId]),
        CONSTRAINT [FK_UserChatReads_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserChatReads_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE TABLE [UserChats] (
        [ChatId] int NOT NULL,
        [UserId] int NOT NULL,
        [IsPrivateMask] bit NOT NULL,
        [IsUserJoined] bit NOT NULL,
        CONSTRAINT [PK_UserChats] PRIMARY KEY ([ChatId], [UserId]),
        CONSTRAINT [FK_UserChats_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserChats_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Chats_ChatOwnerId] ON [Chats] ([ChatOwnerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Chats_ImageId] ON [Chats] ([ImageId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Friends_UserId] ON [Friends] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_LastChatDatas_ChatId] ON [LastChatDatas] ([ChatId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Messages_ChatId] ON [Messages] ([ChatId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Messages_ImageId] ON [Messages] ([ImageId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Messages_OriginalMessageId] ON [Messages] ([OriginalMessageId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_UserChatReads_UserId] ON [UserChatReads] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_UserChats_UserId] ON [UserChats] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Users_ActiveKeyId] ON [Users] ([ActiveKeyId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Users_AuthenticationId] ON [Users] ([AuthenticationId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    CREATE INDEX [IX_Users_ImageId] ON [Users] ([ImageId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812120537_Init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200812120537_Init', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812134823_Chat_Members')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Chats]') AND [c].[name] = N'IsOnlyJoin');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Chats] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Chats] DROP COLUMN [IsOnlyJoin];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812134823_Chat_Members')
BEGIN
    CREATE TABLE [ChatMembers] (
        [UserId] int NOT NULL,
        [ChatId] int NOT NULL,
        CONSTRAINT [PK_ChatMembers] PRIMARY KEY ([ChatId], [UserId]),
        CONSTRAINT [FK_ChatMembers_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [Chats] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChatMembers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812134823_Chat_Members')
BEGIN
    CREATE INDEX [IX_ChatMembers_UserId] ON [ChatMembers] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812134823_Chat_Members')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200812134823_Chat_Members', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812140130_UserChat_Removed_IsJoin')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserChats]') AND [c].[name] = N'IsUserJoined');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [UserChats] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [UserChats] DROP COLUMN [IsUserJoined];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812140130_UserChat_Removed_IsJoin')
BEGIN
    ALTER TABLE [ChatMembers] ADD [IsUserJoined] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812140130_UserChat_Removed_IsJoin')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200812140130_UserChat_Removed_IsJoin', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200817142810_Detect')
BEGIN
    ALTER TABLE [Users] ADD [DetectId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200817142810_Detect')
BEGIN
    CREATE TABLE [Detects] (
        [Id] int NOT NULL IDENTITY,
        [DeviceType] nvarchar(max) NULL,
        [Os] nvarchar(max) NULL,
        [Browser] nvarchar(max) NULL,
        CONSTRAINT [PK_Detects] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200817142810_Detect')
BEGIN
    CREATE INDEX [IX_Users_DetectId] ON [Users] ([DetectId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200817142810_Detect')
BEGIN
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Detects_DetectId] FOREIGN KEY ([DetectId]) REFERENCES [Detects] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200817142810_Detect')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200817142810_Detect', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Detects_DetectId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    DROP INDEX [IX_Users_DetectId] ON [Users];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'DetectId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Users] DROP COLUMN [DetectId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    ALTER TABLE [Detects] ADD [UserId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    CREATE INDEX [IX_Detects_UserId] ON [Detects] ([UserId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    ALTER TABLE [Detects] ADD CONSTRAINT [FK_Detects_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820084327_Added_Detects_List')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200820084327_Added_Detects_List', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820093005_Added_Detects_List_UserId')
BEGIN
    ALTER TABLE [Detects] DROP CONSTRAINT [FK_Detects_Users_UserId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820093005_Added_Detects_List_UserId')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Detects]') AND [c].[name] = N'UserId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Detects] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Detects] ALTER COLUMN [UserId] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820093005_Added_Detects_List_UserId')
BEGIN
    ALTER TABLE [Detects] ADD CONSTRAINT [FK_Detects_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200820093005_Added_Detects_List_UserId')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200820093005_Added_Detects_List_UserId', N'3.1.7');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200826085748_User_Ip')
BEGIN
    ALTER TABLE [Detects] ADD [UserIp] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200826085748_User_Ip')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200826085748_User_Ip', N'3.1.7');
END;

GO

