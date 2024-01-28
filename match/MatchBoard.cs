using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;


public partial class MatchBoard : Node
{
	[Export] public int sizeX;
	[Export] public int sizeY;
	[Export] PackedScene tileTemplate;
	[Export] public PackedScene gemScene;
	[Export] public Hand hand;
	[Export] public Mana mana;
	[Export] public Score score;
	[Export] public AudioStreamPlayer2D audioStreamPlayer2D;
	[Export] public Node2D boardHolder;
	[Export] public TextureProgressBar progressBar;
	private double progressValue = 0;
	[Export] private double progressStep;

	[Export]
	public AudioStream matchAudioStream;
	[Export]
	public AudioStream popAudioStream;
	[Export]
	public AudioStream switchAudioStream;

	private Timer timer;
	private bool thingsAreMoving = true;

	HashSet<Tile> tileSet = new HashSet<Tile>();
	List<Tile> tilesSelected = new List<Tile>();
	HashSet<Tile> tilesAvailibleToBeSelected = new HashSet<Tile>();
	HashSet<Gem> gemsToBeDeleted = new HashSet<Gem>();
	Dictionary<Vector2, Tile> tileMap = new Dictionary<Vector2, Tile>();
	private Vector2 tileSize;
	private Vector2 tileSizeUnScaled;

