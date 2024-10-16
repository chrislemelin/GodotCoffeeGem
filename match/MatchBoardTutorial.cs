using Godot;
using System;
using System.Collections.Generic;

public partial class MatchBoardTutorial : ControllerInput
{
   	[Export] public bool tutorial = false;
	protected Dictionary<Vector2I, GemType> tutorialStartingBoard = new Dictionary<Vector2I, GemType>() {
		{new Vector2I(0,0), GemType.Milk},
		{new Vector2I(1,0), GemType.Leaf},
		{new Vector2I(2,0), GemType.Leaf},
		{new Vector2I(3,0), GemType.Sugar},
		{new Vector2I(4,0), GemType.Vanilla},
		{new Vector2I(5,0), GemType.Milk},

		{new Vector2I(0,1), GemType.Milk},
		{new Vector2I(1,1), GemType.Coffee},
		{new Vector2I(2,1), GemType.Vanilla},
		{new Vector2I(3,1), GemType.Coffee},
		{new Vector2I(4,1), GemType.Milk},
		{new Vector2I(5,1), GemType.Vanilla},

		{new Vector2I(0,2), GemType.Leaf},
		{new Vector2I(1,2), GemType.Sugar},
		{new Vector2I(2,2), GemType.Leaf},
		{new Vector2I(3,2), GemType.Leaf},
		{new Vector2I(4,2), GemType.Vanilla},
		{new Vector2I(5,2), GemType.Sugar},


		{new Vector2I(0,3), GemType.Coffee},
		{new Vector2I(1,3), GemType.Leaf},
		{new Vector2I(2,3), GemType.Coffee},
		{new Vector2I(3,3), GemType.Coffee},
		{new Vector2I(4,3), GemType.Vanilla},
		{new Vector2I(5,3), GemType.Milk},

		{new Vector2I(0,4), GemType.Sugar},
		{new Vector2I(1,4), GemType.Coffee},
		{new Vector2I(2,4), GemType.Vanilla},
		{new Vector2I(3,4), GemType.Leaf},
		{new Vector2I(4,4), GemType.Coffee},
		{new Vector2I(5,4), GemType.Vanilla},
	};

	protected List<GemType> tutorialGemSpawns = new List<GemType>() {
		GemType.Vanilla,GemType.Coffee, GemType.Coffee, GemType.Leaf, GemType.Coffee, GemType.Vanilla, GemType.Coffee, GemType.Coffee
	};

}
