using Godot;
using System;

public partial class MainOptions : Control
{
	Menu options;
	public override void _Ready(){
		options = GetNode<Menu>("ButtonBox");
		options.ButtonPressed += OnMenuButtonPressed;
		options.ButtonFocus(0);
	}
	public void OnMenuButtonPressed(BaseButton baseButton){
		if(baseButton is Button button){
			switch(button.Text){
				case "Back":
					GetTree().ChangeSceneToFile("res://main_menu.tscn");
					break;
				default:
					{
					if(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen){
						DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
						button.Text = "Fullscreen: OFF";
					}
					else{
						DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
						button.Text = "Fullscreen: ON";
					}
					break;
					}
			}
		}
	}
}
