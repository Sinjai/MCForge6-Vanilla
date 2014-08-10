/*
Copyright 2011 MCForge
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.opensource.org/licenses/ecl2.php
http://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MCForge.Utils.Settings;

namespace MCForge.Utils.Settings {
    /// <summary>
    /// Settings Utility
    /// </summary>
    public class ExtraSettings {
        public ExtraSettings(string settingsName, bool init=true) {
            this.SettingsName = settingsName;
            if(init) Init();
        }
        public ExtraSettings(string settingsName, SettingNode[] defaultValues, bool init=true)
            : this(settingsName, false) {
                this.defaultValues = defaultValues;
                if (init) Init();
        }
        public string PropertiesPath { get { return ServerSettings.GetSetting("configpath") + SettingsName + ".cfg"; } }
        public string SettingsName;
        private bool _initCalled;
        private List<SettingNode> Values;
        private readonly SettingNode[] defaultValues = {};


        /// <summary>
        /// This event is triggered when a setting node is changed in anyway
        /// </summary>
        public event EventHandler<SettingsChangedEventArgs> OnSettingChanged;

        public List<SettingNode> All() { return this.Values; }
        /// <summary>
        /// Starts the Settings Object
        /// </summary>
        /// <remarks>Must be called before any methods are invoked</remarks>
        public virtual void Init() {
            //Do not set to static ServerSettings(){}

            if (_initCalled)
                throw new ArgumentException("\"Init()\" can only be called once");

            _initCalled = true;
            Values = new List<SettingNode>(defaultValues);
            if (!Directory.Exists(FileUtils.PropertiesPath))
                Directory.CreateDirectory(FileUtils.PropertiesPath);

            if (!File.Exists(PropertiesPath)) {
                FileUtils.CreateDirIfNotExist("config");
                using (var writer = File.CreateText(PropertiesPath)) {
                    foreach (var v in Values) {
                        writer.WriteLine(v.Description == null
                                             ? string.Format("{0}={1}\n", v.Key.ToLower(), v.Value)
                                             : string.Format("#{0}\r\n{1}={2}\n", v.Description, v.Key.ToLower(), v.Value));

                    }
                }
            }

            LoadSettings();
            UpgradeSettings();
        }

        private void UpgradeSettings() {
            if (Values.Count < defaultValues.Length) {
                Logger.Log("Upgrading settings...", LogType.Warning);
                for (int i = 0; i < defaultValues.Length; i++) {
                    var value = defaultValues[i];
                    if (!HasKey(value.Key))
                        Values.Insert(i, value);
                }

                Save();
            }
        }



        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting values, use [0] at end if it only has 1 value</returns>
        public string[] GetSettingArray(string key) {
            if (key == null) return new[] { "" };
            key = key.ToLower();
            var pair = GetNode(key);
            return pair == null ? new[] { "" } : GetNode(key).Value.Split(','); //We don't want to return a null object
        }


        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value</returns>
        /// <remarks>Returns the first value if multiple values are present</remarks>
        public string GetSetting(string key) {
            key = key.ToLower();
            return GetSettingArray(key)[0];
        }

        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value specified by the key, or -1 if the setting is not found or could not be parsed</returns>
        public int GetSettingInt(string key) {
            key = key.ToLower();
            int i;
            var pair = GetNode(key);
            if (pair == null)
                return -1;
            if (!int.TryParse(GetNode(key).Value, out i)) {
                Logger.Log("ServerSettings: integer expected as first value for '" + key.SqlEscape() + "'", Color.Red, Color.Black);
                return -1;
            }
            return i;
        }

        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value specified by the key, or false if the setting is not found</returns>
        public bool GetSettingBoolean(string key) {
            key = key.ToLower();
            return GetSetting(key).ToLower() == "true";
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <param name="values">for each string in values, it will be seperated by a comma ','</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public void SetSetting(string key, string description = null, params string[] values) {
            key = key.ToLower();
            var pair = GetNode(key);
            if (pair == null) {
                pair = new SettingNode(key, string.Join(",", values), description);
                Values.Add(pair);
                if (OnSettingChanged != null)
                    OnSettingChanged(null, new SettingsChangedEventArgs(key, null, pair.Value));
                return;
            }
            if (OnSettingChanged != null)
                OnSettingChanged(null, new SettingsChangedEventArgs(key, pair.Value, string.Join(",", values)));

            pair.Description = description ?? pair.Description; ;
            pair.Value = string.Join(",", values);
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="value">value (or multiple values sperated by a comma ',') to set setting to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public void SetSetting(string key, int value, string description = null) {
            key = key.ToLower();
            var pair = GetNode(key);
            if (pair == null) {
                pair = new SettingNode(key, value.ToString(CultureInfo.InvariantCulture), description);
                Values.Add(pair);
                if (OnSettingChanged != null)
                    OnSettingChanged(null, new SettingsChangedEventArgs(key, null, pair.Value));
                return;
            }
            if (OnSettingChanged != null)
                OnSettingChanged(null, new SettingsChangedEventArgs(key, pair.Value, value.ToString(CultureInfo.InvariantCulture)));

            pair.Description = description;
            pair.Value = string.Join(",", value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="value">value to set setting to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public void SetSetting(string key, bool value, string description = null) {
            key = key.ToLower();
            var pair = GetNode(key);
            if (pair == null) {
                pair = new SettingNode(key, value.ToString(CultureInfo.InvariantCulture), description);
                Values.Add(pair);
                if (OnSettingChanged != null)
                    OnSettingChanged(null, new SettingsChangedEventArgs(key, null, pair.Value));
                return;
            }
            if (OnSettingChanged != null)
                OnSettingChanged(null, new SettingsChangedEventArgs(key, pair.Value, value.ToString(CultureInfo.InvariantCulture)));

            pair.Description = description;
            pair.Value = string.Join(",", value.ToString(CultureInfo.InvariantCulture));
        }

        internal SettingNode GetNode(string key) {
            key = key.ToLower();
            return Values.FirstOrDefault(pair => (pair.Key == null) ? false : pair.Key.ToLower() == key.ToLower());
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        public void Save() {

            using (var writer = new StreamWriter(PropertiesPath)) {
                foreach (var v in Values) {

                    writer.WriteLine(v.Description == null && v.Key == null
                        ? v.Value
                        : v.Description == null
                            ? string.Format("{0}={1}" + (v != Values.Last() ? Environment.NewLine : ""), v.Key, v.Value)
                            : string.Format("#{0}" + Environment.NewLine + "{1}={2}", v.Description, v.Key, v.Value));

                }
            }
        }

        /// <summary>
        /// Loads all the settings into the memory, if no properties file is found nothing will happen
        /// </summary>
        public void LoadSettings() {
            if (!File.Exists(PropertiesPath))
                return;
            string[] text = File.ReadAllLines(PropertiesPath);
            Values.Clear();
            for (int i = 0; i < text.Count(); i++) {
                string read = text[i];
                SettingNode pair;

                if (String.IsNullOrWhiteSpace(read)) {
                    Values.Add(new SettingNode(null, read, null));
                    continue;
                }

                if (read[0] == '#' && ((i + 1 < text.Length) ? text[i + 1][0] == '#' || String.IsNullOrWhiteSpace(text[i + 1]) : true)) {
                    Values.Add(new SettingNode(null, read, null));
                    continue;
                }

                if (read[0] == '#' && (i + 1 < text.Length) ? text[i + 1][0] != '#' && !String.IsNullOrWhiteSpace(text[i + 1]) : false) {
                    i++;
                    var split = text[i].Split('=');
                    pair = new SettingNode(split[0].Trim().ToLower(),
                                           String.Join("=", split, 1, split.Length - 1).Trim(),
                                           read.Substring(1));
                }
                else {
                    if (read[0] != '#') {
                        var split = text[i].Split('=');
                        pair = new SettingNode(split[0].Trim().ToLower(),
                                               String.Join("=", split, 1, split.Length - 1).Trim(),
                                               null);
                    }
                    else pair = new SettingNode(null, read, null);
                }
                Values.Add(pair);
            }

        }

        public bool HasKey(string key) {
            return GetNode(key) != null;
        }
        public string GetDescription(string key) {
            return GetNode(key).Description;
        }
    }

    /// <summary>
    /// Called When a setting node is changed
    /// </summary>
    public class SettingsChangedEventArgs : EventArgs {

        /// <summary>
        /// The key of the setting
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// The value before it was changed
        /// </summary>
        public string OldValue { get; set; }
        /// <summary>
        /// The new value of the setting
        /// </summary>
        public string NewValue { get; set; }


        /// <summary>
        /// Create a new Settings Changed Event Class
        /// </summary>
        /// <param name="key">Name of key in lowercase</param>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">value to change</param>
        public SettingsChangedEventArgs(string key, string oldValue, string newValue) {
            Key = key.ToLower();
            OldValue = oldValue;
            NewValue = newValue;
        }

    }

    /// <summary>
    /// A simple class housing information of a setting key, value, and description
    /// </summary>
    public class SettingNode {

        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public SettingNode(string key, string value, string description) {
            if (key != null)
                Key = key.ToLower();
            Value = value;
            Description = description;
        }
    }



}