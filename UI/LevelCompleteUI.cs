using Godot;
using System;

public partial class LevelCompleteUI : ToggleVisibilityOnButtonPress
{
	[Export] private RichTextLabel richTextLabel;


	public void setCoinsGained(int coinsGained) {
		resetAnimation();
		richTextLabel.Text = "You gained " + coinsGained.ToString();
		setVisible(true);
	}

}
