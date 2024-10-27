using Godot;
using Godot.Collections;
using System;

public partial class CharacterPortraitLayer : Sprite2D
{
	[Export] public CharacterLayerType type;
	[Export] Array<CharacterPortraitResource> backgrounds;
	int valueIndex = 0;


   public override void _Ready()
	{
		base._Ready();
	}

	private void setImage() {
		Color color = backgrounds[valueIndex].color;
		Texture2D image = backgrounds[valueIndex].image;
		if (image != null) {
			Modulate = new Color(1,1,1);
			Texture = image;
		} else {
			Modulate = color;
			//Texture = null;
		}
	}

	public void setValueToResource(String name) {
		for(int a = 0; a < backgrounds.Count; a++) {
			if (backgrounds[a].name == name) {
				valueIndex = a;
				setImage();
				break;
			}
		}
	}

	public void changeValue(int value) {
		valueIndex += value;
		if (valueIndex < 0) {
			valueIndex += backgrounds.Count;
		}
		valueIndex = valueIndex % backgrounds.Count;
		setImage();
	}

	public CharacterPortraitResource getCurrentResource() {
		return backgrounds[valueIndex];
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	if(@event.IsActionPressed("ui_left"))
	// 	{
	// 		changeValue(-1);
	// 	}
	// 	if (@event.IsActionPressed("ui_right"))
	// 	{
	// 		changeValue(1);
	// 	}
	// 	base._Input(@event);
	// }

}
