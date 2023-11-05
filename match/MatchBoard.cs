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

	HashSet<Tile> tileSet = new HashSet<Tile>();
	Dictionary<Vector2, Tile> tileMap = new Dictionary<Vector2, Tile>();

	private Vector2 tileSize;
	private Vector2 tileSizeUnScaled;



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
				tileSet.Add(newTile);
				tileMap.Add(new Vector2(newX, newY), newTile);
				newTile.x = newX;
				newTile.y = newY;

				newTile.Position = new Vector2(newX * tileSize.X, newY * tileSize.Y);
			}
		}
		populateBoard();
	}

	public override void _Process(double delta)
	{
	}

	private void checkForMatches()
	{
		HashSet<HashSet<Tile>> matches = new HashSet<HashSet<Tile>>();
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				HashSet<Tile> tileMatches = new HashSet<Tile>();
				Optional<Tile> maybeTile = getTileFromGrid(currentPosition);
				Optional<Gem> maybeGem = Optional.FromNullable(maybeTile.GetValue().Gem);
				GemType gemType = maybeGem.GetValue().Type;
				tileMatches.Add(maybeTile.GetValue());


				HashSet<Tile> leftMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Left, Vector2.Left, gemType);
				HashSet<Tile> rightMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Right, Vector2.Right, gemType);
				HashSet<Tile> horizontalMatches = new HashSet<Tile>(leftMatches.Union(rightMatches));

				if (horizontalMatches.Count >= 2)
				{
					tileMatches.UnionWith(horizontalMatches);
				}

				HashSet<Tile> upMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Up, Vector2.Up, gemType);
				HashSet<Tile> downMatches = getGemsInDirectionOfSameType(currentPosition + Vector2.Down, Vector2.Down, gemType);
				HashSet<Tile> verticalMatches = new HashSet<Tile>(upMatches.Union(downMatches));

				if (verticalMatches.Count >= 2)
				{
					tileMatches.UnionWith(verticalMatches);
				}

				if (tileMatches.Count >= 3)
				{
					matches.Add(tileMatches);
				}
			}
		}
		foreach (HashSet<Tile> currentMatches in matches)
		{
			foreach (Tile currentTile in currentMatches)
			{
				if (currentTile.Gem != null)
				{
					currentTile.Gem.QueueFree();
					currentTile.Gem = null;
				}
			}
		}
		if (matches.Count > 0)
		{
			checkGemsForDrops();
		}
	}

	private HashSet<Tile> getGemsInDirectionOfSameType(Vector2 startingPosition, Vector2 direction, GemType type)
	{
		HashSet<Tile> gemMatches = new HashSet<Tile>();
		Optional<Tile> maybeTile = getTileFromGrid(startingPosition);
		if (maybeTile.HasValue)
		{
			if (maybeTile.GetValue().Gem.Type == type)
			{
				gemMatches.Add(maybeTile.GetValue());
				gemMatches.UnionWith(getGemsInDirectionOfSameType(startingPosition + direction, direction, type));

			}
		}
		return gemMatches;
	}

	private Optional<Tile> getNextTileWithGemInDirection(Vector2 startingPosition, Vector2 direction)
	{
		Optional<Tile> maybeTile = getTileFromGrid(startingPosition);
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
			int newGemCount = 0;

			for (int y = sizeY - 1; y >= 0; y--)
			{
				Vector2 currentPosition = new Vector2(x, y);
				Tile tile = getTileFromGrid(currentPosition).GetValue();
				if (tile.Gem == null)
				{
					Optional<Tile> upperTileWithGem = getNextTileWithGemInDirection(currentPosition, Vector2.Up);
					Gem upperGem;
					if (upperTileWithGem.HasValue)
					{
						upperGem = upperTileWithGem.GetValue().Gem;
						upperTileWithGem.GetValue().Gem = null;
						tile.Gem = upperGem;
						var location = upperGem.GlobalPosition;
						upperTileWithGem.GetValue().RemoveChild(upperGem);
						tile.AddChild(upperGem);
						upperGem.GlobalPosition = location;
						upperGem.moveToPostion(Vector2.Zero);

					}
					else
					{
						upperGem = generateGemForTile(currentPosition);
						upperGem.Position = new Vector2(0, (y + 1 + newGemCount) * tileSizeUnScaled.Y * -1);
						upperGem.moveToPostion(Vector2.Zero);
						newGemCount++;
					}
				}
			}
		}
	}

	private Optional<Tile> getTileFromGrid(Vector2 position)
	{
		if (!tileMap.ContainsKey(position))
		{
			return Optional.None<Tile>();
		}
		return Optional.Some<Tile>(tileMap[position]);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("click"))
		{
			checkForMatches();
		}
	}

	private void populateBoard()
	{
		for (int x = 0; x < sizeX; x++)
		{
			for (int y = 0; y < sizeY; y++)
			{
				Vector2 currentPosition = new Vector2(x, y);
				generateGemForTile(currentPosition);
			}
		}
	}
	private Gem generateGemForTile(Vector2 position)
	{
		Gem gem = gemScene.Instantiate() as Gem;
		Tile tile = getTileFromGrid(position).GetValue();

		GemType gemType = GemTypeHelper.getRandomColor();
		gem.setType(gemType);
		tile.AddChild(gem);
		tile.Gem = gem;
		return gem;
	}
}
