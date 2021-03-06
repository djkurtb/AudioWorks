/* Copyright � 2018 Jeremy Herbison

This file is part of AudioWorks.

AudioWorks is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public
License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later
version.

AudioWorks is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
details.

You should have received a copy of the GNU Affero General Public License along with AudioWorks. If not, see
<https://www.gnu.org/licenses/>. */

using System;
using AudioWorks.Common;
using AudioWorks.TestUtilities;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace AudioWorks.Extensibility.Tests
{
    public sealed class SampleBufferTests
    {
        public SampleBufferTests([NotNull] ITestOutputHelper outputHelper)
        {
            LoggerManager.AddSingletonProvider(() => new XunitLoggerProvider()).OutputHelper = outputHelper;
        }

        [Fact(DisplayName = "SampleBuffer.Empty returns an empty SampleBuffer")]
        public void SampleBufferEmptyReturnsEmpty()
        {
            Assert.Equal(0, SampleBuffer.Empty.Frames);
        }

        [Fact(DisplayName = "Constructor (float, interleaved) throws an exception when the number of interleaved samples is invalid")]
        public void ConstructorFloatInterleavedInvalidSamplesThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new float[3], 2));
        }

        [Fact(DisplayName = "Constructor (float, interleaved) throws an exception when the number of channels is too low")]
        public void ConstructorFloatInterleavedChannelsTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 0));
        }

        [Fact(DisplayName = "Constructor (float, interleaved) throws an exception when the number of channels is too high")]
        public void ConstructorFloatInterleavedChannelsTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 3));
        }

        [Fact(DisplayName = "Constructor (int, mono) throws an exception when the bits per sample is too high")]
        public void ConstructorIntMonoBitsPerSampleTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 33));
        }

        [Fact(DisplayName = "Constructor (int, mono) throws an exception when the bits per sample is too low")]
        public void ConstructorIntMonoBitsPerSampleTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 0));
        }

        [Fact(DisplayName = "Constructor (int, stereo) throws an exception when the channels contain different numbers of samples")]
        public void ConstructorIntStereoSamplesDoNotMatchThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new int[1], new int[2], 16));
        }

        [Fact(DisplayName = "Constructor (int, stereo) throws an exception when the bits per sample is too high")]
        public void ConstructorIntStereoBitsPerSampleTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], new int[2], 33));
        }

        [Fact(DisplayName = "Constructor (int, stereo) throws an exception when the bits per sample is too low")]
        public void ConstructorIntStereoBitsPerSampleTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], new int[2], 0));
        }

        [Fact(DisplayName = "Constructor (int, interleaved) throws an exception when the number of interleaved samples is invalid")]
        public void ConstructorIntInterleavedInvalidSamplesThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new int[3], 2, 16));
        }

        [Fact(DisplayName = "Constructor (int, interleaved) throws an exception when the number of channels is too low")]
        public void ConstructorIntInterleavedChannelsTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 0, 16));
        }

        [Fact(DisplayName = "Constructor (int, interleaved) throws an exception when the number of channels is too high")]
        public void ConstructorIntInterleavedChannelsTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 3, 16));
        }

        [Fact(DisplayName = "Constructor (int, interleaved) throws an exception when the bits per sample is too high")]
        public void ConstructorIntIntInterleavedBitsPerSampleTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 2, 33));
        }

        [Fact(DisplayName = "Constructor (int, interleaved) throws an exception when the bits per sample is too low")]
        public void ConstructorIntIntInterleavedBitsPerSampleTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new int[2], 2, 0));
        }

        [Fact(DisplayName = "Constructor (packed, interleaved) throws an exception when the number of interleaved samples is invalid")]
        public void ConstructorPackedInterleavedInvalidSamplesThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new byte[5], 2, 16));
        }

        [Fact(DisplayName = "Constructor (packed, interleaved) throws an exception when the number of channels is too low")]
        public void ConstructorPackedInterleavedChannelsTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new byte[4], 0, 16));
        }

        [Fact(DisplayName = "Constructor (packed, interleaved) throws an exception when the number of channels is too high")]
        public void ConstructorPackedInterleavedChannelsTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new byte[4], 3, 16));
        }

        [Fact(DisplayName = "Constructor (packed, interleaved) throws an exception when the bits per sample is too high")]
        public void ConstructorPackedIntInterleavedBitsPerSampleTooHighThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new byte[4], 2, 33));
        }

        [Fact(DisplayName = "Constructor (packed, interleaved) throws an exception when the bits per sample is too low")]
        public void ConstructorPackedIntInterleavedBitsPerSampleTooLowThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new byte[4], 2, 0));
        }

        [Fact(DisplayName = "CopyTo (float, mono) throws an exception when the object has been disposed")]
        public void CopyToFloatMonoThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[1], 1);
            var outSamples = new float[1];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                samples.CopyTo(outSamples));
        }

        [Fact(DisplayName = "CopyTo (float, mono) throws an exception when the samples are in stereo")]
        public void CopyToFloatMonoThrowsExceptionWhenStereo()
        {
            var outSamples = new float[2];

            Assert.Throws<InvalidOperationException>(() =>
                new SampleBuffer(new float[2], 2).CopyTo(outSamples));
        }

        [Fact(DisplayName = "CopyTo (float, stereo) throws an exception when the object has been disposed")]
        public void CopyToFloatStereoThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[2], 2);
            var leftOutSamples = new float[1];
            var rightOutSamples = new float[1];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                samples.CopyTo(leftOutSamples, rightOutSamples));
        }

        [Fact(DisplayName = "CopyTo (float, stereo) throws an exception when the samples are in mono")]
        public void CopyToFloatStereoThrowsExceptionWhenMono()
        {
            var leftOutSamples = new float[1];
            var rightOutSamples = new float[1];

            Assert.Throws<InvalidOperationException>(() =>
                new SampleBuffer(new float[2], 1).CopyTo(leftOutSamples, rightOutSamples));
        }

        [Fact(DisplayName = "CopyTo (float, stereo) returns the expected results for interleaved input")]
        public void CopyToFloatStereoReturnsExpectedResultsForInterleaved()
        {
            var leftOutSamples = new float[2];
            var rightOutSamples = new float[2];

            using (var samples = new SampleBuffer(new[] { 1f, 2f, 1f, 2f }, 2))
                samples.CopyTo(leftOutSamples, rightOutSamples);

            Assert.All(leftOutSamples, value => Assert.Equal(1f, value));
            Assert.All(rightOutSamples, value => Assert.Equal(2f, value));
        }

        [Fact(DisplayName = "CopyTo (int, stereo) throws an exception when the object has been disposed")]
        public void CopyToIntStereoThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[2], 2);
            var leftOutSamples = new int[1];
            var rightOutSamples = new int[1];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() => samples.CopyTo(leftOutSamples, rightOutSamples, 16));
        }

        [Fact(DisplayName = "CopyTo (int, stereo) throws an exception when the samples are in mono")]
        public void CopyToIntStereoThrowsExceptionWhenMono()
        {
            var leftOutSamples = new int[1];
            var rightOutSamples = new int[1];

            Assert.Throws<InvalidOperationException>(() =>
                new SampleBuffer(new float[2], 1).CopyTo(leftOutSamples, rightOutSamples, 16));
        }

        [Fact(DisplayName = "CopyTo (int, stereo) returns the expected results for interleaved input")]
        public void CopyToIntStereoReturnsExpectedResultsForInterleaved()
        {
            var leftOutSamples = new int[2];
            var rightOutSamples = new int[2];

            using (var samples = new SampleBuffer(new[] { 1, 2, 1, 2 }, 2, 16))
                samples.CopyTo(leftOutSamples, rightOutSamples, 16);

            Assert.All(leftOutSamples, value => Assert.Equal(1, value));
            Assert.All(rightOutSamples, value => Assert.Equal(2, value));
        }

        [Fact(DisplayName = "CopyTo (int, stereo) returns the expected results for stereo input")]
        public void CopyToIntStereoReturnsExpectedResultsForStereo()
        {
            var leftOutSamples = new int[2];
            var rightOutSamples = new int[2];

            using (var samples = new SampleBuffer(new[] { 1, 1 }, new[] { 2, 2 }, 16))
                samples.CopyTo(leftOutSamples, rightOutSamples, 16);

            Assert.All(leftOutSamples, value => Assert.Equal(1, value));
            Assert.All(rightOutSamples, value => Assert.Equal(2, value));
        }

        [Fact(DisplayName = "CopyToInterleaved (float) throws an exception when the object has been disposed")]
        public void CopyToInterleavedFloatThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[2], 2);
            var outSamples = new float[2];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                samples.CopyToInterleaved(outSamples));
        }

        [Fact(DisplayName = "CopyToInterleaved (float) throws an exception when the destination it too short")]
        public void CopyToInterleavedFloatThrowsExceptionWhenDestinationTooShort()
        {
            var outSamples = new float[1];

            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples));
        }

        [Fact(DisplayName = "CopyToInterleaved (float) returns the expected results for interleaved input")]
        public void CopyToInterleavedFloatReturnsExpectedResultsForInterleaved()
        {
            var inSamples = new[] { 1f, 2f, 1f, 2f };
            var outSamples = new float[4];

            using (var samples = new SampleBuffer(inSamples, 2))
                samples.CopyToInterleaved(outSamples);

            Assert.Equal(inSamples, outSamples);
        }

        [Fact(DisplayName = "CopyToInterleaved (int) throws an exception when the object has been disposed")]
        public void CopyToInterleavedIntThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[2], 2);
            var outSamples = new int[2];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                samples.CopyToInterleaved(outSamples, 16));
        }

        [Fact(DisplayName = "CopyToInterleaved (int) throws an exception when the destination it too short")]
        public void CopyToInterleavedIntThrowsExceptionWhenDestinationTooShort()
        {
            var outSamples = new int[1];

            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 16));
        }

        [Fact(DisplayName = "CopyToInterleaved (int) throws an exception when bitsPerSample is too high")]
        public void CopyToInterleavedIntThrowsExceptionWhenBitsPerSampleTooHigh()
        {
            var outSamples = new int[2];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 33));
        }

        [Fact(DisplayName = "CopyToInterleaved (int) throws an exception when bitsPerSample is too low")]
        public void CopyToInterleavedIntThrowsExceptionWhenBitsPerSampleTooLow()
        {
            var outSamples = new int[2];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 0));
        }

        [Fact(DisplayName = "CopyToInterleaved (int) returns the expected results for interleaved input")]
        public void CopyToInterleavedIntReturnsExpectedResultsForInterleaved()
        {
            var inSamples = new[] { 1, 2, 1, 2 };
            var outSamples = new int[4];

            using (var samples = new SampleBuffer(inSamples, 16))
                samples.CopyToInterleaved(outSamples, 16);

            Assert.Equal(inSamples, outSamples);
        }

        [Fact(DisplayName = "CopyToInterleaved (int) returns the expected results for stereo input")]
        public void CopyToInterleavedIntReturnsExpectedResultsForStereo()
        {
            var outSamples = new int[4];

            using (var samples = new SampleBuffer(new[] { 1, 1 }, new[] { 2, 2 }, 16))
                samples.CopyToInterleaved(outSamples, 16);

            Assert.Equal(new[] { 1, 2, 1, 2 }, outSamples);
        }

        [Fact(DisplayName = "CopyToInterleaved (packed) throws an exception when the object has been disposed")]
        public void CopyToInterleavedPackedThrowsExceptionWhenDisposed()
        {
            var samples = new SampleBuffer(new float[2], 2);
            var outSamples = new byte[4];

            samples.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                samples.CopyToInterleaved(outSamples, 16));
        }

        [Fact(DisplayName = "CopyToInterleaved (packed) throws an exception when the destination it too short")]
        public void CopyToInterleavedPackedThrowsExceptionWhenDestinationTooShort()
        {
            var outSamples = new byte[2];

            Assert.Throws<ArgumentException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 16));
        }

        [Fact(DisplayName = "CopyToInterleaved (packed) throws an exception when bitsPerSample is too high")]
        public void CopyToInterleavedPackedThrowsExceptionWhenBitsPerSampleTooHigh()
        {
            var outSamples = new byte[8];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 33));
        }

        [Fact(DisplayName = "CopyToInterleaved (packed) throws an exception when bitsPerSample is too low")]
        public void CopyToInterleavedPackedThrowsExceptionWhenBitsPerSampleTooLow()
        {
            var outSamples = new byte[8];

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SampleBuffer(new float[2], 2).CopyToInterleaved(outSamples, 0));
        }

        [Fact(DisplayName = "CopyToInterleaved (packed) returns the expected results for interleaved input")]
        public void CopyToInterleavedPackedReturnsExpectedResultsForInterleaved()
        {
            var inSamples = new byte[] { 1, 2, 1, 2 };
            var outSamples = new byte[4];

            using (var samples = new SampleBuffer(inSamples, 2, 16))
                samples.CopyToInterleaved(outSamples, 16);

            Assert.Equal(inSamples, outSamples);
        }

        [Fact(DisplayName = "32-bit integers don't overflow")]
        public void IntegersDoNotOverflow()
        {
            var inSamples = new[] { int.MaxValue, int.MinValue };
            var outSamples = new int[inSamples.Length];

            using (var samples = new SampleBuffer(inSamples, 32))
                samples.CopyToInterleaved(outSamples, 32);

            Assert.True(true);
        }

        [Theory(DisplayName = "Integers stay in range after conversion to float")]
        [InlineData(1, -1, 0)]
        [InlineData(2, -2, 1)]
        [InlineData(3, -4, 3)]
        [InlineData(4, -8, 7)]
        [InlineData(5, -0x10, 0xF)]
        [InlineData(6, -0x20, 0x1F)]
        [InlineData(7, -0x40, 0x3F)]
        [InlineData(8, -0x80, 0x7F)]
        [InlineData(9, -0x100, 0xFF)]
        [InlineData(10, -0x200, 0x1FF)]
        [InlineData(11, -0x400, 0x3FF)]
        [InlineData(12, -0x800, 0x7FF)]
        [InlineData(13, -0x1000, 0xFFF)]
        [InlineData(14, -0x2000, 0x1FFF)]
        [InlineData(15, -0x4000, 0x3FFF)]
        [InlineData(16, -0x8000, 0x7FFF)]
        [InlineData(17, -0x1_0000, 0xFFFF)]
        [InlineData(18, -0x2_0000, 0x1_FFFF)]
        [InlineData(19, -0x4_0000, 0x3_FFFF)]
        [InlineData(20, -0x8_0000, 0x7_FFFF)]
        [InlineData(21, -0x10_0000, 0xF_FFFF)]
        [InlineData(22, -0x20_0000, 0x1F_FFFF)]
        [InlineData(23, -0x40_0000, 0x3F_FFFF)]
        [InlineData(24, -0x80_0000, 0x7F_FFFF)]
        [InlineData(25, -0x100_0000, 0xFF_FFFF)]
        [InlineData(26, -0x200_0000, 0x1FF_FFFF)]
        [InlineData(27, -0x400_0000, 0x3FF_FFFF)]
        [InlineData(28, -0x800_0000, 0x7FF_FFFF)]
        [InlineData(29, -0x1000_0000, 0xFFF_FFFF)]
        [InlineData(30, -0x2000_0000, 0x1FFF_FFFF)]
        [InlineData(31, -0x4000_0000, 0x3FFF_FFFF)]
        [InlineData(32, -0x8000_0000, 0x7FFF_FFFF)]
        public void SamplesStayInRange(int bitsPerSample, int minValue, int maxValue)
        {
            var inSamples = new int[4096];
            for (int i = 1, j = minValue, k = maxValue; i < inSamples.Length; i += 2, j++, k--)
            {
                inSamples[i - 1] = Math.Min(j, maxValue);
                inSamples[i] = Math.Max(k, minValue);
            }
            var outSamples = new float[inSamples.Length];

            using (var samples = new SampleBuffer(inSamples, bitsPerSample))
                samples.CopyTo(outSamples);

            Assert.All(outSamples, sample =>
                Assert.True(sample >= -1.0 && sample <= 1.0));
        }

        [Theory(DisplayName = "Integers are preserved exactly")]
        [InlineData(1, -1, 0)]
        [InlineData(2, -2, 1)]
        [InlineData(3, -4, 3)]
        [InlineData(4, -8, 7)]
        [InlineData(5, -0x10, 0xF)]
        [InlineData(6, -0x20, 0x1F)]
        [InlineData(7, -0x40, 0x3F)]
        [InlineData(8, -0x80, 0x7F)]
        [InlineData(9, -0x100, 0xFF)]
        [InlineData(10, -0x200, 0x1FF)]
        [InlineData(11, -0x400, 0x3FF)]
        [InlineData(12, -0x800, 0x7FF)]
        [InlineData(13, -0x1000, 0xFFF)]
        [InlineData(14, -0x2000, 0x1FFF)]
        [InlineData(15, -0x4000, 0x3FFF)]
        [InlineData(16, -0x8000, 0x7FFF)]
        [InlineData(17, -0x1_0000, 0xFFFF)]
        [InlineData(18, -0x2_0000, 0x1_FFFF)]
        [InlineData(19, -0x4_0000, 0x3_FFFF)]
        [InlineData(20, -0x8_0000, 0x7_FFFF)]
        [InlineData(21, -0x10_0000, 0xF_FFFF)]
        [InlineData(22, -0x20_0000, 0x1F_FFFF)]
        [InlineData(23, -0x40_0000, 0x3F_FFFF)]
        [InlineData(24, -0x80_0000, 0x7F_FFFF)]
        [InlineData(25, -0x100_0000, 0xFF_FFFF)]
        public void IntegersArePreservedExactly(int bitsPerSample, int minValue, int maxValue)
        {
            var inSamples = new int[4096];
            for (int i = 1, j = minValue, k = maxValue; i < inSamples.Length; i += 2, j++, k--)
            {
                inSamples[i - 1] = Math.Min(j, maxValue);
                inSamples[i] = Math.Max(k, minValue);
            }
            var outSamples = new int[inSamples.Length];

            using (var samples = new SampleBuffer(inSamples, 2, bitsPerSample))
                samples.CopyToInterleaved(outSamples, bitsPerSample);

            Assert.Equal(inSamples, outSamples);
        }
    }
}
