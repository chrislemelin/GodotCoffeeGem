using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterPortraitCustomizer : Node
{
	[Export] CharacterPortrait portrait;

	[Export] Node characterPortraitOptionsHolder;

	List<CharacterLayerOption> optionControls;
	
   public override void _Ready()
	{
		base._Ready();
		portrait._Ready();
		setUpOptionControls();

	}

	private void setUpOptionControls(){
		optionControls = new List<CharacterLayerOption>();
		foreach(Node child in characterPortraitOptionsHolder.GetChildren()) {
			if (child is CharacterLayerOption option) {
				optionControls.Add(option);
				setOptionLabel(option);

				option.rightButton.Pressed += () => {
					portrait.changePortraitResource(option.type, 1);
					setOptionLabel(option);                
				};
				option.leftButton.Pressed += () => {
					portrait.changePortraitResource(option.type, 1);
					setOptionLabel(option);
				};
			}
		}
	}

	private void setOptionLabel(CharacterLayerOption option) {
		option.updateLabelValue(portrait.getPortraitResource(option.type).name);
	}
}
