﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using FellowOakDicom;
using FellowOakDicom.IO.Buffer;
using Microsoft.Health.Dicom.Core.Exceptions;

namespace Microsoft.Health.Dicom.Core.Features.Validation;

internal class DateValidation : StringElementValidation
{
    private const string DateFormatDA = "yyyyMMdd";

    protected override void ValidateStringElement(string name, string value, DicomVR vr, IByteBuffer buffer)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (!DateTime.TryParseExact(value, DateFormatDA, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _))
        {
            throw new ElementValidationException(name, DicomVR.DA, ValidationErrorCode.DateIsInvalid);
        }
    }
}
