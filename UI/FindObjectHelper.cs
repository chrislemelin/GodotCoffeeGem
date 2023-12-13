using Godot;
using System;
using System.Xml.Serialization;

public partial class FindObjectHelper
{

    public static readonly String NEW_BUTTON_NAME = "%NewTurnButton";
    public static readonly String MANA_NAME = "%Mana";
    public static readonly String HAND_NAME = "%Hand";

    public static readonly String GAME_MANAGER_NAME = "%NewTurnButton";
    public static readonly String HOME_GAME_MANAGER_NAME = "%NewTurnButton";



    public static Hand getHand(Node node) {
        return node.GetNode<Hand>(FindObjectHelper.HAND_NAME);
    }

    public static NewTurnButton getNewTurnButton(Node node) {
        return node.GetNode<NewTurnButton>(FindObjectHelper.NEW_BUTTON_NAME);
    }
    
    public static GameManagerIF getGameManager(Node node) {
        return (GameManagerIF)node.GetTree().CurrentScene;
    }

}
