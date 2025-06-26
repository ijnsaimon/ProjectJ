using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Menu : Container
{
	[Signal]
	public delegate void ButtonPressedEventHandler(BaseButton button);
	
	int index = 0;

	public override void _Ready()
	{
		foreach(var child in GetButtons())
		{
			if(child is Button button)
			{
				button.Pressed += () => OnButtonPressed(button);
			}
		}
		UpdateFocus();
	}
	public void UpdateFocus(){
		List<Button> buttons = GetChildren().OfType<Button>().Where(b => !b.Disabled).ToList();
		if(buttons.Count == 0) return;
		Container self = this;
		String _class = GetClass();
		if (_class.StartsWith("Grid") && self is GridContainer grid){
			List<Button> top_row = new List<Button> ();
			List<Button> bottom_row = new List<Button> ();
			int columns = grid.GetColumns();
			double round_thingy = buttons.Count / columns;
			int rows = (int) Math.Round(round_thingy);
			int [] btm_range = {rows * columns - columns, rows * columns - 1};
			for(int i = 0; i < columns; i++)
			{
				if(buttons[i] is Button button)
				top_row.Add(button);
			}
			for(int i = btm_range[0]; i < btm_range[1] + 1; i++){
				if(i > buttons.Count)
				{
					if(buttons[i - columns] is Button button)
					{
						bottom_row.Add(button);
						continue;
					}
					continue;
				}
				if(buttons[i] is Button bbutton){
					bottom_row.Add(bbutton); 
				}
			}
			for(int i = 0; i < columns; i++){
				Button top_button = top_row[i];
				Button bottom_button = bottom_row[i];
				if(top_button == bottom_button)
					continue;
				top_button.FocusNeighborTop = bottom_button.GetPath();
				bottom_button.FocusNeighborBottom = top_button.GetPath();
			}
			for(int i = 0; i < buttons.Count; i+=columns){
				Button left_button;
				Button right_button;
				if(buttons[i] is Button button)
				{
					left_button = button;
					if(buttons[i + columns -1] is Button bbutton)
					{
					right_button = bbutton;
					left_button.FocusNeighborLeft = right_button.GetPath();
					right_button.FocusNeighborRight = left_button.GetPath();
					}
				}
			}
		}
		else if (_class.StartsWith("VBox"))
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				buttons[i].FocusNeighborLeft = buttons[i].GetPath();
				buttons[i].FocusNeighborRight = buttons[i].GetPath();
				
				int topIndex = (i == 0) ? buttons.Count - 1 : i - 1;
				int bottomIndex = (i == buttons.Count - 1) ? 0 : i + 1;
				
				buttons[i].FocusNeighborTop = buttons[topIndex].GetPath();
				buttons[i].FocusNeighborBottom = buttons[bottomIndex].GetPath();
			}
		}
		else if (_class.StartsWith("HBox")){
			for (int i = 0; i < buttons.Count; i++){
				buttons[i].FocusNeighborTop = buttons[i].GetPath();
				buttons[i].FocusNeighborBottom = buttons[i].GetPath();
				int leftIndex = (i == 0) ? buttons.Count - 1 : i - 1;
				int rightIndex = (i == buttons.Count - 1) ? 0 : i + 1;
				buttons[i].FocusNeighborLeft = buttons[leftIndex].GetPath();
				buttons[i].FocusNeighborRight = buttons[rightIndex].GetPath();
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
	public void OnButtonPressed(Button button)
	{
		EmitSignal(SignalName.ButtonPressed, button);
	}
}
