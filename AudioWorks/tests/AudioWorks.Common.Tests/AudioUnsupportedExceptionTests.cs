/* Copyright � 2018 Jeremy Herbison

This file is part of AudioWorks.

AudioWorks is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public
License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later
version.

AudioWorks is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
details.

You should have received a copy of the GNU Affero General Public License along with AudioWorks. If not, see
<https://www.gnu.org/licenses/>. */

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AudioWorks.TestUtilities;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace AudioWorks.Common.Tests
{
    public sealed class AudioUnsupportedExceptionTests
    {
        public AudioUnsupportedExceptionTests([NotNull] ITestOutputHelper outputHelper)
        {
            LoggerManager.AddSingletonProvider(() => new XunitLoggerProvider()).OutputHelper = outputHelper;
        }

        [Fact(DisplayName = "AudioUnsupportedException is an AudioException")]
        public void IsAudioException()
        {
            Assert.IsAssignableFrom<AudioException>(new AudioUnsupportedException());
        }

        [Fact(DisplayName = "AudioUnsupportedException's Path property is properly serialized")]
        public void PathIsSerialized()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, new AudioUnsupportedException(null, "Foo"));
                stream.Position = 0;
                Assert.Equal("Foo", ((AudioUnsupportedException) formatter.Deserialize(stream)).Path);
            }
        }
    }
}
