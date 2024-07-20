using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class UnlockableCardPackList : Resource
{
	[Export] public Array<UnlockableCardPack> packs;


	public UnlockableCardPackList () {
	
	}
}
