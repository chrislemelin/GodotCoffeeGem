using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class DeckSelectionResource : Resource
{
	[Export] public CardList cardList;
	[Export] public CardList cardPool;

	[Export] public Array<RelicResource> relics;
	[Export] public Array<ActivatableRelicResource> activatableRelics;

	[Export] public String title;
	[Export(PropertyHint.MultilineText)] public String description;
	[Export] public Color color;
	[Export] public Texture2D faceCard;
	[Export] public Texture2D faceCardFront;
}
