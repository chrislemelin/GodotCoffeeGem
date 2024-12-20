using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

public partial class MatchBoard : MatchBoardTutorial
{
	[Export] public int sizeX;
	[Export] public int sizeY;
	[Export] PackedScene tileTemplate;
	[Export] PackedScene tileTemplateSmall;
	[Export] PackedScene futureTilesSmallTemplate;
	[Export] PackedScene futureTilesTemplate;
	[Export] public PackedScene gemScene;
	[Export] public PackedScene gemSceneSmall;
	[Export] public Hand hand;
	[Export] public Mana mana;
	[Export] public Score score;
	[Export] public AudioPlayer audioStreamPlayer2D;
	[Export] public AudioPlayer audioScoreStreamPlayer2D;
	[Export] public Node2D boardHolder;
	[Export] public Node2D futureTileHolder;
	[Export] public PackedScene matchOrbPackedScene;
	[Export] public Node2D matchOrbHolder;

	private int columnUpgrades = 0;
	private bool showFutureTiles = false;

	// score stats
	private int scoreValue = 0;
	private int scoreProgressStep = 20;
	private int startingScoreProgressStep = 20;
	private int scoreProgressStepIncrease = 10;
	private int scoreCurrent = 0;
	private int scoreNeeded = 0;
	[Export] int shakeLevel = 10;
	[Export] int shakeFreq = 20;
	[Export] public CoffeePot coffeePot;
	[Export] public AudioStreamPlayer2D coffeePotDropAudio;
	[Export] public AudioStreamPlayer2D victoryMatchAudioStream;
	[Export] public AudioStream matchAudioStream;
	[Export] public AudioStream popAudioStream;
	[Export] public AudioStream switchAudioStream;
	[Export] private GameCamera gameCamera;
	[Export] public RichTextLabel levelLabel;
	[Export] public RichTextLabel scoreLabel;
	[Export] public RichTextLabel multLabel;
	[Export] private float scorePitchMult = .1f;
	[Export] private float scorePitchMax =.3f;
	[Signal]
	public delegate void ingredientDestroyedEventHandler(Gem gem);
	[Signal]
	public delegate void doneMatchingEventHandler();
	[Signal]
	public delegate void ingredientMatchedEventHandler(Match match);
	 [Signal]
	public delegate void ingredientAllMatchesEventHandler(Godot.Collections.Array<Match> matches);
	[Signal]
	public delegate void boardSizeChangedEventHandler();
	[Signal]
	public delegate void blackGemsCreatedThisTurnChangedEventHandler();
	[Signal]
	public delegate void addonsAddedToGemsEventHandler(int gemAdddonType);


	private int hiddenCounter = 0;
	private Timer timer;
	private bool thingsAreMoving = true;

	List<FutureTile> futureTiles = new List<FutureTile>();
	HashSet<Tile> tileSet = new HashSet<Tile>();
	List<Tile> tilesSelected = new List<Tile>();
	HashSet<Tile> tilesAvailibleToBeSelected = new HashSet<Tile>();
	HashSet<Gem> gemsToBeDeleted = new HashSet<Gem>();
	Dictionary<Vector2, Tile> tileMap = new Dictionary<Vector2, Tile>();
	private Vector2 tileSize;
	private Vector2 tileSizeUnScaled;

	private Vector2 tileSizeSmall;
	private Vector2 tileSizeUnScaledSmall;

	[Export] private bool hasUIFocus = false;

	private Vector2 tileHoveredPosition = new Vector2(0,0);
	Optional<Tile> tileHovered = Optional.None<Tile>();
	private int columnsRemoved = 0;

	public bool spawnBlacks = false;
	public bool blacksMatchable = false;
	Dictionary<Vector2, Gem> boardResizeGemMap = new Dictionary<Vector2, Gem>();


	private HashSet<GemType> gemTypesCannotBeSpawned = new HashSet<GemType>();

	//stats
	public HashSet<Match> matchesThisTurn = new HashSet<Match>();
	public HashSet<Match> matchesThisLevel = new HashSet<Match>();

	public int blackGemsCreatedThisTurn = 0;

	private void tileClicked(Tile tile, InputEvent inputEvent) {
		if (InputHelper.isSelectedAction(inputEvent))
		{
			tileSelected(tile);
		}
	} 

	private void tileSelected(Tile tile)
	{
		Optional<CardIF> cardSelectedInHand = hand.getCardSelected();
		if (cardSelectedInHand.HasValue)
		{
			CardIF card = cardSelectedInHand.GetValue();
			if (!canSelectTile(tile, card))
			{
				return;
			}
			SelectionType selectionType = cardSelectedInHand.GetValue().getCardResource().getSelectionType();
			if (selectionType == SelectionType.Single)
			{
				hand.tilesSelectedForCard(this, new List<Vector2> { tile.getTilePosition() });
				hasUIFocus = false;
				hand.setUIFocus(true);
			}
			if (selectionType == SelectionType.Double)
			{
				if (tilesAvailibleToBeSelected.Count == 0)
				{
					if (tile.Gem != null) {
						card.getCardResource().cardEffect.firstSelectedGem = Optional.Some<Gem>(tile.Gem);
					}
					setTilesAvailibleToBeSelectedFromCard(card, tile);
				}
				else
				{
					card.getCardResource().cardEffect.firstSelectedGem = Optional.None<Gem>();
					if (!tilesAvailibleToBeSelected.Contains(tile))
					{
						clearTilesSelected();
						return;
					}
				}
				addTileToSelectedList(tile);
				if (tilesSelected.Count == 2)
				{
					hand.tilesSelectedForCard(this, tilesSelected.Select(tile => tile.getTilePosition()).ToList());
					clearTilesSelected();
					setUIFocus(false);
					hand.setUIFocus(true);
				}
			}
		}
	}

	public void setTutorialTiles(HashSet<Vector2I> tiles) {
		foreach(Tile tile in tileSet) {
			if(tiles.Contains((Vector2I)tile.getTilePosition())) {
				tile.setTutorialColor(true);
			}else {
				tile.setTutorialColor(false);
			}
		}
	}

	public void clearUIFocus(){
		setUIFocus(false);
	}

	public void clearUIFocus(bool value){
		setUIFocus(false);
	}

	public void generateMatchOrbs(HashSet<Match> matches) {
		foreach(Match match in matches) {
			if (match.ingredients.Count > 0) {
				Vector2 startingPosition = match.ingredients[0].GlobalPosition + new Vector2(40,40);
				MatchEffectOrb matchOrb = (MatchEffectOrb)matchOrbPackedScene.Instantiate();
				matchOrbHolder.AddChild(matchOrb);
				float length = matchOrb.generatePathToCenter(startingPosition);
				//matchOrb.GlobalPosition = startingPosition;
				
				Tween tweenMovement = GetTree().CreateTween();
				tweenMovement.TweenProperty(matchOrb.pathFollow2D, "progress_ratio", 1.0f, Math.Clamp(length /500.0f, 1.0f, 2.0f));
				tweenMovement.SetEase(Tween.EaseType.InOut);
				tweenMovement.TweenCallback(Callable.From(matchOrb.destroy));
				tweenMovement.TweenCallback(Callable.From(coffeePot.flash));
				tweenMovement.TweenCallback(Callable.From(coffeePot.scoreChanged));
				tweenMovement.TweenCallback(Callable.From(() => coffeePotDropAudio.Play()));
				
				
				Tween tweenVisible = GetTree().CreateTween();
				Color color = match.GetGemType().GetColor();
				color.A = 0.0f;
				matchOrb.orb.Modulate = color;
				Color colorEnd = match.GetGemType().GetColor();
				colorEnd.A = 1.0f;
				tweenVisible.TweenProperty(matchOrb.orb, "modulate", colorEnd, .2f);

				//flash
			}
		}
	}

