using Godot;
using System;
using System.Collections.Generic;

public partial class BattleActor : Resource
{
	[Signal]
	public delegate void HpChangedEventHandler(int hp, int change);
	[Signal]
	public delegate void MpChangedEventHandler(int mewMp);
	[Export]
	public String name { get; set; } = "null";
	[Export]
	public int max_hp { get; set; } = 5;
	[Export]
	public int hp { get; set; } = 0;
	[Export]
	public int max_mp {get; set;} = 0;
	[Export]
	public int mp {get; set; } = 5;
	[Export]
	public int strength { get; set; } = 5;
	public List<Spell> Spells {get; set; } = null;
	public bool isAlive = true;
	public bool Poisoned = false;
	public bool Protected = false;
	public bool Defending = false;
	public BattleActor(){
		
	}
	public BattleActor(String Name, int MaxHp, int MaxMp, int Strength, List<Spell> spells){
		name = Name;
		max_hp = MaxHp;
		hp = max_hp;
		max_mp = MaxMp;
		mp = max_mp;
		strength = Strength;
		Spells = spells;
	}
	public void Initialize(){
		hp = max_hp;
		isAlive = true;
	}
	public int GetStrength(){
		return strength;
	}
	public int GetMaxHP(){
		return max_hp;
	}
	public BattleActor(String s){
		hp = max_hp;
		name = s;
	}
	public BattleActor(String s, int health){
		max_hp = health;
		hp = max_hp;
		name = s;
	}
	public void SetName(String s){
		name = s;
	}
	public String getName(){
		return name;
	}
	public void HealOrDamage(int val){
		if(!isAlive)
			return;
		int beg = hp;
		int change = 0;
		hp += val;
		hp = Mathf.Clamp(hp, 0, max_hp);
		change = Math.Abs(hp - beg);
		if(hp <= 0){
			isAlive = false;
		}
		EmitSignal(SignalName.HpChanged, hp, change);
	}
	public void UseMp(int val){
		if(mp < val){
			GD.Print("No mp");
		}
		else{
			mp -= val;
			EmitSignal(SignalName.MpChanged, mp);
		}
	}
}
