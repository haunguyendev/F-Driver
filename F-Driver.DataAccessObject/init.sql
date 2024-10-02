IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CancellationReasons] (
    [Id] int NOT NULL IDENTITY,
    [Content] varchar(255) NOT NULL,
    CONSTRAINT [PK__Cancella__A4F8C0E7BD34C5C7] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Name] varchar(255) NOT NULL,
    [Email] varchar(255) NOT NULL,
    [PhoneNumber] varchar(15) NOT NULL,
    [PasswordHash] varchar(255) NOT NULL,
    [ProfileImageURL] varchar(255) NOT NULL,
    [StudentIdCardURL] varchar(255) NOT NULL,
    [Role] varchar(20) NOT NULL,
    [StudentId] varchar(50) NULL,
    [Verified] bit NULL DEFAULT CAST(0 AS bit),
    [VerificationStatus] nvarchar(max) NULL,
    [IsMailValid] bit NULL,
    [DeviceId] nvarchar(max) NULL,
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Users__1788CC4C5AA6AB44] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Zones] (
    [Id] int NOT NULL IDENTITY,
    [ZoneName] varchar(255) NOT NULL,
    [Description] varchar(255) NULL,
    CONSTRAINT [PK__Zones__601667B5715283B6] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Drivers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [LicenseNumber] varchar(50) NOT NULL,
    [Verified] bit NULL DEFAULT CAST(0 AS bit),
    [LicenseImageURL] varchar(255) NOT NULL,
    CONSTRAINT [PK__Drivers__F1B1CD049D52C77F] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Drivers__UserId__4222D4EF] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Wallets] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [Balance] decimal(10,2) NULL DEFAULT 0.0,
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Wallets__84D4F90E8673D7BE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Wallets__UserId__75A278F5] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PriceTable] (
    [Id] int NOT NULL IDENTITY,
    [FromZoneId] int NOT NULL,
    [ToZoneId] int NOT NULL,
    [UnitPrice] decimal(10,2) NOT NULL,
    CONSTRAINT [PK__PriceTab__49575BAF1A1D0D0D] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__PriceTabl__FromZ__6EF57B66] FOREIGN KEY ([FromZoneId]) REFERENCES [Zones] ([Id]),
    CONSTRAINT [FK__PriceTabl__ToZon__6FE99F9F] FOREIGN KEY ([ToZoneId]) REFERENCES [Zones] ([Id])
);
GO

CREATE TABLE [TripRequests] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [FromZoneId] int NOT NULL,
    [ToZoneId] int NOT NULL,
    [TripDate] date NOT NULL,
    [StartTime] time NOT NULL,
    [Slot] int NULL,
    [Status] varchar(20) NULL DEFAULT 'available',
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__TripRequ__33A8517AEC13D92A] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__TripReque__FromZ__5070F446] FOREIGN KEY ([FromZoneId]) REFERENCES [Zones] ([Id]),
    CONSTRAINT [FK__TripReque__ToZon__5165187F] FOREIGN KEY ([ToZoneId]) REFERENCES [Zones] ([Id]),
    CONSTRAINT [FK__TripReque__UserI__4F7CD00D] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Vehicles] (
    [Id] int NOT NULL IDENTITY,
    [DriverId] int NULL,
    [LicensePlate] varchar(10) NOT NULL,
    [VehicleType] varchar(50) NOT NULL,
    [IsVerified] bit NULL DEFAULT CAST(0 AS bit),
    [Registration] varchar(255) NOT NULL,
    [VehicleImageURL] varchar(255) NULL,
    [RegistrationImageURL] varchar(255) NULL,
    CONSTRAINT [PK__Vehicles__476B549262DB2AF0] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Vehicles__Driver__46E78A0C] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [WalletId] int NULL,
    [Amount] decimal(10,2) NOT NULL,
    [Type] varchar(20) NOT NULL,
    [TransactionDate] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Transact__55433A6B4AC4DF18] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Transacti__Walle__7A672E12] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id])
);
GO

CREATE TABLE [TripMatches] (
    [Id] int NOT NULL IDENTITY,
    [DriverRequestId] int NULL,
    [PassengerRequestId] int NULL,
    [MatchedAt] datetime NULL DEFAULT ((getdate())),
    [Status] varchar(20) NULL DEFAULT 'pending',
    CONSTRAINT [PK__TripMatc__4218C817E65D171B] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__TripMatch__Drive__571DF1D5] FOREIGN KEY ([DriverRequestId]) REFERENCES [TripRequests] ([Id]),
    CONSTRAINT [FK__TripMatch__Passe__5812160E] FOREIGN KEY ([PassengerRequestId]) REFERENCES [TripRequests] ([Id])
);
GO

CREATE TABLE [Cancellations] (
    [Id] int NOT NULL IDENTITY,
    [TripMatchId] int NULL,
    [ReasonId] int NULL,
    [CanceledAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Cancella__6A2D9A3A6734385B] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Cancellat__Reaso__01142BA1] FOREIGN KEY ([ReasonId]) REFERENCES [CancellationReasons] ([Id]),
    CONSTRAINT [FK__Cancellat__TripM__00200768] FOREIGN KEY ([TripMatchId]) REFERENCES [TripMatches] ([Id])
);
GO

