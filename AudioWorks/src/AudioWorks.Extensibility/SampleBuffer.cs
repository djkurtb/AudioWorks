﻿/* Copyright © 2018 Jeremy Herbison

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
using System.Buffers;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace AudioWorks.Extensibility
{
    /// <summary>
    /// Represents a block of audio samples.
    /// </summary>
    [PublicAPI]
    public sealed class SampleBuffer : IDisposable
    {
        /// <summary>
        /// Gets a <see cref="SampleBuffer"/> with 0 frames.
        /// </summary>
        /// <value>An empty <see cref="SampleBuffer"/>.</value>
        public static SampleBuffer Empty { get; } = new SampleBuffer();

        [CanBeNull] readonly IMemoryOwner<float> _buffer;
        bool _isDisposed;

        /// <summary>
        /// Gets a value indicating whether the samples are interleaved internally.
        /// </summary>
        /// <remarks>
        /// Always returns <c>false</c> when <see cref="Channels"/> equals 1.
        /// </remarks>
        /// <value><c>true</c> if the samples are interleaved; otherwise, <c>false</c>.</value>
        public bool IsInterleaved { get; }

        /// <summary>
        /// Gets the # of channels.
        /// </summary>
        /// <value>The # of channels.</value>
        public int Channels { get; }

        /// <summary>
        /// Gets the frame count.
        /// </summary>
        /// <value>The frame count.</value>
        public int Frames { get; }

        SampleBuffer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer"/> class using interleaved floating-point
        /// samples.
        /// </summary>
        /// <param name="interleavedSamples">The interleaved samples.</param>
        /// <param name="channels">The channels.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="interleavedSamples"/> is not a multiple of
        /// channels.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="channels"/> is out of range.
        /// </exception>
        public SampleBuffer(ReadOnlySpan<float> interleavedSamples, int channels)
        {
            if (channels < 1 || channels > 2)
                throw new ArgumentOutOfRangeException(nameof(channels),
                    $"{nameof(channels)} must be 1 or 2.");
            if (interleavedSamples.Length % channels != 0)
                throw new ArgumentException($"{nameof(interleavedSamples)} has an invalid length.",
                    nameof(interleavedSamples));

            Channels = channels;
            Frames = interleavedSamples.Length / channels;
            _buffer = MemoryPool<float>.Shared.Rent(Frames * channels);
            if (channels > 1)
                IsInterleaved = true;

            // ReSharper disable once PossibleNullReferenceException
            interleavedSamples.CopyTo(_buffer.Memory.Span);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer"/> class for a single channel, using integer
        /// samples.
        /// </summary>
        /// <param name="monoSamples">The samples.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bitsPerSample"/> is out of range.
        /// </exception>
        public SampleBuffer(ReadOnlySpan<int> monoSamples, int bitsPerSample)
        {
            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample),
                    $"{nameof(bitsPerSample)} is out of range.");

            Channels = 1;
            Frames = monoSamples.Length;
            _buffer = MemoryPool<float>.Shared.Rent(Frames);

            // ReSharper disable once PossibleNullReferenceException
            SampleProcessor.Convert(monoSamples, _buffer.Memory.Span, bitsPerSample);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer"/> class for 2 channels, using integer samples.
        /// </summary>
        /// <param name="leftSamples">The left channel samples.</param>
        /// <param name="rightSamples">The right channel samples.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rightSamples"/> has a different length than
        /// <paramref name="leftSamples"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="bitsPerSample"/> is out of range.
        /// </exception>
        public SampleBuffer(ReadOnlySpan<int> leftSamples, ReadOnlySpan<int> rightSamples, int bitsPerSample)
        {
            if (leftSamples.Length != rightSamples.Length)
                throw new ArgumentException(
                    $"{nameof(rightSamples)} does not match the length of {nameof(leftSamples)}", nameof(rightSamples));
            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample),
                    $"{nameof(bitsPerSample)} is out of range.");

            Channels = 2;
            Frames = leftSamples.Length;
            _buffer = MemoryPool<float>.Shared.Rent(Frames * 2);

            // ReSharper disable once PossibleNullReferenceException
            SampleProcessor.Convert(leftSamples, _buffer.Memory.Span.Slice(0, Frames), bitsPerSample);
            SampleProcessor.Convert(rightSamples, _buffer.Memory.Span.Slice(Frames), bitsPerSample);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer"/> class using interleaved integer samples.
        /// </summary>
        /// <param name="interleavedSamples">The interleaved samples.</param>
        /// <param name="channels">The channels.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="interleavedSamples"/> is not a multiple of
        /// channels.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="channels"/> or
        /// <paramref name="bitsPerSample"/> is out of range.</exception>
        public SampleBuffer(ReadOnlySpan<int> interleavedSamples, int channels, int bitsPerSample)
        {
            if (channels < 1 || channels > 2)
                throw new ArgumentOutOfRangeException(nameof(channels),
                    $"{nameof(channels)} must be 1 or 2.");
            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample),
                    $"{nameof(bitsPerSample)} is out of range.");
            if (interleavedSamples.Length % channels != 0)
                throw new ArgumentException($"{nameof(interleavedSamples)} has an invalid length.",
                    nameof(interleavedSamples));

            Channels = channels;
            Frames = interleavedSamples.Length / channels;
            _buffer = MemoryPool<float>.Shared.Rent(Frames * channels);
            if (channels > 1)
                IsInterleaved = true;

            // ReSharper disable once PossibleNullReferenceException
            SampleProcessor.Convert(interleavedSamples, _buffer.Memory.Span, bitsPerSample);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer"/> class using interleaved integer samples, which
        /// are packed on the byte boundary according to <paramref name="bitsPerSample"/>.
        /// </summary>
        /// <param name="interleavedSamples">The interleaved samples.</param>
        /// <param name="channels">The # of channels.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="interleavedSamples"/> is not a valid length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="channels"/> or
        /// <paramref name="bitsPerSample"/> is out of range.</exception>
        public SampleBuffer(ReadOnlySpan<byte> interleavedSamples, int channels, int bitsPerSample)
        {
            if (channels < 1 || channels > 2)
                throw new ArgumentOutOfRangeException(nameof(channels),
                    $"{nameof(channels)} must be 1 or 2.");
            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample),
                    $"{nameof(bitsPerSample)} is out of range.");
            var bytesPerSample = (int) Math.Ceiling(bitsPerSample / 8.0);
            if (interleavedSamples.Length % (channels * bytesPerSample) != 0)
                throw new ArgumentException($"{nameof(interleavedSamples)} has an invalid length.",
                    nameof(interleavedSamples));

            Channels = channels;
            Frames = interleavedSamples.Length / channels / bytesPerSample;
            _buffer = MemoryPool<float>.Shared.Rent(Frames * channels);
            if (channels > 1)
                IsInterleaved = true;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (bytesPerSample)
            {
                case 1:
                    // ReSharper disable once PossibleNullReferenceException
                    SampleProcessor.Convert(interleavedSamples, _buffer.Memory.Span, bitsPerSample);
                    break;
                case 2:
                    var interleavedInt16Samples = MemoryMarshal.Cast<byte, short>(interleavedSamples);
                    // ReSharper disable once PossibleNullReferenceException
                    SampleProcessor.Convert(interleavedInt16Samples, _buffer.Memory.Span, bitsPerSample);
                    break;
                case 3:
                    var interleavedInt24Samples = MemoryMarshal.Cast<byte, Int24>(interleavedSamples);
                    // ReSharper disable once PossibleNullReferenceException
                    SampleProcessor.Convert(interleavedInt24Samples, _buffer.Memory.Span, bitsPerSample);
                    break;
                case 4:
                    var interleavedInt32Samples = MemoryMarshal.Cast<byte, int>(interleavedSamples);
                    // ReSharper disable once PossibleNullReferenceException
                    SampleProcessor.Convert(interleavedInt32Samples, _buffer.Memory.Span, bitsPerSample);
                    break;
            }
        }

        /// <summary>
        /// Copies the single channel of audio samples in normalized floating-point format.
        /// </summary>
        /// <remarks>
        /// The samples are floating-point values normalized within -1.0 and 1.0.
        /// </remarks>
        /// <param name="monoDestination">The destination.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the Channels property does not equal 1.</exception>
        public void CopyTo(Span<float> monoDestination)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (Channels != 1)
                throw new InvalidOperationException("Not a single-channel SampleBuffer.");

            _buffer.Memory.Span.Slice(0, Frames).CopyTo(monoDestination);
        }

        /// <summary>
        /// Copies both channels of audio samples in normalized floating-point format.
        /// </summary>
        /// <remarks>
        /// The samples are floating-point values normalized within -1.0 and 1.0.
        /// </remarks>
        /// <param name="leftDestination">The destination for the left channel.</param>
        /// <param name="rightDestination">The destination for the right channel.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the Channels property does not equal 2.</exception>
        public void CopyTo(Span<float> leftDestination, Span<float> rightDestination)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (Channels != 2)
                throw new InvalidOperationException("Not a 2-channel SampleBuffer.");

            if (IsInterleaved)
                SampleProcessor.DeInterleave(
                    _buffer.Memory.Span.Slice(0, Frames * 2),
                    leftDestination,
                    rightDestination);
            else
            {
                _buffer.Memory.Span.Slice(0, Frames).CopyTo(leftDestination);
                _buffer.Memory.Span.Slice(Frames, Frames).CopyTo(rightDestination);
            }
        }

        /// <summary>
        /// Copies both channels of audio samples in integer format.
        /// </summary>
        /// <remarks>
        /// The samples are floating-point values normalized within -1.0 and 1.0.
        /// </remarks>
        /// <param name="leftDestination">The destination for the left channel.</param>
        /// <param name="rightDestination">The destination for the right channel.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the Channels property does not equal 2.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitsPerSample"/> is out of range.</exception>
        public void CopyTo(Span<int> leftDestination, Span<int> rightDestination, int bitsPerSample)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (Channels != 2)
                throw new InvalidOperationException("Not a 2-channel SampleBuffer.");

            if (IsInterleaved)
            {
                Span<float> leftBuffer = stackalloc float[Frames];
                Span<float> rightBuffer = stackalloc float[Frames];
                SampleProcessor.DeInterleave(_buffer.Memory.Span.Slice(0, Frames * 2), leftBuffer, rightBuffer);
                SampleProcessor.Convert(leftBuffer, leftDestination, bitsPerSample);
                SampleProcessor.Convert(rightBuffer, rightDestination, bitsPerSample);
            }
            else
            {
                SampleProcessor.Convert(_buffer.Memory.Span.Slice(0, Frames), leftDestination, bitsPerSample);
                SampleProcessor.Convert(_buffer.Memory.Span.Slice(Frames, Frames), rightDestination, bitsPerSample);
            }
        }

        /// <summary>
        /// Copies the interleaved channels of audio samples, in normalized floating-point format.
        /// </summary>
        /// <remarks>
        /// The samples are floating-point values normalized within -1.0 and 1.0. Stereo samples are interleaved,
        /// beginning with the left channel.
        /// </remarks>
        /// <param name="destination">The destination.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is not long enough to store the samples.
        /// </exception>
        public void CopyToInterleaved(Span<float> destination)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (destination.Length < Frames * Channels)
                throw new ArgumentException("destination is not long enough to store the samples.",
                    nameof(destination));

            if (Channels == 1 || IsInterleaved)
                _buffer.Memory.Span.Slice(0, Frames * Channels).CopyTo(destination);
            else
                SampleProcessor.Interleave(
                    _buffer.Memory.Span.Slice(0, Frames),
                    _buffer.Memory.Span.Slice(Frames, Frames),
                    destination);
        }

        /// <summary>
        /// Copies the interleaved channels of audio samples, in integer format.
        /// </summary>
        /// <remarks>
        /// The samples are signed and right-justified. Stereo samples are interleaved, beginning with the left
        /// channel.
        /// </remarks>
        /// <param name="destination">The destination.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is not long enough to store the samples.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitsPerSample"/> is out of range.</exception>
        public void CopyToInterleaved(Span<int> destination, int bitsPerSample)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (destination.Length < Frames * Channels)
                throw new ArgumentException("destination is not long enough to store the samples.",
                    nameof(destination));
            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample), "bitsPerSample is out of range.");

            if (Channels == 1 || IsInterleaved)
                SampleProcessor.Convert(_buffer.Memory.Span.Slice(0, Frames * Channels), destination, bitsPerSample);
            else
            {
                Span<float> interleavedBuffer = stackalloc float[Frames * 2];
                SampleProcessor.Interleave(
                    _buffer.Memory.Span.Slice(0, Frames),
                    _buffer.Memory.Span.Slice(Frames, Frames),
                    interleavedBuffer);
                SampleProcessor.Convert(interleavedBuffer, destination, bitsPerSample);
            }
        }

        /// <summary>
        /// Copies the interleaved channels of audio samples, in packed integer format.
        /// </summary>
        /// <remarks>
        /// The samples are stored as little-endian integers, aligned at the byte boundary. If
        /// <paramref name="bitsPerSample"/> is 8 or less, they are unsigned. Otherwise, they are signed. Stereo
        /// samples are interleaved, beginning with the left channel.
        /// </remarks>
        /// <param name="destination">The destination.</param>
        /// <param name="bitsPerSample">The # of bits per sample.</param>
        /// <exception cref="ObjectDisposedException">This <see cref="SampleBuffer"/> has been disposed.</exception>
        /// <exception cref="ArgumentException"><paramref name="destination"/> is not long enough to store the samples.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="bitsPerSample"/> is out of range.</exception>
        public void CopyToInterleaved(Span<byte> destination, int bitsPerSample)
        {
            if (_isDisposed) throw new ObjectDisposedException(GetType().ToString());
            if (_buffer == null) return;

            if (bitsPerSample < 1 || bitsPerSample > 32)
                throw new ArgumentOutOfRangeException(nameof(bitsPerSample), "bitsPerSample is out of range.");
            var bytesPerSample = (int) Math.Ceiling(bitsPerSample / 8.0);
            if (destination.Length < Frames * Channels * bytesPerSample)
                throw new ArgumentException("destination is not long enough to store the samples.",
                    nameof(destination));

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (bytesPerSample)
            {
                case 1:
                    if (Channels == 1 || IsInterleaved)
                        SampleProcessor.Convert(
                            _buffer.Memory.Span.Slice(0, Frames * Channels),
                            destination,
                            bitsPerSample);
                    else
                    {
                        Span<float> interleavedBuffer = stackalloc float[Frames * 2];
                        SampleProcessor.Interleave(
                            _buffer.Memory.Span.Slice(0, Frames),
                            _buffer.Memory.Span.Slice(Frames, Frames),
                            interleavedBuffer);
                        SampleProcessor.Convert(interleavedBuffer, destination, bitsPerSample);
                    }
                    break;
                case 2:
                    var int16Destination = MemoryMarshal.Cast<byte, short>(destination);
                    if (Channels == 1 || IsInterleaved)
                        SampleProcessor.Convert(
                            _buffer.Memory.Span.Slice(0, Frames * Channels),
                            int16Destination,
                            bitsPerSample);
                    else
                    {
                        Span<float> interleavedBuffer = stackalloc float[Frames * 2];
                        SampleProcessor.Interleave(
                            _buffer.Memory.Span.Slice(0, Frames),
                            _buffer.Memory.Span.Slice(Frames, Frames),
                            interleavedBuffer);
                        SampleProcessor.Convert(interleavedBuffer, int16Destination, bitsPerSample);
                    }
                    break;
                case 3:
                    var int24Destination = MemoryMarshal.Cast<byte, Int24>(destination);
                    if (Channels == 1 || IsInterleaved)
                        SampleProcessor.Convert(
                            _buffer.Memory.Span.Slice(0, Frames * Channels),
                            int24Destination,
                            bitsPerSample);
                    else
                    {
                        Span<float> interleavedBuffer = stackalloc float[Frames * 2];
                        SampleProcessor.Interleave(
                            _buffer.Memory.Span.Slice(0, Frames),
                            _buffer.Memory.Span.Slice(Frames, Frames),
                            interleavedBuffer);
                        SampleProcessor.Convert(interleavedBuffer, int24Destination, bitsPerSample);
                    }
                    break;
                case 4:
                    var int32Destination = MemoryMarshal.Cast<byte, int>(destination);
                    if (Channels == 1 || IsInterleaved)
                        SampleProcessor.Convert(
                            _buffer.Memory.Span.Slice(0, Frames * Channels),
                            int32Destination,
                            bitsPerSample);
                    else
                    {
                        Span<float> interleavedBuffer = stackalloc float[Frames * 2];
                        SampleProcessor.Interleave(
                            _buffer.Memory.Span.Slice(0, Frames),
                            _buffer.Memory.Span.Slice(Frames, Frames),
                            interleavedBuffer);
                        SampleProcessor.Convert(interleavedBuffer, int32Destination, bitsPerSample);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_isDisposed) return;

            _buffer?.Dispose();
            _isDisposed = true;
        }
    }
}