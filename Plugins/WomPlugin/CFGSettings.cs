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
*/﻿
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MCForge.World;
using MCForge.Core;
using MCForge.Utils;
using MCForge.Utils.Settings;

namespace Plugins.WoMPlugin
{
    public class CFGSettings : ExtraSettings {
        public CFGSettings(Level l)
            : base("CFGSettings_"+l.Name, new SettingNode[]{
            new SettingNode("server.name", ServerSettings.GetSetting("ServerName"), "The name of your server. (Top right line)"),
            new SettingNode("server.detail", ServerSettings.GetSetting("MOTD"), "The MOTD of the server. (Second line)"),
            new SettingNode("detail.user", "Welcome to my server! $name", "The User Detail Line text. (Third line)"),
            new SettingNode("server.sendwomid","true", "Causes the client to send the server a /womid (VERSION) message upon load."),
            new SettingNode("environment.fog", "1377559", "The RGB value (Decimal, not Hex) of the colour to use for the fog."),
            new SettingNode("environment.sky", "1377559", "The RGB value (Decimal, not Hex) of the colour to use for the sky."),
            new SettingNode("environment.cloud", "1377559", "The RGB value (Decimal, not Hex) of the colour to use for the clouds."),
            new SettingNode("environment.level", "0", "The elevation of \"ground level\" to use for this map. (Affects where the \"sea\" is on the map."),
            new SettingNode("environment.edge", "685cfceb13ccee86d3b93f163f4ac6e4a28347bf", null),
            new SettingNode("environment.terrain", "f3dac271d7bce9954baad46e183a6a910a30d13b", null),
            new SettingNode("environment.side", "7c0fdebeb6637929b9b3170680fa7a79b656c3f7", null)}, false)
        {
            this.l = l;
        }
        private Level l;
    }
}
