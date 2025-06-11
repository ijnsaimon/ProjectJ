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
		EmitSignal(SignalName.ButtonFocusedEventHandler, button);
	}
	public void OnButtonPressed(Button button)
	{
		EmitSignal(SignalName.ButtonPressedEventHandler, button);
	}
}