CREATE TABLE [Feedback] (
    [Id] int NOT NULL IDENTITY,
    [MatchId] int NULL,
    [PassengerId] int NULL,
    [DriverId] int NULL,
    [Rating] int NULL,
    [Comment] text NULL,
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Feedback__6A4BEDD6392927F6] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Feedback__Driver__6C190EBB] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]),
    CONSTRAINT [FK__Feedback__MatchI__6A30C649] FOREIGN KEY ([MatchId]) REFERENCES [TripMatches] ([Id]),
    CONSTRAINT [FK__Feedback__Passen__6B24EA82] FOREIGN KEY ([PassengerId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [MatchId] int NULL,
    [SenderId] int NULL,
    [Content] text NOT NULL,
    [SentAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Messages__C87C0C9C71DCD5C1] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Messages__MatchI__5BE2A6F2] FOREIGN KEY ([MatchId]) REFERENCES [TripMatches] ([Id]),
    CONSTRAINT [FK__Messages__Sender__5CD6CB2B] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [MatchId] int NULL,
    [PassengerId] int NULL,
    [DriverId] int NULL,
    [Amount] decimal(10,2) NOT NULL,
    [PaymentMethod] varchar(20) NOT NULL,
    [Status] varchar(20) NULL DEFAULT 'pending',
    [PaidAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Payments__9B556A387BED1B1C] PRIMARY KEY ([Id]),
    CONSTRAINT [FK__Payments__Driver__656C112C] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([Id]),
    CONSTRAINT [FK__Payments__MatchI__6383C8BA] FOREIGN KEY ([MatchId]) REFERENCES [TripMatches] ([Id]),
    CONSTRAINT [FK__Payments__Passen__6477ECF3] FOREIGN KEY ([PassengerId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_Cancellations_ReasonId] ON [Cancellations] ([ReasonId]);
GO

CREATE INDEX [IX_Cancellations_TripMatchId] ON [Cancellations] ([TripMatchId]);
GO

CREATE UNIQUE INDEX [UQ__Drivers__1788CC4D29B77C06] ON [Drivers] ([UserId]) WHERE [UserId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [UQ__Drivers__E8890166980DC78F] ON [Drivers] ([LicenseNumber]);
GO

CREATE INDEX [IX_Feedback_DriverId] ON [Feedback] ([DriverId]);
GO

CREATE INDEX [IX_Feedback_MatchId] ON [Feedback] ([MatchId]);
GO

CREATE INDEX [IX_Feedback_PassengerId] ON [Feedback] ([PassengerId]);
GO

CREATE INDEX [IX_Messages_MatchId] ON [Messages] ([MatchId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Payments_DriverId] ON [Payments] ([DriverId]);
GO

CREATE INDEX [IX_Payments_MatchId] ON [Payments] ([MatchId]);
GO

CREATE INDEX [IX_Payments_PassengerId] ON [Payments] ([PassengerId]);
GO

CREATE INDEX [IX_PriceTable_FromZoneId] ON [PriceTable] ([FromZoneId]);
GO

CREATE INDEX [IX_PriceTable_ToZoneId] ON [PriceTable] ([ToZoneId]);
GO

CREATE INDEX [IX_Transactions_WalletId] ON [Transactions] ([WalletId]);
GO

CREATE INDEX [IX_TripMatches_DriverRequestId] ON [TripMatches] ([DriverRequestId]);
GO

CREATE INDEX [IX_TripMatches_PassengerRequestId] ON [TripMatches] ([PassengerRequestId]);
GO

CREATE INDEX [IX_TripRequests_FromZoneId] ON [TripRequests] ([FromZoneId]);
GO

CREATE INDEX [IX_TripRequests_ToZoneId] ON [TripRequests] ([ToZoneId]);
GO

CREATE INDEX [IX_TripRequests_UserId] ON [TripRequests] ([UserId]);
GO

CREATE UNIQUE INDEX [UQ__Users__32C52B985591AF99] ON [Users] ([StudentId]) WHERE [StudentId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [UQ__Users__85FB4E3824AF3E00] ON [Users] ([PhoneNumber]);
GO

CREATE UNIQUE INDEX [UQ__Users__A9D1053460F37784] ON [Users] ([Email]);
GO

CREATE INDEX [IX_Vehicles_DriverId] ON [Vehicles] ([DriverId]);
GO

CREATE UNIQUE INDEX [UQ__Vehicles__026BC15CE964ACF2] ON [Vehicles] ([LicensePlate]);
GO

CREATE UNIQUE INDEX [UQ__Wallets__1788CC4DD4869EDC] ON [Wallets] ([UserId]) WHERE [UserId] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241002095902_init', N'8.0.8');
GO

COMMIT;
GO

