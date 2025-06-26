using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
// TODO Receivers
public partial class Party : Node
{
	static public List<Spell> Warrior = new List<Spell>{
		{new Spell("Protect", Spell.Target.SINGLE, Spell.Type.STATUS, 0, 2)}
	};
	static public List<Spell> Assassin = new List<Spell>{
		{new Spell("Shadow Ball", Spell.Target.SINGLE, Spell.Type.DAMAGE, 15, 3)},
		{new Spell("Small Heal", Spell.Target.SINGLE, Spell.Type.HEAL, 15, 3)}
	};
	static public List<Spell> SunMage = new List<Spell>{
		{new Spell("Sunny Day", Spell.Target.MULTI, Spell.Type.HEAL, 30, 5)},
		{new Spell("Re-Creation", Spell.Target.SINGLE, Spell.Type.STATUS, 1, 10)},
		{new Spell("Wall Of Fire", Spell.Target.MULTI, Spell.Type.DAMAGE, 45, 5)},
		{new Spell("Warm Sensation", Spell.Target.SINGLE, Spell.Type.HEAL, 60, 5)}
	};
	static public List<Spell> MoonMage = new List<Spell>{
		{new Spell("Oblivion", Spell.Target.SINGLE, Spell.Type.DAMAGE, 99999, 15)},
		{new Spell("Posion Gas", Spell.Target.MULTI, Spell.Type.STATUS, 0, 5)},
		{new Spell("Soul Drain", Spell.Target.SINGLE, Spell.Type.DAMAGE, 30, 5)},
		{new Spell("Dark Spikes", Spell.Target.MULTI, Spell.Type.DAMAGE, 60, 10)}
	};
	public static Dictionary<String, BattleActor> players = new Dictionary<String, BattleActor>{
		{"Christopher", new BattleActor("Christopher", 600, 5, 55, Warrior)},
		{"Abigail", new BattleActor("Abigail", 320, 25, 10, MoonMage)},
		{"Samuel", new BattleActor("Samuel", 280, 10, 50, Assassin)},
		{"Lucia", new BattleActor("Lucia", 400, 35, 10, SunMage)}
	};
	public static BattleActor[] party = players.Values.ToArray();
	public Party(){
		Utility.SetKeys(players);
	}
}
