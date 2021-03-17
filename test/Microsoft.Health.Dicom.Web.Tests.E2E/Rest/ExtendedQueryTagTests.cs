﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dicom;
using EnsureThat;
using Microsoft.Health.Dicom.Client;
using Microsoft.Health.Dicom.Client.Models;
using Microsoft.Health.Dicom.Core.Extensions;
using Microsoft.Health.Dicom.Tests.Common;
using Microsoft.Health.Dicom.Web.Tests.E2E.Comparers;
using Xunit;

namespace Microsoft.Health.Dicom.Web.Tests.E2E.Rest
{
    public class ExtendedQueryTagTests : IClassFixture<HttpIntegrationTestFixture<Startup>>
    {
        private readonly IDicomWebClient _client;

        public ExtendedQueryTagTests(HttpIntegrationTestFixture<Startup> fixture)
        {
            EnsureArg.IsNotNull(fixture, nameof(fixture));
            EnsureArg.IsNotNull(fixture.Client, nameof(fixture.Client));
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenValidExtendedQueryTags_WhenGoThroughEndToEndScenario_ThenShouldSucceed()
        {
            // Prepare 3 extended query tags.
            // One is private tag on Instance level
            DicomTag privateTag = new DicomTag(0x0407, 0x1001, "PrivateCreator1");
            ExtendedQueryTag privateQueryTag = new ExtendedQueryTag { Path = privateTag.GetPath(), VR = DicomVRCode.SS, Level = ExtendedQueryTagLevel.Instance };

            // One is standard tag on Series level
            DicomTag standardTagSeries = DicomTag.ManufacturerModelName;
            ExtendedQueryTag standardTagSeriesQueryTag = new ExtendedQueryTag { Path = standardTagSeries.GetPath(), VR = standardTagSeries.GetDefaultVR().Code, Level = ExtendedQueryTagLevel.Series };

            // One is standard tag on Study level
            DicomTag standardTagStudy = DicomTag.PatientSex;
            ExtendedQueryTag standardTagStudyQueryTag = new ExtendedQueryTag { Path = standardTagStudy.GetPath(), VR = standardTagStudy.GetDefaultVR().Code, Level = ExtendedQueryTagLevel.Study };

            ExtendedQueryTag[] queryTags = new ExtendedQueryTag[] { privateQueryTag, standardTagSeriesQueryTag, standardTagStudyQueryTag };

            // Create 3 test files on same studyUid.
            string studyUid = TestUidGenerator.Generate();
            string seriesUid1 = TestUidGenerator.Generate();
            string seriesUid2 = TestUidGenerator.Generate();
            string instanceUid1 = TestUidGenerator.Generate();
            string instanceUid2 = TestUidGenerator.Generate();
            string instanceUid3 = TestUidGenerator.Generate();

            // One is on seriesUid1 and instanceUid1
            DicomDataset dataset1 = Samples.CreateRandomInstanceDataset(studyInstanceUid: studyUid, seriesInstanceUid: seriesUid1, sopInstanceUid: instanceUid1);
            dataset1.Add(new DicomSignedShort(privateTag, 1));
            dataset1.Add(standardTagSeries, "ManufacturerModelName1");
            dataset1.Add(standardTagStudy, "0");

            // One is on seriesUid1 and instanceUid2
            DicomDataset dataset2 = Samples.CreateRandomInstanceDataset(studyInstanceUid: studyUid, seriesInstanceUid: seriesUid1, sopInstanceUid: instanceUid2);
            dataset2.Add(new DicomSignedShort(privateTag, 2));
            dataset2.Add(standardTagSeries, "ManufacturerModelName2");
            dataset2.Add(standardTagStudy, "0");

            // One is on seriesUid2 and instanceUid3
            DicomDataset dataset3 = Samples.CreateRandomInstanceDataset(studyInstanceUid: studyUid, seriesInstanceUid: seriesUid2, sopInstanceUid: instanceUid3);
            dataset3.Add(new DicomSignedShort(privateTag, 3));
            dataset3.Add(standardTagSeries, "ManufacturerModelName3");
            dataset3.Add(standardTagStudy, "1");
            try
            {
                // Add extended query tags

                await _client.AddExtendedQueryTagAsync(queryTags);
                try
                {
                    foreach (var queryTag in queryTags)
                    {
                        ExtendedQueryTag returnTag = await (await _client.GetExtendedQueryTagAsync(queryTag.Path)).GetValueAsync();
                        Assert.Equal(queryTag, returnTag, new ExtendedQueryTagEqualityComparer(true));
                    }

                    // Upload test files
                    IEnumerable<DicomFile> dicomFiles = new DicomDataset[] { dataset1, dataset2, dataset3 }.Select(dataset => new DicomFile(dataset));

                    await _client.StoreAsync(dicomFiles, studyInstanceUid: string.Empty, cancellationToken: default);

                    // Query on instance for private tag
                    DicomWebAsyncEnumerableResponse<DicomDataset> queryInstanceResponse = await _client.QueryAsync($"/instances?{privateTag.GetPath()}=3", cancellationToken: default);
                    DicomDataset[] instanceResult = await queryInstanceResponse.ToArrayAsync();
                    Assert.Single(instanceResult);
                    Assert.Equal(instanceUid3, instanceResult[0].GetSingleValue<string>(DicomTag.SOPInstanceUID));

                    // Query on series for standardTagSeries
                    DicomWebAsyncEnumerableResponse<DicomDataset> querySeriesResponse = await _client.QueryAsync($"/series?{standardTagSeries.GetPath()}=ManufacturerModelName2", cancellationToken: default);
                    DicomDataset[] seriesResult = await querySeriesResponse.ToArrayAsync();
                    Assert.Single(seriesResult);
                    Assert.Equal(seriesUid1, seriesResult[0].GetSingleValue<string>(DicomTag.SeriesInstanceUID));

                    // Query on study for standardTagStudy
                    DicomWebAsyncEnumerableResponse<DicomDataset> queryStudyResponse = await _client.QueryAsync($"/studies?{standardTagStudy.GetPath()}=1", cancellationToken: default);
                    DicomDataset[] studyResult = await queryStudyResponse.ToArrayAsync();
                    Assert.Single(studyResult);
                    Assert.Equal(studyUid, seriesResult[0].GetSingleValue<string>(DicomTag.StudyInstanceUID));
                }
                finally
                {
                    await _client.DeleteStudyAsync(studyUid);
                }
            }
            finally
            {
                // Cleanup extended query tags, also verify GetCustomTagsAsync.
                var responseQueryTags = await (await _client.GetExtendedQueryTagsAsync()).GetValueAsync();
                foreach (var rTag in responseQueryTags)
                {
                    if (queryTags.Any(tag => tag.Path == rTag.Path))
                    {
                        await _client.DeleteExtendedQueryTagAsync(rTag.Path);
                    }
                }
            }
        }
    }
}
