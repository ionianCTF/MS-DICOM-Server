﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using Dicom;

namespace Microsoft.Health.Dicom.Core.Features.Validation.Errors
{
    public class NotRequiredLengthError : ElementValidationError
    {
        private readonly int _requiredLength;

        public NotRequiredLengthError(string name, DicomVR vr, string value, int requiredLength) : base(name, vr, value)
        {
            _requiredLength = requiredLength;
        }

        public NotRequiredLengthError(string name, DicomVR vr, int requiredLength) : base(name, vr)
        {
            _requiredLength = requiredLength;
        }

        public override ValidationErrorCode ErrorCode => ValidationErrorCode.NotRequiredLength;

        public override string GetBriefMessage()
        {
            throw new NotImplementedException();
        }

        protected override string GetErrorMessage()
        {
            return string.Format(CultureInfo.InvariantCulture, DicomCoreResource.NotRequiredLengthError, _requiredLength);
        }
    }
}
