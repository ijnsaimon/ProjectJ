using Godot;
using System;
using System.Threading;

public partial class BattleEnemy : BattleButton
{
	[Export]
	public String EnemyType {get; set;} = "Bat";
	[Signal]
	public delegate void EnemyDiedEventHandler(BattleEnemy enemy);
	[Signal]
	public delegate void AtbReadyEventHandler();
	ATBBar atb_bar;
	public override void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		atb_bar = GetNode<ATBBar>("ATBBar");
		atb_bar.MaxValueReached += OnAtbBarFilled;
		atb_bar.Hide();
		sprite.Play("Idle");
		int frameCount = sprite.SpriteFrames.GetFrameCount("Idle");
		if(frameCount > 0)
		{
			sprite.Frame = GD.RandRange(0, frameCount - 1);
		}
		
		var enemiesManager = GetNode<Enemies>("/root/Enemies");
		
		if(enemiesManager.data.TryGetValue(EnemyType, out BattleActor enemyDataTemplate))
		{
			BattleActor enemyData = (BattleActor)enemyDataTemplate.Duplicate();
			Initialize(enemyData);
			//GD.Print(data.getName());
		}
		else
		{
			GD.Print("Brak danych");
		}
	}
	public void ResetAtb(){
		atb_bar.Reset();
	}
	public void AtbPause(){
		atb_bar.Pause();
	}
	public void AtbResume(){
		atb_bar.Resume();
	}
	public void OnAtbBarFilled(){
		EmitSignal(SignalName.AtbReady);
	//	GD.Print("Atb filled");
	}
	public override void OnHpChanged(int hp, int change){
		base.OnHpChanged(hp, change);
		if(!data.isAlive)
		{
			EmitSignal(SignalName.EnemyDied, this);
			Tween tween2 = CreateTween();
			tween2.TweenProperty(this, "modulate", new Color(1,1,1,0), 0.9f);
		}
	}
}
