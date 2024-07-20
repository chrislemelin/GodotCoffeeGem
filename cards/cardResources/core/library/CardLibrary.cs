using Godot;
using System;

public partial class CardLibrary : Node
{
	
	[Export] public CardList defaultCardPool;
	[Export] public UnlockableCardPackList cardPacks;	
	[Export] public CardList defaultCardList;
	
}
