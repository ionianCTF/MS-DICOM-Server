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
    using Microsoft.Health.SqlServer.Features.Schema.Model;

    internal class VLatest
    {
        internal readonly static InstanceTable Instance = new InstanceTable();
        internal readonly static SchemaVersionTable SchemaVersion = new SchemaVersionTable();
        internal readonly static SeriesMetadataCoreTable SeriesMetadataCore = new SeriesMetadataCoreTable();
        internal readonly static StudyMetadataCoreTable StudyMetadataCore = new StudyMetadataCoreTable();
        internal readonly static AddInstanceProcedure AddInstance = new AddInstanceProcedure();
        internal readonly static GetInstanceProcedure GetInstance = new GetInstanceProcedure();
        internal readonly static SelectCurrentSchemaVersionProcedure SelectCurrentSchemaVersion = new SelectCurrentSchemaVersionProcedure();
        internal readonly static UpsertSchemaVersionProcedure UpsertSchemaVersion = new UpsertSchemaVersionProcedure();
        internal class InstanceTable : Table
        {
            internal InstanceTable(): base("dbo.Instance")
            {
            }

            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly VarCharColumn SopInstanceUid = new VarCharColumn("SopInstanceUid", 64);
            internal readonly BigIntColumn Watermark = new BigIntColumn("Watermark");
            internal readonly TinyIntColumn Status = new TinyIntColumn("Status");
            internal readonly DateTime2Column LastStatusUpdatedDate = new DateTime2Column("LastStatusUpdatedDate", 7);
            internal readonly DateTime2Column CreatedDate = new DateTime2Column("CreatedDate", 7);
        }

        internal class SchemaVersionTable : Table
        {
            internal SchemaVersionTable(): base("dbo.SchemaVersion")
            {
            }

            internal readonly IntColumn Version = new IntColumn("Version");
            internal readonly VarCharColumn Status = new VarCharColumn("Status", 10);
        }

        internal class SeriesMetadataCoreTable : Table
        {
            internal SeriesMetadataCoreTable(): base("dbo.SeriesMetadataCore")
            {
            }

            internal readonly BigIntColumn StudyId = new BigIntColumn("StudyId");
            internal readonly VarCharColumn SeriesInstanceUid = new VarCharColumn("SeriesInstanceUid", 64);
            internal readonly IntColumn Version = new IntColumn("Version");
            internal readonly NullableNVarCharColumn Modality = new NullableNVarCharColumn("Modality", 16);
            internal readonly NullableDateColumn PerformedProcedureStepStartDate = new NullableDateColumn("PerformedProcedureStepStartDate");
        }

        internal class StudyMetadataCoreTable : Table
        {
            internal StudyMetadataCoreTable(): base("dbo.StudyMetadataCore")
            {
            }

            internal readonly BigIntColumn Id = new BigIntColumn("Id");
            internal readonly VarCharColumn StudyInstanceUid = new VarCharColumn("StudyInstanceUid", 64);
            internal readonly IntColumn Version = new IntColumn("Version");
            internal readonly NVarCharColumn PatientId = new NVarCharColumn("PatientId", 64);
            internal readonly NullableNVarCharColumn PatientName = new NullableNVarCharColumn("PatientName", 325);
            internal readonly NullableNVarCharColumn ReferringPhysicianName = new NullableNVarCharColumn("ReferringPhysicianName", 325);
            internal readonly NullableDateColumn StudyDate = new NullableDateColumn("StudyDate");
            internal readonly NullableNVarCharColumn StudyDescription = new NullableNVarCharColumn("StudyDescription", 64);
            internal readonly NullableNVarCharColumn AccessionNumber = new NullableNVarCharColumn("AccessionNumber", 16);
            internal const string PatientNameWords = "PatientNameWords";
        }

        internal class AddInstanceProcedure : StoredProcedure
        {
            internal AddInstanceProcedure(): base("dbo.AddInstance")
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
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid, System.String patientId, System.String patientName, System.String referringPhysicianName, System.Nullable<System.DateTime> studyDate, System.String studyDescription, System.String accessionNumber, System.String modality, System.Nullable<System.DateTime> performedProcedureStepStartDate, System.Byte initialStatus)
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

        internal class GetInstanceProcedure : StoredProcedure
        {
            internal GetInstanceProcedure(): base("dbo.GetInstance")
            {
            }

            private readonly ParameterDefinition<System.Byte> _invalidStatus = new ParameterDefinition<System.Byte>("@invalidStatus", global::System.Data.SqlDbType.TinyInt, false);
            private readonly ParameterDefinition<System.String> _studyInstanceUid = new ParameterDefinition<System.String>("@studyInstanceUid", global::System.Data.SqlDbType.VarChar, false, 64);
            private readonly ParameterDefinition<System.String> _seriesInstanceUid = new ParameterDefinition<System.String>("@seriesInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);
            private readonly ParameterDefinition<System.String> _sopInstanceUid = new ParameterDefinition<System.String>("@sopInstanceUid", global::System.Data.SqlDbType.VarChar, true, 64);
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Byte invalidStatus, System.String studyInstanceUid, System.String seriesInstanceUid, System.String sopInstanceUid)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetInstance";
                _invalidStatus.AddParameter(command.Parameters, invalidStatus);
                _studyInstanceUid.AddParameter(command.Parameters, studyInstanceUid);
                _seriesInstanceUid.AddParameter(command.Parameters, seriesInstanceUid);
                _sopInstanceUid.AddParameter(command.Parameters, sopInstanceUid);
            }
        }

        internal class SelectCurrentSchemaVersionProcedure : StoredProcedure
        {
            internal SelectCurrentSchemaVersionProcedure(): base("dbo.SelectCurrentSchemaVersion")
            {
            }

            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.SelectCurrentSchemaVersion";
            }
        }

        internal class UpsertSchemaVersionProcedure : StoredProcedure
        {
            internal UpsertSchemaVersionProcedure(): base("dbo.UpsertSchemaVersion")
            {
            }

            private readonly ParameterDefinition<System.Int32> _version = new ParameterDefinition<System.Int32>("@version", global::System.Data.SqlDbType.Int, false);
            private readonly ParameterDefinition<System.String> _status = new ParameterDefinition<System.String>("@status", global::System.Data.SqlDbType.VarChar, false, 10);
            public void PopulateCommand(global::System.Data.SqlClient.SqlCommand command, System.Int32 version, System.String status)
            {
                command.CommandType = global::System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.UpsertSchemaVersion";
                _version.AddParameter(command.Parameters, version);
                _status.AddParameter(command.Parameters, status);
            }
        }
    }
}