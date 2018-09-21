﻿/* Copyright © 2018 Jeremy Herbison

This file is part of AudioWorks.

AudioWorks is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later
version.

AudioWorks is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
details.

You should have received a copy of the GNU Lesser General Public License along with AudioWorks. If not, see
<https://www.gnu.org/licenses/>. */

using System;
#if !NETCOREAPP2_1
using System.Buffers;
#endif
using System.IO;
using AudioWorks.Common;
using AudioWorks.Extensibility;

namespace AudioWorks.Extensions.Vorbis
{
    [AudioInfoDecoderExport(".ogg")]
    public sealed class VorbisAudioInfoDecoder : IAudioInfoDecoder
    {
        public unsafe AudioInfo ReadAudioInfo(FileStream stream)
        {
            OggStream oggStream = null;
            SafeNativeMethods.VorbisCommentInit(out var vorbisComment);
#if NETCOREAPP2_1
            Span<byte> buffer = stackalloc byte[4096];
#else
            var buffer = ArrayPool<byte>.Shared.Rent(4096);
#endif

            try
            {
                using (var sync = new OggSync())
                using (var decoder = new VorbisDecoder())
                {
                    OggPage page;

                    do
                    {
                        // Read from the buffer into a page:
                        while (!sync.PageOut(out page))
                        {
#if NETCOREAPP2_1
                            var bytesRead = stream.Read(buffer);
#else
                            var bytesRead = stream.Read(buffer, 0, buffer.Length);
#endif
                            if (bytesRead == 0)
                                throw new AudioInvalidException("No Ogg stream was found.", stream.Name);

                            var nativeBuffer = new Span<byte>(sync.Buffer(bytesRead).ToPointer(), bytesRead);
#if NETCOREAPP2_1
                            buffer.Slice(0, bytesRead).CopyTo(nativeBuffer);
#else
                            buffer.AsSpan().Slice(0, bytesRead).CopyTo(nativeBuffer);
#endif
                            sync.Wrote(bytesRead);
                        }

                        if (oggStream == null)
                            oggStream = new OggStream(SafeNativeMethods.OggPageSerialNo(page));

                        oggStream.PageIn(page);

                        while (oggStream.PacketOut(out var packet))
                        {
                            if (!SafeNativeMethods.VorbisSynthesisIdHeader(packet))
                                throw new AudioUnsupportedException("Not a Vorbis file.", stream.Name);

                            decoder.HeaderIn(vorbisComment, packet);

                            var info = decoder.GetInfo();
                            return AudioInfo.CreateForLossy(
                                "Vorbis",
                                info.Channels,
#if WINDOWS
                                info.Rate,
                                0,
                                Math.Max(info.BitRateNominal, 0));
#else
                                (int) info.Rate,
                                0,
                                Math.Max((int) info.BitRateNominal, 0));
#endif
                        }
                    } while (!SafeNativeMethods.OggPageEos(page));

                    throw new AudioInvalidException("The end of the Ogg stream was reached without finding the header.",
                        stream.Name);
                }
            }
            finally
            {
#if !NETCOREAPP2_1
                ArrayPool<byte>.Shared.Return(buffer);
#endif
                SafeNativeMethods.VorbisCommentClear(ref vorbisComment);
                oggStream?.Dispose();
            }
        }
    }
}
