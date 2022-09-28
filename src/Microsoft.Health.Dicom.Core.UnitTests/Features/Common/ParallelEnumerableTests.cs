// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Health.Dicom.Core.Features.Common;
using Xunit;

namespace Microsoft.Health.Dicom.Core.UnitTests.Features.Common;

public class ParallelEnumerableTests
{
    [Fact]
    public async Task GivenInvalidInput_WhenSelectingInParallel_ThenThrowArgumentException()
    {
        var input = new List<string> { "1", "2", "3" };

        await Assert.ThrowsAsync<ArgumentNullException>(() => ((List<string>)null).SelectParallel(ParseAsync, new ParallelEnumerationOptions()).ToListAsync().AsTask());
        await Assert.ThrowsAsync<ArgumentNullException>(() => input.SelectParallel((Func<string, CancellationToken, ValueTask<int>>)null, new ParallelEnumerationOptions()).ToListAsync().AsTask());
        await Assert.ThrowsAsync<ArgumentNullException>(() => input.SelectParallel(ParseAsync, null).ToListAsync().AsTask());

        var options = new ParallelEnumerationOptions { MaxBufferedItems = 1, MaxDegreeOfParallelism = -1 };
        await Assert.ThrowsAsync<ArgumentException>(() => input.SelectParallel(ParseAsync, options).ToListAsync().AsTask());

        options = new ParallelEnumerationOptions { MaxBufferedItems = 5, MaxDegreeOfParallelism = 6 };
        await Assert.ThrowsAsync<ArgumentException>(() => input.SelectParallel(ParseAsync, options).ToListAsync().AsTask());
    }

    [Fact]
    public async Task GivenNoElements_WhenSelectingInParallel_ThenReturnEmpty()
    {
        var input = Enumerable.Empty<string>();
        Assert.Empty(await input.SelectParallel(ParseAsync, new ParallelEnumerationOptions()).ToListAsync());
    }

    [Fact]
    public async Task GivenOneElement_WhenSelectingInParallel_ThenReturnOneElement()
    {
        var input = new List<string> { "42" };
        Assert.Equal(42, await input.SelectParallel(ParseAsync, new ParallelEnumerationOptions()).SingleAsync());
    }

    [Fact]
    public async Task GivenFewerThenParallelism_WhenSelectingInParallel_ThenReturnAllResults()
    {
        var input = new List<string> { "1", "2", "3" };
        await AssertValuesAsync(
            input.SelectParallel(ParseAsync, new ParallelEnumerationOptions { MaxDegreeOfParallelism = input.Count + 1 }),
            1, 2, 3);
    }

    [Fact]
    public async Task GivenMoreThenParallelism_WhenSelectingInParallel_ThenReturnAllResults()
    {
        var input = new List<string> { "1", "2", "3", "4", "5" };
        await AssertValuesAsync(
            input.SelectParallel(ParseAsync, new ParallelEnumerationOptions { MaxDegreeOfParallelism = input.Count - 2 }),
            1, 2, 3, 4, 5);
    }

    [Fact]
    public async Task GivenSource_WhenEnumeratingMultipleTimes_ThenReturnSameishResults()
    {
        var input = new List<string> { "1", "2", "3", "4", "5" };
        for (int i = 0; i < 5; i++)
        {
            await AssertValuesAsync(
                input.SelectParallel(ParseAsync, new ParallelEnumerationOptions { MaxDegreeOfParallelism = input.Count - 2 }),
                1, 2, 3, 4, 5);
        }
    }

    [Fact]
    public async Task GivenError_WhenSelectingInParallel_ThenRethrowError()
    {
        var input = new List<string> { "1", "foo", "3" };
        await Assert.ThrowsAsync<FormatException>(
            () => input.SelectParallel(ParseAsync, new ParallelEnumerationOptions()).ToListAsync().AsTask());
    }

    [Fact]
    public async Task GivenCancelledToken_WhenSelectingInParallel_ThenThrowException()
    {
        using var tokenSource = new CancellationTokenSource();

        int count = 0;
        var input = new List<string> { "1", "2", "3", "4", "5" };
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => input
                .SelectParallel(
                    (x, t) =>
                    {
                        if (Interlocked.Increment(ref count) == 4)
                            tokenSource.Cancel();

                        return ParseAsync(x, t);
                    },
                    new ParallelEnumerationOptions { MaxDegreeOfParallelism = 2 },
                    tokenSource.Token)
                .ToListAsync()
                .AsTask());
    }

    [Fact]
    public async Task GivenMaxBuffer_WhenSelectingInParallel_ThenWaitForConsumption()
    {
        const int MaxBuffered = 3;

        int bufferCount = 0;
        var resolved = new ConcurrentDictionary<int, object>(); // Dictionary for key-based lookups
        using var bufferFullEvent = new ManualResetEventSlim(false);

        var input = new List<string> { "1", "2", "3", "4", "5", "6" };
        IAsyncEnumerator<int> results = input
            .SelectParallel(
                async (x, t) =>
                {
                    int result = await ParseAsync(x, t);

                    Assert.True(resolved.TryAdd(result, null));
                    if (Interlocked.Increment(ref bufferCount) == MaxBuffered + 1)
                        bufferFullEvent.Set();

                    return result;
                },
                new ParallelEnumerationOptions
                {
                    MaxBufferedItems = MaxBuffered,
                    MaxDegreeOfParallelism = MaxBuffered - 1,
                })
            .GetAsyncEnumerator();

        var actual = new HashSet<int>();
        Assert.True(await results.MoveNextAsync());
        Assert.True(actual.Add(results.Current));

        // Beginning the enumerable should have triggered the producer, but only up to the buffered max of 3.
        // Let E = { 1, 2, 3 }. Let e be the element of E that was yielded first by the producer.
        // After yielding the element e, the buffered elements shall be the set F = E U { e } U { 4 }.
        // E.g. If e = 1 because 1 was yielded first, then F = { 2, 3, 4 } OR if 3 was yielded first,
        // then { 1, 2, 4 } are buffered and waiting to be read by the consumer
        bufferFullEvent.Wait();

        // So far, 4 elements have been resolved. The 1 that was yielded and the 3 buffered values.
        Assert.Equal(MaxBuffered + 1, resolved.Count);
        Assert.All(resolved.Keys, x => Assert.InRange(x, 1, 4));

        // Wait a bit longer -- nothing more is going to be added
        await Task.Delay(1000);

        Assert.Equal(MaxBuffered + 1, resolved.Count);
        Assert.All(resolved.Keys, x => Assert.InRange(x, 1, 4));

        // Finish enumerating and validate the yielded elements
        while (await results.MoveNextAsync())
        {
            Assert.True(actual.Add(results.Current));
        }

        Assert.Equal(6, actual.Count);
        Assert.All(resolved.Keys, x => Assert.InRange(x, 1, 6));
        Assert.All(actual, x => Assert.InRange(x, 1, 6));
    }

    private static ValueTask<int> ParseAsync(string s, CancellationToken cancellationToken)
        => new ValueTask<int>(Task.Run(() => int.Parse(s, CultureInfo.InvariantCulture), cancellationToken));

    private static async ValueTask AssertValuesAsync<T>(IAsyncEnumerable<T> actual, params T[] expected)
    {
        HashSet<T> set = await actual.ToHashSetAsync();

        Assert.Equal(expected.Length, set.Count);
        foreach (T e in expected)
        {
            Assert.True(set.Remove(e));
        }
    }
}
