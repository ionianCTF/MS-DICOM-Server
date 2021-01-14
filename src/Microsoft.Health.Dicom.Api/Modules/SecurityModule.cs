﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using EnsureThat;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Health.Core.Features.Security;
using Microsoft.Health.Dicom.Api.Configs;
using Microsoft.Health.Dicom.Core.Configs;
using Microsoft.Health.Dicom.Core.Features.Context;
using Microsoft.Health.Dicom.Core.Features.Security;
using Microsoft.Health.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Health.Dicom.Api.Modules
{
    public class SecurityModule : IStartupModule
    {
        private readonly SecurityConfiguration _securityConfiguration;

        public SecurityModule(DicomServerConfiguration dicomServerConfiguration)
        {
            EnsureArg.IsNotNull(dicomServerConfiguration, nameof(dicomServerConfiguration));
            _securityConfiguration = dicomServerConfiguration.Security;
        }

        public void Load(IServiceCollection services)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            if (_securityConfiguration.Enabled)
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = _securityConfiguration.Authentication.Authority;
                    options.RequireHttpsMetadata = true;
                    options.Challenge = $"Bearer authorization_uri=\"{_securityConfiguration.Authentication.Authority}\", resource_id=\"{_securityConfiguration.Authentication.Audience}\", realm=\"{_securityConfiguration.Authentication.Audience}\"";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new[]
                        {
                            _securityConfiguration.Authentication.Audience,
                            GenerateSecondaryAudience(),
                        },
                    };
                });

                services.AddControllers(mvcOptions =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    mvcOptions.Filters.Add(new AuthorizeFilter(policy));
                });
            }

            services.Add<DicomRequestContextAccessor>()
                .Singleton()
                .AsSelf()
                .AsService<IDicomRequestContextAccessor>();

            services.AddSingleton<IClaimsExtractor, PrincipalClaimsExtractor>();
        }

        internal string GenerateSecondaryAudience()
        {
            if (_securityConfiguration.Authentication.Audience.EndsWith('/'))
            {
                return _securityConfiguration.Authentication.Audience.TrimEnd('/');
            }

            return _securityConfiguration.Authentication.Audience + '/';
        }
    }
}
