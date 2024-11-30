using Godot;
using System;

public partial class CollectGameplayDataTutorial : ToggleVisibilityOnButtonPress
{
	[Export] Button disableButton;

	public override void _Ready()
	{
		base._Ready();
		disableButton.Pressed += () => {
			setVisible(false);
			FindObjectHelper.getGameManager(this).setCollectData(false);
		};
	}
}
