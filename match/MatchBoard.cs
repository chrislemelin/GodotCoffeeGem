using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;


public partial class MatchBoard : Node
{
	[Export] public int sizeX;
	[Export] public int sizeY;
	[Export] PackedScene tileTemplate;
	[Export] public PackedScene gemScene;
	[Export] public Hand hand;

	private Timer timer;
	private bool thingsAreMoving = true;


	HashSet<Tile> tileSet = new HashSet<Tile>();
	List<Tile> tilesSelected = new List<Tile>();
	HashSet<Tile> tilesAvailibleToBeSelected = new HashSet<Tile>();
	HashSet<Gem> gemsToBeDeleted = new HashSet<Gem>();


	Dictionary<Vector2, Tile> tileMap = new Dictionary<Vector2, Tile>();

	private Vector2 tileSize;
	private Vector2 tileSizeUnScaled;

	private void tileClicked(Tile tile, InputEvent inputEvent) {
		if(inputEvent.IsActionPressed("click")) {
			Optional<Card> cardSelectedInHand = hand.getCardSelected();
			if (cardSelectedInHand.HasValue) {
				Card card = cardSelectedInHand.GetValue();
				SelectionType selectionType = cardSelectedInHand.GetValue().getCardResource().getSelectionType();
				if (selectionType == SelectionType.Single) 
				{
					hand.tilesSelectedForCard(this, new List<Vector2> { tile.getPosition() }); 
				}
				if (selectionType == SelectionType.Double) {
					if(tilesAvailibleToBeSelected.Count == 0) {
						setTilesAvailibleToBeSelectedFromCard(card, tile);
					} else {
						 if (!tilesAvailibleToBeSelected.Contains(tile)) {
							clearTilesSelected();
							return;
						 }
					}
					addTileToSelectedList(tile);
					if (tilesSelected.Count == 2) {
						hand.tilesSelectedForCard(this, tilesSelected.Select(tile => tile.getPosition()).ToList());
						clearTilesSelected();
					}
				}

			}
		}
	}

	public void clearTilesSelected() {
		foreach(Tile tile in tileSet) {
			tile.turnHighlightOff();
			tile.turnSelectedOff();
		}
		tilesSelected.Clear();
		tilesAvailibleToBeSelected.Clear();
	}

	public void addTileToSelectedList(Tile tile) {
		tile.turnSelectedOn();
		tilesSelected.Add(tile);
	}

	public void addTileToAvailibleToSelectList(Tile tile) {
		tile.turnHighlightOn();
		tilesAvailibleToBeSelected.Add(tile);
	}

	private void setTilesAvailibleToBeSelectedFromCard(Card card, Tile center) {
		if (card.getCardResource().getSelectionType() == SelectionType.Double && tilesAvailibleToBeSelected.Count == 0){
			foreach (Tile currentTile in tileSet) {
				if( currentTile.getPosition().DistanceTo(center.getPosition()) <= card.getCardResource().cardEffect.Range){
					if (currentTile != center) {
						addTileToAvailibleToSelectList(currentTile);
					}
				}
			}

		}
	}

	private void checkForDrops() {
		if (thingsAreMoving && gemsToBeDeleted.Count == 0) {
			bool dropsAreDone = checkIfDropsAreDone();
			if (dropsAreDone) {
				thingsAreMoving = checkForMatches();
			}
		}
	}

