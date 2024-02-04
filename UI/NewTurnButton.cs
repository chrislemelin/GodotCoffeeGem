using Godot;
using System;

public partial class NewTurnButton : Button
{
	[Signal] public delegate void BeforeTurnCleanUpEventHandler();
	[Signal] public delegate void TurnCleanUpEventHandler();
	[Signal] public delegate void AfterTurnCleanUpEventHandler();
	[Signal] public delegate void StartNewTurnEventHandler();

	//[Signal] public delegate void SetUpNewTurnEventHandler();
	//[Signal] public delegate void StartNewTurnEventHandler();

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => {
			if (!FindObjectHelper.getScore(this).scoreReached()) {
				EmitSignal(SignalName.BeforeTurnCleanUp);
				EmitSignal(SignalName.TurnCleanUp);
				EmitSignal(SignalName.AfterTurnCleanUp);
				EmitSignal(SignalName.StartNewTurn);
			} else {
				FindObjectHelper.getScore(this).newturn();
			}

		};
	}
}
