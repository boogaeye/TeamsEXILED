﻿using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;
using Exiled.API.Enums;

namespace TeamsEXILED.API
{
    public class Subteams
    {
        [Description("Sets the Subclasses name")]
        public string Name { get; set; }

        [Description("sets the subclasses HP")]
        public int HP { get; set; } = 100;

        [Description("sets what this class has")]
        public string[] Inventory { get; set; }

        [Description("Define ammo so that this class has this ammo")]
        public Dictionary<AmmoType, uint> Ammo { get; set; } = new Dictionary<AmmoType, uint>() { { AmmoType.Nato556, 0u }, { AmmoType.Nato762, 0u }, { AmmoType.Nato9, 0u } };

        [Description("Sets what this role is supposed to look like")]
        public RoleType ModelRole { get; set; }

        [Description("sets the role name on the player to easily define who you are")]
        public string RoleName { get; set; }

        [Description("What message is displayed for the role itself")]
        public string RoleMessage { get; set; }

        [Description("the amount of players that can be changed to this role when spawning setting this to -1 will make the rest of the players this Subteam")]
        public int NumOfAllowedPlayers { get; set; } = -1;
    }
}