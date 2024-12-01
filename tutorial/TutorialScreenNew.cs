using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class TutorialScreenNew : Control
{
	[Export] bool showAdvanceButton;
	[Export] bool showEndTurnButton = false;
	[Export] bool waitTillThingsAreDoneMoving = false;
	[Export] bool advanceAfterRelicActivated = false;

	[Export] public Button advanceButton;
	[Export] Array<Vector2I> tutorialTiles;
	[Export] private int highLightCard = -1;
	[Signal] public delegate void AdvanceEventHandler();
	private bool hasAdvanced = false;

	public override void _Ready()
	{
		base._Ready();
		FindObjectHelper.getHand(this).cardPlayed += (card) =>
		{
			if (!showAdvanceButton && Visible)
			{
				FindObjectHelper.getHand(this).disableAllCardsBut(-1);
				FindObjectHelper.getMatchBoard(this).doneMatching += () =>
				{
					if (!hasAdvanced)
					{
						GetTree().CreateTimer(.5).Timeout += () => EmitSignal(SignalName.Advance);
						hasAdvanced = true;
					}

				};

			}
		};
		FindObjectHelper.getNewTurnButton(this).StartNewTurn += () =>
		{
			if (showEndTurnButton && Visible)
			{
				GetTree().CreateTimer(0).Timeout += () => EmitSignal(SignalName.Advance);
			}
		};
		if (advanceAfterRelicActivated)
		{
			FindObjectHelper.getActivatableRelicUI(this).getActivatableRelic().Activated += () => GetTree().CreateTimer(.5).Timeout += () => EmitSignal(SignalName.Advance);
		}
	}

	public void setVisible(bool value)
	{
		Visible = value;
		if (value)
		{
			if (advanceButton != null)
			{
				advanceButton.Pressed += () => EmitSignal(SignalName.Advance);
				advanceButton.Visible = showAdvanceButton;
			}
			FindObjectHelper.getMatchBoard(this).setTutorialTiles(new HashSet<Vector2I>(tutorialTiles));
			FindObjectHelper.getNewTurnButton(this).Disabled = !showEndTurnButton;
			if (highLightCard != -1)
			{
				FindObjectHelper.getHand(this).disableAllCardsBut(highLightCard);
			}
			if (advanceButton != null)
			{
				((CustomButton)advanceButton).grabFocus = true;
				((CustomButton)advanceButton).checkForFocus();
				FindObjectHelper.getHand(this).setUIFocus(false);
			}
			else
			{
				FindObjectHelper.getHand(this).setUIFocus(true);
			}

		}
	}

	private void tryAdvance()
	{
		if (Visible)
		{
			EmitSignal(SignalName.Advance);
		}
	}




}
