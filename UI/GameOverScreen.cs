using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] public CustomButton restartButton;
	[Export] public RichTextLabel label;
	[Export] public RichTextLabel metaCoinLabel;
	[Export] public RichTextLabel debtLabel;
	[Export] public DebtProgressBar debtProgressBar;
	[Export] public ToggleVisibilityOnButtonPress debtReward;


	public override void _Ready()
	{
		setVisible(false);
		debtProgressBar.debtReward = debtReward;
	}

	public void setVisible(bool value) {
		Visible = value;
		if(Visible) {
			restartButton.checkForFocus();
		}
	}

	public void setUpMainMenu() {
		restartButton.Pressed += () => GetTree().ChangeSceneToFile("res://mainScenes/MainMenu.tscn");
	}

	public void setMetaCoins (int metaCoins) {
		metaCoinLabel.Text = "Gained " + metaCoins;
		debtLabel.Text = "Debt reduced by $"+ metaCoins * 10;
	}

	public int setDebt(int startingDebtProgress, int debtValue) {
		// debtProgressBar.currentDebtPaid = startingDebtProgress;
		// GetTree().CreateTimer(.5f).Timeout += ()=> debtProgressBar.addDebtProgress(debtValue);
		// debtLabel.Text = "Gained "+ debtValue + " barista exp";
		//return debtProgressBar.segmentsPassed(startingDebtProgress, debtValue);
		return 0;
	}
}
