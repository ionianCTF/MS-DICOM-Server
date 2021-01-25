//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace Microsoft.Health.Dicom.SqlServer.Features.Schema.Model
{
    using Microsoft.Health.SqlServer.Features.Client;
    using Microsoft.Health.SqlServer.Features.Schema.Model;

    internal class VLatest
    {
        internal readonly static ChangeFeedTable ChangeFeed = new ChangeFeedTable();
        internal readonly static CustomTagTable CustomTag = new CustomTagTable();
        internal readonly static CustomTagJobTable CustomTagJob = new CustomTagJobTable();
        internal readonly static DeletedInstanceTable DeletedInstance = new DeletedInstanceTable();
        internal readonly static InstanceTable Instance = new InstanceTable();
        internal readonly static JobTable Job = new JobTable();
        internal readonly static SeriesTable Series = new SeriesTable();
        internal readonly static StudyTable Study = new StudyTable();
        internal readonly static AcquireCustomTagJobsProcedure AcquireCustomTagJobs = new AcquireCustomTagJobsProcedure();
        internal readonly static AddCustomTagProcedure AddCustomTag = new AddCustomTagProcedure();
        internal readonly static AddInstanceProcedure AddInstance = new AddInstanceProcedure();
        internal readonly static DeleteCustomTagProcedure DeleteCustomTag = new DeleteCustomTagProcedure();
        internal readonly static DeleteDeletedInstanceProcedure DeleteDeletedInstance = new DeleteDeletedInstanceProcedure();
        internal readonly static DeleteInstanceProcedure DeleteInstance = new DeleteInstanceProcedure();
        internal readonly static GetChangeFeedProcedure GetChangeFeed = new GetChangeFeedProcedure();
        internal readonly static GetChangeFeedLatestProcedure GetChangeFeedLatest = new GetChangeFeedLatestProcedure();
        internal readonly static GetCustomTagJobProcedure GetCustomTagJob = new GetCustomTagJobProcedure();
        internal readonly static GetCustomTagsOnJobProcedure GetCustomTagsOnJob = new GetCustomTagsOnJobProcedure();
        internal readonly static GetInstanceProcedure GetInstance = new GetInstanceProcedure();
        internal readonly static GetInstancesInThePastProcedure GetInstancesInThePast = new GetInstancesInThePastProcedure();
        internal readonly static GetLatestInstanceProcedure GetLatestInstance = new GetLatestInstanceProcedure();
        internal readonly static IncrementDeletedInstanceRetryProcedure IncrementDeletedInstanceRetry = new IncrementDeletedInstanceRetryProcedure();
        internal readonly static RetrieveDeletedInstanceProcedure RetrieveDeletedInstance = new RetrieveDeletedInstanceProcedure();
        internal readonly static UpdateCustomTagStatusProcedure UpdateCustomTagStatus = new UpdateCustomTagStatusProcedure();
        internal readonly static UpdateInstanceStatusProcedure UpdateInstanceStatus = new UpdateInstanceStatusProcedure();

        internal class ChangeFeedTable : Table
        {
            internal ChangeFeedTable() : base("dbo.ChangeFeed")
            {
            }

            internal readonly BigIntColumn Sequence = new BigIntColumn("Sequence");
            internal readonly DateTimeOffsetColumn Timestamp = new DateTimeOffsetColumn("Timestamp", 7);
            internal readonly TinyIntColumn Action = new TinyIntColumn("Action");
            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly VarCharColumn SopInstanceUid = new VarCharColumn("SopInstanceUid", 64);
            internal readonly BigIntColumn OriginalWatermark = new BigIntColumn("OriginalWatermark");
            internal readonly NullableBigIntColumn CurrentWatermark = new NullableBigIntColumn("CurrentWatermark");
            internal readonly Index IXC_ChangeFeed = new Index("IXC_ChangeFeed");
            internal readonly Index IX_ChangeFeed_StudyInstanceUid_SeriesInstanceUid_SopInstanceUid = new Index("IX_ChangeFeed_StudyInstanceUid_SeriesInstanceUid_SopInstanceUid");
        }

        internal class CustomTagTable : Table
        {
            internal CustomTagTable() : base("dbo.CustomTag")
            {
            }

            internal readonly BigIntColumn Key = new BigIntColumn("Key");
            internal readonly VarCharColumn Path = new VarCharColumn("Path", 64);
            internal readonly VarCharColumn VR = new VarCharColumn("VR", 2);
            internal readonly TinyIntColumn Level = new TinyIntColumn("Level");
            internal readonly TinyIntColumn Status = new TinyIntColumn("Status");
            internal readonly Index IXC_CustomTag = new Index("IXC_CustomTag");
        }

        internal class CustomTagJobTable : Table
        {
            internal CustomTagJobTable() : base("dbo.CustomTagJob")
            {
            }

            internal readonly BigIntColumn JobKey = new BigIntColumn("JobKey");
            internal readonly BigIntColumn TagKey = new BigIntColumn("TagKey");
        }

        internal class DeletedInstanceTable : Table
        {
            internal DeletedInstanceTable() : base("dbo.DeletedInstance")
            {
            }

            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly VarCharColumn SopInstanceUid = new VarCharColumn("SopInstanceUid", 64);
            internal readonly BigIntColumn Watermark = new BigIntColumn("Watermark");
            internal readonly DateTimeOffsetColumn DeletedDateTime = new DateTimeOffsetColumn("DeletedDateTime", 0);
            internal readonly IntColumn RetryCount = new IntColumn("RetryCount");
            internal readonly DateTimeOffsetColumn CleanupAfter = new DateTimeOffsetColumn("CleanupAfter", 0);
            internal readonly Index IXC_DeletedInstance = new Index("IXC_DeletedInstance");
            internal readonly Index IX_DeletedInstance_RetryCount_CleanupAfter = new Index("IX_DeletedInstance_RetryCount_CleanupAfter");
        }

        internal class InstanceTable : Table
        {
            internal InstanceTable() : base("dbo.Instance")
            {
            }

            internal readonly BigIntColumn InstanceKey = new BigIntColumn("InstanceKey");
            internal readonly BigIntColumn SeriesKey = new BigIntColumn("SeriesKey");
            internal readonly BigIntColumn StudyKey = new BigIntColumn("StudyKey");
            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly VarCharColumn SopInstanceUid = new VarCharColumn("SopInstanceUid", 64);
            internal readonly BigIntColumn Watermark = new BigIntColumn("Watermark");
            internal readonly TinyIntColumn Status = new TinyIntColumn("Status");
            internal readonly DateTime2Column LastStatusUpdatedDate = new DateTime2Column("LastStatusUpdatedDate", 7);
            internal readonly DateTime2Column CreatedDate = new DateTime2Column("CreatedDate", 7);
            internal readonly Index IXC_Instance = new Index("IXC_Instance");
            internal readonly Index IX_Instance_StudyInstanceUid_SeriesInstanceUid_SopInstanceUid = new Index("IX_Instance_StudyInstanceUid_SeriesInstanceUid_SopInstanceUid");
            internal readonly Index IX_Instance_StudyInstanceUid_Status = new Index("IX_Instance_StudyInstanceUid_Status");
            internal readonly Index IX_Instance_StudyInstanceUid_SeriesInstanceUid_Status = new Index("IX_Instance_StudyInstanceUid_SeriesInstanceUid_Status");
            internal readonly Index IX_Instance_SopInstanceUid_Status = new Index("IX_Instance_SopInstanceUid_Status");
            internal readonly Index IX_Instance_Watermark = new Index("IX_Instance_Watermark");
            internal readonly Index IX_Instance_SeriesKey_Status = new Index("IX_Instance_SeriesKey_Status");
            internal readonly Index IX_Instance_StudyKey_Status = new Index("IX_Instance_StudyKey_Status");
        }

        internal class JobTable : Table
        {
            internal JobTable() : base("dbo.Job")
            {
            }

            internal readonly BigIntColumn Key = new BigIntColumn("Key");
            internal readonly IntColumn Type = new IntColumn("Type");
            internal readonly NullableBigIntColumn CompletedWatermark = new NullableBigIntColumn("CompletedWatermark");
            internal readonly BigIntColumn MaxWatermark = new BigIntColumn("MaxWatermark");
            internal readonly NullableDateTime2Column HeartBeatTimeStamp = new NullableDateTime2Column("HeartBeatTimeStamp", 7);
            internal readonly IntColumn Status = new IntColumn("Status");
        }

        internal class SeriesTable : Table
        {
            internal SeriesTable() : base("dbo.Series")
            {
            }

            internal readonly BigIntColumn SeriesKey = new BigIntColumn("SeriesKey");
            internal readonly BigIntColumn StudyKey = new BigIntColumn("StudyKey");
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly NullableNVarCharColumn Modality = new NullableNVarCharColumn("Modality", 16);
            internal readonly NullableDateColumn PerformedProcedureStepStartDate = new NullableDateColumn("PerformedProcedureStepStartDate");
            internal readonly Index IXC_Series = new Index("IXC_Series");
            internal readonly Index IX_Series_SeriesKey = new Index("IX_Series_SeriesKey");
            internal readonly Index IX_Series_SeriesInstanceUid = new Index("IX_Series_SeriesInstanceUid");
            internal readonly Index IX_Series_Modality = new Index("IX_Series_Modality");
            internal readonly Index IX_Series_PerformedProcedureStepStartDate = new Index("IX_Series_PerformedProcedureStepStartDate");
        }

        internal class StudyTable : Table
        {
            internal StudyTable() : base("dbo.Study")
            {
            }

            internal readonly BigIntColumn StudyKey = new BigIntColumn("StudyKey");
            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly NVarCharColumn PatientId = new NVarCharColumn("PatientId", 64);
            internal readonly NullableNVarCharColumn PatientName = new NullableNVarCharColumn("PatientName", 200, "SQL_Latin1_General_CP1_CI_AI");
            internal readonly NullableNVarCharColumn ReferringPhysicianName = new NullableNVarCharColumn("ReferringPhysicianName", 200, "SQL_Latin1_General_CP1_CI_AI");
            internal readonly NullableDateColumn StudyDate = new NullableDateColumn("StudyDate");
            internal readonly NullableNVarCharColumn StudyDescription = new NullableNVarCharColumn("StudyDescription", 64);
            internal readonly NullableNVarCharColumn AccessionNumber = new NullableNVarCharColumn("AccessionNumber", 16);
            internal const string PatientNameWords = "PatientNameWords";
            internal readonly Index IXC_Study = new Index("IXC_Study");
            internal readonly Index IX_Study_StudyInstanceUid = new Index("IX_Study_StudyInstanceUid");
            internal readonly Index IX_Study_PatientId = new Index("IX_Study_PatientId");
            internal readonly Index IX_Study_PatientName = new Index("IX_Study_PatientName");
            internal readonly Index IX_Study_ReferringPhysicianName = new Index("IX_Study_ReferringPhysicianName");
            internal readonly Index IX_Study_StudyDate = new Index("IX_Study_StudyDate");
            internal readonly Index IX_Study_StudyDescription = new Index("IX_Study_StudyDescription");
            internal readonly Index IX_Study_AccessionNumber = new Index("IX_Study_AccessionNumber");
        }

        internal class AcquireCustomTagJobsProcedure : StoredProcedure
        {
            internal AcquireCustomTagJobsProcedure() : base("dbo.AcquireCustomTagJobs")
            {
            }

            private readonly ParameterDefinition<System.Int32> _maxCount = new ParameterDefinition<System.Int32>("@maxCount", global::System.Data.SqlDbType.Int, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int32 maxCount)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.AcquireCustomTagJobs";
                _maxCount.AddParameter(command.Parameters, maxCount);
            }
        }

        internal class AddCustomTagProcedure : StoredProcedure
        {
            internal AddCustomTagProcedure() : base("dbo.AddCustomTag")
            {
            }

            private readonly ParameterDefinition<System.String> _path = new ParameterDefinition<System.String>("@path", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _vr = new ParameterDefinition<System.String>("@vr", global::System.Data.SqlDbType.VarChar, false, 2);
            private readonly ParameterDefinition<System.Byte> _level = new ParameterDefinition<System.Byte>("@level", global::System.Data.SqlDbType.TinyInt, false);
            private readonly ParameterDefinition<System.Byte> _status = new ParameterDefinition<System.Byte>("@status", global::System.Data.SqlDbType.TinyInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.String path, System.String vr, System.Byte level, System.Byte status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.AddCustomTag";
                _path.AddParameter(command.Parameters, path);
                _vr.AddParameter(command.Parameters, vr);
                _level.AddParameter(command.Parameters, level);
                _status.AddParameter(command.Parameters, status);
            }
        }

        internal class AddInstanceProcedure : StoredProcedure
        {
            internal AddInstanceProcedure() : base("dbo.AddInstance")
            {
            }

            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _patientId = new ParameterDefinition<System.String>("@patientId", global::System.Data.SqlDbType.NVarChar, false, 64);
            private readonly ParameterDefinition<System.String> _patientName = new ParameterDefinition<System.String>("@patientName", global::System.Data.SqlDbType.NVarChar, true, 325);
            private readonly ParameterDefinition<System.String> _referringPhysicianName = new ParameterDefinition<System.String>("@referringPhysicianName", global::System.Data.SqlDbType.NVarChar, true, 325);
            private readonly ParameterDefinition<System.Nullable<System.DateTime>> _studyDate = new ParameterDefinition<System.Nullable<System.DateTime>>("@studyDate", global::System.Data.SqlDbType.Date, true);
            private readonly ParameterDefinition<System.String> _studyDescription = new ParameterDefinition<System.String>("@studyDescription", global::System.Data.SqlDbType.NVarChar, true, 64);
            private readonly ParameterDefinition<System.String> _accessionNumber = new ParameterDefinition<System.String>("@accessionNumber", global::System.Data.SqlDbType.NVarChar, true, 64);
            private readonly ParameterDefinition<System.String> _modality = new ParameterDefinition<System.String>("@modality", global::System.Data.SqlDbType.NVarChar, true, 16);
            private readonly ParameterDefinition<System.Nullable<System.DateTime>> _performedProcedureStepStartDate = new ParameterDefinition<System.Nullable<System.DateTime>>("@performedProcedureStepStartDate", global::System.Data.SqlDbType.Date, true);
            private readonly ParameterDefinition<System.Byte> _initialStatus = new ParameterDefinition<System.Byte>("@initialStatus", global::System.Data.SqlDbType.TinyInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid, System.String patientId, System.String patientName, System.String referringPhysicianName, System.Nullable<System.DateTime> studyDate, System.String studyDescription, System.String accessionNumber, System.String modality, System.Nullable<System.DateTime> performedProcedureStepStartDate, System.Byte initialStatus)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.AddInstance";
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
                _patientId.AddParameter(command.Parameters, patientId);
                _patientName.AddParameter(command.Parameters, patientName);
                _referringPhysicianName.AddParameter(command.Parameters, referringPhysicianName);
                _studyDate.AddParameter(command.Parameters, studyDate);
                _studyDescription.AddParameter(command.Parameters, studyDescription);
                _accessionNumber.AddParameter(command.Parameters, accessionNumber);
                _modality.AddParameter(command.Parameters, modality);
                _performedProcedureStepStartDate.AddParameter(command.Parameters, performedProcedureStepStartDate);
                _initialStatus.AddParameter(command.Parameters, initialStatus);
            }
        }

        internal class DeleteCustomTagProcedure : StoredProcedure
        {
            internal DeleteCustomTagProcedure() : base("dbo.DeleteCustomTag")
            {
            }

            private readonly ParameterDefinition<System.Int64> _key = new ParameterDefinition<System.Int64>("@key", global::System.Data.SqlDbType.BigInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int64 key)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.DeleteCustomTag";
                _key.AddParameter(command.Parameters, key);
            }
        }

        internal class DeleteDeletedInstanceProcedure : StoredProcedure
        {
            internal DeleteDeletedInstanceProcedure() : base("dbo.DeleteDeletedInstance")
            {
            }

            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.Int64> _watermark = new ParameterDefinition<System.Int64>("@watermark", global::System.Data.SqlDbType.BigInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid, System.Int64 watermark)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.DeleteDeletedInstance";
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
                _watermark.AddParameter(command.Parameters, watermark);
            }
        }

        internal class DeleteInstanceProcedure : StoredProcedure
        {
            internal DeleteInstanceProcedure() : base("dbo.DeleteInstance")
            {
            }

            private readonly ParameterDefinition<System.DateTimeOffset> _cleanupAfter = new ParameterDefinition<System.DateTimeOffset>("@cleanupAfter", global::System.Data.SqlDbType.DateTimeOffset, false, 0);
            private readonly ParameterDefinition<System.Byte> _createdStatus = new ParameterDefinition<System.Byte>("@createdStatus", global::System.Data.SqlDbType.TinyInt, false);
            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);

            public void PopulateCommand(SqlCommandWrapper command, System.DateTimeOffset cleanupAfter, System.Byte createdStatus, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.DeleteInstance";
                _cleanupAfter.AddParameter(command.Parameters, cleanupAfter);
                _createdStatus.AddParameter(command.Parameters, createdStatus);
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
            }
        }

        internal class GetChangeFeedProcedure : StoredProcedure
        {
            internal GetChangeFeedProcedure() : base("dbo.GetChangeFeed")
            {
            }

            private readonly ParameterDefinition<System.Int32> _limit = new ParameterDefinition<System.Int32>("@limit", global::System.Data.SqlDbType.Int, false);
            private readonly ParameterDefinition<System.Int64> _offset = new ParameterDefinition<System.Int64>("@offset", global::System.Data.SqlDbType.BigInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int32 limit, System.Int64 offset)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetChangeFeed";
                _limit.AddParameter(command.Parameters, limit);
                _offset.AddParameter(command.Parameters, offset);
            }
        }

        internal class GetChangeFeedLatestProcedure : StoredProcedure
        {
            internal GetChangeFeedLatestProcedure() : base("dbo.GetChangeFeedLatest")
            {
            }

            public void PopulateCommand(SqlCommandWrapper command)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetChangeFeedLatest";
            }
        }

        internal class GetCustomTagJobProcedure : StoredProcedure
        {
            internal GetCustomTagJobProcedure() : base("dbo.GetCustomTagJob")
            {
            }

            private readonly ParameterDefinition<System.Int64> _jobKey = new ParameterDefinition<System.Int64>("@jobKey", global::System.Data.SqlDbType.BigInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int64 jobKey)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetCustomTagJob";
                _jobKey.AddParameter(command.Parameters, jobKey);
            }
        }

        internal class GetCustomTagsOnJobProcedure : StoredProcedure
        {
            internal GetCustomTagsOnJobProcedure() : base("dbo.GetCustomTagsOnJob")
            {
            }

            private readonly ParameterDefinition<System.Int64> _jobKey = new ParameterDefinition<System.Int64>("@jobKey", global::System.Data.SqlDbType.BigInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int64 jobKey)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetCustomTagsOnJob";
                _jobKey.AddParameter(command.Parameters, jobKey);
            }
        }

        internal class GetInstanceProcedure : StoredProcedure
        {
            internal GetInstanceProcedure() : base("dbo.GetInstance")
            {
            }

            private readonly ParameterDefinition<System.Byte> _validStatus = new ParameterDefinition<System.Byte>("@validStatus", global::System.Data.SqlDbType.TinyInt, false);
            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);

            public void PopulateCommand(SqlCommandWrapper command, System.Byte validStatus, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetInstance";
                _validStatus.AddParameter(command.Parameters, validStatus);
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
            }
        }

        internal class GetInstancesInThePastProcedure : StoredProcedure
        {
            internal GetInstancesInThePastProcedure() : base("dbo.GetInstancesInThePast")
            {
            }

            private readonly ParameterDefinition<System.Int64> _maxWatermark = new ParameterDefinition<System.Int64>("@maxWatermark", global::System.Data.SqlDbType.BigInt, false);
            private readonly ParameterDefinition<System.Int32> _top = new ParameterDefinition<System.Int32>("@top", global::System.Data.SqlDbType.Int, false);
            private readonly ParameterDefinition<System.Byte> _status = new ParameterDefinition<System.Byte>("@status", global::System.Data.SqlDbType.TinyInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int64 maxWatermark, System.Int32 top, System.Byte status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetInstancesInThePast";
                _maxWatermark.AddParameter(command.Parameters, maxWatermark);
                _top.AddParameter(command.Parameters, top);
                _status.AddParameter(command.Parameters, status);
            }
        }

        internal class GetLatestInstanceProcedure : StoredProcedure
        {
            internal GetLatestInstanceProcedure() : base("dbo.GetLatestInstance")
            {
            }

            public void PopulateCommand(SqlCommandWrapper command)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetLatestInstance";
            }
        }

        internal class IncrementDeletedInstanceRetryProcedure : StoredProcedure
        {
            internal IncrementDeletedInstanceRetryProcedure() : base("dbo.IncrementDeletedInstanceRetry")
            {
            }

            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.Int64> _watermark = new ParameterDefinition<System.Int64>("@watermark", global::System.Data.SqlDbType.BigInt, false);
            private readonly ParameterDefinition<System.DateTimeOffset> _cleanupAfter = new ParameterDefinition<System.DateTimeOffset>("@cleanupAfter", global::System.Data.SqlDbType.DateTimeOffset, false, 0);

            public void PopulateCommand(SqlCommandWrapper command, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid, System.Int64 watermark, System.DateTimeOffset cleanupAfter)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.IncrementDeletedInstanceRetry";
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
                _watermark.AddParameter(command.Parameters, watermark);
                _cleanupAfter.AddParameter(command.Parameters, cleanupAfter);
            }
        }

        internal class RetrieveDeletedInstanceProcedure : StoredProcedure
        {
            internal RetrieveDeletedInstanceProcedure() : base("dbo.RetrieveDeletedInstance")
            {
            }

            private readonly ParameterDefinition<System.Int32> _count = new ParameterDefinition<System.Int32>("@count", global::System.Data.SqlDbType.Int, false);
            private readonly ParameterDefinition<System.Int32> _maxRetries = new ParameterDefinition<System.Int32>("@maxRetries", global::System.Data.SqlDbType.Int, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int32 count, System.Int32 maxRetries)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.RetrieveDeletedInstance";
                _count.AddParameter(command.Parameters, count);
                _maxRetries.AddParameter(command.Parameters, maxRetries);
            }
        }

        internal class UpdateCustomTagStatusProcedure : StoredProcedure
        {
            internal UpdateCustomTagStatusProcedure() : base("dbo.UpdateCustomTagStatus")
            {
            }

            private readonly ParameterDefinition<System.Int64> _key = new ParameterDefinition<System.Int64>("@key", global::System.Data.SqlDbType.BigInt, false);
            private readonly ParameterDefinition<System.Byte> _status = new ParameterDefinition<System.Byte>("@status", global::System.Data.SqlDbType.TinyInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.Int64 key, System.Byte status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.UpdateCustomTagStatus";
                _key.AddParameter(command.Parameters, key);
                _status.AddParameter(command.Parameters, status);
            }
        }

        internal class UpdateInstanceStatusProcedure : StoredProcedure
        {
            internal UpdateInstanceStatusProcedure() : base("dbo.UpdateInstanceStatus")
            {
            }

            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.Int64> _watermark = new ParameterDefinition<System.Int64>("@watermark", global::System.Data.SqlDbType.BigInt, false);
            private readonly ParameterDefinition<System.Byte> _status = new ParameterDefinition<System.Byte>("@status", global::System.Data.SqlDbType.TinyInt, false);

            public void PopulateCommand(SqlCommandWrapper command, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid, System.Int64 watermark, System.Byte status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.UpdateInstanceStatus";
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
                _watermark.AddParameter(command.Parameters, watermark);
                _status.AddParameter(command.Parameters, status);
            }
        }
    }
}