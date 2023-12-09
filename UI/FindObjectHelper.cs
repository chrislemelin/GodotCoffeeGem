using Godot;
using System;

public partial class FindObjectHelper
{

    public static readonly String NEW_BUTTON_NAME = "%NewTurnButton";
    public static readonly String MANA_NAME = "%Mana";
    public static readonly String HAND_NAME = "%Hand";

    public static Hand getHand(Node node) {
        return node.GetNode<Hand>(FindObjectHelper.HAND_NAME);
    }

    public static NewTurnButton getNewTurnButton(Node node) {
        return node.GetNode<NewTurnButton>(FindObjectHelper.NEW_BUTTON_NAME);
    }


}
