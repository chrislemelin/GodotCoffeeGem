using Godot;
using System;

public partial class MetaCoinDisplay : Node
{
	[Export] RichTextLabel richTextLabel;
	[Export] GameManagerIF gameManagerIF;
	public override void _Ready() {
		gameManagerIF.metaCoinsChanged += (value) => updateLabel();
		updateLabel();
	}
	private void updateLabel() {
		richTextLabel.Text = gameManagerIF.getMetaCoins().ToString();
	}
}
