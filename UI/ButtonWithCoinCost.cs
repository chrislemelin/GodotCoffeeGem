using Godot;
using System;

public partial class ButtonWithCoinCost : Button
{
    [Export] public int cost;

    public void setCurrentCoin(int coin) {
        if (coin < cost) {
            Disabled = true;
        } else {
            Disabled = false;
        }
    }

}
