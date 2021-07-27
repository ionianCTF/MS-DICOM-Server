﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Health.Blob.Configs;
using Microsoft.Health.Dicom.Functions.Configuration;
using Microsoft.Health.SqlServer.Configs;
using Microsoft.IO;
using Newtonsoft.Json.Converters;

namespace Microsoft.Health.Dicom.Functions.Registration
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFunctionsOptions<T>(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName)
            where T : class
        {
            EnsureArg.IsNotNull(services, nameof(services));
            EnsureArg.IsNotNull(configuration, nameof(configuration));
            EnsureArg.IsNotEmptyOrWhiteSpace(sectionName, nameof(sectionName));

            services.Configure<T>(configuration
                .GetSection(DicomFunctionsConfiguration.SectionName)
                .GetSection(sectionName));

            return services;
        }

        public static IServiceCollection AddRecyclableMemoryStreamManager(this IServiceCollection services, Func<RecyclableMemoryStreamManager> factory = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            factory ??= () => new RecyclableMemoryStreamManager();

            services.TryAddSingleton(sp => factory());

            return services;
        }

        public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            EnsureArg.IsNotNull(configuration, nameof(configuration));

            IConfigurationSection blobSection = configuration.GetSection(BlobDataStoreConfiguration.SectionName);
            new DicomFunctionsBuilder(services)
                .AddSqlServer(c => configuration.GetSection(SqlServerDataStoreConfiguration.SectionName).Bind(c))
                .AddMetadataStorageDataStore(
                    blobSection.GetSection(DicomBlobContainerConfiguration.SectionName).Get<DicomBlobContainerConfiguration>().Metadata,
                    c => blobSection.Bind(c));

            return services;
        }

        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            services
                .AddMvcCore()
                .AddNewtonsoftJson(x => x.SerializerSettings.Converters
                    .Add(new StringEnumConverter()));

            return services;
        }
    }
}
