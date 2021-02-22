﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using MediatR;

namespace Microsoft.Health.Dicom.Core.Messages.CustomTag
{
    public class DeleteCustomTagRequest : IRequest<DeleteCustomTagResponse>
    {
        public DeleteCustomTagRequest(string tagPath)
        {
            TagPath = tagPath;
        }

        public string TagPath { get; }
    }
}
