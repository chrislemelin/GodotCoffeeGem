using Godot;
using System;

public partial class CardLibrary : Node
{
	[Export] public bool useTestDefaultCardPool;
	[Export] public CardList defaultCardPool;

	[Export] public UnlockableCardPackList cardPacks;	
	[Export] private CardList defaultCardList;
	[Export] private CardList defaultTestCardList;


	public override void _Ready()
	{
		base._Ready();
		defaultCardPool.getCards();
	}


	public CardList getDefaultCardPool() {
		if (useTestDefaultCardPool) {
			return defaultTestCardList;
		}
		return defaultCardList;
	}
}
