using Godot;
using System;

public partial class NewTurnButton : Button
{
	[Signal] public delegate void SetUpNewTurnEventHandler();
	[Signal] public delegate void StartNewTurnEventHandler();

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => {
			EmitSignal(SignalName.SetUpNewTurn);
			EmitSignal(SignalName.StartNewTurn);
		};
	}
}
