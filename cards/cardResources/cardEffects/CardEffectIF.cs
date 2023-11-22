using Godot;
using System;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class CardEffectIF: Resource
{
	[Export] public int Range {get; protected set;} = 1;
	public CardEffectIF() {

	}

	public virtual void doEffect(MatchBoard matchBoard, Mana mana, List<Vector2> selectedTiles){

	}

	public virtual SelectionType getSelectionType(){
		return SelectionType.Single;
	}

	public virtual List<Vector2> getAllTilesSelectable(List<Vector2> selectedTiles){
		return new List<Vector2>();
	}

	public virtual List<Vector2> getAllTilesToHighlightOnHover(MatchBoard matchBoard, Tile tile){
		return getAllTilesToEffect(matchBoard, tile);
	}
	public virtual List<Vector2> getAllTilesToEffect(MatchBoard matchBoard, Tile tile){
		return new List<Vector2>();
	}
}
