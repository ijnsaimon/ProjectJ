using Godot;
using System;

public partial class MainMenu : Control
{
	Sprite2D sprite;
	Menu options;
	public override void _Ready(){
		options = GetNode<Menu>("ButtonBox");
		sprite = GetNode<Sprite2D>("SpriteMoon");
		options.ButtonPressed += OnMenuButtonPressed;
		options.ButtonFocus(0);
		Tween tween = CreateTween();
		tween.SetLoops();
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(sprite, "modulate", new Color(1.5f, 1.5f, 1.2f), 3.0);
		tween.TweenProperty(sprite, "modulate", new Color(0.8f, 0.8f, 0.9f), 3.0);
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
