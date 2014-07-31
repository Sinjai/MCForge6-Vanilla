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
using MCForge.Utils.Settings;
using MCForge.Utils.Settings;
using MCForge.Utils.Settings;

namespace MCForge.Utils.Settings {
    /// <summary>
    /// Settings Utility
    /// </summary>
    public class ServerSettings {
        private static ExtraSettings settings = new ExtraSettings("ServerSettings", new SettingNode[]{
                    new SettingNode("ServerName", "[MCForge] Default", "Name of your server"),
                    new SettingNode("Port", "25565", "Server Port"),
                    new SettingNode("Main-Level", "main", "The name of the main level. If tihs is empty or doesn't exist, it will generate a flat level."),
                    new SettingNode("Enable-Remote", "false", "If true, it will create a listener for remote consoles to connect to"),
                    new SettingNode("Remote-IP", "0.0.0.0", "The ip to bind the listener to, if you are using a different ip. Don't touch if you don't know what it does"),
                    new SettingNode("Remote-Port", "5050", "The port the remote will connnect to"),
                    new SettingNode("Use-UPnP", "false", "if enabled will automatically forward port for you, this is not recommended, and will not work in some cases"),
                    new SettingNode("MOTD", "Welcome to my server!", "Message that shows up when you start server"),
                    new SettingNode("MaxPlayers", "20", "Max players that can play on your server at a time"),
                    new SettingNode("Public", "true", "if set to true, your server will show up on MCForge.net server list and Minecraft.net's server list"),
                    new SettingNode("VerifyNames", "true", "Check to see if the player logging in owns that account."),
                    new SettingNode("MoneyName", "moneys", "The name of the server currency."),
                    new SettingNode("ServerOwner", "Notch", "The username of the server owner."),
                    new SettingNode("LoadAllLevels", "false", "Load all levels on startup."),
                    new SettingNode("Verifying", "false", "Do people need to use /pass upon login?"),
                    new SettingNode("VerifyGroup", "operator", "The name of the minimum group that needs to verify using /pass."),
                    new SettingNode("ShowFirstRunScreen", "true", "Whether or not to show the first run screen when the server is started."),
                    new SettingNode("UsingConsole", "true", "Set to \"false\" if you want GUI. If using mono set to \"true\"."),
                    new SettingNode("ShutdownMessage", "Server shutting down!", "Message to show when server is shutting down"),
                    new SettingNode("WelcomeMessage", "Welcome $name to $server<br>Enjoy your stay", "Welcome message, to signify a line break use \"<br>\""),
                    new SettingNode("ConfigPath", "config/", "File path for group player properties, do not mess with unless you know what you are doing"),
                    new SettingNode("MessageAppending", "true", "allow use of message appending, ex using \">\" at the end of your message will allow you to finish your statement on a new chat segment"),
                    new SettingNode("DefaultGroup", "guest", "The name of the default group, if it doesn't exist it will cause problems"),
                    new SettingNode("Check-Core-Updates", "true", "Check for core updates"),
                    new SettingNode("Check-Misc-Updates", "true", "Check for plugin and command updates"),
                    new SettingNode("Allow-Patch-Updates", "true", "Allow MCForge to download Patch updates to fix security flaws or other problems while updating."),
                    new SettingNode("Auto-Update", "false", "If enabled, commands, plugins, and the core will automatically update WITH notification (Ignored if silent-update is enabled)"),
                    new SettingNode("Silent-Update", "true", "If enabled, commands and plugins will be updated without notification"),
                    new SettingNode("Ask-Before-Core", "true", "If enabled, the server will ask before updating the core (Ignored if auto-update or silent-core-update is enabled)"),
                    new SettingNode("Ask-Before-Misc", "false", "If enabled, the server will ask before updating plugins and commands (Ignored if auto-update or silent-update is enabled)"),
                    new SettingNode("Silent-Core-Update", "false", "If enabled, the server will attempt to udpate when server activity is low"),
                    new SettingNode("Updatecheck-Interval", "10", "How often to check for updates (in minutes)"),
                    //new SettingNode("Offline", "false", "if set to true, it will skip authentication, causing a major security flaw"), Isnt this just verify names?
                    new SettingNode("AllowHigherRankTp", "true", "Allow players of a lower rank to teleport to a user of a higher rank"),
                    new SettingNode("DatabaseType", "sqlite", "The type of database you want to use (mysql/sqlite)"),
                    new SettingNode("MySQL-IP", "127.0.0.1", "The IP of the sql (mysql/sqlite) database"),
                    new SettingNode("MySQL-Port", "3306", "The port for the mysql database (sqlite does not need a port, leave this blank)"),
                    new SettingNode("MySQL-Username", "root", "The username for the mysql database"),
                    new SettingNode("MySQL-Password", "password", "The password for the mysql database"),
                    new SettingNode("MySQL-Pooling", "True", null),
                    new SettingNode("MySQL-DBName", "MCForge", "The database name for MySQL"),
                    new SettingNode("SQLite-InMemory", "False", "If set to \"True\" the Database is running in memory and gets regularly backuped to SQLite-Filepath (\"True\" speeds up the server, but could loose data up to the last backup on a crash)"),
                    new SettingNode("SQLite-Filepath", "_mcforge.db", "The filepath for the database"),
                    new SettingNode("SQLite-Pooling", "True", null),
                    new SettingNode("Database-Queuing", "True", null),
                    new SettingNode("Database-Flush_Interval", "20", null),
                    new SettingNode("IRC-Enabled", "false", "If set to true, IRC is enabled."),
                    new SettingNode("IRC-Server", "127.0.0.1", "IRC server to connect to"),
                    new SettingNode("IRC-Port", "6667", "IRC server port"),
                    new SettingNode("IRC-Nickname", "MCForge-" + MathUtils.Random.Next(1000, 9999), "IRC nickname"),
                    new SettingNode("IRC-Channel", "#", "IRC channel to connect to"),
                    new SettingNode("IRC-OPChannel", "#", "IRC operator channel to connect to"),
                    new SettingNode("IRC-NickServ", "password", "IRC NickServ password (optional when IRC is enabled)"),
                    new SettingNode("AgreeingToRules", "true", "If set to true players below op will need to read the rules and agree before they can use commands"),
                    new SettingNode("$Before$Name", "true", "Puts a $ before the name of the variable if used. For example, if $name was typed in chat it would bring up $headdetect instead of just headdetect"),
                    new SettingNode("TreesGoThrough", "false", "Can /tree go through other blocks. Set to false to prevent players from using /tree to grief!"),
                    new SettingNode("ReviewModeratorPerm", "80", "Determines what rank gets to use /review next and /review remove. Use the permission number, not the rank name!"),
                    new SettingNode("OpChatPermission", "80", "Determines what rank can use the operator chat"),
                    new SettingNode("AdminChatPermission", "100", "Determines what rank can use the admin chat"),
                    new SettingNode("DefaultColor", "&a", "Determines the server's default color"),
                    new SettingNode("BackupFiles", "true", "If set to true, files will be backed up to the backup folder"),
                    new SettingNode("BackupInterval", "300", "the interval to backup your files (in seconds)"),
                    new SettingNode("PhysicsInterval","100","the interval to do physics ticks in milliseconds"),
                    new SettingNode("AutoTimeout", "30", "if during this amount of seconds no data is sent from the client to the server, the player gets kicked (use 0 to disable this behavior)"),
                    new SettingNode("StartFlyMessage", "You are now flying. &cJump!","can be empty"),
                    new SettingNode("StopFlyMessage", "You're not flying anymore","can be empty"),
                    new SettingNode("FlyGlassSize", "5, 2", "The size of the glass panel. The first value describes width and length. The second value describes the height (default: 2)."),
                    new SettingNode("Fly+", "true", "'true' allows players to use '/fly +' which displays an aditional block at the players position"),
                    new SettingNode("FlyWaterSize", "5, 5, 5", "The size of the water cuboid to swim in the air. Use one number to set all dimension to the same size or use three numbers for different sizes. If it is set to false '/fly water' will be disabled")
               }, false);

