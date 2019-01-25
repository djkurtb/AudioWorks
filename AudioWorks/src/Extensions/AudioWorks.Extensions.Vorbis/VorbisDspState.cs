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
using System.Runtime.InteropServices;

namespace AudioWorks.Extensions.Vorbis
{
    [StructLayout(LayoutKind.Sequential)]
    readonly struct VorbisDspState
    {
        readonly int AnalysisP;

        readonly IntPtr VorbisInfo;

        readonly IntPtr Pcm;

        readonly IntPtr PcmRet;

        readonly int PcmStorage;

        readonly int PcmCurrent;

        readonly int PcmReturned;

        readonly int PreExtrapolate;

        readonly int EofFlag;

#if WINDOWS
        readonly int Lw;

        readonly int W;

        readonly int Nw;

        readonly int CenterW;
#else
        readonly long Lw;

        readonly long W;

        readonly long Nw;

        readonly long CenterW;
#endif

        readonly long GranulePosition;

        readonly long Sequence;

        readonly long GlueBits;

        readonly long TimeBits;

        readonly long FloorBits;

        readonly long ResBits;

        readonly IntPtr BackendState;
    }
}