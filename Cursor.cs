using Godot;
using System;

	
public partial class Cursor : TextureRect
{
	Vector2 OFFSET = new Vector2(-45, 30);
	const bool PRINT_CURRENT_FOCUS = false;
	Node target = null;
	public override void _Ready(){
		GetViewport().GuiFocusChanged += OnViewportGuiFocusChanged;
		HideAndClearTarget();
	}
	
	public override void _Process(double _delta){
		if (IsInstanceValid(target) && target is Control controlNode)
		{
			GlobalPosition = controlNode.GlobalPosition + OFFSET;
		}
		else{
			HideAndClearTarget();
		}
	}
	public void OnViewportGuiFocusChanged(Control node){
		GD.Print("Focus changed! New focused node is: ", node.Name);
		if(node is BaseButton button)
		{
			GD.Print($"Focus GRANTED to button: {button.Name}");
			target = node;
			Show();
			SetProcess(true);
			GD.Print(target.Name);
		}
		else{
			if (node != null)
			{
				GD.PrintErr($"Focus LOST! Stolen by: '{node.Name}' (Type: {node.GetClass()})");
				HideAndClearTarget();
			}
			else
			{
				GD.PrintErr("Focus LOST! New focus is null.");
				HideAndClearTarget();
			}
		}
	}
	public void HideAndClearTarget()
	{
		GD.Print("Clearing");
   		target = null;
		Hide();
		SetProcess(false);
	}
}
