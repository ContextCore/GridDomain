CREATE TABLE Journal (
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp DATETIME2 NOT NULL,
  IsDeleted BIT NOT NULL,
  Manifest NVARCHAR(500) NOT NULL,
  Payload VARBINARY(MAX) NOT NULL
  CONSTRAINT PK_Journal PRIMARY KEY (PersistenceID, SequenceNr)
);

CREATE TABLE Snapshots (
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  Timestamp DATETIME2 NOT NULL,
  Manifest NVARCHAR(500) NOT NULL,
  Snapshot VARBINARY(MAX) NOT NULL
  CONSTRAINT PK_Snapshots PRIMARY KEY (PersistenceID, SequenceNr)
);

CREATE TABLE Metadata (
  PersistenceID NVARCHAR(255) NOT NULL,
  SequenceNr BIGINT NOT NULL,
  CONSTRAINT PK_Metadata PRIMARY KEY (PersistenceID, SequenceNr)
);