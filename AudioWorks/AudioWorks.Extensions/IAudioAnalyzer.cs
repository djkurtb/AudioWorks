﻿using System.Diagnostics.CodeAnalysis;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions
{
    /// <summary>
    /// An extension that can analyze an audio file stream.
    /// </summary>
    [PublicAPI]
    public interface IAudioAnalyzer
    {
        /// <summary>
        /// Gets information about the settings that can be passed to the <see cref="Initialize"/> method.
        /// </summary>
        /// <value>
        /// The setting information.
        /// </value>
        [NotNull]
        SettingInfoDictionary SettingInfo { get; }

        /// <summary>
        /// Initializes the analyzer.
        /// </summary>
        /// <param name="audioInfo">The audio information.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="groupToken">The group token.</param>
        void Initialize(
            [NotNull] AudioInfo audioInfo,
            [NotNull] SettingDictionary settings,
            [NotNull] GroupToken groupToken);

        /// <summary>
        /// Submits samples for processing.
        /// </summary>
        /// <param name="samples">The samples.</param>
        void Submit([NotNull] SampleCollection samples);

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>The result.</returns>
        [NotNull]
        [SuppressMessage("Design", "CA1024:Use properties where appropriate",
            Justification = "Order of execution is important")]
        AudioMetadata GetResult();

        /// <summary>
        /// Gets the result for a group.
        /// </summary>
        /// <returns>The result.</returns>
        [NotNull]
        [SuppressMessage("Design", "CA1024:Use properties where appropriate",
            Justification = "Order of execution is important")]
        AudioMetadata GetGroupResult();
    }
}