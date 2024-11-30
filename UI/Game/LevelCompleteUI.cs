using Godot;
using System;

public partial class LevelCompleteUI : ToggleVisibilityOnButtonPress
{
	[Export] private RichTextLabel standardWagesLabel;
	[Export] private RichTextLabel  overtimeWagesLabel;



	public void setCoinsGained(int coinsGained, int overtimeCoins) {
		resetAnimation();
		standardWagesLabel.Text += " "+coinsGained.ToString();
		overtimeWagesLabel.Text += " "+overtimeCoins.ToString();
		setVisible(true);
	}

}
