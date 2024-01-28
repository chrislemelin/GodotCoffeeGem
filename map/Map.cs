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

		foreach(Control control in topPathControls) {
			MapLocation mapLocation = createMapLocation(MapEventTypeHelper.getRandom());
			control.AddChild(mapLocation);
			topPath.AddRange(new List<MapLocation>{mapLocation});
		}
		foreach(Control control in botPathControls) {
			MapLocation mapLocation = createMapLocation(MapEventTypeHelper.getRandom());
			control.AddChild(mapLocation);
			botPath.AddRange(new List<MapLocation>{mapLocation});
		}
		home.GuiInput += (inputEvent) => {
			if (inputEvent.IsActionPressed("click")) {
				locationClicked(home);
		}};
		character.doneMovingSignal += () => locationVisited();
		setOnlyFirstLocationInPathToBeActive();
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
			manMoving = true;
			character.moveToGlobalPostion(mapLocation.GlobalPosition);
			locationMovingTo = mapLocation;
		}
	}

	private void locationVisited() {
		GD.Print("visiting location");
		MapLocation mapLocation = locationMovingTo;
		manMoving = false;
		if (pathTaken == 0) {
			pathTaken = 1;
			bool topSelected = true;
			foreach(MapLocation currentMapLocation in botPath) {
				if (mapLocation == currentMapLocation) {
					topSelected = false;
				}
			}
			List<Line2D> pathsToDeactive = !topSelected? topLines.ToList() : botlines.ToList();
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
}
