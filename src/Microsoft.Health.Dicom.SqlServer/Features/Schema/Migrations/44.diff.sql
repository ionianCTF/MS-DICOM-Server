﻿SET XACT_ABORT ON
BEGIN TRANSACTION

/*************************************************************
    FilePropertyTableType TableType
    To use when inserting rows of FileProperty
**************************************************************/
IF TYPE_ID(N'FilePropertyTableType') IS NULL
BEGIN
CREATE TYPE dbo.FilePropertyTableType AS TABLE
(
    Watermark BIGINT          NOT NULL,
    FilePath  NVARCHAR (4000) NOT NULL,
    ETag      NVARCHAR (4000) NOT NULL
)
END
GO

/*************************************************************
    sproc updates
**************************************************************/

/*************************************************************
    DeleteInstanceV6 altered to select FileProperty on 
    watermark as well as instanceKey to support Update 
    operation which will result in N rows of FileProperty per
    instance
**************************************************************/

CREATE OR ALTER PROCEDURE dbo.DeleteInstanceV6
@cleanupAfter DATETIMEOFFSET (0), @createdStatus TINYINT, @partitionKey INT, @studyInstanceUid VARCHAR (64), @seriesInstanceUid VARCHAR (64)=NULL, @sopInstanceUid VARCHAR (64)=NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @deletedInstances AS TABLE (
        PartitionKey      INT         ,
        StudyInstanceUid  VARCHAR (64),
        SeriesInstanceUid VARCHAR (64),
        SopInstanceUid    VARCHAR (64),
        Status            TINYINT     ,
        Watermark         BIGINT      ,
        OriginalWatermark BIGINT      ,
        InstanceKey       INT         );
    DECLARE @studyKey AS BIGINT;
    DECLARE @seriesKey AS BIGINT;
    DECLARE @instanceKey AS BIGINT;
    DECLARE @deletedDate AS DATETIME2 = SYSUTCDATETIME();
    DECLARE @imageResourceType AS TINYINT = 0;
    SELECT @studyKey = StudyKey,
           @seriesKey = CASE @seriesInstanceUid WHEN NULL THEN NULL ELSE SeriesKey END,
           @instanceKey = CASE @sopInstanceUid WHEN NULL THEN NULL ELSE InstanceKey END
    FROM   dbo.Instance
    WHERE  PartitionKey = @partitionKey
           AND StudyInstanceUid = @studyInstanceUid
           AND SeriesInstanceUid = ISNULL(@seriesInstanceUid, SeriesInstanceUid)
           AND SopInstanceUid = ISNULL(@sopInstanceUid, SopInstanceUid);
    DELETE dbo.Instance
    OUTPUT deleted.PartitionKey, deleted.StudyInstanceUid, deleted.SeriesInstanceUid, deleted.SopInstanceUid, deleted.Status, deleted.Watermark, deleted.OriginalWatermark, deleted.InstanceKey INTO @deletedInstances
    WHERE  PartitionKey = @partitionKey
           AND StudyInstanceUid = @studyInstanceUid
           AND SeriesInstanceUid = ISNULL(@seriesInstanceUid, SeriesInstanceUid)
           AND SopInstanceUid = ISNULL(@sopInstanceUid, SopInstanceUid);
    IF @@ROWCOUNT = 0
        THROW 50404, 'Instance not found', 1;
    DELETE FP
    FROM   dbo.FileProperty AS FP
           INNER JOIN
           @deletedInstances AS DI
           ON DI.InstanceKey = FP.InstanceKey
              AND DI.Watermark = FP.Watermark;
    DECLARE @deletedTags AS TABLE (
        TagKey BIGINT);
    DELETE XQTE
    OUTPUT deleted.TagKey INTO @deletedTags
    FROM   dbo.ExtendedQueryTagError AS XQTE
           INNER JOIN
           @deletedInstances AS DI
           ON XQTE.Watermark = DI.Watermark;
    IF EXISTS (SELECT *
               FROM   @deletedTags)
        BEGIN
            DECLARE @deletedTagCounts AS TABLE (
                TagKey     BIGINT,
                ErrorCount INT   );
            INSERT INTO @deletedTagCounts (TagKey, ErrorCount)
            SELECT   TagKey,
                     COUNT(1)
            FROM     @deletedTags
            GROUP BY TagKey;
            UPDATE XQT
            SET    XQT.ErrorCount = XQT.ErrorCount - DTC.ErrorCount
            FROM   dbo.ExtendedQueryTag AS XQT
                   INNER JOIN
                   @deletedTagCounts AS DTC
                   ON XQT.TagKey = DTC.TagKey;
        END
    DELETE dbo.ExtendedQueryTagString
    WHERE  SopInstanceKey1 = @studyKey
           AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
           AND SopInstanceKey3 = ISNULL(@instanceKey, SopInstanceKey3)
           AND PartitionKey = @partitionKey
           AND ResourceType = @imageResourceType;
    DELETE dbo.ExtendedQueryTagLong
    WHERE  SopInstanceKey1 = @studyKey
           AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
           AND SopInstanceKey3 = ISNULL(@instanceKey, SopInstanceKey3)
           AND PartitionKey = @partitionKey
           AND ResourceType = @imageResourceType;
    DELETE dbo.ExtendedQueryTagDouble
    WHERE  SopInstanceKey1 = @studyKey
           AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
           AND SopInstanceKey3 = ISNULL(@instanceKey, SopInstanceKey3)
           AND PartitionKey = @partitionKey
           AND ResourceType = @imageResourceType;
    DELETE dbo.ExtendedQueryTagDateTime
    WHERE  SopInstanceKey1 = @studyKey
           AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
           AND SopInstanceKey3 = ISNULL(@instanceKey, SopInstanceKey3)
           AND PartitionKey = @partitionKey
           AND ResourceType = @imageResourceType;
    DELETE dbo.ExtendedQueryTagPersonName
    WHERE  SopInstanceKey1 = @studyKey
           AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
           AND SopInstanceKey3 = ISNULL(@instanceKey, SopInstanceKey3)
           AND PartitionKey = @partitionKey
           AND ResourceType = @imageResourceType;
    INSERT INTO dbo.DeletedInstance (PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, Watermark, DeletedDateTime, RetryCount, CleanupAfter, OriginalWatermark)
    SELECT PartitionKey,
           StudyInstanceUid,
           SeriesInstanceUid,
           SopInstanceUid,
           Watermark,
           @deletedDate,
           0,
           @cleanupAfter,
           OriginalWatermark
    FROM   @deletedInstances;
    IF NOT EXISTS (SELECT *
                   FROM   dbo.Instance WITH (HOLDLOCK, UPDLOCK)
                   WHERE  PartitionKey = @partitionKey
                          AND StudyKey = @studyKey
                          AND SeriesKey = ISNULL(@seriesKey, SeriesKey))
        BEGIN
            DELETE dbo.Series
            WHERE  StudyKey = @studyKey
                   AND SeriesInstanceUid = ISNULL(@seriesInstanceUid, SeriesInstanceUid)
                   AND PartitionKey = @partitionKey;
            DELETE dbo.ExtendedQueryTagString
            WHERE  SopInstanceKey1 = @studyKey
                   AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagLong
            WHERE  SopInstanceKey1 = @studyKey
                   AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagDouble
            WHERE  SopInstanceKey1 = @studyKey
                   AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagDateTime
            WHERE  SopInstanceKey1 = @studyKey
                   AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagPersonName
            WHERE  SopInstanceKey1 = @studyKey
                   AND SopInstanceKey2 = ISNULL(@seriesKey, SopInstanceKey2)
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
        END
    IF NOT EXISTS (SELECT *
                   FROM   dbo.Series WITH (HOLDLOCK, UPDLOCK)
                   WHERE  Studykey = @studyKey
                          AND PartitionKey = @partitionKey)
        BEGIN
            DELETE dbo.Study
            WHERE  StudyKey = @studyKey
                   AND PartitionKey = @partitionKey;
            DELETE dbo.ExtendedQueryTagString
            WHERE  SopInstanceKey1 = @studyKey
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagLong
            WHERE  SopInstanceKey1 = @studyKey
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagDouble
            WHERE  SopInstanceKey1 = @studyKey
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagDateTime
            WHERE  SopInstanceKey1 = @studyKey
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
            DELETE dbo.ExtendedQueryTagPersonName
            WHERE  SopInstanceKey1 = @studyKey
                   AND PartitionKey = @partitionKey
                   AND ResourceType = @imageResourceType;
        END
    INSERT INTO dbo.ChangeFeed (Action, PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, OriginalWatermark)
    SELECT 1,
           DI.PartitionKey,
           DI.StudyInstanceUid,
           DI.SeriesInstanceUid,
           DI.SopInstanceUid,
           DI.Watermark
    FROM   @deletedInstances AS DI
    WHERE  Status = @createdStatus;
    UPDATE CF
    SET    CF.CurrentWatermark = NULL
    FROM   dbo.ChangeFeed AS CF WITH (FORCESEEK)
           INNER JOIN
           @deletedInstances AS DI
           ON CF.PartitionKey = DI.PartitionKey
              AND CF.StudyInstanceUid = DI.StudyInstanceUid
              AND CF.SeriesInstanceUid = DI.SeriesInstanceUid
              AND CF.SopInstanceUid = DI.SopInstanceUid;
    COMMIT TRANSACTION;
