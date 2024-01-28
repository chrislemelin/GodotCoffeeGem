using Godot;
using System;
using System.Collections.Generic;

public partial class DeckViewUI : Control
{
	[Export] PackedScene cardScene;
	[Export] PackedScene marginContainerScene;
	[Export] GridContainer gridContainer;
	[Export] Button closeButton;
	[Export] RichTextLabel title;

	private List<CardInfoLoader> cardInfoLoaders = new List<CardInfoLoader>();

	private Action<CardResource> cardCallBack;
	private CardResource cardSelected;

	public override void _Ready()
	{
		base._Ready();
		closeButton.Pressed += () => {
			if (cardCallBack != null) {
				cardCallBack(cardSelected);
			}
			Visible = false;
		};
	}

	private void setUpInternal(List<CardResource> cards) {
		cardInfoLoaders.Clear();
		Godot.Collections.Array<Node> nodes = gridContainer.GetChildren();
		foreach (Node node in nodes)
		{
			gridContainer.RemoveChild(node);
			node.QueueFree();
		}

		foreach(CardResource cardResource in cards) {
			CardInfoLoader cardInfoLoader = (CardInfoLoader)cardScene.Instantiate();
			cardInfoLoaders.Add(cardInfoLoader);
			cardInfoLoader.GuiInput += (inputEvent) => cardClicked(inputEvent, cardResource, cardInfoLoader);
			cardInfoLoader.setUpCard(cardResource);
			MarginContainer marginContainer = (MarginContainer)marginContainerScene.Instantiate();
			marginContainer.AddChild(cardInfoLoader);
			gridContainer.AddChild(marginContainer);
		}
		Visible = true;
	}

	public void setUp(List<CardResource> cards, Action<CardResource> cardCallBack, string title) {
		this.cardCallBack = cardCallBack;
		if (cardCallBack == null) {
			closeButton.Text = "close";
			closeButton.Disabled = false;
		} else {
			closeButton.Text = "confirm";
			closeButton.Disabled = true;
		}
		this.title.Text = title;
		setUpInternal(cards);
	}

	public void setUp(List<CardResource> cards) {
		setUp(cards, null, "");
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource, CardInfoLoader cardInfoLoader) {
		if (inputEvent.IsActionPressed("click"))
		{
			if(cardCallBack != null) {
				cardSelected = cardResource;
				closeButton.Disabled = false;
				foreach(CardInfoLoader currentCardInfoLoader in cardInfoLoaders) {
					if(currentCardInfoLoader == cardInfoLoader) {
						currentCardInfoLoader.setForceHighlight(true);
					} else {
						currentCardInfoLoader.setForceHighlight(false);
					}
				}
			}
		};
	}

}
