using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectIF : Resource
{
	[Export] private float Range = 1.0f;
	[Export] private int Value = -1;
	[Export] public int BlackGems { get; private set; } = 0;
	[Export] public int ManaGems { get; private set; } = 0;
	[Export] public int CardGems { get; private set; } = 0;
	[Export] public int CoinGems { get; private set; } = 0;
	[Export] public int DrawCards { get; private set; } = 0;

	[Export] public RelicResource relicResource = null;

	[Export] private SelectionType selectionType;

	[Export] private int CardsToDiscard = 0;
	[Export] public bool canTargetBlackGems = true;
	[Export] public bool consume = false;
	[Export] public bool retain = false;
	[Export] public CardEffectGemType effectGemType { get; set; }
	[Export] public CardPassive cardPassiveToApplyToHand;
	[Signal] public delegate void CardPassivesChangedEventHandler();
	[Signal] public delegate void ValueChangedEventHandler();
	[Signal] public delegate void CustomTextChangedEventHandler();



	public Node node;

	private List<CardPassive> cardPassives = new List<CardPassive>();


	public CardEffectIF()
	{

	}


	public virtual void init()
	{
	}


	public void turnOver()
	{
		cardPassives.RemoveAll(passive => passive.expireAfterTurnEnd);
	}

	public void doEffect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles, Optional<CardIF> cardMaybe)
	{
		createBlackGems(matchBoard);
		effect(matchBoard, hand, mana, selectedTiles);
		createAddonGems(matchBoard, GemAddonType.Mana, getManaGems());
		createAddonGems(matchBoard, GemAddonType.Card, getCardGems());
		createAddonGems(matchBoard, GemAddonType.Money, getCoinGems());
		hand.drawCards(DrawCards);

		if (cardMaybe.HasValue)
		{
			doEffectOnTargetedCard(cardMaybe.GetValue());
		}
		applyPassiveToHand(hand);
		if (relicResource != null)
		{
			FindObjectHelper.getRelicHolderUI(node).addRelic(relicResource);
		}
	}

	protected virtual void doEffectOnTargetedCard(CardIF card)
	{

	}


	public virtual String getCustomText()
	{
		return "";
	}


	public void addPassive(CardPassive cardPassive)
	{
		cardPassives.Add(cardPassive);
		EmitSignal(SignalName.CardPassivesChanged);
	}

	public List<CardPassive> getPassives()
	{
		return cardPassives;
	}


	private void applyPassiveToHand(Hand hand)
	{
		if (cardPassiveToApplyToHand != null)
		{
			List<CardIF> cardsInHand = hand.getAllCards();
			foreach (CardIF cardIF in cardsInHand)
			{
				CardEffectIF cardEffectIF = cardIF.getCardResource().cardEffect;
				if (cardEffectIF != this)
				{
					cardEffectIF.addPassive(cardPassiveToApplyToHand);
				}
			}
		}
	}

	private void createBlackGems(MatchBoard matchBoard)
	{
		List<Tile> tiles = matchBoard.getRandomNonBlackNonAddonTiles(BlackGems);
		if (tiles.Count != BlackGems)
		{
			tiles = matchBoard.getRandomNonBlackTile(BlackGems);
		}
		matchBoard.changeGemsColorAtPosition(tiles.Select(x => x.getTilePosition()).ToList(), GemType.Black);
	}

	private void createAddonGems(MatchBoard matchBoard, GemAddonType type, int count)
	{
		List<Tile> tiles = matchBoard.getRandomNonBlackNonAddonTiles(count);
		matchBoard.addGemAddons(tiles.Select(x => x.getTilePosition()).ToList(), type);
	}

	public virtual void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{

	}

	public virtual SelectionType getSelectionType()
	{
		return selectionType;
	}

	public float getRange()
	{
		float range = Range;
		foreach (CardPassive cardPassive in cardPassives)
		{
			range += cardPassive.rangeModification;
		}
		return range;
	}

	public virtual int getValue()
	{
		int value = Value;
		foreach (CardPassive cardPassive in cardPassives)
		{
			value += cardPassive.valueModification;
		}
		return value;
	}

	public int getManaGems()
	{
		return ManaGems;
	}

	public int getCardGems()
	{
		return CardGems;
	}

	public int getCoinGems()
	{
		return CoinGems;
	}

	public virtual String getValueString()
	{
		int value = Value;
		foreach (CardPassive cardPassive in cardPassives)
		{
			value += cardPassive.valueModification;
		}
		if (value != Value)
		{
			return "[color=#2c8518]" + value.ToString() + "[/color]";
		}
		return value.ToString();
	}


	/// <summary>
	///  Get all tiles that can be selected after the first tile selection. This can be manually set if the range value wont cut it.
	///  This is currently only used for the double selection type
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesSelectableAfterFirstSelection(MatchBoard matchBoard, Tile tile)
	{
		return new List<Vector2>();
	}

	/// <summary>
	/// Get all tiles that we are going to be effected if the tile is clicked
	/// This is currently only used for the single selection type
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesToHighlightOnHover(MatchBoard matchBoard, Tile tile)
	{
		return getAllTilesToEffect(matchBoard, tile);
	}

	/// <summary>
	/// Helper function for getAllTilesToHighlightOnHover so the effect function doens't have to re-code which tiles we care about
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile)
	{
		return new List<Vector2>();
	}
}
