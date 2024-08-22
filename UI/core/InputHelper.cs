using Godot;
using System;

public partial class InputHelper
{

    public static bool isSelectedAction(InputEvent inputEvent) {
        return inputEvent.IsActionPressed("click") || inputEvent.IsActionPressed("ui_accept");
    }
    public static bool isClick(InputEvent inputEvent) {
        return inputEvent.IsActionPressed("click");
    }
     public static bool isControllerAccept(InputEvent inputEvent) {
        return inputEvent.IsActionPressed("ui_accept");
    }

}