        internal static string Salt { get; set; }
        internal static int Version { get { return 7; } }

        /// <summary>
        /// This event is triggered when a setting node is changed in anyway
        /// </summary>
        public static event EventHandler<SettingsChangedEventArgs> OnSettingChanged {
            add {
                settings.OnSettingChanged += value;
            }
            remove {
                settings.OnSettingChanged -= value;
            }
        }


        /// <summary>
        /// Starts the Settings Object
        /// </summary>
        /// <remarks>Must be called before any methods are invoked</remarks>
        public static void Init() {
            //Do not set to static ServerSettings(){}

            GenerateSalt();
            settings.Init();
        }



        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting values, use [0] at end if it only has 1 value</returns>
        public static string[] GetSettingArray(string key) {
            return settings.GetSettingArray(key);
        }


        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value</returns>
        /// <remarks>Returns the first value if multiple values are present</remarks>
        public static string GetSetting(string key) {
            return settings.GetSetting(key);
        }

        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value specified by the key, or -1 if the setting is not found or could not be parsed</returns>
        public static int GetSettingInt(string key) {
            return settings.GetSettingInt(key);
        }

        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value specified by the key, or false if the setting is not found</returns>
        public static bool GetSettingBoolean(string key) {
            return settings.GetSettingBoolean(key);
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <param name="values">for each string in values, it will be seperated by a comma ','</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public static void SetSetting(string key, string description = null, params string[] values) {
            settings.SetSetting(key, description, values);
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="value">value (or multiple values sperated by a comma ',') to set setting to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public static void SetSetting(string key, int value, string description = null) {
            settings.SetSetting(key, value, description);
        }

        /// <summary>
        /// Set the setting
        /// </summary>
        /// <param name="key">key to save value to</param>
        /// <param name="value">value to set setting to</param>
        /// <param name="description">Write a description (optional)</param>
        /// <remarks>If the setting does not exist, it will create a new one</remarks>
        public static void SetSetting(string key, bool value, string description = null) {
            settings.SetSetting(key, value, description);
        }

        internal static SettingNode GetNode(string key) {
           return settings.GetNode(key);
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        public static void Save() {
            settings.Save();
        }

        /// <summary>
        /// Loads all the settings into the memory, if no properties file is found nothing will happen
        /// </summary>
        public static void LoadSettings() {
            settings.LoadSettings();
        }


        internal static void GenerateSalt() {
            using (var numberGen = new RNGCryptoServiceProvider()) {
                var data = new byte[20];
                numberGen.GetBytes(data);
                Salt = Convert.ToBase64String(data);
            }
        }

        public static bool HasKey(string key) {
            return settings.HasKey(key);
        }
        public static string GetDescription(string key) {
            return settings.GetDescription(key);
        }
    }
}