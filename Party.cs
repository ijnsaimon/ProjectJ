using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
// TODO Receivers
public partial class Party : Node
{
	public static Dictionary<String, BattleActor> players;
	public static BattleActor[] party;
	public Party(){
		GD.Print("Party");
		var protectSpell = GD.Load<Spell>("res://spells/Protect.tres");
		var smallHealSpell = GD.Load<Spell>("res://spells/SmallHeal.tres");
		var shadowBallSpell = GD.Load<Spell>("res://spells/ShadowBall.tres");
		var sunnyDaySpell = GD.Load<Spell>("res://spells/SunnyDay.tres");
		var recreationSpell = GD.Load<Spell>("res://spells/Recreation.tres");
		var wallOfFireSpell = GD.Load<Spell>("res://spells/WallOfFire.tres");
		var warmthSpell = GD.Load<Spell>("res://spells/Warmth.tres");
		var oblivionSpell = GD.Load<Spell>("res://spells/Oblivion.tres");
		var poisonGasSpell = GD.Load<Spell>("res://spells/PoisonGas.tres");
		var soulDrainSpell = GD.Load<Spell>("res://spells/SoulDrain.tres");
		var darkSpikesSpell = GD.Load<Spell>("res://spells/DarkSpikes.tres");
		var warriorSpells = new List<Spell> {protectSpell};
		var assassinSpells = new List<Spell> {smallHealSpell, shadowBallSpell};
		var sunMageSpells = new List <Spell> {sunnyDaySpell, recreationSpell, wallOfFireSpell, warmthSpell};
		var moonMageSpells = new List <Spell> {oblivionSpell, poisonGasSpell, soulDrainSpell, darkSpikesSpell};
		var warriorFrames = GD.Load<SpriteFrames> ("res://Battle_UI_Art/Players/KnightIdle.tres");
		var moonFrames = GD.Load<SpriteFrames> ("res://Battle_UI_Art/Players/MoonMageIdle.tres");
		var sunFrames = GD.Load<SpriteFrames> ("res://Battle_UI_Art/Players/SunMageIdle.tres");
		var assassinFrames = GD.Load<SpriteFrames> ("res://Battle_UI_Art/Players/AssassinIdle.tres");
		players = new Dictionary<String, BattleActor>{
			{"Christopher", new BattleActor("Christopher", 600, 5, 55, warriorSpells, warriorFrames)},
			{"Abigail", new BattleActor("Abigail", 320, 25, 10, moonMageSpells, moonFrames)},
			{"Samuel", new BattleActor("Samuel", 280, 10, 50, assassinSpells, assassinFrames)},
			{"Lucia", new BattleActor("Lucia", 400, 35, 10, sunMageSpells, sunFrames)}
		};
		party = players.Values.ToArray();
		foreach(var player in party){
			GD.Print(player.name);
		}
	}
}
