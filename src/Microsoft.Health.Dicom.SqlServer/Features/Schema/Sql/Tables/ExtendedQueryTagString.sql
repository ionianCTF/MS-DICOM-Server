﻿/*************************************************************
    Extended Query Tag Data Table for VR Types mapping to String
    Note: Watermark is primarily used while re-indexing to determine which TagValue is latest.
            For example, with multiple instances in a series, while indexing a series level tag,
            the Watermark is used to ensure that if there are different values between instances,
            the value on the instance with the highest watermark wins.

            This table includes tags used by both Image SOP instances and UPS SOP instances, which are distinguished by resource type column.
            - ResourceType = 0 means Image SOP instance, ResourceType = 1 means UPS SOP instance.
            - SopInstanceKey1 refers to either StudyKey or WorkItemKey.
            - SopInstanceKey2 refers to SeriesKey if ResourceType is Image else NULL.
            - SopInstanceKey3 refers to InstanceKey if ResourceType is Image else NULL.
**************************************************************/
CREATE TABLE dbo.ExtendedQueryTagString (
    TagKey                  INT                  NOT NULL,              --PK
    TagValue                NVARCHAR(64)         NOT NULL,
    SopInstanceKey1         BIGINT               NOT NULL,              --FK
    SopInstanceKey2         BIGINT               NULL,                  --FK
    SopInstanceKey3         BIGINT               NULL,                  --FK
    Watermark               BIGINT               NOT NULL,
    PartitionKey            INT                  NOT NULL DEFAULT 1,    --FK
    ResourceType            TINYINT              NOT NULL DEFAULT 0 
) WITH (DATA_COMPRESSION = PAGE)

-- Used in QIDO
CREATE UNIQUE CLUSTERED INDEX IXC_ExtendedQueryTagString ON dbo.ExtendedQueryTagString
(
    PartitionKey,
    ResourceType,
    TagKey,
    TagValue,
    SopInstanceKey1,
    SopInstanceKey2,
    SopInstanceKey3
)

-- Used in IIndexInstanceCore
CREATE NONCLUSTERED INDEX IX_ExtendedQueryTagString_PartitionKey_TagKey_ResourceType_SopInstanceKey1_SopInstanceKey2_SopInstanceKey3 on dbo.ExtendedQueryTagString
(
    PartitionKey,
    ResourceType,
    TagKey,
    SopInstanceKey1,
    SopInstanceKey2,
    SopInstanceKey3
)
INCLUDE
(
    Watermark
)
WITH (DATA_COMPRESSION = PAGE)

-- Used in DeleteInstance
CREATE NONCLUSTERED INDEX IX_ExtendedQueryTagString_PartitionKey_ResourceType_SopInstanceKey1_SopInstanceKey2_SopInstanceKey3 on dbo.ExtendedQueryTagString
(
    PartitionKey,
    ResourceType,
    SopInstanceKey1,
    SopInstanceKey2,
    SopInstanceKey3
)
WITH (DATA_COMPRESSION = PAGE)
