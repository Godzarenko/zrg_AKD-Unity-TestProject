using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : Usable
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReinit()
    {
        //domain reset
    }

    public string UID;
    public Sprite MyUseIcon;
    public override Sprite GetUseIcon(PlayerScript User)
    {
        return MyUseIcon;
    }
    public override string GetUseText(PlayerScript User)
    {
        if (User.CurrentPickable != null)
        {
            return "Hands occupied";
        }
        else
        {
            return "E - Pickup";
        }
    }
    public override void Use(PlayerScript User)
    {
        if (User.CurrentPickable == null)
        {
            User.PlayerPickupItem(this);
        }
    }
}
