﻿namespace AdvancedTeamCreation.Helper
{
    using System.Collections.Generic;
    using System.Linq;
    using API;
    using Exiled.API.Features;
    using MEC;
    using Exiled.API.Enums;
    using static AdvancedTeamCreation;

    public static class TeamMethods
    {
        // Not in use
        public static void RefNextTeamSpawn()
        {
            Log.Debug("Getting Team Referances", Instance.Config.Debug);
            var list = Instance.Config.Teams.Where(x => x.SpawnTypes.Contains(Respawn.NextKnownTeam) && x.Active == true).ToList();
            if (list.Count == 0)
            {
                Instance.EventHandlers.HasReference = true;
                return;
            }

            Log.Debug("Got list", Instance.Config.Debug);
            var team = list[Rand.Next(0, list.Count)];
            Log.Debug("Got team", Instance.Config.Debug);
            var handler = new TeamEvents.ReferencingTeamEventArgs(team, Respawning.SpawnableTeamType.None);
            Log.Debug("Got Handler and invoking", Instance.Config.Debug);
            handler.StartInvoke();
        }

        public static void RefNextTeamSpawn(Respawning.SpawnableTeamType spawnableTeamType)
        {
            Log.Debug("Getting Team Referances", Instance.Config.Debug);
            Log.Debug($"Spawning on side {spawnableTeamType}", Instance.Config.Debug);
            var list = Instance.Config.Teams.Where(x => x.SpawnTypes.Contains(spawnableTeamType) && x.Active == true).ToList();
            if (list.Count == 0)
            {
                Instance.EventHandlers.HasReference = true;
                return;
            }

            var team = list[Rand.Next(0, list.Count)];
            var handler = new TeamEvents.ReferencingTeamEventArgs(team, spawnableTeamType);
            handler.StartInvoke();
        }

        public static void ChangePlysToTeam(List<Player> p, AdvancedTeam team)
        {
            //finding teams
            Log.Debug("Got team " + team.Name + " from referance method", Instance.Config.Debug);
            int i = 0;
            int selectedSubclass = 0;

            foreach (Player y in p)
            {
                if (team.Subclasses[selectedSubclass].NumOfAllowedPlayers > i)
                {
                    y.SetAdvancedTeamSubteam(team, team.Subclasses[selectedSubclass]);
                    Log.Debug("allowed subteam " + team.Subclasses[selectedSubclass].Name + " from referance method", Instance.Config.Debug);
                }
                else if (team.Subclasses[selectedSubclass].NumOfAllowedPlayers == -1)
                {
                    y.SetAdvancedTeamSubteam(team, team.Subclasses[selectedSubclass]);
                    Log.Debug("allowed subteam " + team.Subclasses[selectedSubclass].Name + " from referance method with -1 players allowed(making everyone else this role)", Instance.Config.Debug);
                }
                else
                {
                    i = 0;
                    selectedSubclass++;
                    Log.Debug("Going to the next subclass, because the max number of players is full", Instance.Config.Debug);
                }
                i++;
            }
        }

        public static void ChangeTeam(Player p, AdvancedTeam t, SubTeam s, bool keepInv = false)
        {
            var handler = new TeamEvents.SettingPlayerTeamEventArgs(t, s, p, keepItems:keepInv);
            handler.StartInvoke();
        }

        public static void RemoveTeamReference()
        {
            if (Instance.EventHandlers.RemoveChosenTeam != null)
            {
                if (Instance.EventHandlers.RemoveChosenTeam.IsRunning)
                {
                    Timing.KillCoroutines(Instance.EventHandlers.RemoveChosenTeam);
                }

                var cor = Timing.RunCoroutine(RemoveReferenceCouroutine());
                Instance.EventHandlers.RemoveChosenTeam = cor;
                Instance.EventHandlers.coroutineHandle.Add(cor);
            }
        }

        private static IEnumerator<float> RemoveReferenceCouroutine()
        {
            yield return Timing.WaitForSeconds(5f);
            if (assemblyTimer)
            {
                Methods.DefaultTimerConfig();
            }

            Instance.EventHandlers.chosenTeam = null;
            Instance.EventHandlers.HasReference = false;
        }

        public static AdvancedTeam[] DefaultTeams = new AdvancedTeam[]{ new AdvancedTeam {
            Active = true,
            Name = "goc",
            Subclasses = new SubTeam[]{ new SubTeam {
                Name = "rookie", Ammo = new Dictionary<AmmoType, uint>(){ { AmmoType.Nato762, 200 } }, Inventory = new string[]{ "KeycardNTFLieutenant", "Radio", "Medkit", "5", "2", "2", "0" }, HP = 135, ModelRole = RoleType.ChaosInsurgency, RoleName = "<color=yellow>GOC</color>", RoleMessage = "You are the GOC"
            } },
            Friendlys = new string[] { "goc" },
            Requirements = new string[] { "scp", "cdp", "tta", "rsc", "opcf", "aes" },
            SpawnTypes = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.ChaosInsurgency },
            CassieMessageChaosMessage = "pitch_0.6 .g5 .g5 .g5 pitch_1 the g o c has entered the facility bell_end",
            CassieMessageChaosAnnounceChance = 100,
            Chance = 65,
            Color = "yellow",
            Neutral = new string[]{ "mtf", "chi", "gru" }
        }, new AdvancedTeam{
            Active = true,
            Name = "gru",
            Subclasses = new SubTeam[]{ new SubTeam {
                Name = "rookie", Ammo = new Dictionary<AmmoType, uint>(){ { AmmoType.Nato762, 200 } }, Inventory = new string[]{ "GunE11SR", "KeycardChaosInsurgency", "Adrenaline", "WeaponManagerTablet", "GrenadeFlash", "7", "6" }, HP = 150, ModelRole = RoleType.FacilityGuard, RoleName = "<color=yellow>GRU</color>", RoleMessage = ""
            } },
            Friendlys = new string[] { "gru" },
            Requirements = new string[] { "scp", "tta", "cdp", "rsc", "aes" },
            SpawnTypes = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.NineTailedFox },
            CassieMessageMTFSpawn = ".g5 .g5 the g r u has entered the facility. there are {SCP} scpsubjects",
            Chance = 65,
            Color = "yellow",
            Neutral = new string[]{ "mtf", "chi", "goc" },
        },new AdvancedTeam{
            Active = true,
            Name = "tta",
            Subclasses = new SubTeam[]{ new SubTeam {
                Name = "officer", Ammo = new Dictionary<AmmoType, uint>(){ { AmmoType.Nato9, 200 } }, Inventory = new string[]{ "GunUSP", "KeycardFacilityManager", "Adrenaline", "WeaponManagerTablet", "0", "0" }, HP = 100, ModelRole = RoleType.NtfScientist, RoleName = "<color=red>TTA</color>", RoleMessage = "You are the TTA\nKill everything in sight"
            } },
            Friendlys = new string[] { "tta" },
            Requirements = new string[] { "chi", "scp", "goc", "gru", "mtf", "cdp", "rsc", "opcf", "aes" },
            SpawnTypes = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.NineTailedFox, Respawning.SpawnableTeamType.ChaosInsurgency },
            CassieMessageMTFSpawn = "pitch_0.1 .g3 .g3 .g3 pitch_1 The Tactical Target Agent C has entered the facility bell_end",
            CassieMessageChaosAnnounceChance = 75,
            CassieMessageChaosMessage = "pitch_0.1 .g3 .g3 .g3 pitch_1 The Tactical Target Agent C unit {nato} {SCP} has entered the facility bell_end",
            Chance = 50,
            Color = "red",
            SpawnLocation = SpawnLocation.SurfaceNuke
        },new AdvancedTeam{
            Active = true,
            Name = "opcf",
            Subclasses = new SubTeam[]{
                new SubTeam{
                    Name = "commander",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 } },
                    HP = 160,
                    ModelRole = RoleType.FacilityGuard,
                    RoleName = "OPCF Commander",
                    RoleMessage = "You are part of the branch of the NTF\n<color=blue>Operation Chaos Force</color>\nKill everything you see and help the MTF",
                    Inventory = new string[]{ "GunLogicer", "GunProject90", "KeycardChaosInsurgency", "Radio", "Medkit", "Medkit", "0" },
                    NumOfAllowedPlayers = 1
                },
                new SubTeam{
                    Name = "officer",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 } },
                    HP = 120,
                    ModelRole = RoleType.NtfLieutenant,
                    RoleName = "OPCF Officer",
                    RoleMessage = "You are part of the branch of the NTF\n<color=blue>Operation Chaos Force</color>\nKill everything you see and help the MTF",
                    Inventory = new string[]{ "GunLogicer", "GunProject90", "KeycardNTFLieutenant", "Radio", "Medkit", "Painkillers", "0" },
                    NumOfAllowedPlayers = 2
                },
                new SubTeam{
                    Name = "cadet",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 } },
                    HP = 100,
                    ModelRole = RoleType.NtfCadet,
                    RoleName = "OPCF Cadet",
                    RoleMessage = "You are part of the branch of the NTF\n<color=blue>Operation Chaos Force</color>\nKill everything you see and help the MTF",
                    Inventory = new string[]{ "GunProject90", "KeycardGuard", "Radio", "Painkillers", "GrenadeFlash", "0" },
                    NumOfAllowedPlayers = -1
                },
            },
            Friendlys = new string[] { "opcf", "mtf" },
            Requirements = new string[] { "chi", "scp", "goc", "gru", "cdp", "tta", "aes" },
            SpawnTypes = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.ChaosInsurgency },
            CassieMessageChaosAnnounceChance = 100,
            CassieMessageChaosMessage = "pitch_0.1 .g5 .g5 .g5 pitch_1 operation chaos force has entered the facility bell_end",
            Chance = 100,
            Color = "green",
            Neutral = new string[]{ "rsc" },
            SpawnLocation = SpawnLocation.Escape
        }, new AdvancedTeam{
            Active = true,
            Name = "aes",
            Subclasses = new SubTeam[]{
                new SubTeam{
                    Name = "commander",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 }, { AmmoType.Nato762, 450 } },
                    HP = 100,
                    ModelRole = RoleType.ChaosInsurgency,
                    RoleName = "AES Commander",
                    RoleMessage = "You are to eliminate all scp subjects",
                    Inventory = new string[]{ "GunProject90", "KeycardO5", "Medkit", "Medkit", "WeaponManagerTablet", "Radio", "6" },
                    NumOfAllowedPlayers = 1
                },
                new SubTeam{
                    Name = "firstofficer",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 }, { AmmoType.Nato762, 450 } },
                    HP = 100,
                    ModelRole = RoleType.ChaosInsurgency,
                    RoleName = "AES First Officer",
                    RoleMessage = "You are to eliminate all scp subjects",
                    Inventory = new string[]{ "GunProject90", "KeycardChaosInsurgency", "Medkit", "Medkit", "WeaponManagerTablet", "Radio" },
                    NumOfAllowedPlayers = 1
                },
                new SubTeam{
                    Name = "officer",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 }, { AmmoType.Nato762, 450 } },
                    HP = 100,
                    ModelRole = RoleType.ChaosInsurgency,
                    RoleName = "AES Officer",
                    RoleMessage = "You are to eliminate all scp subjects",
                    Inventory = new string[]{ "GunMP7", "KeycardZoneManager", "Medkit", "Medkit", "WeaponManagerTablet" },
                    NumOfAllowedPlayers = 2
                },
                new SubTeam{
                    Name = "rookie",
                    Ammo = new Dictionary<AmmoType, uint>{ { AmmoType.Nato9, 450 }, { AmmoType.Nato762, 450 } },
                    HP = 100,
                    ModelRole = RoleType.ChaosInsurgency,
                    RoleName = "AES rookie",
                    RoleMessage = "You are to eliminate all scp subjects",
                    Inventory = new string[]{ "GunMP7", "KeycardZoneManager", "Medkit", "WeaponManagerTablet", "WeaponManagerTablet" },
                    NumOfAllowedPlayers = -1
                },
            },
            Friendlys = new string[] { "aes" },
            Requirements = new string[] { "scp", "tta" },
            SpawnTypes = new Respawning.SpawnableTeamType[] { Respawning.SpawnableTeamType.NineTailedFox },
            CassieMessageChaosAnnounceChance = 0,
            CassieMessageChaosMessage = "",
            Chance = 50,
            Color = "red",
            Neutral = new string[]{ "mtf", "opcf", "goc", "gru", "chi", "rsc", "cdp" },
            SpawnLocation = SpawnLocation.SCP106,
            CassieMessageMTFSpawn = "cassie pitch_0.6 .g3 .g3 pitch_1 the arrival of the anomaly emergency squad {nato} {unit} has entered the facility. pitch_0.96 please escort2 all scpsubjects to surface zone. pitch_1 all scpsubjects need to be secured. please wait inside of your designated evacuation shelters until this emergency has been completed. there are {SCP} scpsubjects remaining."
        }
        };

        public static string[] NormalTeamsNames = new string[]
        {
            "scp",
            "rsc",
            "cdp",
            "chi",
            "mtf",
            "rip",
            "tut"
        };
    }
}
