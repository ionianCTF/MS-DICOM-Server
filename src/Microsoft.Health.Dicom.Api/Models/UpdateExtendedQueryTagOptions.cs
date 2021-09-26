﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Health.Dicom.Core.Features.ExtendedQueryTag;

namespace Microsoft.Health.Dicom.Api.Models
{
    public class UpdateExtendedQueryTagOptions : IValidatableObject
    {
        /// <summary>
        /// Gets or sets query status.
        /// </summary>
        [Required]
        public QueryStatus? QueryStatus { get; set; }

        [JsonExtensionData]
        [SuppressMessage("Design", "CA2227:Collection properties should be read only", Justification = "Used by JsonDeserializer to store extension data.")]
        public IDictionary<string, JsonElement> ExtensionData { get; set; }

        public UpdateExtendedQueryTagEntry ToEntry()
        {
            return new UpdateExtendedQueryTagEntry(QueryStatus.Value);
        }

        public string FormatToLog()
        {
            // When there is ExtensionData, request fail for BadRequest, so no need to log.
            return $"{nameof(QueryStatus)}: '{QueryStatus}'";
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExtensionData != null && ExtensionData.Count != 0)
            {
                return ExtensionData.Select(x => new ValidationResult(string.Format(CultureInfo.InvariantCulture, DicomApiResource.UnsupportedField, x.Key), new[] { x.Key }));
            }
            else
            {
                return Array.Empty<ValidationResult>();
            }
        }
    }
}
