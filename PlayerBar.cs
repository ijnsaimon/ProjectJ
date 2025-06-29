using Godot;
using System;

public partial class PlayerBar : HBoxContainer
{
	[Signal]
	public delegate void AtbReadyEventHandler();
	ATBBar bar;
	AnimationPlayer animation;
	Label label;
	Label hp;
	Label mp;
	BattleActor data;
	ControlMenu party;
	BattlePlayer thisPlayer;
	public bool dead = false;
	public override void _Ready(){
		animation = GetNode<AnimationPlayer>("BarAnimation");
		label = GetNode<Label>("Name");
		hp = GetNode<Label>("HP");
		mp = GetNode<Label>("MP");
		bar = GetNode<ATBBar>("ATBBar");
//		party = GetNode<ControlMenu>("Party");
		var playersManager = GetNode<Party>("/root/Party");
		data = Party.party[GetIndex()];
		data.HpChanged += OnHpChanged;
	//	foreach(BattlePlayer player in party.GetChildren()){
	//		if(player.GetData() == data){
	//			thisPlayer = player;
	//		}
	//	}
	//	thisPlayer.PlayerDied += OnPlayerDied;
		label.Text = data.GetName();
		hp.Text = data.hp.ToString();
		mp.Text = data.mp.ToString();
		bar.MaxValueReached += OnMaxValueReached;
		animation.Play("RESET");
	}
	public void OnPlayerDied(){
		bar.Pause();
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate", new Color(1,1,1,0), 0.9f);
		animation.Play("RESET");
		ReleaseFocus();
		dead = true;
	}
	public void OnHpChanged(int newHp, int change){
		hp.Text = newHp.ToString();
		GD.Print("Hp: " + newHp);
	}
	public void Highlight(){
		animation.Play("highlight");
	}
	public void Reset(){
		animation.Play("RESET");
		bar.Reset();
	}
	public void AtbPause(){
		bar.Pause();
	}
	public void AtbResume(){
		bar.Resume();
	}
	public void OnMaxValueReached(){
		 //animation.Play("highlight");
		EmitSignal(SignalName.AtbReady);
	}
}
