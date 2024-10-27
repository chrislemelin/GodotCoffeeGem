using Godot;
using System;

public partial class CharacterLayerOption : Node
{
	[Export] public CharacterLayerType type;
	[Export] public Button leftButton;
	[Export] public Button rightButton;
	[Export] RichTextLabel label;
	[Export] RichTextLabel typeLabel;

	public override void _Ready()
	{
		base._Ready();
		typeLabel.Text = TextHelper.centered(type + "");
	}

	public void updateLabelValue(String value) {
		label.Text = TextHelper.centered(value);
	}

}
