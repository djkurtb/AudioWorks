﻿#if NETCOREAPP2_1
using System;
#endif
using System.Buffers.Binary;
using System.IO;
using System.Text;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Mp4
{
    sealed class Mp4Reader : BinaryReader
    {
#if !NETCOREAPP2_1
        [NotNull] readonly byte[] _buffer = new byte[4];

#endif
        internal Mp4Reader([NotNull] Stream input)
            : base(input, CodePagesEncodingProvider.Instance.GetEncoding(1252), true)
        {
        }

        [NotNull]
        internal string ReadFourCc()
        {
            //TODO avoid allocating an array on netcore2.1
            var readChars = ReadChars(4);
            if (readChars.Length < 4)
                throw new AudioInvalidException("File is unexpectedly truncated.", ((FileStream) BaseStream).Name);

            return new string(readChars);
        }

        internal uint ReadUInt32BigEndian()
        {
#if NETCOREAPP2_1
            Span<byte> buffer = stackalloc byte[4];
            if (Read(buffer) < 4)
                throw new AudioInvalidException("File is unexpectedly truncated.", ((FileStream) BaseStream).Name);

            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
#else
            if (Read(_buffer, 0, 4) < 4)
                throw new AudioInvalidException("File is unexpectedly truncated.", ((FileStream) BaseStream).Name);

            return BinaryPrimitives.ReadUInt32BigEndian(_buffer);
#endif
        }
    }
}