	private void tileClicked(Tile tile, InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("click"))
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
				}
				if (selectionType == SelectionType.Double)
				{
					if (tilesAvailibleToBeSelected.Count == 0)
					{
						setTilesAvailibleToBeSelectedFromCard(card, tile);
					}
					else
					{
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
					}
				}
			}
		}
	}

	public void clearTilesSelected()
	{
		foreach (Tile tile in tileSet)
		{
			tile.turnHighlightOff();
			tile.turnSelectedOff();
		}
		tilesSelected.Clear();
		tilesAvailibleToBeSelected.Clear();
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
				HashSet<Tile> tilesInRange = getTilesInRange(center.getTilePosition(),card.getCardResource().cardEffect.getRange());
				foreach(Tile tile in tilesInRange) {
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

	private HashSet<Tile> getTilesInRange(Vector2 startPosition, float range) {
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
			}
		}
	}

	private bool checkIfThingsAreDoneMoving()
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
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
			List<Vector2> tilesToHover = card.getCardResource().cardEffect.getAllTilesToHighlightOnHover(this, tile);
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

	public void addRandomBlockedTiles(int numberOfTiles) {
		List<Tile> tiles = getRandomTiles(numberOfTiles);
		foreach(Tile tile in tiles) {
			tile.setBlocked(true);
			deleteGemAtPosition(tile.getTilePosition());
		}
	}

	private void scoreChanged(int score, int scoreNeeded) {
		progressValue = (double)score / scoreNeeded * 100;
		
	}

	private void updateScoreChanged() {
		double currentProgress = progressBar.Value;
		if (Math.Abs(currentProgress - progressValue) > progressStep) {
			progressBar.Value = currentProgress + progressStep;
		} else {
			progressBar.Value = progressValue;
		}

	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GetNode<Button>("%NewTurnButton").Pressed += () => checkForMatches();
		Vector2 gridSize = FindObjectHelper.getGameManager(this).getGridSize();
		sizeX = (int)gridSize.X;
		sizeY = (int)gridSize.Y;
		progressBar.Value = 0;

		score.scoreChange += scoreChanged;
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .1;
		timer.Timeout += () => updateScoreChanged();
		timer.Start();


		Tile sizeTile = tileTemplate.Instantiate() as Tile;
		tileSize = new Vector2(sizeTile.sprite2D.Texture.GetWidth() * sizeTile.Scale.X * sizeTile.sprite2D.Scale.X, sizeTile.sprite2D.Texture.GetHeight() * sizeTile.Scale.Y * sizeTile.sprite2D.Scale.Y);
		tileSizeUnScaled = new Vector2(sizeTile.sprite2D.Texture.GetWidth() * sizeTile.sprite2D.Scale.X, sizeTile.sprite2D.Texture.GetHeight() * sizeTile.sprite2D.Scale.Y);

		sizeTile.QueueFree();

		for (int newX = 0; newX < sizeX; newX++)
		{
			for (int newY = 0; newY < sizeY; newY++)
			{
				Tile newTile = tileTemplate.Instantiate() as Tile;
				boardHolder.AddChild(newTile);
				newTile.control.GuiInput += (inputEvent) => tileClicked(newTile, inputEvent);
				tileSet.Add(newTile);
				tileMap.Add(new Vector2(newX, newY), newTile);
				newTile.x = newX;
				newTile.y = newY;

				newTile.Position = new Vector2(newX * tileSize.X, newY * tileSize.Y);
				newTile.control.MouseEntered += () => tileHovered(newTile);
				newTile.control.MouseExited += () => clearHover(newTile);
			}
		}
		populateBoard();
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .1;
		timer.Timeout += () => checkForDrops();
		timer.Start();
	}
	private void tileHovered(Tile tile)
	{
		bool tileIsDisabled = checkForTilesToDisable(tile);
		if (!tileIsDisabled)
		{
			checkForExtraHighlightsOnTileHover(tile);
		}
	}

	private void clearHover(Tile tile)
	{
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
		if(tile.Gem == null){
			return false;
		}
		if (tile.Gem != null && !card.getCardResource().cardEffect.canTargetBlackGems && tile.Gem.Type == GemType.Black)
		{
			return false;
		}
		return true;
	}

	private HashSet<HashSet<Tile>> getMatches()
	{
		HashSet<HashSet<Tile>> matches = new HashSet<HashSet<Tile>>(HashSet<Tile>.CreateSetComparer());
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				HashSet<Tile> tileMatches = new HashSet<Tile>();

				Tile tile = getTile(currentPosition);
				if(tile.Gem == null) {
					continue;
				}

				Optional<Gem> maybeGem = Optional.FromNullable(tile.Gem);
				GemType gemType = maybeGem.GetValue().Type;
				if (!gemType.matchable())
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
		return matches;
	}

	private bool checkForMatches()
	{
		HashSet<HashSet<Tile>> matches = getMatches();

		HashSet<Vector2> tilesToDeleteGem = new HashSet<Vector2>();
		HashSet<Tile> tilesToClearBlocked = new HashSet<Tile>();

		foreach (HashSet<Tile> currentMatches in matches)
		{
			foreach (Tile currentTile in currentMatches)
			{
				tilesToDeleteGem.Add(currentTile.getTilePosition());
				currentTile.Gem.doAddonEffect();
				HashSet<Tile> tileAdjacent = getTilesInRange(currentTile.getTilePosition(), 1.0f);

				tilesToClearBlocked.UnionWith(tileAdjacent);
			}
		}
		foreach(Tile tile in tilesToClearBlocked) {
			tile.setBlocked(false);
		}

		if (tilesToDeleteGem.Count > 0) 
		{
			audioStreamPlayer2D.Stream = matchAudioStream;
			audioStreamPlayer2D.Play();
		}
		score.scoreMatches(matches);
		deleteGemAtPositionsFromMatches(matches);
		return matches.Count > 0;
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
			List<Gem> gemsToDelete = tilesToDelete.Select(tile => tile.Gem).ToList();
			gemsToBeDeleted.UnionWith(gemsToDelete);
			foreach (Gem gem in gemsToDelete)
			{
				gem.doneDyingSignal += gemDoneDying;
			}

			foreach (Tile tile in tilesToDelete)
			{
				deleteGemAtPositionInternal(tile.getTilePosition());
			}
		}

	}

	private void deleteGemAtPositions(IEnumerable<Vector2> positions, bool playSound)
	{
		List<Gem> gemsToDelete = positions.Select(pos => tileMap[pos].Gem).Where(gem => gem != null).ToList();
		if (playSound)
		{
			audioStreamPlayer2D.Stream = popAudioStream;
			audioStreamPlayer2D.Play();
		}
		gemsToBeDeleted.UnionWith(gemsToDelete);
		foreach (Gem gem in gemsToDelete)
		{
			if (gem != null) {
				gem.doneDyingSignal += gemDoneDying;
			}
		}

		foreach (Vector2 position in positions)
		{
			deleteGemAtPositionInternal(position);
		}
	}

	private void deleteGemAtPositionInternal(Vector2 position)
	{
		Optional<Tile> tileMaybe = getTileOptional(position);
		if (tileMaybe.HasValue)
		{
			Tile tile = tileMaybe.GetValue();
			if (tile.Gem != null)
			{
				tile.Gem.startDying();
				tileMaybe.GetValue().Gem = null;
			}
		}
	}
	public HashSet<Tile> getTilesInDirectionOfSameGemType(Vector2 startingPosition, Vector2 direction, Optional<GemType> type)
	{
		HashSet<Tile> gemMatches = new HashSet<Tile>();
		Optional<Tile> maybeTile = getTileOptional(startingPosition);
		if (maybeTile.HasValue) // ignore missing gems?
		{
			if (type.HasValue)
			{
				if (maybeTile.GetValue().Gem != null && (maybeTile.GetValue().Gem.Type == type.GetValue() || maybeTile.GetValue().Gem.Type == GemType.Rainbow))
				{
					gemMatches.Add(maybeTile.GetValue());
					gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type));
				}
			}
			else
			{
				gemMatches.Add(maybeTile.GetValue());
				gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type));
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
			if (maybeTile.GetValue().Gem != null && maybeTile.GetValue().Gem.Type.falls() && !maybeTile.GetValue().getIsBlocked())
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
		for (int x = 0; x < sizeX; x++)
		{
			int newGemCountInRow = 0;

			for (int y = sizeY - 1; y >= 0; y--)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Tile tile = getTileOptional(currentPosition).GetValue();
				if (tile.getIsBlocked()) {
					continue;
				}
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
						upperGem = generateRandomGemForTile(currentPosition);
						upperGem.Position = new Vector2(0, (y + 1 + newGemCountInRow) * tileSizeUnScaled.Y * -1);
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

	private void sendGemToTile(Gem gem, Tile tileTo, Tile removeFromTile, bool teleport = false)
	{
		tileTo.Gem = gem;
		Vector2 location = gem.GlobalPosition;
		clearTilesGem(removeFromTile, gem);
		tileTo.gemParent.AddChild(gem);
		if (teleport)
		{
			gem.Position = Vector2.Zero;
		}
		else
		{
			gem.GlobalPosition = location;
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

	private void populateBoard()
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Tile tile = getTile(currentPosition);
				if(!tile.getIsBlocked()){
					generateRandomGemForTile(currentPosition);
				}
			}
		}
		HashSet<HashSet<Tile>> matches = getMatches();
		while (matches.Count != 0)
		{
			foreach (HashSet<Tile> match in matches)
			{
				changeGemsColorAtPosition(match.First().getTilePosition(), GemTypeHelper.getRandomColor());
			}
			matches = getMatches();
		}
	}
	private Gem generateRandomGemForTile(Vector2 position)
	{
		Gem gem = gemScene.Instantiate() as Gem;
		Tile tile = getTileOptional(position).GetValue();

		GemType gemType = GemTypeHelper.getRandomColor();
		gem.setType(gemType);
		tile.gemParent.AddChild(gem);
		tile.Gem = gem;
		gem.Position = new Vector2(0, 0);
		return gem;
	}

	public List<Tile> getRandomTilesWithCondition(int count, Func<Tile, bool> condition)
	{
		List<Tile> tileList = getTilesWithCondition(condition);
		RandomHelper.Shuffle(tileList);
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

	public void deleteGemAtPositions(IEnumerable<Vector2> positions)
	{
		deleteGemAtPositions(positions, true);
	}

	public void deleteGemAtPosition(Vector2 position)
	{
		deleteGemAtPositions(new List<Vector2> { position }, true);
	}

	public HashSet<Tile> getTilesInDirection(Vector2 startingPosition, Vector2 direction)
	{
		return getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.None<GemType>());
	}

	public HashSet<Tile> getTilesInDirections(Vector2 startingPosition, HashSet<Vector2> directions)
	{
		return directions.SelectMany(direction => getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.None<GemType>())).ToHashSet();
	}

	public void changeGemsColorAtPosition(Vector2 position, GemType gemType)
	{
		changeGemsColorAtPosition(new HashSet<Vector2> { position }, gemType);
	}

	public List<Tile> getRandomNonBlackTile(int count)
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

	public List<Tile> getRandomNonBlackNonAddonTiles(int count)
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
			if (
				tile.Gem.AddonType != GemAddonType.None
			)
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
		foreach (Vector2 position in positions)
		{
			Tile tile = getTile(position);
			if (tile.Gem != null)
			{
				tile.Gem.setAddonType(gemAddonType);
			}
		}
	}

	public void changeGemsColorAtPosition(IEnumerable<Vector2> positions, GemType gemType)
	{
		foreach (Vector2 position in positions)
		{
			Tile tile = getTile(position);
			tile.Gem.setType(gemType);
		}
		thingsAreMoving = true;
	}

	public void switchGemsInPositions(Vector2 position1, Vector2 position2, bool teleport = false)
	{
		Tile tile1 = tileMap[position1];
		Tile tile2 = tileMap[position2];
		Optional<Gem> gem1 = Optional.FromNullable(tile1.Gem);
		Optional<Gem> gem2 = Optional.FromNullable(tile2.Gem);
		audioStreamPlayer2D.Stream = switchAudioStream;
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

}
