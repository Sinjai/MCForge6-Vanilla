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
using MCForge.World;
using MCForge.Core;
using MCForge.Utils;
using MCForge.Utils.Settings;

namespace Plugins.WoMPlugin
{
    public class WoMPluginSettings : ExtraSettings
    {
        public WoMPluginSettings()
            : base("WoMPluginSettings", new SettingNode[]{
            new SettingNode("notify-ops", "True" , "Notifies ops if a player using the WoM client joins."),
            new SettingNode("joinleave-alert", "True", "All join and leave messages will be transferred to the top-right corner as a temporary alert to all WoM users(Saves chat space)."),
        }, false) { }
    }
}
