using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
[GlobalClass, Tool]
public partial class CardResource : Resource
{
	[Export] public string Title { get; set; }
	[Export(PropertyHint.MultilineText)] public string Description { private get; set; }
	[Export] public Array<String> UpgradeDescriptions { private get; set; } = new Array<String>();

	[Export] private int Cost { get; set; }
	[Export] public CardRarity rarity = CardRarity.Common;
	[Export] public Texture2D Picture { get; private set; }
	[Export] private int coinCost { get; set; }
	//[Export] public bool playable { get; set; } = true; 
	[Export] public int coinPlayRequirement { get; set; } = 0;
	[Export] public int coinPlayCost { get; set; } = 0;
	public Node node;

	[Export] public CardEffectIF cardEffect { get; set; }

	[Export] private Array<CardResource> cardUpgrades { get; set; } = new Array<CardResource>();
	[Export] private CardPassive cardUpgradeFromPassive = null;
	[Export] private EffectResource cardUpgradeFromExtraEffect = null;
	[Export] private String cardUpgradeTitle { get; set; } = "";



	public CardResource() : this(null, null, 0, null) { }

	public CardResource(string title, string description, int cost, CardEffectIF cardEffect)
	{
		this.Title = title;
		this.Description = description;
		this.Cost = cost;
		this.cardEffect = cardEffect;
	}

	public List<CardResource> getCardUpgrades() {
		List<CardResource> cards = new List<CardResource>();
		cards.AddRange(cardUpgrades);
		if(cardUpgradeFromPassive != null) {
			CardResource newCard = (CardResource)Duplicate();
			newCard.cardUpgradeFromPassive = null;
			newCard.cardEffect = (CardEffectIF)newCard.cardEffect.Duplicate();
			newCard.cardEffect.addPassive(cardUpgradeFromPassive);
			if (String.IsNullOrEmpty(cardUpgradeTitle)){
				newCard.Title = newCard.Title + " +";
			} else {
				newCard.Title = cardUpgradeTitle;
			}
			cards.Add(newCard);
		}
		if(cardUpgradeFromExtraEffect != null) {
			CardResource newCard = (CardResource)Duplicate();
			newCard.cardUpgradeFromExtraEffect = null;
			newCard.cardEffect = (CardEffectIF)newCard.cardEffect.Duplicate();
			newCard.cardEffect.extraEffects.Add(cardUpgradeFromExtraEffect);
			newCard.Title = newCard.Title + " +";
			cards.Add(newCard);
		}

		return cards;
	}

	public SelectionType getSelectionType()
	{
		return cardEffect.getSelectionType();
	}

	public int getEnergyCost()
	{
		int cost = Cost;
		foreach (CardPassive cardPassive in cardEffect.getPassives())
		{
			cost += cardPassive.costModification;
		}
		return Math.Max(cost, 0);
	}

	public bool canPlayCard() {
		// if(!playable) {
		// 	return false;
		// }
		GameManagerIF gameManager = FindObjectHelper.getGameManager(node);
		if (coinPlayRequirement > gameManager.getCoins()) {
			return false;
		}
		if (coinPlayCost > gameManager.getCoins()) {
			return false;
		}
		return true;
	}

	public string getEnergyCostString()
	{
		int cost = getEnergyCost();
		if (cardEffect.spendX){
			return "X";
		}
		if (cost != Cost)
		{
			return "[color=#2c8518]" + cost.ToString() + "[/color]";
		}
		return cost.ToString();
	}

	public int getCoinCost()
	{
		if (coinCost == 0)
		{
			if (rarity == CardRarity.Common)
			{
				return 30;
			}
			if (rarity == CardRarity.Uncommon)
			{
				return 45;
			}
			if (rarity == CardRarity.Rare)
			{
				return 60;
			}
		}
		return coinCost;
	}

	public String getDescription()
	{
		String newDescription = Description;
		if (UpgradeDescriptions != null) {
			foreach (String upgradeDesription in UpgradeDescriptions) {
				newDescription = newDescription.Replace(upgradeDesription, "[color=#2c8518]" + upgradeDesription + "[/color]");
			}
		}
		foreach(EffectResource effectResource in cardEffect.extraEffects) {
			newDescription += "[color=#2c8518]" + effectResource.getDescription() + "[/color]";
		}
		
		return getDescriptionWithReplacements(newDescription);
	}

	private String getDescriptionWithReplacements(String description) {
		String newDescription = description.Replace("$value", cardEffect.getValueString());
		newDescription = newDescription.Replace("$manaGems", cardEffect.getManaGems().ToString());
		newDescription = newDescription.Replace("$cardGems", cardEffect.getCardGems().ToString());
		newDescription = newDescription.Replace("$moneyGems", cardEffect.getCoinGems().ToString());
		newDescription = newDescription.Replace("$customText", cardEffect.getCustomText().ToString());

		//var values = Enum.GetValues(typeof(Foos));
		foreach(GemType gemType in Enum.GetValues(typeof(GemType))) {
			newDescription = newDescription.Replace("$"+gemType.getString(), TextHelper.getIngredientImage(gemType));
		}
		newDescription = newDescription.Replace("$energy", TextHelper.getEnergyImage());
		newDescription = newDescription.Replace("$draw", TextHelper.getCardImage());
		newDescription = newDescription.Replace("$coin", TextHelper.getCoinImage());

		newDescription = newDescription.Replace("nuke", TextHelper.toolTip("nuke", ""));
		newDescription = newDescription.Replace("Nuke", TextHelper.toolTip("Nuke", ""));

		if (cardEffect.consume)
		{
			newDescription += " " + TextHelper.toolTip("Consume.", "Consume cards are trashed until the next level");
		}
		if (cardEffect.retain)
		{
			newDescription += " " + TextHelper.toolTip("Retain.", "Retain cards arn't discarded at the end of the turn");
		}
		if (cardEffect.matchy)
		{
			newDescription += " " + TextHelper.toolTip("Matchy.", "If this card destroys 3+ ingredients of the same type count it as a match");
		}
		if (cardEffect.innate)
		{
			newDescription += " " + TextHelper.toolTip("Innate.", "This card is always drawn in the starting hand");
		}
		return newDescription;
	}

			
	public String getToolTip() {
		String returnString = "";
		if (cardEffect.consume)
		{
			returnString += "Consume cards are trashed until the next level\n";
		}
		if (cardEffect.retain)
		{
			returnString += "Retain cards are not discarded at the end of the turn\n";
		}
		if (cardEffect.matchy)
		{
			returnString += "Matchy - If this card destroys 3+ ingredients of the same type count it as a match\n";
		}
		if (cardEffect.nuke) {
			returnString += "Nuke - This ingredient type is wiped from the board and cannot be spawned in anymore\n";
		}
		if (cardEffect.innate) {
			returnString += "Innate - This card is always drawn in the starting hand\n";
		}
		if (Description.Contains("lead")) {
			returnString += TextHelper.getIngredientImage(GemType.Lead)+" - cannot be matched or selected. Scores 300 points plus 200 points for each upgrade level when it reaches the bottom\n";
		}
		return returnString;
	}


	public bool equalToCard(CardResource cardResource)
	{
		return Title.Equals(cardResource.Title);
	}
	public void init()
	{
		cardEffect.init();
	}

	public CardResource duplicate() {
		CardResource cardResourceReturn = (CardResource)Duplicate(true);
	 	return cardResourceReturn;
	}

}

