using Godot;
using System;
using System.Collections.Generic;

public partial class CardLibraryView : Control
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] ScrollContainer scrollContainer;
	private List<CardInfoLoader> cardInfoLoaders = new List<CardInfoLoader>();
	private List<CardInfoLoader> lockedCards = new List<CardInfoLoader>();


	public override void _Ready()
	{

		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += GrabFocus;
	}
	

	public void setUpInternal(List<CardResource> cards) {
		Visible = true;
		cardInfoLoaders.Clear();
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}
		

		foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			//MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			//marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(cardInfoLoader);
			//cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			//cardInfoLoader.setForceHighlightOff(true);
			cardInfoLoader.setUpCard(cardResource);
			cardInfoLoader.cardScrollContainer = scrollContainer;
			cardInfoLoaders.Add(cardInfoLoader);
		}
		//gridContainer.Columns = (cardInfoLoaders.Count+2)/3;
		GrabFocus();
	}

	
	public void addLockedCards(int count) {
		lockedCards.Clear();		

		for(int a = 0; a < count; a++) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
			//cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			cardInfoLoader.setForceHighlightOff(true);
			cardInfoLoader.setUpLockedCard();
			cardInfoLoader.cardScrollContainer = scrollContainer;
			lockedCards.Add(cardInfoLoader);
		}
		//gridContainer.Columns = (cardInfoLoaders.Count+2)/3;
		GrabFocus();
	}

	private void GrabFocus(bool value) {
	   GrabFocus();
	}

	private void GrabFocus() {
		if(cardInfoLoaders.Count > 0 && FindObjectHelper.getControllerHelper(this).isUsingController()) {
			cardInfoLoaders[0].GrabFocus();
		}
	}
	
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= GrabFocus;
	}
}
