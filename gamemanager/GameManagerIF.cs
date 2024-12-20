using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManagerIF : Node2D
{
	protected Global global;
	protected MetaGlobal metaGlobal;
	protected CardLibrary cardLibrary;
	//[Export] public CardList defaultCardPool;
	//[Export] private Godot.Collections.Array<UnlockableCardPack> cardPacks;
	[Export] public RelicList relicList;
	//[Export] public CardList defaultCardList;

	[Signal]
	public delegate void healthChangedEventHandler(int newHealth);

	[Signal]
	public delegate void metaCoinsChangedEventHandler(int newCoins);

	[Signal]
	public delegate void coinsChangedEventHandler(int newCoins, int valueChanged);
	[Signal]
	public delegate void coinsGainedEventHandler(int newCoins);

	public bool isIntialized()
	{
		return getGlobal().deckCardList != null;
	}
	public List<CardResource> getDeckList()
	{
		loadGlobalAndSetDeckToDefault();
		return new List<CardResource>(global.deckCardList.getCards());
	}

	public List<CardResource> getCardPool() {
		List<CardResource> cards = getDefaultCardPool();
		//GD.Print(cards.Count + " cards in the default pool");

		cards.AddRange(getCardsUnlocked());
		//GD.Print(cards.Count + " cards in the pool");
		return cards;
	}

	public List<CardResource> getLockedCards() {
		List<CardResource> cards = new List<CardResource>();
		List<UnlockableCardPack> lockedCardPacks = getLockedCardPacks();
		foreach(UnlockableCardPack unlockableCardPack in lockedCardPacks) {
			cards.AddRange(unlockableCardPack.getCards());
		}
		return cards;
	}

	public List<CardResource> getDefaultCardPool() {
		HashSet<String> allUnlockableCards = new HashSet<String>();
		foreach(UnlockableCardPack pack in getCardLibrary().cardPacks.packs) {

			allUnlockableCards.UnionWith(pack.getCards().Select(card =>  {
				return card.Title;
				}).ToList());
		}


		List<CardResource> cards = cardLibrary.defaultCardPool.getCards();
		cards.RemoveAll(card => allUnlockableCards.Contains(card.Title));
		return cards;
	}

	public List<ActivatableRelicResource> getActivatableRelics() {
		return getGlobal().activableRelics;
	}

	public void addActivatableRelics(ActivatableRelicResource activableRelicResource) {
		getGlobal().activableRelics.Add((ActivatableRelicResource)activableRelicResource.Duplicate());
	}

	private HashSet<CardResource> getCardsUnlocked() {
		HashSet<string> cardPacksUnlocked = getMetaGlobal().cardPacksUnlocked;
		HashSet<CardResource> cardsUnlocked = new HashSet<CardResource>();

		foreach(UnlockableCardPack pack in getCardLibrary().cardPacks.packs) {
			if(cardPacksUnlocked.Contains(pack.title)) {
				cardsUnlocked.UnionWith(pack.getCards());
			}
		}
		return cardsUnlocked;
	}
	public List<UnlockableCardPack> getLockedCardPacks() {
		HashSet<string> cardPacksUnlocked = getMetaGlobal().cardPacksUnlocked;
		List<UnlockableCardPack> lockedCardPackList = new List<UnlockableCardPack>();

		foreach(UnlockableCardPack pack in getCardLibrary().cardPacks.packs) {
			if(!cardPacksUnlocked.Contains(pack.title)) {
				lockedCardPackList.Add(pack);
			}
		}
		return lockedCardPackList;
	}

	public List<UnlockableCardPack> getUnlockedCardPacks() {
		HashSet<string> cardPacksUnlocked = getMetaGlobal().cardPacksUnlocked;
		List<UnlockableCardPack> lockedCardPackList = new List<UnlockableCardPack>();

		foreach(UnlockableCardPack pack in getCardLibrary().cardPacks.packs) {
			if(cardPacksUnlocked.Contains(pack.title)) {
				lockedCardPackList.Add(pack);
			}
		}
		return lockedCardPackList;
	}

	public void unlockCardPack(UnlockableCardPack unlockableCardPack) {
		getMetaGlobal().addUnlockedCardPack(unlockableCardPack);
	}

	public bool getMechanicUnlocked() {
		return true;
		//return getMetaGlobal().mechanicUnlocked;
	}

	public void setMechanicUnlocked(bool value) {
		getMetaGlobal().mechanicUnlocked = value;
		getMetaGlobal().save();
	}

	public void addCardToDeckList(CardResource cardResource)
	{
		if (cardResource == null)
		{
			return;
		}
		getGlobal().deckCardList.getRealList().Add(cardResource);
	}

	public void removeCardFromDeckList(CardResource cardResource)
	{
		global.deckCardList.getRealList().Remove(cardResource);
	}

	public override void _Ready()
	{
		loadGlobalAndSetDeckToDefault();
		//loadUserId();
		setUpMusicPlayer();
	}

	private void setUpMusicPlayer()
	{
		FindObjectHelper.GetMusicPlayer(this).setUp();
	}

	public void setStartTime()
	{
		getGlobal().timeStartedRun = Time.GetTicksMsec();
	}

	public ulong getStartTime()
	{
		return getGlobal().timeStartedRun;
	}

	public void setMusicVolume(float value)
	{
		getGlobal().musicVolume = value;
		getGlobal().save();
	}

	public float getMusicVolume()
	{
		return getGlobal().musicVolume;
	}

	public bool isMusicMuted() {
		return getGlobal().musicVolume < .03;
	}

	public void setSFXVolume(float value)
	{
		getGlobal().sfXvolume = value;
		getGlobal().save();
	}

	public bool isSFXMuted() {
		return getGlobal().sfXvolume < .03;
	}

	public float getSFXVolume()
	{
		return getGlobal().sfXvolume;
	}

	public bool getSeenBossDialouge() {
		return getGlobal().shownBossDialouge;
	}

	public void setSeenBossDialouge(bool value) {
		getGlobal().shownBossDialouge = value;
		getGlobal().save();
	}


	private void loadGLobal()
	{
		if (global == null)
		{
			global = GetNode<Global>(Global.LOAD_STRING);
		}
	}

	private void loadMetaGLobal()
	{
		if (metaGlobal == null)
		{
			metaGlobal = GetNode<MetaGlobal>(MetaGlobal.LOAD_STRING);
		}
	}
	private void loadCardLibrary()
	{
		if (cardLibrary == null)
		{
			cardLibrary = FindObjectHelper.getCardLibrary(this);
		}
	}

	protected void loadGlobalAndSetDeckToDefault()
	{

		if (getGlobal().deckCardList == null || getGlobal().deckCardList.getCards().Count == 0)
		{
			Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(getCardLibrary().getDefaultCardPool().getCards());
			CardList newCardList = new CardList();
			newCardList.setCards(cardList);
			getGlobal().deckCardList = newCardList;
		}
	}

	protected void setCardPool(CardList cardList) {
		getCardLibrary().defaultCardPool = cardList;
	}
	public void setZenMode(bool value)
	{
		getGlobal().zenMode = value;
	}

	public void resetGlobals()
	{
		getGlobal().reset(getMetaGlobal());
	}

	public void setCardList(List<CardResource> cardResources)
	{
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(cardResources).Duplicate();
		CardList newCardList = new CardList();
		newCardList.setCards(cardList);
		getGlobal().deckCardList = newCardList;
	}

	public void addCardToGlobalDeckAndAdvanceLevel(CardResource cardResource)
	{
		addCardToDeckList(cardResource);
		advanceLevel();
	}

	public List<RelicResource> getRelicPool()
	{
		HashSet<string> currentRelics = getRelics().Select(relic => relic.title).ToHashSet();
		return relicList.getRelics().ToList().Where((relic) => !currentRelics.Contains(relic.title)).ToList();
	}

	public virtual void advanceLevel()
	{
	}

	public void setCollectData(bool value)
	{
		getGlobal().collectData = value;
		getGlobal().save();
	}

	public int getDebtMax()
	{
		return getGlobal().debtMax;
	}


	public int getDebt()
	{
		return getGlobal().debt;
	}
	public int getDebtProgress()
	{
		return getGlobal().debtMax - getGlobal().debt;
	}

	public void changeDebt(int value)
	{
		getGlobal().debt += value;
		getGlobal().save();
	}

	public int getNumberOfLevelsPlayed()
	{
		return getGlobal().numberOfLevelsPlayed;
	}

	public void incrementNumberOfLevelsPlayed()
	{
		getGlobal().numberOfLevelsPlayed += 1;
		getGlobal().save();
	}	


	public bool getCollectData()
	{
		return getGlobal().collectData;
	}

	public void setHealth(int newHealth)
	{
		getGlobal().currentHealth = Math.Clamp(newHealth, 0, getGlobal().maxHealth);
		GD.Print("health set to " + getGlobal().currentHealth);
		EmitSignal(SignalName.healthChanged, getGlobal().currentHealth);
	}

	public int getHealth()
	{
		return getGlobal().currentHealth;
	}

	public int getUserId()
	{
		return getGlobal().userId;
	}

	public int getNumberOfCardToChoose()
	{
		return getGlobal().numberOfCardsToChoose;
	}

	public DeckSelectionResource getDeckSelection()
	{
		return getGlobal().deckSelection;
	}

	public void setDeckSelection(DeckSelectionResource deckSelectionResource)
	{
		getGlobal().deckSelection = deckSelectionResource;
		Godot.Collections.Array<CardResource> cardList = new Godot.Collections.Array<CardResource>(deckSelectionResource.cardList.getCards());
		CardList newCardList = new CardList();
		newCardList.setCards(cardList);
		getGlobal().deckCardList = newCardList;
}

	public void setNumberOfCardToChoose(int value)
	{
		getGlobal().numberOfCardsToChoose = value;
	}

	public void setMaxHealth(int newHealth)
	{
		getGlobal().maxHealth = newHealth;
	}

	public int getMaxHealth()
	{
		return getGlobal().maxHealth;
	}

	public bool getGooRightRow()
	{
		return getGlobal().gooRightRow;
	}

	public void setGooRightRow(bool value)
	{
		getGlobal().gooRightRow = value;
	}

	public void setShownWelcomeTutorial(bool value)
	{
		getGlobal().shownWelcomeTutorial = value;
		getGlobal().save();
	}

	public void setShownGameplayCollectionTutorial(bool value)
	{
		getGlobal().shownGameplayCollectionTutorial = value;
		getGlobal().save();
	}
	public bool getShownGameplayCollectionTutorial()
	{
		return getGlobal().shownGameplayCollectionTutorial;
	}


	public int getCoins()
	{
		return getGlobal().currentCoins;
	}

	public int getAllCoinsGained()
	{
		return getGlobal().allCoinsGained;
	}

	public int getStartingCoinsUpgradeLevel()
	{
		return getMetaGlobal().startingCoins;
	}

	public int getStartingCoinsUpgradeLevelMax()
	{
		return getMetaGlobal().startingCoinsMax;
	}


	public void addStartingCoins()
	{
		getMetaGlobal().startingCoins +=1;
		getMetaGlobal().save();
	}

	public int getShopCardChoiceUpgradeLevel()
	{
		return getMetaGlobal().cardsInShopBonus;
	}

	public int getShopCardChoiceUpgradeLevelMax()
	{
		return getMetaGlobal().cardsInShopBonusMax;
	}

	public void addCardsInShop()
	{
		getMetaGlobal().cardsInShopBonus +=1;
		getMetaGlobal().save();
	}
	public int getCardInShop()
	{
		return getGlobal().numberOfCardsInShop;
	}


	public int getLevel()
	{
		return getGlobal().currentLevel;
	}

	public Vector2 getGridSize()
	{
		return getGlobal().gridSize;
	}

	public int getGridUpgrades()
	{
		return getGlobal().gridUpgrades;
	}

	public void addGridUpgrade() {
		getGlobal().gridUpgrades++;
	}

	public int getTotalScore()
	{
		return getGlobal().totalScore;
	}

	public List<RunResource> getHighScores()
	{
		return getGlobal().getHighScores();
	}

	public void submitScore(RunResource runResource)
	{
		getGlobal().addNewHighScore(runResource);
	}


	public List<RelicResource> getRelics()
	{
		return getGlobal().relics;
	}

	public void addRelic(RelicResource relicResource)
	{
		relicResource.executeAddedToInvEffects(this);
		getGlobal().relics.Add((RelicResource)relicResource.Duplicate());
	}

	public void setRelics(List<RelicResource> relicResources)
	{
		getGlobal().relics = relicResources;
	}

	public void addCoins(int coinValue)
	{
		Global global = getGlobal();
		global.currentCoins += coinValue;
		if (global.currentCoins < 0) {
			global.currentCoins = 0;
		}
		if (coinValue > 0)
		{
			getGlobal().allCoinsGained += coinValue;
		}
		if (coinValue > 0) {
			EmitSignal(SignalName.coinsGained, coinValue);
		}
		if (coinValue!= 0) {
			EmitSignal(SignalName.coinsChanged, global.currentCoins, coinValue);
		}
	}

	public void addColorUpgrade(ColorUpgrade colorUpgrade)
	{
		getGlobal().colorUpgrades.Add(colorUpgrade);
	}

	public Godot.Collections.Array<ColorUpgrade> getColorUpgrades() {
		Godot.Collections.Array<ColorUpgrade> upgrades = getGlobal().colorUpgrades.Duplicate();
		upgrades.AddRange(getMetaGlobal().getColorUpgrades());
		return upgrades;
	}

	public void upgradeColorMeta(GemType gemType) {
		getMetaGlobal().upgradeGemType(gemType);
	}
	public int getColorUpgradeMeta(GemType gemType) {
		return getMetaGlobal().getGemUpgrade(gemType);
	}

	public int getColorUpgradeMetaMax() {
		return getMetaGlobal().gemUpgradeMax;
	}

	public int getCoinDropRate() {
		return getMetaGlobal().coinDropRate;
	}

	public int getCoinDropRateMax() {
		return getMetaGlobal().coinDropRateMax;
	}

	public void addCoinDropRate(int value) {
		getMetaGlobal().addCoinDropRate(value);
	}

	public int getMetaCoinDropRate() {
		return getMetaGlobal().metaCoinDropRate;
	}

	public int getMetaCoinDropRateMax() {
		return getMetaGlobal().metaCoinDropRateMax;
	}

	public void addMetaCoinDropRate(int value) {
		getMetaGlobal().addMetaCoinDropRate(value);
	}


	public void resetMetaProgression() {
		getMetaGlobal().reset();
		getGlobal().resetMetaProgression();
	}

	public void addMetaCoins(int value) {
		getGlobal().currentMetaCoins += value;
		getGlobal().save();
		if (value != 0) {
			EmitSignal(SignalName.metaCoinsChanged, getGlobal().currentMetaCoins);
		}
	}

	public int getMetaCoins() {
		return getGlobal().currentMetaCoins;
	}



	protected Global getGlobal()
	{
		loadGLobal();
		return global;
	}

	protected MetaGlobal getMetaGlobal()
	{
		loadMetaGLobal();
		return metaGlobal;
	}
	protected CardLibrary getCardLibrary()
	{
		loadCardLibrary();
		return cardLibrary;
	}
}
