using Godot;
using System;

public partial class NewTurnButton : CustomToolTipButton
{
	[Signal] public delegate void BeforeTurnCleanUpEventHandler();
	[Signal] public delegate void TurnCleanUpEventHandler();
	[Signal] public delegate void AfterTurnCleanUpEventHandler();
	[Signal] public delegate void StartNewTurnEventHandler();

	[Export] public PlayableCardsUI playableCardsUI;
	bool hasShownPlayableCardsUI = false;

	//[Signal] public delegate void SetUpNewTurnEventHandler();
	//[Signal] public delegate void StartNewTurnEventHandler();
	

	public override void _Ready()
	{
		base._Ready();
		Pressed += () => {
			Hand hand = FindObjectHelper.getHand(this);
			if (hand.hasPlayableCards() && shouldShowPlayableCardsUI()) {
				hasShownPlayableCardsUI = true;
				playableCardsUI.Visible = true;
				return;
			} else {
				hasShownPlayableCardsUI = false;
				if (!FindObjectHelper.getScore(this).scoreReached()) {
					EmitSignal(SignalName.BeforeTurnCleanUp);
					EmitSignal(SignalName.TurnCleanUp);
					EmitSignal(SignalName.AfterTurnCleanUp);
					EmitSignal(SignalName.StartNewTurn);
				} else {
					FindObjectHelper.getScore(this).newturn();
				}
			}
		};
	}

	private bool shouldShowPlayableCardsUI() {
		if (!hasShownPlayableCardsUI && !playableCardsUI.showUI.ButtonPressed && !FindObjectHelper.getScore(this).scoreReached()) {
			return true;
		}
		return false;
	}
}