	public void setUIFocus(bool value) {
		if(FindObjectHelper.getControllerHelper(this).isUsingController()) {
			hasUIFocus = value;
			if (hasUIFocus) {
				changeHoveredTile(new Vector2(0,0));
			} else {
				clearTilesHovered();

			}
			//cle ar
		}
	}

	public bool getUIFocus() {
		return hasUIFocus;
	}


	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_accept") && hasUIFocus)
		{
			if(tileHovered.HasValue) {
				tileSelected(tileHovered.GetValue());
			}
		}
		if (@event.IsActionPressed("space"))
		{
			//addColumn();
		}
		if (@event.IsActionPressed("right click"))
		{
			//removeColumn();
		}

		base._Input(@event);
	}

	protected override void controllerLeft() {
		if(hasUIFocus) {
			changeHoveredTile(Vector2.Left);
		}
	}
	protected override void controllerRight() {
		if(hasUIFocus) {
			changeHoveredTile(Vector2.Right);
		}
	}
	protected override void controllerUp() {
		if(hasUIFocus) {
			changeHoveredTile(Vector2.Up);
		}
	}
	protected override void controllerDown() {
		if(hasUIFocus) {
			changeHoveredTile(Vector2.Down);
		}
	}

	private void changeHoveredTile(Vector2 newVector) {
		int newX = Math.Clamp((int)newVector.X + (int)tileHoveredPosition.X, 0, getGridSize().X-1);
		int newY = Math.Clamp((int)newVector.Y + (int)tileHoveredPosition.Y, 0, getGridSize().Y-1);
		tileHoveredPosition = new Vector2(newX, newY);
		Tile tile = getTile(tileHoveredPosition);
		FindObjectHelper.getHandLine(this).setPosition(tile.GlobalPosition + (tileSize/2));
		setTileHovered(getTile(tileHoveredPosition));
	}




	public void clearTilesSelected()
	{
		foreach (Tile tile in tileSet)
		{
			tile.turnHighlightOff();
			tile.turnSelectedOff();
			//tile.mouseExit();
		}
		tilesSelected.Clear();
		tilesAvailibleToBeSelected.Clear();
	}

	public void clearTilesHovered()
	{
		foreach (Tile tile in tileSet)
		{
			tile.mouseExit();
			tile.turnHighlightOff();
			tile.turnSelectedOff();
			tile.setDisabled(false);
		}
	}

	public void addTileToSelectedList(Tile tile)
	{
		tile.turnSelectedOn();
		tilesSelected.Add(tile);
	}

	public void addTileToAvailibleToSelectList(Tile tile)
	{
		tile.turnHighlightOn();
		tilesAvailibleToBeSelected.Add(tile);
	}

	private void setTilesAvailibleToBeSelectedFromCard(CardIF card, Tile center)
	{
		if (card.getCardResource().getSelectionType() == SelectionType.Double && tilesAvailibleToBeSelected.Count == 0)
		{
			List<Vector2> customSelectableListOfTiles = card.getCardResource().cardEffect.getAllTilesSelectableAfterFirstSelection(this, center);
			if (customSelectableListOfTiles.Count == 0)
			{
				HashSet<Tile> tilesInRange = getTilesInRange(center.getTilePosition(), card.getCardResource().cardEffect.getRange());
				foreach (Tile tile in tilesInRange)
				{
					//if (card.getCardResource().cardEffect.canTargetTile(tile))
					addTileToAvailibleToSelectList(tile);
				}
			}
			else
			{
				foreach (Vector2 position in customSelectableListOfTiles)
				{
					Optional<Tile> tileMaybe = getTileOptional(position);
					if (tileMaybe.HasValue && tileMaybe.GetValue() != center)
					{
						addTileToAvailibleToSelectList(tileMaybe.GetValue());
					}
				}
			}

		}
	}

	private HashSet<Tile> getTilesInRange(Vector2 startPosition, float range)
	{
		HashSet<Tile> tiles = new HashSet<Tile>();
		foreach (Tile currentTile in tileSet)
		{
			if (currentTile.getTilePosition().DistanceTo(startPosition) <= range)
			{
				if (currentTile.getTilePosition() != startPosition)
				{
					tiles.Add(currentTile);
				}
			}
		}

		return tiles;
	}

	private void checkForDrops()
	{
		if (thingsAreMoving && gemsToBeDeleted.Count == 0)
		{
			bool thingsAreDoneMoving = checkIfThingsAreDoneMoving();
			if (thingsAreDoneMoving)
			{
				thingsAreMoving = checkForMatches();
				if (!thingsAreMoving) {
					EmitSignal(SignalName.doneMatching);
				}
			}
		}
		if (!thingsAreMoving) {
			EmitSignal(SignalName.doneMatching);
		}
	}

	private bool checkIfThingsAreDoneMoving()
	{
		Vector2 gridSize = getGridSize();
		for (int x = 0; x < gridSize.X; x++)
		{
			for (int y = 0; y < gridSize.Y; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Optional<Tile> maybeTile = getTileOptional(currentPosition);
				Optional<Gem> maybeGem = Optional.FromNullable(maybeTile.GetValue().Gem);
				if (maybeGem.HasValue)
				{
					Gem gem = maybeGem.GetValue();
					if (gem.Position.Length() > .1f)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private void checkForExtraHighlightsOnTileHover(Tile tile)
	{
		if (hand.getCardSelected().HasValue)
		{
			CardIF card = hand.getCardSelected().GetValue();
			List<Vector2> tilesToHover = filterOutInvalidPosition(card.getCardResource().cardEffect.getAllTilesToHighlightOnHover(this, tile));
			if (tilesToHover.Count > 0)
			{
				clearTilesSelected();
				foreach (Vector2 position in tilesToHover)
				{
					addTileToAvailibleToSelectList(getTile(position));
				}
			}
		}
	}

	public void addRandomBlockedTiles(int numberOfTiles)
	{
		List<Tile> tiles = getRandomTilesWithCondition(numberOfTiles, (tile) => !tile.getIsBlocked());
		blockTiles(tiles);
	}

	public void blockTiles(List<Tile> tiles)
	{
		foreach (Tile tile in tiles)
		{
			tile.setBlocked(true);
			//deleteGemAtPosition(tile.getTilePosition());
		}
	}

	private void scoreChanged(int score, int scoreNeeded)
	{
		scoreValue = score;
		this.scoreNeeded = scoreNeeded;

	}



	private void updateScoreChanged()
	{
		scoreCurrent += scoreProgressStep;
		if (scoreCurrent > scoreValue) {
			audioScoreStreamPlayer2D.PitchScale = 1.0f;
			audioScoreStreamPlayer2D.setBaseVolumeMult(1.0f);

			scoreProgressStep = startingScoreProgressStep;
			scoreCurrent = scoreValue;
			scoreLabel.Text = TextHelper.centered(scoreCurrent + "/" + scoreNeeded);
		} else {
			audioScoreStreamPlayer2D.PitchScale +=.2f;
			audioScoreStreamPlayer2D.setBaseVolumeMult(audioScoreStreamPlayer2D.baseVolumeMult + .05f);
			//audioScoreStreamPlayer2D.Play();
			scoreProgressStep += scoreProgressStepIncrease;
			scoreLabel.Text = TextHelper.centered(scoreCurrent.ToString()+ "/" + scoreNeeded);
		}
		
	}



	private void resetTurnStats()
	{
		matchesThisTurn = new HashSet<Match>();
		blackGemsCreatedThisTurn = 0;
	}

	public void setShowFutureTiles(bool showFutureTiles) {
		this.showFutureTiles = showFutureTiles;
		futureTileHolder.Visible = showFutureTiles;

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		setShowFutureTiles(false);
		//GetNode<Button>("%NewTurnButton").Pressed += () => checkForMatches();
		ControllerHelper controllerHelper = FindObjectHelper.getControllerHelper(this);
		controllerHelper.UsingControllerChanged += clearUIFocus;

		Timer timer = new();
		AddChild(timer);
		timer.WaitTime = 3.5f;
		timer.OneShot = false;
		timer.Timeout += shineRandomIngredientType;
		timer.Start();

		FindObjectHelper.getSettingsMenu(this).windowOpened += clearUIFocus;

		//Vector2 gridSize = getGridSize();
		//sizeX = (int)gridSize.X;
		//sizeY = (int)gridSize.Y;

		FindObjectHelper.getNewTurnButton(this).TurnCleanUp += resetTurnStats;
		FindObjectHelper.getNewTurnButton(this).StartNewTurn += () => setUIFocus(false);

		levelLabel.Text = TextHelper.centered(FindObjectHelper.getGameManager(this).getLevel() + "/8");

		score.scoreChange += (int newScore, int oldScore) => GetTree().CreateTimer(.25f).Timeout += () => scoreChanged(newScore, oldScore);
		score.multChange += (value) =>  {
			multLabel.Text = "[center]"+Mult.getMultString(value);
			multLabel.Modulate = Mult.getMultColor(value);
		};
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .25;
		timer.Timeout += () => updateScoreChanged();
		timer.Start();


		Tile sizeTile = tileTemplate.Instantiate() as Tile;
		tileSize = new Vector2(sizeTile.sprite2D.Texture.GetWidth() * sizeTile.Scale.X * sizeTile.sprite2D.Scale.X, sizeTile.sprite2D.Texture.GetHeight() * sizeTile.Scale.Y * sizeTile.sprite2D.Scale.Y);
		tileSizeUnScaled = new Vector2(sizeTile.sprite2D.Texture.GetWidth() * sizeTile.sprite2D.Scale.X, sizeTile.sprite2D.Texture.GetHeight() * sizeTile.sprite2D.Scale.Y);

		Tile sizeTileSmall = tileTemplateSmall.Instantiate() as Tile;
		tileSizeSmall = new Vector2(sizeTileSmall.sprite2D.Texture.GetWidth() * sizeTileSmall.Scale.X * sizeTileSmall.sprite2D.Scale.X, sizeTileSmall.sprite2D.Texture.GetHeight() * sizeTileSmall.Scale.Y * sizeTileSmall.sprite2D.Scale.Y);
		tileSizeUnScaledSmall = new Vector2(sizeTileSmall.sprite2D.Texture.GetWidth() * sizeTileSmall.sprite2D.Scale.X, sizeTileSmall.sprite2D.Texture.GetHeight() * sizeTileSmall.sprite2D.Scale.Y);

		sizeTile.QueueFree();

 		generateTiles();
		populateBoard();
		gooIntialTiles();
		
		hand.cardAboutToPlay += (card) => clearMatchActions();

		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .1;
		timer.Timeout += () => checkForDrops();
		timer.Start();
	}

	public void addColumns(int value) {
		columnUpgrades += value;
		redrawBoard();
	}

	public void redrawBoard() {
		gemsToBeDeleted.Clear();
		generateTiles();
		populateBoard();
	}


	public void addColumn() {
		addColumns(1);
	}

	private bool usingSmallSize() {
		return getGridSize().Y != sizeY;
	}

	private Vector2I getGridSize() {
		Vector2I gridMod = new Vector2I(0,0);
		int gridUpgrades = columnUpgrades - columnsRemoved + FindObjectHelper.getGameManager(this).getGridUpgrades();

		if (gridUpgrades < 0) {
			gridMod = new Vector2I(gridUpgrades, 0);
		} else {
			if (gridUpgrades-- > 0) {
				gridMod += new Vector2I(1,0);
			}
			if (gridUpgrades-- > 0) {
				gridMod += new Vector2I(1,0);
			}
			if (gridUpgrades-- > 0) {
				gridMod += new Vector2I(0,1);
			}
			if (gridUpgrades-- > 0) {
				gridMod += new Vector2I(1,0);
			}
			if (gridUpgrades-- > 0) {
				gridMod += new Vector2I(0,1);
			}
		}

		Vector2I returnVector = new Vector2I(sizeX, sizeY) + gridMod;

		if (returnVector.X < 1) {

			return new Vector2I(1, returnVector.Y);
		}

		return returnVector;
	}

	private PackedScene getGemScene() {
		return usingSmallSize() ? gemSceneSmall: gemScene;
	}

	private PackedScene getTileScene() {
		return usingSmallSize() ? tileTemplateSmall: tileTemplate;
	}

	private Vector2 getTileSize() {
		return usingSmallSize() ? tileSizeSmall: tileSize;
	}

	private Vector2 getTileSizeUnscaled() {
		return usingSmallSize() ? tileSizeUnScaledSmall: tileSizeUnScaled;
	}


	public void nukeIngredientType(GemType type) {
		List<Tile> ingredients = getTilesWithColorOfGem(type);
		deleteGemAtPositions(ingredients.Select(tile => tile.getTilePosition()).ToList(), true);
		gemTypesCannotBeSpawned.Add(type);
	}

	public void removeColumn() {
		// Vector2 getCurrentSize = getGridSize();
		// if (getCurrentSize.X == 1) {
		// 	return;
		// }
		// for (int newY = 0; newY < getCurrentSize.Y; newY++) {
		// 	deleteTile(new Vector2(sizeX-1, newY));
		// }
		// sizeX --;
		columnsRemoved++;
		redrawBoard();
		EmitSignal(SignalName.boardSizeChanged);
	}

	public int getColumnsRemoved() {
		return columnsRemoved;
	}

	public void deleteTile(Vector2 position) {
		Tile tile = getTile(position);
		if (tile != null) {
			tileMap.Remove(position);
			tileSet.Remove(tile);
			tile.Gem?.QueueFree();
			tile.QueueFree();
		}
	}

	private void generateTiles() {
		PackedScene tileSceneToUse = usingSmallSize() ? tileTemplateSmall: tileTemplate;
		clearTilesHovered();
		tileHovered = Optional.None<Tile>();
		clearTilesSelected();
		tileSet.Clear();
		//PackedScene gemSceneToUse = columnUpgrades > 2 ? gemScene: gemSceneSmall;

		foreach (Node node in boardHolder.GetChildren()){
			node.QueueFree();
		}

		Vector2 gridSize = getGridSize();
		HashSet<Vector2> gridPositions = new HashSet<Vector2>();

		for (int newX = 0; newX < gridSize.X; newX++)
		{
			for (int newY = 0; newY < gridSize.Y; newY++)
			{
				bool gooTile = false;
				Vector2 position = new Vector2(newX, newY);
				if (tileMap.ContainsKey(position) && tileMap[position].Gem != null) {
					boardResizeGemMap.Add(position, tileMap[position].Gem);
					if (tileMap[position].getIsBlocked()) {
						gooTile = true;
					}
					//continue;
				}
				Tile newTile = tileSceneToUse.Instantiate() as Tile;
				boardHolder.AddChild(newTile);
				newTile.control.GuiInput += (inputEvent) => tileClicked(newTile, inputEvent);
				tileSet.Add(newTile);
				tileMap.Remove(new Vector2(newX, newY));
				tileMap.Add(new Vector2(newX, newY), newTile);
				newTile.x = newX;
				newTile.y = newY;

				newTile.Position = new Vector2(newX * getTileSize().X, newY * getTileSize().Y);
				newTile.control.MouseEntered += () => setTileHovered(newTile);
				newTile.control.MouseExited += () => clearHover(newTile);
				gridPositions.Add(position);

				if (gooTile) {
					newTile.setBlocked(true);
				}
			}
		}
		foreach (Vector2 mapPosition in tileMap.Keys) {
			if (!gridPositions.Contains(mapPosition)){
				tileMap.Remove(mapPosition);
			}
		}
		generateFutureTiles();
		
	}

	private void generateFutureTiles() {
		PackedScene futureTileSceneToUse = usingSmallSize() ? futureTilesSmallTemplate: futureTilesTemplate;

		foreach (Node node in futureTileHolder.GetChildren()){
			node.QueueFree();
		}
		futureTiles.Clear();
		Vector2 gridSize = getGridSize();
		for (int newX = 0; newX < gridSize.X; newX++)
		{
			FutureTile futureTile = futureTileSceneToUse.Instantiate() as FutureTile;
			futureTileHolder.AddChild(futureTile);
			futureTile.Position = new Vector2(getTileSize().X * newX,0);
			futureTiles.Add(futureTile);
		}
	}
	private void setTileHovered(Tile tile)
	{
		tile.mouseEnter();
		bool tileIsDisabled = checkForTilesToDisable(tile);
		if (!tileIsDisabled)
		{
			checkForExtraHighlightsOnTileHover(tile);
		} else{
			//clearTilesSelected();
		}
		if(tileHovered.HasValue && tileHovered.GetValue() != tile) {
			tileHovered.GetValue().mouseExit();
		}
		tileHovered = Optional.Some(tile);
	}

	private void clearHover(Tile tile)
	{
		tile.mouseExit();
		tile.setDisabled(false);
	}

	private bool checkForTilesToDisable(Tile tile)
	{
		foreach (Tile currentTile in tileSet)
		{
			currentTile.setDisabled(false);
		}
		if (hand.getCardSelected().HasValue)
		{
			CardIF card = hand.getCardSelected().GetValue();
			if (!canSelectTile(tile, card))
			{
				tile.setDisabled(true);
				return true;
			}
		}
		return false;
	}

	private bool canSelectTile(Tile tile, CardIF card)
	{
		if (tile.Gem == null)
		{
			return false;
		}
		if (tile.Gem.AddonType == GemAddonType.Lock)
		{
			return false;
		}
		if (tile.getIsBlocked()){
			return false;
		}
		if (!tile.Gem.Type.selectable()) {
			return false;
		}
		if (!card.getCardResource().cardEffect.canTargetTile(tile, tilesSelected.Count == 0)) {
			return false;
		}
		// letting all cards target burnt ingredients
		// if (tile.Gem != null && !card.getCardResource().cardEffect.canTargetBlackGems && tile.Gem.Type == GemType.Black)
		// {
		// 	return false;
		// }
		return true;
	}

	private HashSet<HashSet<Tile>> getMatches()
	{
		Vector2I gridSize = getGridSize();
		//GD.Print(gridSize);
		HashSet<HashSet<Tile>> matches = new HashSet<HashSet<Tile>>(HashSet<Tile>.CreateSetComparer());
		for (int x = 0; x < gridSize.X; x++)
		{
			for (int y = 0; y < gridSize.Y; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				HashSet<Tile> tileMatches = new HashSet<Tile>();

				Tile tile = getTile(currentPosition);
				if (tile.Gem == null)
				{
					continue;
				}

				Optional<Gem> maybeGem = Optional.FromNullable(tile.Gem);
				GemType gemType = maybeGem.GetValue().Type;
				if (!gemType.matchable() && !blacksMatchable)
				{
					continue;
				}
				tileMatches.Add(tile);

				HashSet<Tile> leftMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Left, Vector2.Left, gemType);
				HashSet<Tile> rightMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Right, Vector2.Right, gemType);
				HashSet<Tile> horizontalMatches = new HashSet<Tile>(leftMatches.Union(rightMatches));
				horizontalMatches.Add(tile);

				if (horizontalMatches.Count >= 3)
				{
					matches.Add(horizontalMatches);
				}

				HashSet<Tile> upMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Up, Vector2.Up, gemType);
				HashSet<Tile> downMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Down, Vector2.Down, gemType);
				HashSet<Tile> verticalMatches = new HashSet<Tile>(upMatches.Union(downMatches));
				verticalMatches.Add(tile);

				if (verticalMatches.Count >= 3)
				{
					matches.Add(verticalMatches);
				}
			}
		}
		//check for lead
		for (int x = 0; x < gridSize.X; x++) {
			int y = gridSize.Y -1;
			Tile tile = getTile(new Vector2(x,y));
			if(tile.Gem != null && tile.Gem.Type.Equals(GemType.Lead)){
				matches.Add(new HashSet<Tile>(){tile});
			}
		}

		//check for combining matches
		Dictionary<Tile,HashSet<Tile>> gemToMatchDictionary = new Dictionary<Tile, HashSet<Tile>>();
		foreach(HashSet<Tile> tiles in matches){
			foreach(Tile tile in tiles) {
				if (gemToMatchDictionary.ContainsKey(tile)){
					HashSet<Tile> combination = new HashSet<Tile>(tiles);
					combination.UnionWith(gemToMatchDictionary[tile]);
					foreach(Tile comboTile in combination){
						gemToMatchDictionary[comboTile] = combination;
					}
				} else {
					gemToMatchDictionary[tile] = tiles;
				}
			}
		}
		return new HashSet<HashSet<Tile>>(gemToMatchDictionary.Values);


		//return matches;
	}

	private bool checkForMatches()
	{
		HashSet<HashSet<Tile>> matches = getMatches();
		return scoreMatches(matches);
	}

	private void shineRandomIngredientType() {
		GemType type = GemTypeHelper.getRandomColor();
		List<Gem> gems = getTilesWithColorOfGem(type).Select(tile => tile.Gem).ToList();
		foreach(Gem gem in gems){
			gem.shine();
		}
	}

	private bool scoreMatches(HashSet<HashSet<Tile>> matches)
	{
		HashSet<Vector2> tilesToDeleteGem = new HashSet<Vector2>();
		HashSet<Tile> tilesToClearBlocked = new HashSet<Tile>();

		foreach (HashSet<Tile> currentMatches in matches)
		{
			List<Gem> gems = currentMatches.Select(tile => tile.Gem).ToList();
			foreach (Tile currentTile in currentMatches)
			{
				tilesToDeleteGem.Add(currentTile.getTilePosition());
				currentTile.Gem.doMatchEffects(gems);
				HashSet<Tile> tileAdjacent = getTilesInRange(currentTile.getTilePosition(), 1.0f);

				tilesToClearBlocked.UnionWith(tileAdjacent);
			}
		}
		foreach (Tile tile in tilesToClearBlocked)
		{
			tile.setBlocked(false);
		}
		bool levelPassedBeforeMatch = score.scoreReached();	
		score.scoreMatches(matches);

		if (tilesToDeleteGem.Count > 0)
		{
			gameCamera.shake();
			playMatchSound(levelPassedBeforeMatch);
		}
		HashSet<Match> matchObjects = new HashSet<Match>();
		foreach (HashSet<Tile> match in matches)
		{
			Match currentMatch = new Match();
			currentMatch.ingredients = match.Select(tile => tile.Gem).ToList();
			matchesThisLevel.Add(currentMatch);
			matchesThisTurn.Add(currentMatch);
			EmitSignal(SignalName.ingredientMatched, currentMatch);
			matchObjects.Add(currentMatch);
		}

		EmitSignal(SignalName.ingredientAllMatches, new Godot.Collections.Array<Match>(matchObjects));
		GD.Print("send signal for "+ new Godot.Collections.Array<Match>(matchObjects));


		generateMatchOrbs(matchObjects);

		// if (matches.Count > 0) {
		// }
		deleteGemAtPositionsFromMatches(matches);
		return matches.Count > 0;
	}

	public void playMatchSound(bool levelPassedBeforeMatch = true) {
		if (!levelPassedBeforeMatch && score.scoreReached()) {
			playMatchVictorySound();
		} else {
			playNormalMatchSound();
		}
	}


	public void playNormalMatchSound() {
		audioStreamPlayer2D.Stream = matchAudioStream;
		audioStreamPlayer2D.PitchScale = 1.0f + Math.Min(score.getMult() * scorePitchMult, scorePitchMax);
		audioStreamPlayer2D.setBaseVolumeMult(1f);
		audioStreamPlayer2D.Play();

		//FindObjectHelper.GetCharacterPortrait(this).happyReaction();
	}

	public void playMatchVictorySound() {
		playNormalMatchSound();

		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .45;
		timer.OneShot = true;
		timer.Timeout += () => victoryMatchAudioStream.Play();
		timer.Start();
	}

	private void playPopSound() {
		audioStreamPlayer2D.Stream = popAudioStream;
		audioStreamPlayer2D.setBaseVolumeMult(1.0f);
		audioStreamPlayer2D.Play();
		audioStreamPlayer2D.PitchScale = 1.0f;
	}

	private void gemDoneDying(Gem gem)
	{
		gemsToBeDeleted.Remove(gem);
		if (gemsToBeDeleted.Count == 0)
		{
			checkGemsForDrops();
			thingsAreMoving = true;
		}
}


	private void deleteGemAtPosition(Vector2 position, bool playSound)
	{
		deleteGemAtPositions(new List<Vector2> { position }, playSound);
	}

	private void deleteGemAtPositionsFromMatches(HashSet<HashSet<Tile>> matches)
	{
		foreach (HashSet<Tile> match in matches)
		{
			List<Tile> tilesToDelete = match.Where(tile => tile.Gem != null).Where(gem => gem != null).ToList();
			List<Tile> gemsWithCombo = tilesToDelete.Where(tile => tile.Gem.getCombo() != 0).ToList();
			List<Gem> ingredientsToDelete = match.Where(tile => tile.Gem != null).Where(gem => gem != null).Select(tile => tile.Gem).ToList();
			if (gemsWithCombo.Count > 0)
			{
				Tile survivingGem = gemsWithCombo[0];
				survivingGem.Gem.addCombo(1);
				gemsWithCombo.Remove(survivingGem);
				tilesToDelete.Remove(survivingGem);
				foreach (Tile gemWithCombo in gemsWithCombo)
				{
					survivingGem.Gem.addCombo(gemWithCombo.Gem.getCombo());
				}
			}
			Match matchObject = new Match();
			matchObject.ingredients = ingredientsToDelete;
			deleteGemAtPositions(tilesToDelete.Select(tile => tile.getTilePosition()).ToList(), true, false, matchObject);
		}

	}

	private List<Vector2> filterOutInvalidPosition(IEnumerable<Vector2> positions)
	{
		return positions.Where(pos => tileMap.ContainsKey(pos)).Where(pos => tileMap[pos].Gem != null).ToList();
	}
	private void deleteGemAtPositions(IEnumerable<Vector2> positions, bool fromMatch, bool explode, Match match = null) {
		deleteGemAtPositions(positions, fromMatch, explode, Optional.None<int>(), match);
	}

	private void deleteGemAtPositions(IEnumerable<Vector2> positions, bool fromMatch, bool explode, Optional<int> scorePointValue, Match match = null)
	{
		positions = filterOutInvalidPosition(positions);
		List<Gem> gemsToDelete = positions.Select(pos => tileMap[pos].Gem).Where(gem => gem != null).ToList();
		if (!fromMatch)
		{
			playPopSound();
		}
		gemsToBeDeleted.UnionWith(gemsToDelete);
		foreach (Gem gem in gemsToDelete)
		{
			if (gem != null)
			{
				gem.doneDyingSignal += gemDoneDying;
			}
		}
		if (scorePointValue.HasValue) {
			HashSet<Tile> tiles = getTiles(positions).ToHashSet();
			HashSet<HashSet<Tile>> tileSet = new HashSet<HashSet<Tile>>();
			foreach(Tile tile in tiles) {
				tileSet.Add(new HashSet<Tile>{tile});
			}
			score.scoreMatchesWithPointOverride(tileSet, scorePointValue);
		}

		foreach (Vector2 position in positions)
		{
			deleteGemAtPositionInternal(position, fromMatch, explode, match);
		}
	}

	private void deleteGemAtPositionInternal(Vector2 position, bool fromMatch, bool explode, Match match = null)
	{
		Optional<Tile> tileMaybe = getTileOptional(position);
		if (tileMaybe.HasValue)
		{
			Tile tile = tileMaybe.GetValue();
			if (tile.Gem != null)
			{
				if (fromMatch)
				{

					tile.Gem.startDyingMatch(match);
					
				}
				else if (explode) {
					EmitSignal(SignalName.ingredientDestroyed, tile.Gem);
					tile.Gem.startDyingExplode();
				}
				else
				{
					EmitSignal(SignalName.ingredientDestroyed, tile.Gem);
					tile.Gem.startDying();
				}
				//tile.Gem.startDying();
				tileMaybe.GetValue().Gem = null;
			}
		}
	}
	public HashSet<Tile> getTilesInDirectionOfSameGemType(Vector2 startingPosition, Vector2 direction, Optional<GemType> type, int range = 999, int count = 0)
	{
		if (count > range)
		{
			return new HashSet<Tile>();
		}
		HashSet<Tile> gemMatches = new HashSet<Tile>();
		Optional<Tile> maybeTile = getTileOptional(startingPosition);
		if (maybeTile.HasValue) // ignore missing gems?
		{
			if (type.HasValue)
			{
				if (maybeTile.GetValue().Gem != null && (maybeTile.GetValue().Gem.Type == type.GetValue() || maybeTile.GetValue().Gem.Type == GemType.Rainbow))
				{
					gemMatches.Add(maybeTile.GetValue());
					gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type, range, count + 1));
				}
			}
			else
			{
				gemMatches.Add(maybeTile.GetValue());
				gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type, range, count + 1));
			}
		}
		return gemMatches;
	}

	public HashSet<Tile> getGemsInDirectionOfSameType(Vector2 startingPosition, Vector2 direction, GemType gemType)
	{
		return getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.Some<GemType>(gemType));
	}

	private Optional<Tile> getNextTileWithGemInDirection(Vector2 startingPosition, Vector2 direction)
	{
		Optional<Tile> maybeTile = getTileOptional(startingPosition);
		if (maybeTile.HasValue)
		{
			if (maybeTile.GetValue().Gem != null && maybeTile.GetValue().Gem.Type.falls())
			{
				return Optional.Some(maybeTile.GetValue());
			}
			else
			{
				return getNextTileWithGemInDirection(startingPosition + direction, direction);
			}
		}
		else
		{
			return Optional.None<Tile>();
		}
	}
	private void checkGemsForDrops()
	{
		Vector2 gridSize = getGridSize();

		for (int x = 0; x < gridSize.X; x++)
		{
			int newGemCountInRow = 0;

			for (int y = (int)gridSize.Y - 1; y >= 0; y--)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Tile tile = getTileOptional(currentPosition).GetValue();
				if (tile.Gem == null)
				{
					Optional<Tile> upperTileWithGem = getNextTileWithGemInDirection(currentPosition, Vector2.Up);
					Gem upperGem;
					if (upperTileWithGem.HasValue)
					{
						upperGem = upperTileWithGem.GetValue().Gem;
						sendGemToTile(upperGem, tile, upperTileWithGem.GetValue());
					}
					else
					{
						upperGem = generateGemForTile(currentPosition, null, true);
						if (newGemCountInRow == 0 && !tutorial) {
							upperGem.setType(futureTiles[x].getGemType());
							futureTiles[x].setGemType(generateGemType());
						}
						upperGem.Position = new Vector2(0, (y + 1 + newGemCountInRow) * getTileSizeUnscaled().Y * -1);
						upperGem.moveToPostion(Vector2.Zero);
						newGemCountInRow++;
					}

				}
			}
		}
	}
	public Optional<Tile> getTileOptional(Vector2 position)
	{
		if (!tileMap.ContainsKey(position))
		{
			return Optional.None<Tile>();
		}
		return Optional.Some<Tile>(tileMap[position]);
	}

	public Tile getTile(Vector2 position)
	{
		return tileMap[position];
	}

	public List<Tile> getTiles(IEnumerable<Vector2> positions)
	{
		return positions.Select(position => tileMap[position]).ToList();
	}

	private void sendGemToTile(Gem gem, Tile tileTo, Tile removeFromTile, bool teleport = false)
	{
		tileTo.Gem = gem;
		Vector2 location = gem.GlobalPosition;
		clearTilesGem(removeFromTile, gem);
		tileTo.gemParent.AddChild(gem);
		gem.GlobalPosition = location;
		if (tileTo.getTilePosition().DistanceTo(removeFromTile.getTilePosition()) > 2 && teleport)
		{
			gem.moveToPostionConstTime(Vector2.Zero, .25f);
		}
		else
		{
			gem.moveToPostion(Vector2.Zero);
		}

	}

	private void clearTilesGem(Tile tile)
	{
		clearTilesGem(tile, tile.Gem);
	}

	private void clearTilesGem(Tile tile, Gem gemToRemove)
	{
		tile.gemParent.RemoveChild(gemToRemove);
		if (gemToRemove == tile.Gem)
		{
			tile.Gem = null;
		}
	}

	private void gooIntialTiles() {
		if (FindObjectHelper.getGameManager(this).getGooRightRow())
		{
			gooColumn(getGridSize().X - 1);
		}
	}

	public void gooRightMostColumn() {
		if (FindObjectHelper.getGameManager(this).getGooRightRow())	
		{
			gooColumn(getGridSize().X - 2);
		} else {
			gooColumn(getGridSize().X - 1);
		}
	}

	public void gooColumn(int col) {
		List<Tile> tilesToBlock = new List<Tile>();
		for (int y = 0; y < getGridSize().Y; y++)
		{
			Vector2 currentPosition = new Vector2(col, y);
			tilesToBlock.Add(getTile(currentPosition));

		}
		blockTiles(tilesToBlock);
	}

	private void populateBoard()
	{
		Vector2 gridSize = getGridSize();
		for (int x = 0; x < gridSize.X; x++)
		{
			for (int y = 0; y < gridSize.Y; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Tile tile = getTile(currentPosition);
				if (tile.Gem == null)
				{
					if (boardResizeGemMap.ContainsKey(currentPosition)) {
						generateGemForTile(currentPosition, boardResizeGemMap[currentPosition]);
						boardResizeGemMap.Remove(currentPosition);

					} else {
						generateGemForTile(currentPosition);

					}
				}
			}
		}
		for (int x = 0; x < gridSize.X; x++)
		{
			FutureTile futureTile = futureTiles[x];
			futureTile.setGemType(generateGemType());
		}
		HashSet<HashSet<Tile>> matches = getMatches();
		while (matches.Count != 0)
		{
			foreach (HashSet<Tile> match in matches)
			{
				changeGemsColorAtPosition(match.First().getTilePosition(), GemTypeHelper.getRandomColor(), false);
			}
			matches = getMatches();
		}
	}

	private GemType generateGemType() {
		return spawnBlacks ? GemTypeHelper.getRandomColorIncludeBlack() : GemTypeHelper.getRandomColor();
	}

	private Gem generateGemForTile(Vector2 position, Gem? oldGem = null, bool drop = false)
	{
		PackedScene gemSceneToUse = usingSmallSize() ? gemSceneSmall : gemScene;
		Gem gem = gemSceneToUse.Instantiate() as Gem;
		Tile tile = getTileOptional(position).GetValue();
		
		GemType gemType = generateGemType();
		if (oldGem != null) {
			gemType = oldGem.Type;
			gem.setAddonType(oldGem.AddonType);
		}
		while (gemTypesCannotBeSpawned.Contains(gemType)) {
			gemType = GemTypeHelper.getRandomColor();
		}
		if (tutorial) {
			if (drop) {
				if (tutorialGemSpawns.Count > 0) {
					gemType = tutorialGemSpawns[0];
					tutorialGemSpawns.RemoveAt(0);
				}
			} else {
				gemType = tutorialStartingBoard[new Vector2I((int)position.X, (int)position.Y)];
			}
		}
		gem.setType(gemType);
		tile.gemParent.AddChild(gem);
		tile.Gem = gem;
		gem.Position = new Vector2(0, 0);
		// if (hiddenCounter++ % 8 == 0)
		// {
		// 	//gem.setAddonType(GemAddonType.Lock);
		// }
		//addRandomAddon(gem);
		return gem;
	}


	// private Gem generateGemForTile(Vector2 position, Gem oldGem)
	// {
	// 	PackedScene gemSceneToUse = usingSmallSize() ? gemSceneSmall : gemScene;
	// 	Gem gem = gemSceneToUse.Instantiate() as Gem;
	// 	Tile tile = getTileOptional(position).GetValue();
		
	// 	GemType gemType = oldGem.Type;
	// 	while (gemTypesCannotBeSpawned.Contains(gemType)) {
	// 		gemType = GemTypeHelper.getRandomColor();
	// 	}
	// 	gem.setType(gemType);
	// 	tile.gemParent.AddChild(gem);
	// 	tile.Gem = gem;
	// 	tile.Gem.setAddonType(oldGem.AddonType);
	// 	tile.Gem.leadLevel = oldGem.leadLevel;

	// 	gem.Position = new Vector2(0, 0);
	// 	if (hiddenCounter++ % 8 == 0)
	// 	{
	// 		//gem.setAddonType(GemAddonType.Lock);
	// 	}
	// 	return gem;
	// }

	// private void addRandomAddon(Gem gem)
	// {
	// 	int randomPercentage = GD.RandRange(0, 99);
	// 	GameManagerIF gameManagerIF = FindObjectHelper.getGameManager(this);
	// 	if (randomPercentage < gameManagerIF.getCoinDropRate())
	// 	{
	// 		gem.setAddonType(GemAddonType.Money);
	// 		return;
	// 	}

	// 	randomPercentage = GD.RandRange(0, 99);
	// 	if (randomPercentage < gameManagerIF.getMetaCoinDropRate())
	// 	{
	// 		gem.setAddonType(GemAddonType.MetaCoin);
	// 		return;
	// 	}
	// }

	public List<Tile> getRandomTilesWithCondition(int count, Func<Tile, bool> condition)
	{
		List<Tile> tileList = getTilesWithCondition(condition);
		RandomHelper.Shuffle(tileList);
		if (count == -1)
		{
			count = tileList.Count;
		}
		return tileList.GetRange(0, Math.Min(count, tileList.Count));
	}

	public List<Tile> getTilesWithCondition(Func<Tile, bool> condition)
	{
		List<Tile> tileList = new List<Tile>();
		foreach (Tile tile in tileSet)
		{
			if (condition.Invoke(tile))
			{
				tileList.Add(tile);
			}
		}
		return tileList;
	}

	//functions that could be called by external sources (cards)

	public List<Tile> getTilesWithColorOfGem(GemType gemType)
	{
		List<Tile> tilesToReturn = new List<Tile>();
		foreach (Tile tile in tileSet)
		{
			Optional<Gem> gemMaybe = Optional.FromNullable(tile.Gem);
			if (gemMaybe.HasValue && gemMaybe.GetValue().Type == gemType)
			{
				tilesToReturn.Add(tile);
			}
		}
		return tilesToReturn;
	}

	public List<Tile> getTilesWithColorOfGem(GemType gemType, int count)
	{
		List<Tile> tilesToReturn = getTilesWithColorOfGem(gemType);
		RandomHelper.Shuffle(tilesToReturn);
		return tilesToReturn.GetRange(0, count);
	}

	public List<Tile> getTilesWithAddon(GemAddonType addonType)
	{
		return getTilesWithCondition((tile) =>
		{
			if (tile.Gem != null)
			{
				return tile.Gem.AddonType == addonType;
			}
			else
			{
				return false;
			}
		});
	}


	public HashSet<Tile> getAllTiles()
	{
		return tileSet;
	}

	public void deleteGemAtPositions(IEnumerable<Vector2> positions, bool explode = false)
	{
		deleteGemAtPositions(positions, false, explode);
	}

	public void deleteGemAtPositionsWithScoringForEach(IEnumerable<Vector2> positions, int points)
	{
		deleteGemAtPositions(positions, true, false, Optional.Some(points)); 
	}

	public void checkManuallyForMatchOrDelete(IEnumerable<Vector2> positions)
	{
		positions = filterOutInvalidPosition(positions);
		HashSet<HashSet<Tile>> matches = getMatchesInPositions(positions).ToHashSet();
		HashSet<Vector2> tilesMatched = matches.SelectMany(matchList => matchList.Select(match => match.getTilePosition()).ToHashSet()).ToHashSet();
		HashSet<Vector2> tilesNeedToDelete = positions.ToHashSet();
		tilesNeedToDelete.ExceptWith(tilesMatched);
		deleteGemAtPositions(tilesNeedToDelete, false);
		scoreMatches(matches);
		deleteGemAtPositions(tilesMatched, true);
	}

	public HashSet<HashSet<Tile>> getMatchesInPositions(IEnumerable<Vector2> positions)
	{
		List<Tile> tiles = getTiles(positions).Where(tile => tile.Gem != null).ToList();
		HashSet<HashSet<Tile>> matches = new HashSet<HashSet<Tile>>();
		List<Vector2> returnTiles = new List<Vector2>();
		foreach (GemType gemType in Enum.GetValues(typeof(GemType)))
		{
			if (gemType.matchable())
			{
				HashSet<Tile> gemsOfCurrentType = tiles.Where(tile => tile.Gem.Type == gemType).ToHashSet();
				if (gemsOfCurrentType.Count >= 3)
				{
					matches.Add(gemsOfCurrentType);
					returnTiles.AddRange(gemsOfCurrentType.Select(tile => tile.getTilePosition()));
					foreach (Tile tile in gemsOfCurrentType)
					{
						EmitSignal(SignalName.ingredientDestroyed, tile.Gem);
					}
				}
			}
		}
		return matches;
	}

	public void deleteGemAtPosition(Vector2 position)
	{
		deleteGemAtPositions(new List<Vector2> { position }, false);
	}

	public HashSet<Tile> getTilesInDirection(Vector2 startingPosition, Vector2 direction)
	{
		return getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.None<GemType>());
	}

	public HashSet<Tile> getTilesInDirections(Vector2 startingPosition, HashSet<Vector2> directions, int range = 999)
	{
		return directions.SelectMany(direction => getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.None<GemType>(), range)).ToHashSet();
	}

	public void changeGemsColorAtPosition(Vector2 position, GemType gemType, bool explode = true)
	{
		changeGemsColorAtPosition(filterOutInvalidPosition(new HashSet<Vector2> { position }), gemType, explode);
	}

	public void generateRandomLead(int count)
	{
		List<Tile> tiles = getRandomTilesWithCondition(count,  tile => tile.getTilePosition().Y == 0 && tile.Gem.Type!= GemType.Lead);
		if (tiles.Count >= 1) {
			changeGemsColorAtPosition(filterOutInvalidPosition(tiles.Select(tile=>tile.getTilePosition()).ToHashSet()), GemType.Lead);
		}
	}

	public List<Tile> getRandomNonBlackTile(int count = int.MaxValue)
	{
		return getRandomTilesWithCondition(count, tile =>
		{
			if (tile.Gem == null)
			{
				return false;
			}
			return tile.Gem.Type != GemType.Black;
		});
	}

	public List<Tile> getRandomNonLeadTile(int count = int.MaxValue)
	{
		return getRandomTilesWithCondition(count, tile =>
		{
			if (tile.Gem == null)
			{
				return false;
			}
			return tile.Gem.Type != GemType.Lead;
		});
	}

	public List<Tile> getRandomNonSpecialNotOfTypeTiles(int count, GemType gemType)
	{
		return getRandomTilesWithCondition(count, tile =>
		{
			if (tile.Gem == null)
			{
				return false;
			}
			if (tile.Gem.Type == GemType.Black)
			{
				return false;
			}
			if (tile.Gem.Type == GemType.Lead)
			{
				return false;
			}
			if (tile.Gem.Type == gemType)
			{
				return false;
			}
			return true;
		});
	}

	public List<Tile> getRandomNonSpecialNonAddonTiles(int count, HashSet<Vector2> excludePositions = null)
	{
		if (excludePositions == null) {
			excludePositions = new HashSet<Vector2>();
		}
		return getRandomTilesWithCondition(count, tile =>
		{
			if (tile.Gem == null)
			{
				return false;
			}
			if (tile.Gem.Type == GemType.Black)
			{
				return false;
			}
			if (tile.Gem.Type == GemType.Lead)
			{
				return false;
			}
			if (tile.Gem.AddonType != GemAddonType.None)
			{
				return false;
			}
			if (excludePositions.Contains(tile.getTilePosition()))
			{
				return false;
			}
			return true;
		});
	}

	public List<Tile> getRandomTiles(int count)
	{
		return getRandomTilesWithCondition(count, tile => true);
	}

	public List<Tile> getRandomTilesWithAddon(int count, GemAddonType gemAddonType)
	{
		return getRandomTilesWithCondition(count, tile =>
		{
			if (tile.Gem == null)
			{
				return false;
			}
			return tile.Gem.AddonType == gemAddonType;
		});
	}

	public void addGemAddons(List<Vector2> positions, GemAddonType gemAddonType)
	{
		addGemAddonsDontFireEvent(positions, gemAddonType);
		if (positions.Count > 0) {
			EmitSignal(SignalName.addonsAddedToGems, (int)gemAddonType);
		}
	}

	public void addGemAddonsDontFireEvent(List<Vector2> positions, GemAddonType gemAddonType)
	{
		foreach (Vector2 position in positions)
		{
			Tile tile = getTile(position);
			if (tile.Gem != null)
			{
				tile.Gem.setAddonType(gemAddonType);
				tile.Gem.explode();
			}
		}
	}

	public void addGemAddonsToRandomTiles(int count, GemAddonType gemAddonType) {
		List<Tile> tiles = getRandomNonSpecialNonAddonTiles(count);
		addGemAddons(tiles.Select(x => x.getTilePosition()).ToList(), gemAddonType);
	}

	public void clearMatchActions() {
		foreach(Tile tile in getAllTiles()) { 
			if(tile.Gem != null) {
				tile.Gem.clearMatchActions();
			}
		}
	}

	public void changeGemsColorAtRandomPositions(GemType gemType, int value)
	{
		List<Vector2> tilePositions = getRandomNonSpecialNotOfTypeTiles(value, gemType).Select((tile) => tile.getTilePosition()).ToList();
		changeGemsColorAtPosition(tilePositions, gemType);
	}

	public void changeGemsColorAtPosition(IEnumerable<Vector2> positions, GemType gemType, bool explode = true)
	{
		positions = filterOutInvalidPosition(positions);
		foreach (Vector2 position in positions)
		{
			Optional<Tile> tile = getTileOptional(position);
			if (tile.HasValue)
			{
				tile.GetValue().Gem.setType(gemType);
				if (explode) {
					tile.GetValue().Gem.explode();
				}
			}
		}
		if (gemType == GemType.Black)
		{
			blackGemsCreatedThisTurn += positions.Count();
			EmitSignal(SignalName.blackGemsCreatedThisTurnChanged);
		}
		thingsAreMoving = true;
	}

	public void addMatchActionsOnPositions(IEnumerable<Vector2> positions, Action<List<Gem>> action)
	{
		positions = filterOutInvalidPosition(positions);
		foreach (Vector2 position in positions)
		{
			Tile tile = getTile(position);
			if (tile.Gem != null)
			{
				tile.Gem.addMatchActions(action);
			}
		}
	}


	public void switchGemsInPositions(Vector2 position1, Vector2 position2, bool teleport = false)
	{
		
		Optional<Tile> tile1Optional = getTileOptional(position1);
		Optional<Tile> tile2Optional = getTileOptional(position2);
		if (!tile1Optional.HasValue || !tile2Optional.HasValue) {
			return;
		}
		Tile tile1 = tile1Optional.GetValue();
		Tile tile2 = tile2Optional.GetValue();
 		Optional<Gem> gem1 = Optional.FromNullable(tile1.Gem);
		Optional<Gem> gem2 = Optional.FromNullable(tile2.Gem);
		audioStreamPlayer2D.Stream = switchAudioStream;
		audioStreamPlayer2D.setBaseVolumeMult(1.0f);
		audioStreamPlayer2D.Play();
		if (gem1.HasValue)
		{
			sendGemToTile(gem1.GetValue(), tile2, tile1, teleport);
		}
		if (gem2.HasValue)
		{
			sendGemToTile(gem2.GetValue(), tile1, tile2, teleport);
		}
		thingsAreMoving = true;
	}

	public List<Match> getMatchesThisTurn(GemType gemType)
	{
		return matchesThisTurn.Where(match => match.GetGemType() == gemType).ToList();
	}

	public static explicit operator MatchBoard(Godot.Collections.Array<Node> v)
	{
		throw new NotImplementedException();
	}

	public override void _ExitTree()
	{
		FindObjectHelper.getControllerHelper(this).UsingControllerChanged -= clearUIFocus;
	}
}
