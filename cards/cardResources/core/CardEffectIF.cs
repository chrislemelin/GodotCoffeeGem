using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class CardEffectIF: Resource
{
	[Export] public float Range {get; protected set;} = 1.0f;
	[Export] public int Value { get; set; }
	[Export] public int BlackGems { get; set; }
	[Export] public CardEffectGemType effectGemType {get;set;}

	public CardEffectIF() {

	}

	public void doEffect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles){
		effect(matchBoard, hand, mana, selectedTiles);
		createBlackGems(matchBoard);
	}

	private void createBlackGems(MatchBoard matchBoard) {
		List<Tile> tiles = matchBoard.getRandomNonBlackNonAddonTiles(BlackGems);
		if (tiles.Count != BlackGems) {
			tiles = matchBoard.getRandomNonBlackTile(BlackGems);
		}
		matchBoard.changeGemsColorAtPosition(tiles.Select(x => x.getPosition()).ToList(), GemType.Black);
	}

	public virtual void effect(MatchBoard matchBoard, Hand hand, Mana mana, List<Vector2> selectedTiles){
		
	}

	public virtual SelectionType getSelectionType(){
		return SelectionType.Single;
	}

	/// <summary>
	///  Get all tiles that can be selected after the first tile selection. This can be manually set if the range value wont cut it.
	///  This is currently only used for the double selection type
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesSelectableAfterFirstSelection(MatchBoard matchBoard, Tile tile){
		return new List<Vector2>();
	}

	/// <summary>
	/// Get all tiles that we are going to be effected if the tile is clicked
	/// This is currently only used for the single selection type
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesToHighlightOnHover(MatchBoard matchBoard, Tile tile){
		return getAllTilesToEffect(matchBoard, tile);
	}

	/// <summary>
	/// Helper function for getAllTilesToHighlightOnHover so the effect function doens't have to re-code which tiles we care about
	/// </summary>
	/// <param name="matchBoard"></param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public virtual List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile){
		return new List<Vector2>();
	}
}
