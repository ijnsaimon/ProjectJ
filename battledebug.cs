using Godot;
using System;

public partial class battledebug : Node
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if(@event is InputEventKey eventkey && eventkey.Pressed)
		{
			var key = eventkey.Keycode;
			switch(key){
				case Key.R:
					GetTree().ReloadCurrentScene();
					break;
				case Key.Q:
					GetTree().Quit();
					break;
			}
		}
	}
}
