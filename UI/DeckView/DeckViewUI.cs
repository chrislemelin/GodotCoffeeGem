using Godot;
using System;
using System.Collections.Generic;

public partial class DeckViewUI : Control
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] Button closeButton;

	public override void _Ready()
	{
		base._Ready();
		closeButton.Pressed += () => Visible = false;
	}

	public void setUp(List<CardResource> cards) {
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}

		foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			cardInfoLoader.setUpCard(cardResource);
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
		}
		Visible = true;
	}
}
