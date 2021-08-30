﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using EnsureThat;
using Microsoft.Health.Dicom.Core.Features.Validation.Errors;

namespace Microsoft.Health.Dicom.Core.Exceptions
{
    public class InvalidIdentifierException : DicomElementValidationException
    {
        public InvalidIdentifierException(UidIsInValidError error)
            : base(EnsureArg.IsNotNull(error, nameof(error)))
        {
        }
    }
}
