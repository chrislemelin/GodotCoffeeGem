using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DeckSelectionGameManager : GameManagerIF
{
	[Export] DeckViewUI deckViewUI;
	[Export] Array<DeckSelectionResource> deckSelections;
	[Export] DeckSelectionResource testDeck;
	[Export] bool includeTestDeck = false;

	[Export] PackedScene deckSelectionUIPackedScene;
	[Export] Control deckSelectionUIParent;
	[Export] Button continueButton;
	[Export] Button viewDeckButton;

	private List<DeckSelectionUI> deckSelectionUIs = new List<DeckSelectionUI>();

	private DeckSelectionResource deckSelectionResource;

	public override void _Ready()
	{
		base._Ready();
		continueButton.Pressed += () => continueButtonClicked();
		viewDeckButton.Pressed += () => viewDeckButtonClicked();
		setUpDeckSelections();
	}
	public override void advanceLevel() {
		GetTree().ChangeSceneToFile("res://mainScenes/game.tscn");
	}

	private void setUpDeckSelections() {
		bool firstDeckSelected = false;
		if (includeTestDeck) {
			deckSelections.Add(testDeck);
		}

		foreach(DeckSelectionResource deckSelectionResource in deckSelections) {
			DeckSelectionUI deckSelectionUI = (DeckSelectionUI)deckSelectionUIPackedScene.Instantiate();
			deckSelectionUI.setDeckSelection(deckSelectionResource);
			deckSelectionUIParent.AddChild(deckSelectionUI);
			deckSelectionUI.GuiInput += (inputEvent) => deckSelectionClicked(inputEvent, deckSelectionResource);
			deckSelectionUIs.Add(deckSelectionUI);
			if (!firstDeckSelected){
				firstDeckSelected = true;
				setSelectedDeck(deckSelectionResource);
			}
		}
	}

	public void deckSelectionClicked(InputEvent inputEvent, DeckSelectionResource deckSelectionResource) {
		if (inputEvent.IsActionPressed("click"))
		{
			setSelectedDeck(deckSelectionResource);
		}
	}

	private void setSelectedDeck(DeckSelectionResource deckSelectionResource) {
		continueButton.Disabled = false;
		viewDeckButton.Disabled = false;

		this.deckSelectionResource = deckSelectionResource;
		foreach(DeckSelectionUI deckSelectionUI in deckSelectionUIs) {
			if (deckSelectionUI.deckSelection != deckSelectionResource) {
				deckSelectionUI.highlightOnHover.setForceHighlight(false);
			} else {
				deckSelectionUI.highlightOnHover.setForceHighlight(true);
			}
		}
		setDeckSelection(deckSelectionResource);
	}

	public void continueButtonClicked() {
		resetGlobals();
		List<CardResource> cards = deckSelectionResource.cardList.getCards();
		setCardList(deckSelectionResource.cardList.getCards());
		if (deckSelectionResource.cardPool != null) {
			setCardPool(deckSelectionResource.cardPool);
		}
		foreach(RelicResource relicResource in deckSelectionResource.relics) {
			addRelic(relicResource);
		}
		
		setDeckSelection(deckSelectionResource);
		advanceLevel();
	}

	public void viewDeckButtonClicked() {
		deckViewUI.setUp(deckSelectionResource.cardList.getCards());
	}

	
}