END

GO

/*************************************************************
    EndUpdateInstanceV44 altered to take in rows of 
    FileProperty from all of the updates blobs and insert them
    into table
**************************************************************/

CREATE OR ALTER PROCEDURE dbo.EndUpdateInstanceV44
@partitionKey INT, @studyInstanceUid VARCHAR (64), @patientId NVARCHAR (64)=NULL, @patientName NVARCHAR (325)=NULL, @patientBirthDate DATE=NULL, @insertFileProperties dbo.FilePropertyTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    DECLARE @currentDate AS DATETIME2 (7) = SYSUTCDATETIME();
    DECLARE @updatedInstances AS TABLE (
        PartitionKey      INT         ,
        StudyInstanceUid  VARCHAR (64),
        SeriesInstanceUid VARCHAR (64),
        SopInstanceUid    VARCHAR (64),
        Watermark         BIGINT      ,
        InstanceKey       BIGINT      );
    DELETE @updatedInstances;
    UPDATE dbo.Instance
    SET    LastStatusUpdatedDate = @currentDate,
           OriginalWatermark     = ISNULL(OriginalWatermark, Watermark),
           Watermark             = NewWatermark,
           NewWatermark          = NULL
    OUTPUT deleted.PartitionKey, @studyInstanceUid, deleted.SeriesInstanceUid, deleted.SopInstanceUid, deleted.NewWatermark, deleted.InstanceKey INTO @updatedInstances
    WHERE  PartitionKey = @partitionKey
           AND StudyInstanceUid = @studyInstanceUid
           AND Status = 1
           AND NewWatermark IS NOT NULL;
    UPDATE dbo.Study
    SET    PatientId        = ISNULL(@patientId, PatientId),
           PatientName      = ISNULL(@patientName, PatientName),
           PatientBirthDate = ISNULL(@patientBirthDate, PatientBirthDate)
    WHERE  PartitionKey = @partitionKey
           AND StudyInstanceUid = @studyInstanceUid;
    IF @@ROWCOUNT = 0
        THROW 50404, 'Study does not exist', 1;
    INSERT INTO dbo.FileProperty (InstanceKey, Watermark, FilePath, ETag)
    SELECT inst.InstanceKey,
           i.Watermark,
           i.FilePath,
           i.ETag
    FROM   @insertFileProperties AS i
           INNER JOIN
           @updatedInstances AS inst
           ON inst.Watermark = i.Watermark;
    INSERT INTO dbo.ChangeFeed (Action, PartitionKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, OriginalWatermark)
    SELECT 2,
           PartitionKey,
           StudyInstanceUid,
           SeriesInstanceUid,
           SopInstanceUid,
           Watermark
    FROM   @updatedInstances;
    UPDATE C
        SET CurrentWatermark = U.Watermark,
            FilePath = I.FilePath
        FROM dbo.ChangeFeed C
        JOIN @updatedInstances U
        ON C.PartitionKey = U.PartitionKey
            AND C.StudyInstanceUid = U.StudyInstanceUid
            AND C.SeriesInstanceUid = U.SeriesInstanceUid
            AND C.SopInstanceUid = U.SopInstanceUid
        LEFT OUTER JOIN @insertFileProperties I
        ON I.Watermark = U.Watermark;
    COMMIT TRANSACTION;
END

GO

COMMIT TRANSACTION