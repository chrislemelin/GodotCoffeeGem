using Godot;
using System;

public partial class Status : Node
{
	[Export] RichTextLabel coinsLabel;
	[Export] RichTextLabel levelLabel;
	[Export] HBoxContainer heartsContainer;
	[Export] Texture2D fullHeart; 
	[Export] Texture2D emptyHeart; 
	[Export] private GameManagerIF gameManager;


	private int coins = 0;
	private int heartsRemaining = 2;
	private int level = 1;

	public  override void _Ready() {
		setMaxHealth(gameManager.getMaxHealth());
		setHeartsRemaining(gameManager.getHealth());
		setCoins(gameManager.getCoins());
		setLevel(gameManager.getLevel());

		gameManager.healthChanged += (newHealth) => setHeartsRemaining(newHealth);
		gameManager.coinsChanged += (newCoins, value) => setCoins(newCoins);
	}

	
	public void setMaxHealth(int newMaxHealth) {
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		if (nodes.Count < newMaxHealth) {
			for(int count = 0; count < newMaxHealth - nodes.Count; count ++){
				Node copyNode = nodes[0];
				Node newNode = copyNode.Duplicate();
				heartsContainer.AddChild(newNode);
			}
		}
	}

	public void setHeartsRemaining(int newValue) {
		heartsRemaining = newValue;
		Godot.Collections.Array<Node> nodes = heartsContainer.GetChildren();
		for(int count = 0; count < nodes.Count; count ++) {
			if (count + 1 <= heartsRemaining) {
				((TextureRect)nodes[count]).Texture = fullHeart;
			} else {
				((TextureRect)nodes[count]).Texture = emptyHeart;
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
