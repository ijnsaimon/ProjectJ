using Godot;
using System;

public partial class BattlePlayer : BattleButton
{
	[Signal]
	public delegate void PlayerDiedEventHandler(BattlePlayer player);
	VBoxContainer players;
	PlayerBar pBar;
	public override void _Ready(){
		BattleActor playerData = Party.party[GetIndex()];
		players = GetNode<VBoxContainer>("/root/Battle/BattleMenu/Players/Margin/Players");
		Initialize(playerData);
		sprite = GetNodeOrNull<AnimatedSprite2D>("Sprite");
		if (data.Frames != null)
		{
			sprite.SpriteFrames = data.Frames;
		}
		if(data.name == "Christopher"){
			sprite.Scale = new Vector2(2.0f, 2.0f);
		}
		else if(data.name == "Samuel"){
			sprite.Scale = new Vector2(2.5f, 2.5f);
		}
		sprite.Play("Idle");
		int frameCount = sprite.SpriteFrames.GetFrameCount("Idle");
		if(frameCount > 0)
		{
			sprite.Frame = GD.RandRange(0, frameCount - 1);
		}
		foreach(PlayerBar bar in players.GetChildren()){
			if(bar.GetIndex() == this.GetIndex()){
				pBar = bar;
			}
		}
	}
	public override void OnHpChanged(int hp, int change){
		base.OnHpChanged(hp, change);
		if(!data.isAlive){
			GD.Print($"{data.GetName()} defeated");
			EmitSignal(SignalName.PlayerDied, this);
			Tween tween2 = CreateTween();
			tween2.TweenProperty(this, "modulate", new Color(1,1,1,0), 0.9f);
			pBar.OnHpChanged(hp, change);
		}
	}
}
