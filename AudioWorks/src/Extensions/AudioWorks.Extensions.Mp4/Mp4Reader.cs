﻿using System.Buffers.Binary;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Mp4
{
    sealed class Mp4Reader : BinaryReader
    {
        [NotNull] readonly byte[] _buffer = new byte[4];

        internal Mp4Reader([NotNull] Stream input)
            : base(input, CodePagesEncodingProvider.Instance.GetEncoding(1252), true)
        {
        }

        [NotNull]
        internal string ReadFourCc()
        {
            return new string(ReadChars(4));
        }

        internal uint ReadUInt32BigEndian()
        {
            Read(_buffer, 0, 4);
            return BinaryPrimitives.ReadUInt32BigEndian(_buffer);
        }
    }
}