using Godot;
using System;

[GlobalClass, Tool]
public partial class CardList : Resource
{

	[Export] public Godot.Collections.Array<CardResource> allCards;

	public CardList () {
	
	}

}
