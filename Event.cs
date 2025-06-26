using Godot;
using System;
using System.Collections.Generic;

public partial class Event : GodotObject
{
	public BattleActor Actor {get; private set;}
	public battle.Actions Action {get; private set;}
	public List<BattleActor> Targets {get; private set;}
	public Event(BattleActor actor, battle.Actions action, List<BattleActor> targets){
		Actor = actor;
		Action = action;
		Targets = targets;
	}
}
