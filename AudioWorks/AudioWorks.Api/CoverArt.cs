﻿using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SixLabors.ImageSharp;

namespace AudioWorks.Api
{
    /// <summary>
    /// Represents a cover art image for one or more audio files.
    /// </summary>
    [PublicAPI]
    public sealed class CoverArt
    {
        [NotNull] static readonly string[] _acceptedExtensions = { ".bmp", ".png", ".jpg" };

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverArt"/> class.
        /// </summary>
        /// <param name="path">The fully-qualified path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null or empty.</exception>
        /// <exception cref="FileNotFoundException">Thrown if <paramref name="path"/> does not exist.</exception>
        public CoverArt([NotNull] string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "Value cannot be null or empty.");
            if (!File.Exists(path))
                throw new FileNotFoundException($"The file '{path}' cannot be found.", path);

            if (!_acceptedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
                throw new ImageUnsupportedException("Not a supported image file format.", path);

            using (var image = Image.Load(path))
            {
                Width = image.Width;
                Height = image.Height;
            }
        }
    }
}
