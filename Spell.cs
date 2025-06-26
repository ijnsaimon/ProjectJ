using Godot;
using System;

public partial class Spell : GodotObject
{
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
	public Type type;
	public Target target;
	public Receiver receiver;
	public int hpChange = 0;
	public int mpCost = 0;
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
