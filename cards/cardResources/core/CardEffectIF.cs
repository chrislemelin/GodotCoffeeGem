using Godot;
using Godot.Collections;
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
	[Export] public int GainMana { get; private set; } = 0;

	[Export] public Array<EffectResource> bonusEffects = new Array<EffectResource>();
	[Export] public Array<CardPassive> bonusPassives = new Array<CardPassive>();
	[Export] public Array<EffectResource> matchEffects = new Array<EffectResource>();
	[Export] public Array<EffectResource> extraEffects = new Array<EffectResource>();

	[Export] public int matchEffectsMin = 3;

	[Export] public RelicResource relicResource = null;

	[Export] private SelectionType selectionType;

	[Export] private int CardsToDiscard = 0;
	[Export] public bool canTargetBlackGems = true;
	[Export] public CardEffectGemType effectGemType { get; set; }
	[Export] public CardPassive cardPassiveToApplyToHand;
	[Export] public Array<GemType> cannotSelectGemTypes = new Array<GemType>();
	[Export] public Array<GemType> selectableGemTyleFirstSelection = new Array<GemType>();

	[Export] public bool tutorialCard;
	// [Export] public Vector2I firstPositionTutorial;
	// [Export] public Vector2I secondPositionTutorial;
	[Export] public Array<Vector2I> canOnlySelectTutorial;

	[Export] public Vector2I burntPositionTutorial;
	[Export] public Vector2I addonPositionTutorial;

	[Signal] public delegate void CardPassivesChangedEventHandler();
	[Signal] public delegate void ValueChangedEventHandler();
	[Signal] public delegate void CustomTextChangedEventHandler();

	public Optional<Gem> firstSelectedGem = Optional.None<Gem>();

	// KEYWORDS
	[Export] public bool consume = false;
	[Export] public bool retain = false;
	[Export] public bool matchy = false;
	[Export] public bool spendX = false;
	[Export] public bool nuke = false;
	[Export] public bool innate = false;

	public int spendXValue = 0;

	public Node node;

	[Export] public Array<CardPassive> cardPassives = new Array<CardPassive>();


	public CardEffectIF()
	{

	}


	public virtual void init()
	{
		FindObjectHelper.getMatchBoard(node).ingredientMatched += (match) => EmitSignal(SignalName.CustomTextChanged);
		FindObjectHelper.getNewTurnButton(node).TurnCleanUp += () => EmitSignal(SignalName.CustomTextChanged);
	}


	public void turnOver()
	{
		cardPassives = new Array<CardPassive>(cardPassives.Where(passive => !passive.expireAfterTurnEnd).ToList());
	}

	public virtual bool canTargetTile(Tile tile, bool firstSelection) {
		if (tile.Gem == null) {
			return true;
		} if (cannotSelectGemTypes.Contains(tile.Gem.Type)) {
			return false;
		}
		if (firstSelection && selectableGemTyleFirstSelection.Count > 0 && !selectableGemTyleFirstSelection.Contains(tile.Gem.Type)) {
			return false;
		}
		if (tutorialCard) {
			return canOnlySelectTutorial.Contains((Vector2I)tile.getTilePosition());
		}
		return true;
	}

	public void doEffect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles, Optional<CardIF> cardMaybe)
	{
		createBlackGems(matchBoard, selectedTiles);
		addMatchEffects(matchBoard, selectedTiles);
		effect(matchBoard, hand, mana, selectedTiles);
		createAddonGems(matchBoard, GemAddonType.Mana, getManaGems());
		createAddonGems(matchBoard, GemAddonType.Card, getCardGems());
		createAddonGems(matchBoard, GemAddonType.Money, getCoinGems());
		hand.drawCards(DrawCards);
		mana.modifyMana(GainMana);

		if (cardMaybe.HasValue)
		{
			doEffectOnTargetedCard(cardMaybe.GetValue());
		}
		applyPassiveToHand(hand);
		if (relicResource != null)
		{
			FindObjectHelper.getRelicHolderUI(node).addRelic(relicResource);
		}
		foreach(EffectResource effectResource in extraEffects) {
			effectResource.execute(node);
		}
		if (bonusActive()) {
			foreach(EffectResource bonusEffect in bonusEffects) {
				bonusEffect.execute(node);
			}
		}
	}
	
	private void addMatchEffects(MatchBoard matchBoard, List<Vector2> selectedTiles) {
		bool hasDoneAction = false;
		matchBoard.addMatchActionsOnPositions(selectedTiles, (list) => {
			if (list.Count >= matchEffectsMin && hasDoneAction == false) {
				foreach(EffectResource effect in matchEffects) {
					effect.execute(matchBoard);
				}
				hasDoneAction = true;
			} 
		});
	}
	

	protected virtual void doEffectOnTargetedCard(CardIF card)
	{

	}


	public virtual String getCustomText()
	{
		if (bonusActive()) {
			return "[color=#2c8518]✓[/color]";
		} else {
			return "";
		}
	}


	public void addPassive(CardPassive cardPassive)
	{
		cardPassives.Add(cardPassive);
		EmitSignal(SignalName.CardPassivesChanged);
	}

	public List<CardPassive> getPassives()
	{
		if (bonusActive()) {
			List<CardPassive> bonusCardPassives = new List<CardPassive>();
			for (int a = 0; a < bonusValue(); a++) {
				bonusCardPassives.AddRange(bonusPassives.ToList());
			}
			return cardPassives.Concat(bonusCardPassives).ToList();
		} else {
			return cardPassives.ToList();
		}
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

	private void createBlackGems(MatchBoard matchBoard, List<Vector2> selectedTiles)
	{
		List<Tile> tiles = matchBoard.getRandomNonSpecialNonAddonTiles(BlackGems);
		if (tiles.Count != BlackGems)
		{
			tiles = matchBoard.getRandomNonSpecialNonAddonTiles(BlackGems, selectedTiles.ToHashSet());
		}
		if (tutorialCard && BlackGems > 0) {
			tiles = matchBoard.getTiles(new List<Vector2>{burntPositionTutorial});
		}
		matchBoard.changeGemsColorAtPosition(tiles.Select(x => x.getTilePosition()).ToList(), GemType.Black);
	}

	private void createAddonGems(MatchBoard matchBoard, GemAddonType type, int count)
	{
		List<Tile> tiles = matchBoard.getRandomNonSpecialNonAddonTiles(count);
		if (tutorialCard && count > 0) {
			tiles = matchBoard.getTiles(new List<Vector2>{addonPositionTutorial});
		}
		matchBoard.addGemAddons(tiles.Select(x => x.getTilePosition()).ToList(), type);
	}

	public virtual void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles)
	{

	}

	protected virtual bool bonusActive()
	{
		return false;
	}

	protected virtual int bonusValue()
	{
		return 1;
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
		int updatedValue = Value;
		foreach (CardPassive cardPassive in cardPassives)
		{
			updatedValue += cardPassive.valueModification;
		}
		if (Value < updatedValue)
		{
			return "[color=#2c8518]" + updatedValue.ToString() + "[/color]";
		}
		if (updatedValue < Value) {
			return "[color=#eb4034]" + updatedValue.ToString() + "[/color]";
		}
		return updatedValue.ToString();
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
