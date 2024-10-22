using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Usable : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReinit()
    {
        //domain reset
    }

    public virtual Sprite GetUseIcon(PlayerScript User)
    {
            return null;
    }
    public virtual string GetUseText(PlayerScript User)
    {
            return "";
    }
    public virtual void Use(PlayerScript User)
    {

    }
}
