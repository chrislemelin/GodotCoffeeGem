using Godot;
using System;

public partial class LevelCompleteUI : Control
{
	[Export] public Button continueButton;
	[Export] private RichTextLabel richTextLabel;


	public void setCoinsGained(int coinsGained) {
		richTextLabel.Text = "You gained " + coinsGained.ToString();
	}

}
