using Godot;
using System;
using System.Collections.Generic;

public partial class TutorialManager : Node
{
	[Export] Control tutorialUIs;
	[Export] bool debugMode = false;
	[Export]int tutorialStep;

	public override void _Ready()
	{
		base._Ready();
		
		FindObjectHelper.getGameManager(this).setShownWelcomeTutorial(true);
		if (!debugMode) {
			renderTutorialStep();
		}
		Hand hand = FindObjectHelper.getHand(this);
		foreach(TutorialScreenNew child in tutorialUIs.GetChildren()) {
			child.Advance += () => doStep();
			if (child.advanceButton != null) {
				hand.addWindowInFrontOf(child);
			}
		}

		//get_viewport().connect("gui_focus_changed", self, "_on_focus_changed")
	}

	public void doStep() {
		tutorialStep++;
		FindObjectHelper.getMatchBoard(this).setTutorialTiles(new HashSet<Vector2I>());
		renderTutorialStep();
	}

/*
	Godot.Collections.Array<Node> children = screens.GetChildren();
			for (int index = 0; index < children.Count; index ++) {
				if (screenNumber -1 == index) {
					((Control)children[index]).Visible = true;
				} else {
					((Control)children[index]).Visible = false;
				}
			}
*/
	private void renderTutorialStep() {
		for(int a = 0; a < tutorialUIs.GetChildCount(); a ++) {
			TutorialScreenNew child = (TutorialScreenNew)tutorialUIs.GetChildren()[a];
			if (a == tutorialStep) {
				child.setVisible(true);
			} else {
				child.setVisible(false);
			}
		}
	}

}
