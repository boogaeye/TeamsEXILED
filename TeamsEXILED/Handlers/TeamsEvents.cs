﻿using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Enums;
using Exiled.CustomItems.API.Features;
using System.Linq;
using MEC;
using TeamsEXILED.API;

namespace TeamsEXILED.Handlers
{
    public class TeamsEvents
    {
        private readonly Plugin<Config> plugin;

        public TeamsEvents(Plugin<Config> plugin)
        {
            this.plugin = plugin;
        }

        public void OnReferencingTeam(TeamEvents.ReferencingTeamEventArgs ev)
        {
            Log.Debug($"Forceteam: {ev.ForceTeam}\nIsAllowed: {ev.IsAllowed}\nTeamName: {ev.Team.Name}", this.plugin.Config.Debug);

            if (ev.IsAllowed == false)
            {
                return;
            }

            if (ev.Team == null)
            {
                return;
            }

            if (ev.ForceTeam)
            {
                MainPlugin.Singleton.EventHandlers.chosenTeam = ev.Team;
                MainPlugin.Singleton.EventHandlers.HasReference = true;
                MainPlugin.Singleton.EventHandlers.ForcedTeam = true;
            }
            else
            {
                Log.Debug("Next Known Spawn is " + ev.Spawning, MainPlugin.Singleton.Config.Debug);

                if (MainPlugin.Singleton.EventHandlers.random.Next(0, 100) <= ev.Team.Chance)
                {
                    MainPlugin.Singleton.EventHandlers.chosenTeam = ev.Team;
                    MainPlugin.Singleton.EventHandlers.HasReference = true;
                    Log.Debug("Next Known Chosen Team is " + MainPlugin.Singleton.EventHandlers.chosenTeam.Name, MainPlugin.Singleton.Config.Debug);
                }
                else
                {
                    MainPlugin.Singleton.EventHandlers.chosenTeam = null;
                }
            }
        }

        public void OnSettingPlayerTeam(TeamEvents.SettingPlayerTeamEventArgs ev)
        {
            if (!Round.IsStarted)
            {
                ev.IsAllowed = false;
            }

            if (ev.IsAllowed == false)
            {
                return;
            }

            var p = ev.Player;
            var team = ev.Team;
            var subteams = ev.Subclass;

            p.SetRole(subteams.ModelRole, true);
            p.Health = subteams.HP;
            p.MaxHealth = subteams.HP;

            if (team.spawnLocation != SpawnLocation.Normal)
            {
                var point = MainPlugin.Singleton.EventHandlers.fixedpoints.First(x => x.Type == team.spawnLocation);
                switch (team.spawnLocation)
                {
                    case SpawnLocation.Escape:
                        {
                            p.Position = point.Position;
                            p.Rotations = point.Direction;
                            break;
                        }
                    case SpawnLocation.SCP106:
                        {
                            if (!Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                    case SpawnLocation.SurfaceNuke:
                        {
                            p.Position = point.Position;
                            p.Rotations = point.Direction;
                            break;
                        }
                    case SpawnLocation.SCP012:
                        {
                            if (!Map.IsLCZDecontaminated && !Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                    case SpawnLocation.SCP079:
                        {
                            if (!Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                    case SpawnLocation.SCP096:
                        {
                            if (!Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                    case SpawnLocation.SCP173:
                        {
                            if (!Map.IsLCZDecontaminated && !Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                    case SpawnLocation.Shelter:
                        {
                            if (!Warhead.IsDetonated)
                            {
                                p.Position = point.Position;
                                p.Rotations = point.Direction;
                            }
                            break;
                        }
                }
            }

            var ihandler = new TeamEvents.AddingInventoryItemsEventArgs(p, subteams, keepInv:ev.KeepItems);

            ihandler.StartInvoke();

            if (MainPlugin.Singleton.Config.UseHints)
            {
                p.ShowHint(subteams.RoleMessage, 10);
            }
            else
            {
                p.Broadcast(10, subteams.RoleMessage);
            }

            MainPlugin.Singleton.EventHandlers.coroutineHandle.Add(Timing.CallDelayed(0.2f, () => 
            {
                /*if (MainPlugin.Singleton.EventHandlers.spawnableTeamType == Respawning.SpawnableTeamType.NineTailedFox)
                {
                    p.UnitName = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[MainPlugin.Singleton.EventHandlers.respawns].UnitName;
                }*/

                p.InfoArea &= ~PlayerInfoArea.Role;
                p.CustomInfo = subteams.RoleName;
                p.SetAdvancedTeam(ev.Team);
            }
            ));

            Log.Debug("Changing player " + p.Nickname + " to " + ev.Team.Name, MainPlugin.Singleton.Config.Debug);
        }

        public void OnAddingInventoryItems(TeamEvents.AddingInventoryItemsEventArgs ev)
        {
            Log.Debug($"Giving Inventory Items of the subclass {ev.Subteam.Name}, to {ev.Player.Nickname}", this.plugin.Config.Debug);
            if (ev.IsAllowed == false)
            {
                return;
            }

            if (ev.KeepInv)
            {
                // This leaves the items in the escape area
                ev.Player.DropItems();
            }

            ev.Player.ClearInventory();

            foreach (string i in ev.Subteam.Inventory)
            {
                if (int.TryParse(i, out int citem))
                {
                    CustomItem.TryGive(ev.Player, citem, plugin.Config.DisplayDescription);
                }
                else if (ItemType.TryParse<ItemType>(i, out ItemType item))
                {
                    ev.Player.AddItem(item);
                }
                else
                {
                    Log.Error($"The config item {i} of the subteam {ev.Subteam.Name} isn't valid");
                }
            }

            foreach (KeyValuePair<AmmoType, uint> a in ev.Subteam.Ammo)
            {
                ev.Player.Ammo[(int)a.Key] = a.Value;
            }
        }
    }
}
