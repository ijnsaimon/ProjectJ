using Godot;
using System;
using System.Collections.Generic;


[GlobalClass]
public partial class Spell : Resource
{
	[Export]
	public String spellName = "null";
	public enum Target{
		SINGLE,
		MULTI,
	}
	public enum Type{
		DAMAGE,
		HEAL,
		STATUS
	}
	public enum Receiver{
		ENEMY,
		PLAYER
	}
	[Export]
	public Type type;
	[Export]
	public Target target;
	[Export]
	public Receiver receiver;
	[Export]
	public int hpChange = 0;
	[Export]
	public int mpCost = 0;
	[ExportGroup("VFX")]
	[Export]
	public Color EffectColor {get; set;} = new Color(1, 1, 1, 0);
	[Export]
	public float EffectDuration {get; set;} = 0.5f;
	public Spell(){
		
	}
	public Spell(String name, Target enemy, Type effect, Receiver guyToTarget, int damage, int cost){
		spellName = name;
		target = enemy;
		type = effect;
		receiver = guyToTarget;
		hpChange = damage;
		mpCost = cost;
	}
}
