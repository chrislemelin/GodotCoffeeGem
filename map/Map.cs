using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Map : Node
{
	[Export] GameManagerIF gameManager;
	[Export] MapLocation store;
	[Export] MapLocation home;
	[Export] MapEventResolveUI mapEventResolveUI;
	[Export] PackedScene mapLocationPackedScene;
	[Export] Godot.Collections.Array<Control> topPathControls = new Godot.Collections.Array<Control>();
	[Export] Godot.Collections.Array<Control> botPathControls = new Godot.Collections.Array<Control>();
	[Export] private lerp character;

	[Export] Node2D topPathNodes;
	[Export] Node2D botPathNodes;

	private bool? topSelected = null;
	private Node2D nodeMovingTo;
	private int indexMovingTo = 0;

	private List<MapLocation> topPath = new List<MapLocation>();
	[Export] private Godot.Collections.Array<Line2D> topLines = new Godot.Collections.Array<Line2D>();
	private List<MapLocation> botPath = new List<MapLocation>();
	[Export] private Godot.Collections.Array<Line2D> botlines = new Godot.Collections.Array<Line2D>();
	private int pathTaken = 0;
	private bool manMoving = false;
	private MapLocation locationMovingTo;

	public override void _Ready()
	{
		base._Ready();

		List<List<MapEventType>> map = generateRandomMap();

		int topCount = 0;
		int botCount = 0;
		foreach(Control control in topPathControls) {
			MapLocation mapLocation = createMapLocation(map[0][topCount++]);
			control.AddChild(mapLocation);
			topPath.AddRange(new List<MapLocation>{mapLocation});
		}
		foreach(Control control in botPathControls) {
			MapLocation mapLocation = createMapLocation(map[1][botCount++]);
			control.AddChild(mapLocation);
			botPath.AddRange(new List<MapLocation>{mapLocation});
		}
		home.GuiInput += (inputEvent) => {
			if (inputEvent.IsActionPressed("click")) {
				locationClicked(home);
		}};
		character.doneMovingSignal += () => nodeVisited();
		setOnlyFirstLocationInPathToBeActive();
	}

	private List<List<MapEventType>> generateRandomMap() {
		bool usedHealSpot = false;
		List<List<MapEventType>> map = new List<List<MapEventType>>();
		for(int pathCount = 0; pathCount < 2; pathCount++) {
			List<MapEventType> path = new List<MapEventType>();
			for(int pathLength = 0; pathLength < 2; pathLength++) 
			{
				MapEventType mapEventType = MapEventTypeHelper.getRandom();
				if (mapEventType == MapEventType.Heal) 
				{
					if (!usedHealSpot) 
					{
						usedHealSpot = true;
					} else 
					{
						while(mapEventType == MapEventType.Heal) {
							mapEventType = MapEventTypeHelper.getRandom();
						}
					}
				}
				path.Add(mapEventType);
			}
			map.Add(path);
		}
		return map;

	}

	private MapLocation createMapLocation(MapEventType eventType) {
		MapLocation mapLocation = (MapLocation)mapLocationPackedScene.Instantiate();
		mapLocation.GuiInput += (inputEvent) => {
			if (inputEvent.IsActionPressed("click")) {
				locationClicked(mapLocation);
			}};
	mapLocation.setEventType(eventType);
		return mapLocation;
	}

	private void setOnlyFirstLocationInPathToBeActive(){
		foreach(MapLocation mapLocation in topPath) {
			mapLocation.setActive(false);
		}
		topPath[0].setActive(true);
		foreach(MapLocation mapLocation in botPath) {
			mapLocation.setActive(false);
		}
		botPath[0].setActive(true);
	}

	private void locationClicked(MapLocation mapLocation) {
		if (mapLocation.active && manMoving == false) {
			if (topSelected == null) {
				topSelected = true;
				foreach(MapLocation currentMapLocation in botPath) {
					if (mapLocation == currentMapLocation) {
						topSelected = false;
					}
				}
			}
			manMoving = true;

			List<Node> path = ((bool)topSelected)? topPathNodes.GetChildren().ToList() : botPathNodes.GetChildren().ToList();
			character.moveToGlobalPostion(((Node2D)path[indexMovingTo++]).GlobalPosition);
			locationMovingTo = mapLocation;
		}
	}

	private void locationVisited(MapLocation location) {
		MapLocation mapLocation = location;
		manMoving = false;
		if (pathTaken == 0) {
			pathTaken = 1;
			topSelected = true;
			foreach(MapLocation currentMapLocation in botPath) {
				if (mapLocation == currentMapLocation) {
					topSelected = false;
				}
			}
			List<Line2D> pathsToDeactive = !(bool)topSelected? topLines.ToList() : botlines.ToList();
			foreach(Line2D line in pathsToDeactive) {
				line.Visible = false;
			}
		}

		for(int index = 0; index < topPath.Count; index++) {
			topPath[index].setActive(false);
			if(topPath[index] == mapLocation){
				if (index == topPath.Count -1) {
					home.setActive(true);
				} else {
					topPath[++index].setActive(true);
				}
			}
		}
		for(int index = 0; index < botPath.Count; index++) {
			botPath[index].setActive(false);
			if(botPath[index] == mapLocation){
				if (index == botPath.Count -1) {
					home.setActive(true);
				} else {
					botPath[++index].setActive(true);
				}
			}
		}
		mapLocation.resolveEvent(mapEventResolveUI);
		mapLocation.setActive(false);
	}

	private void nodeVisited() {
		List<Node> path = ((bool)topSelected)? topPathNodes.GetChildren().ToList() : botPathNodes.GetChildren().ToList();
		List<MapLocation> locationPath = ((bool)topSelected)? topPath.ToList() : botPath.ToList();

		if (indexMovingTo == 2) {
			locationVisited(locationPath[0]);
		} else if (indexMovingTo == 3) {
			locationVisited(locationPath[1]);
		} else if (indexMovingTo == path.Count) {
			locationVisited(home);
		} else {
			character.moveToGlobalPostion(((Node2D)path[indexMovingTo++]).GlobalPosition);
		}
	}
}
