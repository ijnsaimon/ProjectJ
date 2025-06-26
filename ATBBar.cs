using Godot;
using System;

public partial class ATBBar : ProgressBar
{	
	[Signal]
	public delegate void MaxValueReachedEventHandler();
	
	const double ATB_SPEED_BASELINE = 0.25;
	public override void _Ready(){
		Random rand = new Random();
		double ranVal = rand.NextDouble();
		Value = ranVal * (MaxValue * 0.75 - MinValue) + MinValue;
	}
	public override void _Process(double _delta){
		Value += ATB_SPEED_BASELINE;
		if(Value == MaxValue){
			SetProcess(false);
			EmitSignal(SignalName.MaxValueReached);
			// TODO ANIMATE
		}
	}
	public void Reset(){
		Value = MinValue;
		SetProcess(true);
	}
	public void Pause(){
		SetProcess(false);
	}
	public void Resume(){
		if(Value < MaxValue){
			SetProcess(true);
		}
	}
	
}
