using Godot;
using System;
using System.Collections.Generic;

public partial class RelicLibraryView : Control
{
	[Export] PackedScene relicUIScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] ScrollContainer scrollContainer;

	
	private List<RelicUI> relicUIs = new List<RelicUI>();
	//private List<CardInfoLoader> lockedCards = new List<CardInfoLoader>();


	public override void _Ready()
	{

		FindObjectHelper.getControllerHelper(this).UsingControllerChanged += GrabFocus;
	}

	
	

	public void setUp(List<RelicResource> relics) {
		Visible = true;
		relicUIs.Clear();
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}
		

		foreach(RelicResource relicResource in relics) {
			RelicUI relicUI = (RelicUI)relicUIScene.Instantiate();
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(relicUI);
			gridContainer.AddChild(marginContainer);
			//cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			relicUI.showTitle = true;
			relicUI.setRelic(relicResource);
		}
		//gridContainer.Columns = (cardInfoLoaders.Count+2)/3;
		GrabFocus();
	}

	
	// public void addLockedCards(int count) {
	// 	lockedCards.Clear();		

	// 	for(int a = 0; a < count; a++) {
	// 		CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
	// 		MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
	// 		marginContainer.AddChild(cardInfoLoader);
	// 		gridContainer.AddChild(marginContainer);
	// 		//cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
	// 		cardInfoLoader.setForceHighlightOff(true);
	// 		cardInfoLoader.setUpLockedCard();
	// 		cardInfoLoader.cardScrollContainer = scrollContainer;
	// 		lockedCards.Add(cardInfoLoader);
	// 	}
	// 	//gridContainer.Columns = (cardInfoLoaders.Count+2)/3;
	// 	GrabFocus();
	// }

	private void GrabFocus(bool value) {
	   GrabFocus();
	}

	private void GrabFocus() {
		if(relicUIs.Count > 0 && FindObjectHelper.getControllerHelper(this).isUsingController()) {
			relicUIs[0].GrabFocus();
		}
	}
	
	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= GrabFocus;
	}
}
