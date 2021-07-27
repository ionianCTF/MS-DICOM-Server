﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Dicom;
using Dicom.IO.Buffer;
using EnsureThat;
using Microsoft.Health.Dicom.Core.Extensions;

namespace Microsoft.Health.Dicom.Core.Features.Validation
{
    public partial class DicomElementMinimumValidator : IDicomElementMinimumValidator
    {
        private static readonly Regex ValidIdentifierCharactersFormat = new Regex("^[0-9\\.]*$", RegexOptions.Compiled);

        private const string DateFormatDA = "yyyyMMdd";

        private static readonly HashSet<DicomVR> BinaryVRs = new HashSet<DicomVR>()
        {
             DicomVR.FL, DicomVR.FD, DicomVR.SL, DicomVR.SS,  DicomVR.UL,  DicomVR.US
        };

        private static readonly IReadOnlyDictionary<DicomVR, (int MaxLength, ValidationErrorCode ErrorCode)> MaxLengthValidations = new Dictionary<DicomVR, (int, ValidationErrorCode)>()
        {
            { DicomVR.AE, (16, ValidationErrorCode.InvalidAEExceedMaxLength) },
            { DicomVR.DS, (16, ValidationErrorCode.InvalidDSExceedMaxLength) },
            { DicomVR.IS, (12, ValidationErrorCode.InvalidISExceedMaxLength) },
            { DicomVR.CS, (16, ValidationErrorCode.InvalidCSExceedMaxLength) },
            { DicomVR.SH, (16, ValidationErrorCode.InvalidSHTooLong) },
            { DicomVR.LO, (64, ValidationErrorCode.InvalidLOTooLong) },
        };

        private static readonly IReadOnlyDictionary<DicomVR, (int RequiredLength, ValidationErrorCode ErrorCode)> RequiredLengthValidations = new Dictionary<DicomVR, (int, ValidationErrorCode)>()
        {
            { DicomVR.AS, (4, ValidationErrorCode.InvalidASNotRequiredLength) },
            { DicomVR.FL, (4, ValidationErrorCode.InvalidFLNotRequiredLength) },
            { DicomVR.FD, (8, ValidationErrorCode.InvalidFDNotRequiredLength) },
            { DicomVR.SL, (4, ValidationErrorCode.InvalidSLNotRequiredLength) },
            { DicomVR.SS, (2, ValidationErrorCode.InvalidSSNotRequiredLength) },
            { DicomVR.UL, (4, ValidationErrorCode.InvalidULNotRequiredLength) },
            { DicomVR.US, (2, ValidationErrorCode.InvalidUSNotRequiredLength) },
        };

        private static readonly IReadOnlyDictionary<DicomVR, Action<DicomElement>> AdditionalValidations = new Dictionary<DicomVR, Action<DicomElement>>()
        {
            {DicomVR.DA, ValidateDA },
            {DicomVR.LO, ValidateLO },
            {DicomVR.PN, ValidatePN },
            {DicomVR.UI, ValidateUI }
        };

        public static void Validate(DicomElement dicomElement)
        {
            EnsureArg.IsNotNull(dicomElement, nameof(dicomElement));
            DicomVR vr = dicomElement.ValueRepresentation;

            // Validate MaxLength
            if (MaxLengthValidations.ContainsKey(vr))
            {
                ValidateLengthNotExceed(vr, dicomElement.Tag.GetFriendlyName(), dicomElement.Get<string>());
            }

            // Validate Required Length
            if (RequiredLengthValidations.ContainsKey(vr))
            {
                if (BinaryVRs.Contains(vr))
                {
                    ValidateByteBufferLengthIsRequired(vr, dicomElement.Tag.ToString(), dicomElement.Buffer);
                }
                else
                {
                    ValidateStringLengthIsRequired(vr, dicomElement.Tag.ToString(), dicomElement.Get<string>());
                }
            }

            // Other validation
            if (AdditionalValidations.ContainsKey(vr))
            {
                AdditionalValidations[vr].Invoke(dicomElement);
            }

        }

        private static void ValidateDA(DicomElement dicomElement)
        {
            string value = dicomElement.Get<string>();
            string name = dicomElement.Tag.ToString();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (!DateTime.TryParseExact(value, DateFormatDA, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _))
            {
                throw new DicomStringElementValidationException(ValidationErrorCode.InvalidDA, name, value, DicomVR.DA, DicomCoreResource.ValueIsInvalidDate);
            }
        }

