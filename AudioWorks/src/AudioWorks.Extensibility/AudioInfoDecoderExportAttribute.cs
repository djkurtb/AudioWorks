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
using System.Composition;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace AudioWorks.Extensibility
{
    /// <summary>
    /// Classes marked with this attribute will be loaded by AudioWorks.
    /// </summary>
    /// <remarks>
    /// Classes marked with this attribute must implement <see cref="IAudioInfoDecoder"/>.
    /// </remarks>
    /// <seealso cref="ExportAttribute"/>
    [PublicAPI, MeansImplicitUse, BaseTypeRequired(typeof(IAudioInfoDecoder))]
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
    public sealed class AudioInfoDecoderExportAttribute : ExportAttribute
    {
        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        [NotNull]
        public string Extension { get; }

        /// <summary>
        /// Gets the audio format.
        /// </summary>
        /// <value>The format.</value>
        [NotNull]
        public string Format { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioInfoDecoderExportAttribute"/> class.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <param name="format">The audio format.</param>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="extension"/> or
        /// <paramref name="format"/> is null or empty.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="extension"/> is not a valid file extension.
        /// </exception>
        public AudioInfoDecoderExportAttribute([NotNull] string extension, [NotNull] string format)
            : base(typeof(IAudioInfoDecoder))
        {
            if (string.IsNullOrEmpty(extension)) throw new ArgumentNullException(nameof(extension));
            if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase)
                || extension.Any(char.IsWhiteSpace)
                || extension.Any(character => Path.GetInvalidFileNameChars().Contains(character)))
                throw new ArgumentException($"'{extension}' is not a valid file extension.", nameof(extension));
            if (string.IsNullOrEmpty(format)) throw new ArgumentNullException(nameof(format));

            Extension = extension;
            Format = format;
        }
    }
}
