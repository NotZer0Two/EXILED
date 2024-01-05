// -----------------------------------------------------------------------
// <copyright file="CustomTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;

    using PlayerRoles;
    using Respawning;
    using Respawning.NamingRules;

    /// <summary>
    /// Abstract base class representing a custom team, providing a foundational structure for custom team management.
    /// </summary>
    /// <remarks>
    /// The <see cref="CustomTeam"/> class forms the basis for creating and managing custom teams within the game architecture.
    /// <para>
    /// This class implements the <see cref="IEquatable{CustomTeam}"/> interface, facilitating straightforward equality comparisons, and also implements <see cref="IEquatable{UInt16}"/> for integer-based comparisons.
    /// <br/>It serves as a versatile framework for handling custom team-related functionalities and interactions.
    /// </para>
    /// </remarks>
    public abstract class CustomTeam : TypeCastObject<CustomTeam>, IEquatable<CustomTeam>, IEquatable<int>
    {
        private static readonly Dictionary<Player, CustomTeam> PlayersValue = new();
        private static readonly List<CustomTeam> Registered = new();
        private uint tickets;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> which contains all registered <see cref="CustomTeam"/>'s.
        /// </summary>
        public static IEnumerable<CustomTeam> List => Registered;

        /// <summary>
        /// Gets all players and their respective <see cref="CustomTeam"/>.
        /// </summary>
        public static IReadOnlyDictionary<Player, CustomTeam> Manager => PlayersValue;

        /// <summary>
        /// Gets all players belonging to a <see cref="CustomTeam"/>.
        /// </summary>
        public static IEnumerable<Player> Players => PlayersValue.Keys.ToHashSet();

        /// <summary>
        /// Gets or sets a collection of <see cref="Type"/> instances representing all custom role types offered as units.
        /// </summary>
        /// <remarks>
        /// This property provides access to a curated collection of <see cref="Type"/> objects, encapsulating all available custom role types within the context of units.
        /// <br/>The collection is designed to be both queried and modified as needed to accommodate dynamic scenarios within the game architecture.
        /// </remarks>
        public virtual IEnumerable<Type> Units { get; protected set; } = new Type[] { };

        /// <summary>
        /// Gets or sets the <see cref="CustomTeam"/>'s id.
        /// </summary>
        public virtual uint Id { get; protected set; }

        /// <summary>
        /// Gets the relative spawn probability of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual int Probability { get; }

        /// <summary>
        /// Gets the <see cref="SpawnableTeamType"/> which is being spawned from.
        /// </summary>
        public virtual SpawnableTeamType RespawnTeam { get; }

        /// <summary>
        /// Gets the name of the <see cref="CustomTeam"/>.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> is enabled.
        /// </summary>
        public virtual bool IsEnabled => true;

        /// <summary>
        /// Gets the name of the <see cref="CustomTeam"/> to be displayed.
        /// </summary>
        public virtual string DisplayName { get; }

        /// <summary>
        /// Gets the color of the <see cref="Name"/> to be displayed.
        /// </summary>
        public virtual string DisplayColor { get; }

        /// <summary>
        /// Gets the size of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual uint Size { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> is using tickets.
        /// </summary>
        public virtual bool UseTickets { get; }

        /// <summary>
        /// Gets the tickets of the <see cref="CustomTeam"/>.
        /// </summary>
        public virtual uint Tickets { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> can be spawned without any scp alive.
        /// </summary>
        public virtual bool CanSpawnWithoutScps => true;

        /// <summary>
        /// Gets the required <see cref="RoleTypeId"/> to allow the <see cref="CustomTeam"/> to spawn.
        /// </summary>
        public virtual RoleTypeId RequiredRoleToSpawn => RoleTypeId.None;

        /// <summary>
        /// Gets the required <see cref="CustomRole"/> to allow the <see cref="CustomTeam"/> to spawn.
        /// </summary>
        public virtual uint RequiredCustomRoleToSpawn { get; }

        /// <summary>
        /// Gets the required leading teams to win.
        /// </summary>
        public virtual Team[] LeadingTeamsToWin => new Team[] { };

        /// <summary>
        /// Gets a value indicating whether the <see cref="CustomTeam"/> is registered.
        /// </summary>
        public bool IsRegistered => Registered.Contains(this);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Player"/> which contains all players belonging to this <see cref="CustomTeam"/>.
        /// </summary>
        public IEnumerable<Player> Owners => PlayersValue.Where(x => x.Value == this).Select(x => x.Key);

        /// <summary>
        /// Gets a random <see cref="CustomRole"/> from the <see cref="Units"/>.
        /// </summary>
        public CustomRole RandomUnit => CustomRole.Get(Units.Random());

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomTeam left, object right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomTeam right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomTeam left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomTeam right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomTeam left, CustomTeam right)
        {
            if (left is null)
            {
                if (right is null)
                    return true;

                return false;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomTeam"/> and <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomTeam"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomTeam"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomTeam left, CustomTeam right) => !(left.Id == right.Id);

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(uint id, out CustomTeam customTeam) => customTeam = Get(id);

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> given the specified name.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(string name, out CustomTeam customTeam) => customTeam = Registered.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomTeam"/> belonging to the specified <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Pawn"/> to look for.</param>
        /// <param name="customTeam">The found <see cref="CustomTeam"/>, null if not registered.</param>
        /// <returns><see langword="true"/> if a <see cref="CustomTeam"/> is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryGet(Pawn player, out CustomTeam customTeam) => PlayersValue.TryGetValue(player, out customTeam);

        /// <summary>
        /// Tries to spawn the specified <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="customTeam">The <see cref="CustomTeam"/> to be spawned.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(CustomTeam customTeam)
        {
            if (!Player.Get(p => p.IsDead).Any() || customTeam is null)
                return false;

            customTeam.Respawn();

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given the specified id.
        /// </summary>
        /// <param name="id">The specified id.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(uint id)
        {
            if (!Player.Get(p => p.IsDead).Any() || TryGet(id, out CustomTeam customTeam))
                return false;

            customTeam.Respawn();

            return true;
        }

        /// <summary>
        /// Tries to spawn a player as a <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> unit to be assigned.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Player player, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Spawn(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn a player as a <see cref="CustomTeam"/> unit given the specified id.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        /// <param name="id">The specified id.</param>
        /// <returns><see langword="true"/> if the player was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(Player player, uint id)
        {
            if (!TryGet(id, out CustomTeam customTeam))
                return false;

            customTeam.Spawn(player);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="IEnumerable{T}"/> of <see cref="Player"/> as a <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> unit to be assigned.</param>
        /// <returns><see langword="true"/> if the players were spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(IEnumerable<Player> players, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Respawn(players);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="IEnumerable{T}"/> of <see cref="Player"/> as a <see cref="CustomTeam"/> unit given the specified id.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="id">The specified id.</param>
        /// <returns><see langword="true"/> if the players were spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(IEnumerable<Player> players, uint id)
        {
            if (!TryGet(id, out CustomTeam customTeam))
                return false;

            customTeam.Respawn(players);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given a specified amount of units.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        /// <param name="customTeam">The <see cref="CustomTeam"/> to be spawned.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(uint amount, CustomTeam customTeam)
        {
            if (customTeam is null)
                return false;

            customTeam.Respawn(amount);

            return true;
        }

        /// <summary>
        /// Tries to spawn a <see cref="CustomTeam"/> given the specified id.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        /// <param name="id">The specified id.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was spawned; otherwise, <see langword="false"/>.</returns>
        public static bool TrySpawn(uint amount, uint id)
        {
            if (TryGet(id, out CustomTeam customTeam))
                return false;

            customTeam.Respawn(amount);

            return true;
        }

        /// <summary>
        /// Enables all the custom teams present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all the enabled custom teams.</returns>
        public static IEnumerable<CustomTeam> EnableAll()
        {
            List<CustomTeam> customTeams = new();
            foreach (Type type in Assembly.GetCallingAssembly().GetTypes())
            {
                CustomTeamAttribute attribute = type.GetCustomAttribute<CustomTeamAttribute>();
                if (type.BaseType != typeof(CustomTeam) || attribute is null)
                    continue;

                CustomTeam customTeam = Activator.CreateInstance(type) as CustomTeam;

                if (!customTeam.IsEnabled)
                    continue;

                if (customTeam.TryRegister(attribute))
                    customTeams.Add(customTeam);
            }

            if (customTeams.Count() != Registered.Count())
                Log.SendRaw($"{customTeams.Count()} custom teams have been successfully registered!", ConsoleColor.Cyan);

            return customTeams;
        }

        /// <summary>
        /// Disables all the custom teams present in the assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomTeam"/> which contains all the disabled custom teams.</returns>
        public static IEnumerable<CustomTeam> DisableAll()
        {
            List<CustomTeam> customTeams = new();
            customTeams.AddRange(Registered.Where(customTeam => customTeam.TryUnregister()));

            Log.SendRaw($"{customTeams.Count()} custom teams have been successfully unregistered!", ConsoleColor.Cyan);

            return customTeams;
        }

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> given the specified id.
        /// </summary>
        /// <param name="id">The id to check.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get(uint id) => Registered.FirstOrDefault(x => x == id);

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> given the specified name.
        /// </summary>
        /// <param name="name">The specified name.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get(string name) => Registered.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Gets a <see cref="CustomTeam"/> from a <see cref="Player"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>The <see cref="CustomTeam"/> matching the search or <see langword="null"/> if not registered.</returns>
        public static CustomTeam Get(Player player)
        {
            if (!PlayersValue.TryGetValue(player, out CustomTeam customTeam))
                return default;

            return customTeam;
        }

        /// <summary>
        /// Determines whether the provided id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(int id) => Id == id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="cr">The custom role to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomTeam cr) => cr is not null && (ReferenceEquals(this, cr) || Id == cr.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (Equals(obj as CustomTeam))
                return true;

            try
            {
                return Equals((int)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Spawns a <see cref="Player"/> as a <see cref="CustomTeam"/> unit.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to be spawned.</param>
        public void Spawn(Player player)
        {
            if (player is null)
                return;

            CustomRole.Spawn(player.Cast<Pawn>(), RandomUnit);
            PlayersValue.Add(player, this);
        }

        /// <summary>
        /// Forces a respawn wave given the specified amount of units.
        /// </summary>
        /// <param name="amount">The amount of units to be spawned.</param>
        public void Respawn(uint amount)
        {
            if (amount <= 0)
                return;

            Player[] players = Player.Get(Team.Dead).ToArray();

            if (players.IsEmpty())
                return;

            players.ForEach(player => Spawn(player));

            if (RespawnTeam is SpawnableTeamType.NineTailedFox && UnitNamingRule.TryGetNamingRule(SpawnableTeamType.NineTailedFox, out UnitNamingRule rule))
                UnitNameMessageHandler.SendNew(SpawnableTeamType.NineTailedFox, rule);
        }

        /// <summary>
        /// Forces a respawn wave given a <see cref="IEnumerable{T}"/> of <see cref="Player"/>.
        /// </summary>
        /// <param name="players">The players to be spawned.</param>
        /// <param name="keepSize">A value indicating whether the team size should be the same as the specified one.</param>
        public void Respawn(IEnumerable<Player> players, bool keepSize = true)
        {
            if (players is null || players.IsEmpty())
                return;

            if (UseTickets && tickets > 0)
                tickets--;

            int count = 0;
            foreach (Player player in players)
            {
                if (!keepSize && count >= Size)
                    break;

                if (player is null)
                    continue;

                Spawn(player);
                count++;
            }

            if (RespawnTeam is SpawnableTeamType.NineTailedFox && UnitNamingRule.TryGetNamingRule(SpawnableTeamType.NineTailedFox, out UnitNamingRule rule))
                UnitNameMessageHandler.SendNew(SpawnableTeamType.NineTailedFox, rule);
        }

        /// <summary>
        /// Forces a respawn wave.
        /// </summary>
        public void Respawn() => Respawn(Size);

        /// <summary>
        /// Adds respawn tickets to the current <see cref="CustomTeam"/> instance given a specified amount.
        /// </summary>
        /// <param name="amount">The amount of tickets to be added to.</param>
        public void AddTickets(uint amount) => tickets += amount;

        /// <summary>
        /// Removes respawn tickets to the current <see cref="CustomTeam"/> instance given a specified amount.
        /// </summary>
        /// <param name="amount">The amount of tickets to remove.</param>
        public void RemoveTickets(uint amount)
        {
            if (tickets < amount)
                return;

            tickets -= amount;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomTeam"/>.
        /// </summary>
        /// <param name="attribute">The specified <see cref="CustomTeamAttribute"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was registered; otherwise, <see langword="false"/>.</returns>
        internal bool TryRegister(CustomTeamAttribute attribute = null)
        {
            if (!Registered.Contains(this))
            {
                if (attribute is not null && Id == 0)
                {
                    if (Id == 0)
                    {
                        if (attribute.Id != 0)
                            Id = attribute.Id;
                        else
                            throw new ArgumentException($"Unable to register {Name}. The ID 0 is reserved for special use.");
                    }

                    if (attribute.Types is not null && !attribute.Types.IsEmpty())
                        Units = Units.Concat(attribute.Types);
                }

                if (Registered.Any(x => x.Id == Id))
                {
                    Log.Warn($"Unable to register {Name}. Another team with the same ID already exists: {Registered.FirstOrDefault(x => x.Id == Id)}");

                    return false;
                }

                Registered.Add(this);

                return true;
            }

            Log.Warn($"Unable to register {Name}. Team already exists.");

            return false;
        }

        /// <summary>
        /// Tries to register a <see cref="CustomTeam"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the <see cref="CustomTeam"/> was unregistered; otherwise, <see langword="false"/>.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Contains(this))
            {
                Log.Debug($"Unable to unregister {Name}. Team is not yet registered.");

                return false;
            }

            Registered.Remove(this);

            return true;
        }
    }
}
