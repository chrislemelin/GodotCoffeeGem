using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterPortrait : CharacterPortraitSave
{
	[Export] CharacterPortraitLayer background;
	[Export] CharacterPortraitLayer baseCharacter;
	[Export] CharacterPortraitLayer clothes;
	[Export] CharacterPortraitLayer hat;
	[Export] CharacterPortraitLayer misc;
	
	Dictionary<CharacterLayerType, CharacterPortraitLayer> characterLayers = new Dictionary<CharacterLayerType, CharacterPortraitLayer>();

   public override void _Ready()
	{
		base._Ready();
		addToDict(background);
		addToDict(baseCharacter);
		addToDict(clothes);
		addToDict(hat);
		addToDict(misc);

		foreach (CharacterPortraitLayer layer in characterLayers.Values) {
			setPortraitResource(layer);
		}
	}

	private void addToDict(CharacterPortraitLayer layer) {
		characterLayers[layer.type] = layer;
	}

	public void changePortraitResource(CharacterLayerType type, int value) {
		changePortraitResource(characterLayers[type], value);
	}
	private void changePortraitResource(CharacterPortraitLayer layer, int value) {
		layer.changeValue(value);
		setCharacterLayer(layer.type, layer.getCurrentResource());
	}

	private void setPortraitResource(CharacterPortraitLayer layer) {
		layer.setValueToResource(characterLayerToType[layer.type]);
	}
	public CharacterPortraitResource getPortraitResource(CharacterLayerType type) {
		return characterLayers[type].getCurrentResource();
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_left"))
		{
			changePortraitResource(background, -1);
		}
		if (@event.IsActionPressed("ui_right"))
		{
			changePortraitResource(background, -1);
		}
		base._Input(@event);
	}



}
