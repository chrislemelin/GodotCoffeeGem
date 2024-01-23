using Godot;
using System;
using System.Collections.Generic;

public partial class DeckViewUI : Control
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] Button closeButton;

	private Action<CardResource> cardCallBack;

	public override void _Ready()
	{
		base._Ready();
		closeButton.Pressed += () => Visible = false;
	}

	private void setUpInternal(List<CardResource> cards) {
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}

		foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource);
			cardInfoLoader.setUpCard(cardResource);
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
		}
		Visible = true;
	}

	public void setUpAndSelectCard(List<CardResource> cards, Action<CardResource> cardCallBack) {
		this.cardCallBack = cardCallBack;
		setUpInternal(cards);
	}

	public void setUp(List<CardResource> cards) {
		cardCallBack = null;
		setUpInternal(cards);
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource) {
		if (inputEvent.IsActionPressed("click"))
		{
			if(cardCallBack != null) {
				cardCallBack(cardResource);
				Visible = false;
			}
		};
	}

}
