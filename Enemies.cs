using Godot;
using System;
using System.Collections.Generic;

public partial class Enemies : Node
{
	public Dictionary<String, BattleActor> data = new Dictionary<String, BattleActor>{
		{"Bat", new BattleActor("Bat", 180)}
	};
	
	public Enemies(){
		Utility.SetKeys(data);
	}
}
