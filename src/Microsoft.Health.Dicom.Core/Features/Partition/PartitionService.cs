﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Features.Partition;
using Microsoft.Health.Dicom.Core.Messages.Partition;

namespace Microsoft.Health.Dicom.Core.Features.ChangeFeed
{
    public class PartitionService : IPartitionService
    {
        private readonly IPartitionStore _partitionStore;

        public PartitionService(IPartitionStore partitionStore)
        {
            EnsureArg.IsNotNull(partitionStore, nameof(partitionStore));

            _partitionStore = partitionStore;
        }

        public async Task<AddPartitionResponse> AddPartitionAsync(string partitionName, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotNull(partitionName, nameof(partitionName));

            var partitionEntry = await _partitionStore.AddPartitionAsync(partitionName, cancellationToken);
            return new AddPartitionResponse(partitionEntry);
        }

        public async Task<GetPartitionResponse> GetPartitionAsync(string partitionName, CancellationToken cancellationToken = default)
        {
            EnsureArg.IsNotNull(partitionName, nameof(partitionName));

            var partitionEntry = await _partitionStore.GetPartitionAsync(partitionName, cancellationToken);
            return new GetPartitionResponse(partitionEntry);
        }

        public async Task<GetPartitionsResponse> GetPartitionsAsync(CancellationToken cancellationToken = default)
        {
            var partitions = await _partitionStore.GetPartitionsAsync(cancellationToken);
            return new GetPartitionsResponse(partitions.ToList());
        }
    }
}
