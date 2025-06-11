using Godot;
using System;

public partial class battle : Control
{
	UiWindow options;
	Menu optionsmenu;
	public override void _Ready()
	{
		options = GetNode<UiWindow>("Options");
		optionsmenu = GetNode<Menu>("Options/BattleOptionsVbox");
		optionsmenu.ButtonPressed += OnOptionsButtonPressed;
		optionsmenu.ButtonFocused += OnOptionsButtonFocused;
		optionsmenu.ButtonFocus(0);
	}
	public void OnOptionsButtonFocused(BaseButton button){
		
	}
	public void OnOptionsButtonPressed(Button button){
		GD.Print(button.Text);
	}
}
