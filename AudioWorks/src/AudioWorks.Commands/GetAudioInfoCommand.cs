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

using System.Diagnostics.CodeAnalysis;
using System.Management.Automation;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Commands
{
    /// <summary>
    /// <para type="synopsis">Gets information about an audio file.</para>
    /// <para type="description">The Get-AudioInfo cmdlet gets information about audio files. This consists of
    /// immutable information that can't be changed without re-encoding the file.</para>
    /// </summary>
    [PublicAPI]
    [Cmdlet(VerbsCommon.Get, "AudioInfo"), OutputType(typeof(AudioInfo))]
    public sealed class GetAudioInfoCommand : LoggingCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the audio file.</para>
        /// </summary>
        [NotNull, SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public IAudioFile AudioFile { get; set; }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            var result = AudioFile.Info;

            ProcessLogMessages();

            WriteObject(result);
        }
    }
}
