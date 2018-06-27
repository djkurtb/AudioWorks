﻿using System;
#if !NETCOREAPP2_1
using System.Buffers;
#endif
using System.IO;
using AudioWorks.Common;

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
                            oggStream = new OggStream(SafeNativeMethods.OggPageSerialNo(ref page));

                        oggStream.PageIn(ref page);

                        while (oggStream.PacketOut(out var packet))
                        {
                            if (!SafeNativeMethods.VorbisSynthesisIdHeader(ref packet))
                                throw new AudioUnsupportedException("Not a Vorbis file.", stream.Name);

                            decoder.HeaderIn(ref vorbisComment, ref packet);

                            var info = decoder.GetInfo();
                            return AudioInfo.CreateForLossy(
                                "Vorbis",
                                info.Channels,
                                info.Rate,
                                0,
                                info.BitrateNominal > 0 ? info.BitrateNominal : 0);
                        }
                    } while (!SafeNativeMethods.OggPageEos(ref page));

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
