using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ControlMenu : Control
{
	[Signal]
	public delegate void ButtonFocusedEventHandler(BaseButton button);
	
	[Signal]
	public delegate void ButtonPressedEventHandler(BaseButton button);
	
	int index = 0;
	
	public override void _Ready()
	{
		ButtonSetting();
		UpdateButtonFocus();
	}
	private void ButtonSetting(){
		foreach(var child in GetChildren()){
			if(child is BaseButton button){
				buttons.Add(button);
				button.FocusEntered += () => OnButtonFocused(button);
				button.Pressed += () => OnButtonPressed(button);
			}
		}
	}
	public void UpdateButtonFocus(){
		List<BaseButton> activeButtons = buttons.Where(b => !b.Disabled).ToList();
		if(activeButtons.Count == 0){
			return;
		}
		if(activeButtons.Count > 1){
			for(int i = 0; i < activeButtons.Count; i++){
				var currentButton = activeButtons[i];
				int leftIndex = (i == 0) ? activeButtons.Count - 1 : i - 1;
				currentButton.FocusNeighborLeft = activeButtons[leftIndex].GetPath();
				int rightIndex = (i == activeButtons.Count - 1) ? 0 : i + 1;
				currentButton.FocusNeighborRight = activeButtons[rightIndex].GetPath();
				currentButton.FocusNeighborTop = currentButton.GetPath();
				currentButton.FocusNeighborBottom = currentButton.GetPath();
			}
		}
		else{
			var lastButton = activeButtons[0];
			lastButton.FocusNeighborLeft = lastButton.GetPath();
			lastButton.FocusNeighborRight = lastButton.GetPath();
		}
	}
	public List<Node> GetButtons(){
		return new List<Node>(GetChildren());
	}
	List<BaseButton> buttons = new List<BaseButton>();
	public void ButtonFocus(int i)
	{
		var buttons = GetButtons();
		if(i >= 0 && i < buttons.Count && buttons[i] is BaseButton button)
			button.GrabFocus();
	}
	public void OnButtonFocused(BaseButton button)
	{
		index = button.GetIndex();
		EmitSignal(SignalName.ButtonFocused, button);
	}
	public void OnButtonPressed(BaseButton button)
	{
		EmitSignal(SignalName.ButtonPressed, button);
	}
}
