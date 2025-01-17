﻿namespace AdvancedTeamCreation.API
{
    using System.ComponentModel;
    using System.Collections.Generic;
    using Exiled.API.Enums;

    public class SubTeam
    {
        [Description("Sets the Subclasses name")]
        public string Name { get; set; } = null;

        [Description("sets the subclasses HP")]
        public int HP { get; set; } = 100;

        [Description("sets what this class has, use ids for CustomItems and ItemType names for normal items")]
        public string[] Inventory { get; set; } = new string[] { };

        [Description("If you have advancedsubclassing and you want to give a specific subclass to this subteam, put here the name of the advancedsubclass")]
        public string AdvancedSubclass { get; set; } = null;

        [Description("Define ammo so that this class has this ammo")]
        public Dictionary<AmmoType, uint> Ammo { get; set; } = new Dictionary<AmmoType, uint>() { { AmmoType.Nato556, 100 }, { AmmoType.Nato762, 100 }, { AmmoType.Nato9, 100 } };

        [Description("Sets what this role is supposed to look like")]
        public RoleType ModelRole { get; set; } = RoleType.None;

        [Description("sets the role name on the player to easily define who you are")]
        public string RoleName { get; set; } = null;

        [Description("What message is displayed for the role itself")]
        public string RoleMessage { get; set; } = null;

        [Description("the amount of players that can be changed to this role when spawning setting this to -1 will make the rest of the players this Subteam")]
        public int NumOfAllowedPlayers { get; set; } = -1;
    }
}