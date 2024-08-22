using Godot;
using System;

public partial class MapBackground : Node
{
	[Export] PackedScene character;
	[Export] Node characterHolder;

	[Export] Node2D paths;

	public override void _Ready()
	{
		Timer timer = new Timer();
		AddChild(timer);
		timer.WaitTime = 5;
		timer.Timeout += () => createNewCharacter();
		timer.Start();

		createNewCharacter();
	}


	private void createNewCharacter() {
		Node2D path = (Node2D)paths.GetChild(0);
		lerp newCharacter = (lerp)character.Instantiate();
		characterHolder.AddChild(newCharacter);
		newCharacter.GlobalPosition = ((Node2D)path.GetChild(0)).GlobalPosition;
		newCharacter.moveToGlobalPostion(((Node2D)path.GetChild(1)).GlobalPosition);
	}
}
