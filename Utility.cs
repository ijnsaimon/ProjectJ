using Godot;
using System;
using System.Collections.Generic;

public partial class Utility : Node
{
	public static void SetKeys(Dictionary<String, BattleActor> dict){
			foreach(var key in dict.Keys){
				dict[key].ResourceName = key;
				dict[key].SetName(key);
			}
		}
}
