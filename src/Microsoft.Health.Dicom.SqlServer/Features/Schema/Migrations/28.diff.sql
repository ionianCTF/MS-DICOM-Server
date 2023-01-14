SET XACT_ABORT ON
BEGIN TRANSACTION

EXEC('ALTER VIEW dbo.StudyResultView
    WITH SCHEMABINDING
    AS
    SELECT  st.StudyInstanceUid,
            st.PatientId,
            st.PatientName,
            st.ReferringPhysicianName,
            st.StudyDate,
            st.StudyDescription,
            st.AccessionNumber,
            st.PatientBirthDate,
            (SELECT STRING_AGG(Modality, '','')
            FROM dbo.Series se 
            WHERE st.StudyKey = se.StudyKey
            AND st.PartitionKey = se.PartitionKey) AS ModalitiesInStudy,
            (SELECT SUM(1) 
            FROM dbo.Instance i 
            WHERE st.PartitionKey = i.PartitionKey
            AND st.StudyKey = i.StudyKey) AS NumberofStudyRelatedInstances,
            st.PartitionKey,
            st.StudyKey
    FROM dbo.Study st')
GO

EXEC('AlTER VIEW dbo.SeriesResultView
WITH SCHEMABINDING
AS
SELECT  se.SeriesInstanceUid,
        se.Modality,
        se.PerformedProcedureStepStartDate,
        se.ManufacturerModelName,
        (SELECT SUM(1)
        FROM dbo.Instance i 
        WHERE se.PartitionKey = i.PartitionKey
        AND se.StudyKey = i.StudyKey
        AND se.SeriesKey = i.SeriesKey) AS NumberofSeriesRelatedInstances,
        se.PartitionKey,
        se.StudyKey,
        se.SeriesKey
FROM dbo.Series se')
GO


CREATE OR ALTER PROCEDURE dbo.AddInstanceV6
    @partitionKey                       INT,
    @studyInstanceUid                   VARCHAR(64),
    @seriesInstanceUid                  VARCHAR(64),
    @sopInstanceUid                     VARCHAR(64),
    @patientId                          NVARCHAR(64),
    @patientName                        NVARCHAR(325) = NULL,
    @referringPhysicianName             NVARCHAR(325) = NULL,
    @studyDate                          DATE = NULL,
    @studyDescription                   NVARCHAR(64) = NULL,
    @accessionNumber                    NVARCHAR(64) = NULL,
    @modality                           NVARCHAR(16) = NULL,
    @performedProcedureStepStartDate    DATE = NULL,
    @patientBirthDate                   DATE = NULL,
    @manufacturerModelName              NVARCHAR(64) = NULL,
    @stringExtendedQueryTags dbo.InsertStringExtendedQueryTagTableType_1 READONLY,
    @longExtendedQueryTags dbo.InsertLongExtendedQueryTagTableType_1 READONLY,
    @doubleExtendedQueryTags dbo.InsertDoubleExtendedQueryTagTableType_1 READONLY,
    @dateTimeExtendedQueryTags dbo.InsertDateTimeExtendedQueryTagTableType_2 READONLY,
    @personNameExtendedQueryTags dbo.InsertPersonNameExtendedQueryTagTableType_1 READONLY,
    @initialStatus                      TINYINT,
    @transferSyntaxUid                  VARCHAR(64) = NULL
AS
BEGIN
    SET NOCOUNT ON

    -- We turn off XACT_ABORT so that we can rollback and retry the INSERT/UPDATE into the study table on failure
    SET XACT_ABORT OFF

    -- The transaction is wrapped in a try...catch block in case the INSERT into the study table fails
    BEGIN TRY

        BEGIN TRANSACTION

            DECLARE @currentDate DATETIME2(7) = SYSUTCDATETIME()
            DECLARE @existingStatus TINYINT
            DECLARE @newWatermark BIGINT
            DECLARE @studyKey BIGINT
            DECLARE @seriesKey BIGINT
            DECLARE @instanceKey BIGINT

            SELECT @existingStatus = Status
            FROM dbo.Instance
            WHERE PartitionKey = @partitionKey
                AND StudyInstanceUid = @studyInstanceUid
                AND SeriesInstanceUid = @seriesInstanceUid
                AND SopInstanceUid = @sopInstanceUid

            IF @@ROWCOUNT <> 0
                -- The instance already exists. Set the state = @existingStatus to indicate what state it is in.
                THROW 50409, 'Instance already exists', @existingStatus;

            -- The instance does not exist, insert it.
            SET @newWatermark = NEXT VALUE FOR dbo.WatermarkSequence
            SET @instanceKey = NEXT VALUE FOR dbo.InstanceKeySequence

            -- Insert Study
            -- If we fail to INSERT, we instead must UPDATE the newly added value
            SELECT @studyKey = StudyKey
            FROM dbo.Study WITH(UPDLOCK)
            WHERE PartitionKey = @partitionKey
                AND StudyInstanceUid = @studyInstanceUid

            IF @@ROWCOUNT = 0
            BEGIN TRY

                SET @studyKey = NEXT VALUE FOR dbo.StudyKeySequence

                INSERT INTO dbo.Study
                    (PartitionKey, StudyKey, StudyInstanceUid, PatientId, PatientName, PatientBirthDate, ReferringPhysicianName, StudyDate, StudyDescription, AccessionNumber)
                VALUES
                    (@partitionKey, @studyKey, @studyInstanceUid, @patientId, @patientName, @patientBirthDate, @referringPhysicianName, @studyDate, @studyDescription, @accessionNumber)

            END TRY
            BEGIN CATCH

                 -- While we could obtain a HOLDLOCK on the table, we optimistically obtain an UPDLOCK instead to avoid the range lock on the study table
                IF ERROR_NUMBER() = 2601
                BEGIN

                    SELECT @studyKey = StudyKey
                    FROM dbo.Study WITH(UPDLOCK)
                    WHERE PartitionKey = @partitionKey
                        AND StudyInstanceUid = @studyInstanceUid

                    -- Latest wins
                    UPDATE dbo.Study
                    SET PatientId = @patientId, PatientName = @patientName, PatientBirthDate = @patientBirthDate, ReferringPhysicianName = @referringPhysicianName, StudyDate = @studyDate, StudyDescription = @studyDescription, AccessionNumber = @accessionNumber
                    WHERE PartitionKey = @partitionKey
                        AND StudyKey = @studyKey


                END
                ELSE
                    THROW

            END CATCH
            ELSE
            BEGIN
                -- Latest wins
                UPDATE dbo.Study
                SET PatientId = @patientId, PatientName = @patientName, PatientBirthDate = @patientBirthDate, ReferringPhysicianName = @referringPhysicianName, StudyDate = @studyDate, StudyDescription = @studyDescription, AccessionNumber = @accessionNumber
                WHERE PartitionKey = @partitionKey
                    AND StudyKey = @studyKey
            END

            -- Insert Series
            SELECT @seriesKey = SeriesKey
            FROM dbo.Series WITH(UPDLOCK)
            WHERE StudyKey = @studyKey
            AND SeriesInstanceUid = @seriesInstanceUid
            AND PartitionKey = @partitionKey

            IF @@ROWCOUNT = 0
            BEGIN
                SET @seriesKey = NEXT VALUE FOR dbo.SeriesKeySequence

                INSERT INTO dbo.Series
                    (PartitionKey, StudyKey, SeriesKey, SeriesInstanceUid, Modality, PerformedProcedureStepStartDate, ManufacturerModelName)
                VALUES
                    (@partitionKey, @studyKey, @seriesKey, @seriesInstanceUid, @modality, @performedProcedureStepStartDate, @manufacturerModelName)
            END
            ELSE
            BEGIN
                -- Latest wins
                UPDATE dbo.Series
                SET Modality = @modality, PerformedProcedureStepStartDate = @performedProcedureStepStartDate, ManufacturerModelName = @manufacturerModelName
                WHERE SeriesKey = @seriesKey
                AND StudyKey = @studyKey
                AND PartitionKey = @partitionKey
            END

            -- Insert Instance
            INSERT INTO dbo.Instance
                (PartitionKey, StudyKey, SeriesKey, InstanceKey, StudyInstanceUid, SeriesInstanceUid, SopInstanceUid, Watermark, Status, LastStatusUpdatedDate, CreatedDate, TransferSyntaxUid)
            VALUES
                (@partitionKey, @studyKey, @seriesKey, @instanceKey, @studyInstanceUid, @seriesInstanceUid, @sopInstanceUid, @newWatermark, @initialStatus, @currentDate, @currentDate, @transferSyntaxUid)

            BEGIN TRY

                EXEC dbo.IIndexInstanceCoreV9
                    @partitionKey,
                    @studyKey,
                    @seriesKey,
                    @instanceKey,
                    @newWatermark,
                    @stringExtendedQueryTags,
                    @longExtendedQueryTags,
                    @doubleExtendedQueryTags,
                    @dateTimeExtendedQueryTags,
                    @personNameExtendedQueryTags

            END TRY
            BEGIN CATCH

                THROW

            END CATCH

            SELECT @newWatermark

        COMMIT TRANSACTION

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW

    END CATCH
END

COMMIT TRANSACTION

IF EXISTS 
(
    SELECT *
    FROM    sys.indexes
    WHERE   NAME = 'IX_Study_StudyInstanceUid_PartitionKey'
        AND Object_id = OBJECT_ID('dbo.Study')
)
BEGIN
    DROP INDEX IX_Study_StudyInstanceUid_PartitionKey ON dbo.Study

    CREATE UNIQUE NONCLUSTERED INDEX IX_Study_PartitionKey_StudyInstanceUid ON dbo.Study
    (
        PartitionKey,
        StudyInstanceUid
    )
    INCLUDE
    (
        StudyKey
    )
    WITH (DATA_COMPRESSION = PAGE, DROP_EXISTING=ON, ONLINE=ON)
END

IF EXISTS 
(
    SELECT *
    FROM    sys.indexes
    WHERE   NAME = 'IX_Series_SeriesKey'
        AND Object_id = OBJECT_ID('dbo.Series')
)
BEGIN
    DROP INDEX IX_Series_SeriesKey ON dbo.Series

    CREATE UNIQUE NONCLUSTERED INDEX IX_Series_PartitionKey_StudyKey_SeriesInstanceUid ON dbo.Series
    (
        PartitionKey,
        StudyKey,
        SeriesInstanceUid
    )
    WITH (DATA_COMPRESSION = PAGE, DROP_EXISTING=ON, ONLINE=ON)
END

IF EXISTS 
(
    SELECT *
    FROM    sys.indexes
    WHERE   NAME = 'IXC_Instance'
        AND Object_id = OBJECT_ID('dbo.Instance')
)
BEGIN
    CREATE UNIQUE CLUSTERED INDEX IXC_Instance on dbo.Instance
    (
        PartitionKey,
        StudyKey,
        SeriesKey,
        InstanceKey
    )
    WITH (DROP_EXISTING=ON, ONLINE=ON)
END

IF EXISTS 
(
    SELECT *
    FROM    sys.indexes
    WHERE   NAME = 'IX_Instance_StudyInstanceUid_Status_PartitionKey'
        AND Object_id = OBJECT_ID('dbo.Instance')
)
BEGIN
    DROP INDEX IX_Instance_StudyInstanceUid_Status_PartitionKey ON dbo.Instance
    DROP INDEX IX_Instance_StudyInstanceUid_SeriesInstanceUid_Status_PartitionKey ON dbo.Instance
    DROP INDEX IX_Instance_SopInstanceUid_Status_PartitionKey ON dbo.Instance

    CREATE NONCLUSTERED INDEX IX_Instance_PartitionKey_Status_StudyInstanceUid_SeriesInstanceUid_SopInstanceUid on dbo.Instance
    (
        PartitionKey,
        Status,
        StudyInstanceUid,
        SeriesInstanceUid, 
        SopInstanceUid
    )
    INCLUDE
    (
        Watermark,
        TransferSyntaxUid
    )
    WITH (DATA_COMPRESSION = PAGE)
END

IF EXISTS 
(
    SELECT *
    FROM    sys.indexes
    WHERE   NAME = 'IX_Instance_SeriesKey_Status_Watermark        '
        AND Object_id = OBJECT_ID('dbo.Instance')
)
BEGIN
    DROP INDEX IX_Instance_SeriesKey_Status_Watermark ON dbo.Instance
    DROP INDEX IX_Instance_StudyKey_Status_Watermark ON dbo.Instance

    CREATE NONCLUSTERED INDEX IX_Instance_PartitionKey_Status_StudyKey_Watermark on dbo.Instance
    (
        PartitionKey,
        Status,
        StudyKey,
        Watermark
    )
    INCLUDE
    (
        StudyInstanceUid,
        SeriesInstanceUid,
        SopInstanceUid  
    )
    WITH (DATA_COMPRESSION = PAGE, DROP_EXISTING=ON, ONLINE=ON)

    CREATE NONCLUSTERED INDEX IX_Instance_PartitionKey_Status_StudyKey_SeriesKey_Watermark on dbo.Instance
    (
        PartitionKey,
        Status,
        StudyKey,
        SeriesKey,
        Watermark
    )
    INCLUDE
    (
        StudyInstanceUid,
        SeriesInstanceUid,
        SopInstanceUid  
    )
    WITH (DATA_COMPRESSION = PAGE, DROP_EXISTING=ON, ONLINE=ON)
END

