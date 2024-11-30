using Godot;
using System;
using System.Collections.Generic;

public partial class UpgradeCardChoiceUI : Control
{

	[Export] Control upgradeCardControl;
	[Export] PackedScene arrow;
	[Export] PackedScene cardUI;
	[Export] PackedScene orUI;

	[Export] Button continueButton;
	[Signal] public delegate void UpgradeCardSelectedEventHandler(CardResource startingCard, CardResource upgradCard);
	[Export] Button cancelButton;
 	[Signal] public delegate void UpgradeCanceledEventHandler();
	List<CardInfoLoader> cardChoices = new List<CardInfoLoader>();
	CardResource upgradeCardSelected;
	CardResource baseCard;


	public override void _Ready()
	{
		continueButton.Pressed += () => continueClick();
		cancelButton.Pressed += () => cancelClick();

	}

	public void previewCard(CardResource card) {
		foreach (Node node in upgradeCardControl.GetChildren()) {
			node.QueueFree();
		}
		cardChoices.Clear();
		baseCard = card;

		upgradeCardControl.AddChild(generateCardPreview(card));
		upgradeCardControl.AddChild(arrow.Instantiate());
		bool firstCard = true;

		foreach(CardResource cardUpgrade in card.getCardUpgrades()) {
			CardInfoLoader cardInfoLoader = generateCardPreview(cardUpgrade);
			if (!firstCard) {
				upgradeCardControl.AddChild(orUI.Instantiate());
			}
			upgradeCardControl.AddChild(cardInfoLoader);
			cardChoices.Add(cardInfoLoader);
			cardInfoLoader.hitBox.GuiInput += (inputEvent) => cardClicked(inputEvent, cardUpgrade, cardInfoLoader);
			firstCard = false;
		}

		if (card.getCardUpgrades().Count == 1) {
			setUpgradedCard(card.getCardUpgrades()[0]);
		} else {
			clearUpgradedCard();
		}
		Show();

	}


	private CardInfoLoader generateCardPreview(CardResource card) {
		CardInfoLoader cardInfoLoader = (CardInfoLoader)cardUI.Instantiate();
		cardInfoLoader.setShowCoinCost(false);
		cardInfoLoader.setUpCard(card);
		return cardInfoLoader;
	}

	private void setUpgradedCard(CardResource cardResource) {
		upgradeCardSelected = cardResource;
		continueButton.Disabled = false;
	}

	private void clearUpgradedCard() {
		upgradeCardSelected = null;
		continueButton.Disabled = true;
	}

	private void cardClicked(InputEvent inputEvent, CardResource cardResource, CardInfoLoader currentCardInfoLoader) {
		if (InputHelper.isClick(inputEvent)) {
			foreach (CardInfoLoader cardInfoLoader in cardChoices) {
				cardInfoLoader.setForceHighlight(false);
			}
			currentCardInfoLoader.setForceHighlight(true);
			setUpgradedCard(cardResource);
		};
	}

	private void continueClick() {
		Hide();
		EmitSignal(SignalName.UpgradeCardSelected, baseCard, upgradeCardSelected);
	}

	private void cancelClick() {
		Hide();
		EmitSignal(SignalName.UpgradeCanceled);
	}
}
