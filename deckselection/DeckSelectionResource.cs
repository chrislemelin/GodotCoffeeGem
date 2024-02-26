using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class DeckSelectionResource : Resource
{
	[Export] public CardList cardList;
	[Export] public Array<RelicResource> relics;
	[Export] public String title;
	[Export(PropertyHint.MultilineText)] public String description;

	[Export(PropertyHint.MultilineText)] public Color color;


}
