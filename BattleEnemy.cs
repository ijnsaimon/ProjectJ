using Godot;
using System;
using System.Threading;

public partial class BattleEnemy : BattleButton
{
	[Export]
	public String EnemyType {get; set;}
	[Signal]
	public delegate void EnemyDiedEventHandler(BattleEnemy enemy);
	[Signal]
	public delegate void AtbReadyEventHandler();
	ATBBar atb_bar;
	public override void _Ready(){
		setEnemyType();
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		atb_bar = GetNode<ATBBar>("ATBBar");
		atb_bar.MaxValueReached += OnAtbBarFilled;
		atb_bar.Hide();
		
		var enemiesManager = GetNode<Enemies>("/root/Enemies");
		
		if(enemiesManager.data.TryGetValue(EnemyType, out BattleActor enemyDataTemplate))
		{
			BattleActor enemyData = (BattleActor)enemyDataTemplate.Duplicate();
			GD.Print(enemyData.max_hp);
			Initialize(enemyData);
			enemyData.hp = enemyData.max_hp;
			if (data.Frames != null)
			{
				sprite.SpriteFrames = data.Frames;
			}
			if(EnemyType == "Skeleton Servant"){
				sprite.FlipH = false;
			}
			sprite.Play("Idle");
			int frameCount = sprite.SpriteFrames.GetFrameCount("Idle");
			if(frameCount > 0)
			{
				sprite.Frame = GD.RandRange(0, frameCount - 1);
			}
			//GD.Print(data.getName());
		}
		else
		{
			GD.Print("Brak danych");
		}
	}
	public void setEnemyType(){
		int value = Global.RNG.Next(0,4);
		switch(value){
			case 0:
				EnemyType = "Bat";
				break;
			case 1:
				EnemyType = "Little Demon";
				break;
			default:
				EnemyType = "Skeleton Servant";
				break;
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
