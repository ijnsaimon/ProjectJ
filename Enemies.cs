using Godot;
using System;
using System.Collections.Generic;

public partial class Enemies : Node
{
	public Dictionary<String, BattleActor> data;
	public override void _Ready(){
	}
	public Enemies(){
		var batTemplate = new BattleActor{
			name = "Bat",
			max_hp = 130,
			strength = 20,
			Frames = GD.Load<SpriteFrames>("res://Battle_UI_Art/Enemies/batFrames.tres")
		};
		var demonTemplate = new BattleActor{
			name = "Little Demon",
			max_hp = 210,
			strength = 60,
			Frames = GD.Load<SpriteFrames>("res://Battle_UI_Art/Enemies/demonFrames.tres")
		};
		var skeletonTemplate = new BattleActor{
			name = "Skeleton Servant",
			max_hp = 400,
			strength = 40,
			Frames = GD.Load<SpriteFrames>("res://Battle_UI_Art/Enemies/skeletonFrames.tres")
		};
		data = new Dictionary<String, BattleActor>{
			{"Bat", batTemplate},
			{"Little Demon", demonTemplate},
			{"Skeleton Servant", skeletonTemplate}
		};
	}
}