        private static void ValidateLO(DicomElement dicomElement)
        {
            string value = dicomElement.Get<string>();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (value.Contains("\\", StringComparison.OrdinalIgnoreCase) || value.ToCharArray().Any(IsControlExceptESC))
            {
                throw new DicomStringElementValidationException(ValidationErrorCode.InvalidLOContainsInvalidCharacters, dicomElement.Tag.ToString(), value, DicomVR.LO, DicomCoreResource.ValueContainsInvalidCharacter);
            }
        }

        // probably can dial down the validation here
        private static void ValidatePN(DicomElement dicomElement)
        {
            string value = dicomElement.Get<string>();
            string name = dicomElement.Tag.GetFriendlyName();
            if (string.IsNullOrEmpty(value))
            {
                // empty values allowed
                return;
            }

            var groups = value.Split('=');
            if (groups.Length > 3)
            {
                throw new DicomStringElementValidationException(ValidationErrorCode.InvalidPNTooManyGroups, name, value, DicomVR.PN, "value contains too many groups");
            }

            foreach (var group in groups)
            {
                if (group.Length > 64)
                {
                    throw new DicomStringElementValidationException(ValidationErrorCode.InvalidPNGroupIsTooLong, name, value, DicomVR.PN, "value exceeds maximum length of 64 characters");
                }

                if (group.ToCharArray().Any(IsControlExceptESC))
                {
                    throw new DicomStringElementValidationException(ValidationErrorCode.InvalidPNGroupContainsInvalidCharacters, name, value, DicomVR.PN, "value contains invalid control character");
                }
            }

            var groupcomponents = groups.Select(group => group.Split('^').Length);
            if (groupcomponents.Any(l => l > 5))
            {
                throw new DicomStringElementValidationException(ValidationErrorCode.InvalidPNTooManyComponents, name, value, DicomVR.PN, "value contains too many components");
            }
        }

        internal static void ValidateSH(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (value.Length > 16)
            {
                throw new DicomStringElementValidationException(ValidationErrorCode.InvalidSHTooLong, name, value, DicomVR.SH, DicomCoreResource.ValueLengthExceeds16Characters);
            }
        }

        internal static void ValidateUI(DicomElement dicomElement)
        {
            string value = dicomElement.Get<string>();
            string name = dicomElement.Tag.GetFriendlyName();
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            // trailling spaces are allowed
            value = value.TrimEnd(' ');

            if (value.Length > 64)
            {
                // UI value is validated in other cases like params for WADO, DELETE. So keeping the exception specific.
                throw new InvalidIdentifierException(value, name);
            }

            if (!ValidIdentifierCharactersFormat.IsMatch(value))
            {
                throw new InvalidIdentifierException(value, name);
            }
        }

        private static void ValidateLengthNotExceed(DicomVR dicomVR, string name, string value)
        {
            (int maxLength, ValidationErrorCode errorCode) = MaxLengthValidations[dicomVR];
            if (value?.Length > maxLength)
            {
                throw new DicomStringElementValidationException(
                    errorCode,
                    name,
                    value,
                    dicomVR,
                    string.Format(CultureInfo.InvariantCulture, DicomCoreResource.ValueLengthBelowMinLength, maxLength));
            }
        }

        private static void ValidateByteBufferLengthIsRequired(DicomVR dicomVR, string name, IByteBuffer value)
        {
            (int requiredLength, ValidationErrorCode errorCode) = RequiredLengthValidations[dicomVR];
            if (value?.Size != requiredLength)
            {
                throw new DicomElementValidationException(
                    errorCode,
                    name,
                    dicomVR,
                    string.Format(CultureInfo.InvariantCulture, DicomCoreResource.ValueLengthIsNotRequiredLength, requiredLength));
            }
        }

        private static void ValidateStringLengthIsRequired(DicomVR dicomVR, string name, string value)
        {
            (int requiredLength, ValidationErrorCode errorCode) = RequiredLengthValidations[dicomVR];
            if (value?.Length != requiredLength)
            {
                throw new DicomStringElementValidationException(
                    errorCode,
                    name,
                    value,
                    dicomVR,
                    string.Format(CultureInfo.InvariantCulture, DicomCoreResource.ValueLengthIsNotRequiredLength, requiredLength));
            }
        }

        private static bool IsControlExceptESC(char c)
            => char.IsControl(c) && (c != '\u001b');
    }
}
