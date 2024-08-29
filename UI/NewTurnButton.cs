using Godot;
using System;

public partial class NewTurnButton : CustomButton
{
	[Signal] public delegate void BeforeTurnCleanUpEventHandler();
	[Signal] public delegate void TurnCleanUpEventHandler();
	[Signal] public delegate void AfterTurnCleanUpEventHandler();
	[Signal] public delegate void StartNewTurnEventHandler();
	[Export] private TextureRect controllerTexture;

	[Export] public PlayableCardsUI playableCardsUI;
	bool hasShownPlayableCardsUI = true;

	//[Signal] public delegate void SetUpNewTurnEventHandler();
	//[Signal] public delegate void StartNewTurnEventHandler();
	

	public override void _Ready()
	{
		base._Ready();
		ControllerHelper controllerHelper = FindObjectHelper.getControllerHelper(this);
		setVisibilityOnControllerTexture(controllerHelper.isUsingController());
		controllerHelper.UsingControllerChanged += setVisibilityOnControllerTexture;
		Pressed += () => {
			Hand hand = FindObjectHelper.getHand(this);
			if (hand.hasPlayableCards() && shouldShowPlayableCardsUI()) {
				hasShownPlayableCardsUI = true;
				playableCardsUI.Visible = true;
				return;
			} else {
				newTurn();
			}
		};
	}

	private void setVisibilityOnControllerTexture(bool usingController){
		controllerTexture.Visible = FindObjectHelper.getControllerHelper(this).isUsingController();
	}

	private void newTurn() {
		Score score = FindObjectHelper.getScore(this);
		if (!score.isLevelOver()) {
			if (goingToLoseHeart()) {
				Disabled = true;
				TooltipText = "You will lose a heart if you end the turn, its time to go home";
				return;
			}

			EmitSignal(SignalName.BeforeTurnCleanUp);
			EmitSignal(SignalName.TurnCleanUp);
			EmitSignal(SignalName.AfterTurnCleanUp);
			EmitSignal(SignalName.StartNewTurn);
			if (goingToLoseHeart()) {
				Disabled = true;
				TooltipText = "You will lose a heart if you end the turn, its time to go home";
			}
		} else {
			FindObjectHelper.getScore(this).newturn();
		}
	}

	private bool goingToLoseHeart() {
		Score score = FindObjectHelper.getScore(this);
		return  score.scoreReached() && score.getTurnsRemaining() == 0 && !((GameManager)FindObjectHelper.getGameManager(this)).gameBeaten();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_new_turn") && !FindObjectHelper.getScore(this).isLevelOver())
		{
			newTurn();
		}
	}

	private bool shouldShowPlayableCardsUI() {
		if (!hasShownPlayableCardsUI && !playableCardsUI.showUI.ButtonPressed && !FindObjectHelper.getScore(this).scoreReached()) {
			return false;
		}
		return false;
	}

	
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= setVisibilityOnControllerTexture;
		base._ExitTree();
	}
}
