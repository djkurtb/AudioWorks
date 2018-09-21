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

using System;
using System.Collections.Generic;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Api
{
    sealed class ValidatingSettingDictionary : SettingDictionary
    {
        [NotNull] readonly SettingInfoDictionary _settingInfos;

        internal ValidatingSettingDictionary([NotNull] SettingInfoDictionary settingInfos, [CanBeNull] SettingDictionary settings = null)
        {
            _settingInfos = settingInfos;

            if (settings == null) return;

            foreach (var setting in settings)
                Add(setting);
        }

        public override void Add(KeyValuePair<string, object> item)
        {
            Validate(item.Key, item.Value);
            base.Add(item);
        }

        public override void Add(string key, object value)
        {
            Validate(key, value);
            base.Add(key, value);
        }

        public override object this[string key]
        {
            get => base[key];
            set
            {
                Validate(key, value);
                base[key] = value;
            }
        }

        void Validate([NotNull] string key, [NotNull] object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (!_settingInfos.TryGetValue(key, out var settingInfo))
                throw new ArgumentException($"{key} is not a supported setting.", nameof(key));

            settingInfo.Validate(value);
        }
    }
}