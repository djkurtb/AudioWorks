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
using System.IO;
using System.Runtime.InteropServices;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Flac
{
    class AudioInfoStreamDecoder : StreamDecoder
    {
        [CanBeNull]
        internal AudioInfo AudioInfo { get; private set; }

        internal AudioInfoStreamDecoder([NotNull] Stream stream)
            : base(stream)
        {
        }

        protected override void MetadataCallback(IntPtr handle, IntPtr metadataBlock, IntPtr userData)
        {
            if ((MetadataType) Marshal.ReadInt32(metadataBlock) != MetadataType.StreamInfo)
                return;

            var streamInfo = Marshal.PtrToStructure<StreamInfoMetadataBlock>(metadataBlock).StreamInfo;
            AudioInfo = AudioInfo.CreateForLossless(
                "FLAC",
                (int) streamInfo.Channels,
                (int) streamInfo.BitsPerSample,
                (int) streamInfo.SampleRate,
                (long) streamInfo.TotalSamples);
        }
    }
}