using Godot;
using System;
using System.Collections.Generic;

public partial class LibraryView : GameManagerIF
{

	[Export] bool showAllCards = false;

	[Export] CardLibraryView cardLibraryView;
	[Export] RelicLibraryView relicLibraryView;
	
	[Export] Button showCardLibraryButton;
	[Export] Button showRelicLibraryButton;

	private void showCards() {
		showCardLibraryButton.Disabled = true;
		showRelicLibraryButton.Disabled = false;
		cardLibraryView.Visible = true;
		relicLibraryView.Visible = false;

	}
	
	private void showRelics() {
		showRelicLibraryButton.Disabled = true;
		showCardLibraryButton.Disabled = false;
		cardLibraryView.Visible = false;
		relicLibraryView.Visible = true;
	}
	public override void _Ready()
	{
		if (showAllCards)
		{
			List<CardResource> cards = getCardPool();
			cards.AddRange(getLockedCards());
			cardLibraryView.setUpInternal(cards);
		} else {
			cardLibraryView.setUpInternal(getCardPool());
			cardLibraryView.addLockedCards(getLockedCards().Count);
		}

		relicLibraryView.setUp(getRelicPool());
		showCardLibraryButton.Pressed += showCards;
		showRelicLibraryButton.Pressed += showRelics;

		showCards();
	}
}
