﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.Dicom.Core.Configs
{
    public class DicomServerConfiguration
    {
        public FeatureConfiguration Features { get; } = new FeatureConfiguration();

        public SecurityConfiguration Security { get; } = new SecurityConfiguration();
    }
}
