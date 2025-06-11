using Godot;
using System;
using System.Collections.Generic;

public partial class Menu : Container
{
	[Signal]
	public delegate void ButtonFocusedEventHandler(Button button);
	
	[Signal]
	public delegate void ButtonPressedEventHandler(Button button);
	
	private int index = 0;
	public override void _Ready()
	{
		foreach(var child in GetButtons())
		{
			if(child is Button button)
			{
				button.FocusEntered += () => OnButtonFocused(button);
				button.Pressed += () => OnButtonPressed(button);
			}
		}
		String _class = GetClass();
		List<Node> buttons = GetButtons();
		bool grid_containers = false;
		if(this is GridContainer container){
			List<Node> top_row = new List<Node> {};
			List<Node> bottom_row = new List<Node> {};
			int columns = container.GetColumns();
			int rows = Math.Round(buttons.size() / columns);
			List<Node> btm_range = new List[rows*columns - columns, rows * columns - 1];
		}
	}
	public List<Node> GetButtons(){
		return new List<Node>(GetChildren());
	}
	
	public void ButtonFocus(int i)
	{
		var buttons = GetButtons();
		if(i >= 0 && i < buttons.Count && buttons[i] is Button button)
			button.GrabFocus();
	}
	public void OnButtonFocused(Button button)
	{
		EmitSignal(SignalName.ButtonFocused, button);
	}
	public void OnButtonPressed(Button button)
	{
		EmitSignal(SignalName.ButtonPressed, button);
	}
}