	private bool checkIfDropsAreDone () {
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Optional<Tile> maybeTile = getTileOptional(currentPosition);
				Optional<Gem> maybeGem = Optional.FromNullable(maybeTile.GetValue().Gem);
				if (maybeGem.HasValue) {
					Gem gem = maybeGem.GetValue();
					if (gem.Position.Length() > .1f) {
						return false;
					}
				}
			}
		}
		return true;
	}

	private void checkForExtraHighlightsOnTileHover(Tile tile) {
		if (hand.getCardSelected().HasValue) {
			Card card = hand.getCardSelected().GetValue();
			List<Vector2> tilesToHover = card.getCardResource().cardEffect.getAllTilesToHighlightOnHover(this, tile);
			if (tilesToHover.Count > 0 ){
				clearTilesSelected();
				foreach(Vector2 position in tilesToHover) {
					addTileToAvailibleToSelectList(getTile(position));
				}
			}
	
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Tile sizeTile = tileTemplate.Instantiate() as Tile;
		tileSize = new Vector2(sizeTile.sprite2D.Texture.GetWidth() * sizeTile.Scale.X, sizeTile.sprite2D.Texture.GetHeight() * sizeTile.Scale.Y);
		tileSizeUnScaled = new Vector2(sizeTile.sprite2D.Texture.GetWidth(), sizeTile.sprite2D.Texture.GetHeight());

		sizeTile.QueueFree();

		for (int newX = 0; newX < sizeX; newX++)
		{
			for (int newY = 0; newY < sizeY; newY++)
			{
				Tile newTile = tileTemplate.Instantiate() as Tile;
				AddChild(newTile);
				newTile.area2D.InputEvent += (viewport, inputEvent, a) => tileClicked(newTile, inputEvent);
				tileSet.Add(newTile);
				tileMap.Add(new Vector2(newX, newY), newTile);
				newTile.x = newX;
				newTile.y = newY;

				newTile.Position = new Vector2(newX * tileSize.X, newY * tileSize.Y);
				newTile.area2D.MouseEntered += () => checkForExtraHighlightsOnTileHover(newTile);
			}
		}
		populateBoard();
		timer = new Timer();
		AddChild(timer);
		timer.WaitTime = .1;
		timer.Timeout += () => checkForDrops();
		timer.OneShot = false;
		timer.Start();
	}

	private bool checkForMatches()
	{
		
		HashSet<HashSet<Tile>> matches = new HashSet<HashSet<Tile>>(HashSet<Tile>.CreateSetComparer());
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				HashSet<Tile> tileMatches = new HashSet<Tile>();
				HashSet<Vector2> vectorMatches = new HashSet<Vector2>();

				Optional<Tile> maybeTile = getTileOptional(currentPosition);
				Optional<Gem> maybeGem = Optional.FromNullable(maybeTile.GetValue().Gem);
				GemType gemType = maybeGem.GetValue().Type;
				tileMatches.Add(maybeTile.GetValue());
				vectorMatches.Add(maybeTile.GetValue().getPosition());

				HashSet<Tile> leftMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Left, Vector2.Left, gemType);
				HashSet<Tile> rightMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Right, Vector2.Right, gemType);
				HashSet<Tile> horizontalMatches = new HashSet<Tile>(leftMatches.Union(rightMatches));

				if (horizontalMatches.Count >= 2)
				{
					tileMatches.UnionWith(horizontalMatches);
					vectorMatches.UnionWith(horizontalMatches.Select(x => x.getPosition()));
				}

				HashSet<Tile> upMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Up, Vector2.Up, gemType);
				HashSet<Tile> downMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Down, Vector2.Down, gemType);
				HashSet<Tile> verticalMatches = new HashSet<Tile>(upMatches.Union(downMatches));

				if (verticalMatches.Count >= 2)
				{
					tileMatches.UnionWith(verticalMatches);
					vectorMatches.UnionWith(verticalMatches.Select(x => x.getPosition()));
				}

				if (tileMatches.Count >= 3)
				{
					matches.Add(tileMatches);
				}
			}
		}
		HashSet<Vector2> tilesToDeleteGem = new HashSet<Vector2>();
		foreach (HashSet<Tile> currentMatches in matches)
		{
			foreach (Tile currentTile in currentMatches)
			{
				tilesToDeleteGem.Add(currentTile.getPosition());
			}
		}
		deleteGemAtPositions(tilesToDeleteGem);
		return matches.Count > 0;
	}
	public void deleteGemAtPosition(Vector2 position) {
		deleteGemAtPositions(new List<Vector2>{position});
	}

	private void gemDoneDying(Gem gem) {
		gemsToBeDeleted.Remove(gem);
		if (gemsToBeDeleted.Count == 0) {
			checkGemsForDrops();
			thingsAreMoving = true;
		}
 	}

	public void deleteGemAtPositions(IEnumerable<Vector2> positions) {
		List<Gem> gemsToDelete = positions.Select(pos => tileMap[pos].Gem).ToList();
		gemsToBeDeleted.UnionWith(gemsToDelete);
		foreach(Gem gem in gemsToDelete) {
			gem.doneDyingSignal += gemDoneDying;
		}

		foreach(Vector2 position in positions) {
			deleteGemAtPositionInternal(position);
		}
	}

	private void deleteGemAtPositionInternal(Vector2 position) {
		Optional<Tile> tileMaybe = getTileOptional(position);
		if (tileMaybe.HasValue) {
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
		if (maybeTile.HasValue && maybeTile.GetValue().Gem != null)
		{
			if (type.HasValue){
				if (maybeTile.GetValue().Gem.Type == type.GetValue())
				{
					gemMatches.Add(maybeTile.GetValue());
					gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type));
				}
			} else {
				gemMatches.Add(maybeTile.GetValue());
				gemMatches.UnionWith(getTilesInDirectionOfSameGemType(startingPosition + direction, direction, type));
			}
		}
		return gemMatches;
	}

	public HashSet<Tile> getTilesInDirection(Vector2 startingPosition, Vector2 direction)
	{
		return getTilesInDirectionOfSameGemType(startingPosition, direction, Optional.None<GemType>());
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
			if (maybeTile.GetValue().Gem != null)
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
	private Optional<Tile> getTileOptional(Vector2 position)
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
	public override void _Input(InputEvent @event)
	{

	}

	public List<Tile> getTilesWithColorOfGem(GemType gemType) {
		List<Tile> tilesToReturn = new List<Tile>();
		foreach(Tile tile in tileSet) {
			Optional<Gem> gemMaybe = Optional.FromNullable(tile.Gem);
			if (gemMaybe.HasValue && gemMaybe.GetValue().Type == gemType) {
				tilesToReturn.Add(tile);
			}
		}
		
		return tilesToReturn;
	}
	private void sendGemToTile (Gem gem, Tile tileTo, Tile removeFromTile){
		tileTo.Gem = gem;
		Vector2 location = gem.GlobalPosition;
		clearTilesGem(removeFromTile, gem);
		
		tileTo.AddChild(gem);
		gem.GlobalPosition = location;
		gem.moveToPostion(Vector2.Zero);
	}

	private void clearTilesGem (Tile tile){ 
		clearTilesGem(tile, tile.Gem);
	}

	
	private void clearTilesGem (Tile tile, Gem gemToRemove){ 
		tile.RemoveChild(gemToRemove);
		if (gemToRemove == tile.Gem) {
			tile.Gem = null;
		}
	}

	public void switchGemsInPositions(Vector2 position1, Vector2 position2) {
		Tile tile1 = tileMap[position1];
		Tile tile2 = tileMap[position2];
		Optional<Gem> gem1 = Optional.FromNullable(tile1.Gem);
		Optional<Gem> gem2 = Optional.FromNullable(tile2.Gem);
		if (gem1.HasValue) {
			sendGemToTile(gem1.GetValue(), tile2, tile1);
		}
		 if (gem2.HasValue) {
		 	sendGemToTile(gem2.GetValue(), tile1, tile2);
		}
		thingsAreMoving = true;
	}


	private void populateBoard()
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				generateRandomGemForTile(currentPosition);
			}
		}
	}
	private Gem generateRandomGemForTile  (Vector2 position)
	{
		Gem gem = gemScene.Instantiate() as Gem;
		Tile tile = getTileOptional(position).GetValue();

		GemType gemType = GemTypeHelper.getRandomColor();
		gem.setType(gemType);
		tile.AddChild(gem);
		tile.Gem = gem;
		return gem;
	}
}
