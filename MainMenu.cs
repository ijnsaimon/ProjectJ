using Godot;
using System;

public partial class MainMenu : Control
{
	Menu options;
	public override void _Ready(){
		options = GetNode<Menu>("ButtonBox");
		options.ButtonPressed += OnMenuButtonPressed;
		options.ButtonFocus(0);
	}
	public void OnMenuButtonPressed(BaseButton baseButton){
		if(baseButton is Button button){
			String text = button.Text;
			GD.Print(text);
			switch(text){
				case "Play":
					GetTree().ChangeSceneToFile("res://battle.tscn");
					break;
				case "Settings":
					GetTree().ChangeSceneToFile("res://main_options.tscn");
					break;
				case "Credits":
					break;
				case "Score":
					break;
				case "Quit":
					GetTree().Quit();
					break;
			}
		}
	}
}
