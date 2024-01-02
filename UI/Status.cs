using Godot;
using System;

public partial class Status : Node
{
	[Export] RichTextLabel coinsLabel;
	[Export] RichTextLabel levelLabel;
	[Export] HBoxContainer heartsContainer; 

	[Export]
	private GameManagerIF gameManager;

	[Export] Color heartColor;


	private int coins = 0;
	private int heartsRemaining = 2;
	private int level = 1;

	public  override void _Ready() {
		setHeartsRemaining(gameManager.getHealth());
		setCoins(gameManager.getCoins());

		gameManager.healthChanged += (newHealth) => setHeartsRemaining(newHealth);
		gameManager.coinsChanged += (newCoins) => setCoins(newCoins);
	}

	public void setHeartsRemaining(int newValue) {
		heartsRemaining = newValue;
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		for(int count = 0; count < nodes.Count; count ++) {
			if (count + 1 <= heartsRemaining) {
				((TextureRect)nodes[count]).Modulate = heartColor;
			} else {
				((TextureRect)nodes[count]).Modulate = new Color(1,1,1);
			}
		}
	}

	public void setCoins(int newValue) {
		coins = newValue;
		coinsLabel.Text = newValue.ToString();
	}

	public void setLevel(int newValue) {
		level = newValue;
		levelLabel.Text = "LEVEL:" + newValue;
	}
}
