using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Usable
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReinit()
    {
        //domain reset
    }

    public BoxCollider StorageBounds;
    public AnimationCurve ItemFlyCurve;
    public float FlyTime = 0.3f;
    List<FlyItem<PickableItem>> pickedUpItems = new List<FlyItem<PickableItem>>(); //can be used to seek contained items
    List<PickableItem> containedItems = new List<PickableItem>();
    void Update()
    {
        for(int i = 0; i < pickedUpItems.Count; i++)
        {
            FlyItem<PickableItem> itm = pickedUpItems[i];
            if (itm.Item == null)
            {
                //item was destroyed
                pickedUpItems.RemoveAt(i);
                i--;
                containedItems.Remove(itm.Item); //remove destroyed item or null
                continue;
            }
            if(itm.Item.transform.parent != transform)
            {
                //item was removed from storage
                pickedUpItems.RemoveAt(i);
                i--;
                containedItems.Remove(itm.Item);
                continue;
            }
            if (itm.t < 1)
            {
                itm.t += Time.deltaTime / FlyTime;
                itm.Item.transform.SetPositionAndRotation(
                    Vector3.Lerp(itm.StartPos, itm.EndPos, itm.t) + ItemFlyCurve.Evaluate(itm.t) * Vector3.up,
                    Quaternion.Lerp(itm.Item.transform.rotation, transform.rotation, itm.t)
                );
                if (itm.t >= 1)
                {
                    itm.Item.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
    public Sprite MyUseIcon;
    public override Sprite GetUseIcon(PlayerScript User)
    {
        return MyUseIcon;
    }
    public override string GetUseText(PlayerScript User)
    {
        if (User.CurrentPickable != null)
        {
            return "E - Store item";
        }
        else
        {
            return "Hands empty";
        }
    }
    public override void Use(PlayerScript User)
    {
        if (User.CurrentPickable != null)
        {
            Additem(User.CurrentPickable);
            User.ItemRemovedFromHands();
        }
    }
    public void Additem(PickableItem NewItem)
    {
        FlyItem<PickableItem> fly = new FlyItem<PickableItem>();
        fly.Item = NewItem;
        fly.StartPos = NewItem.transform.position;
        fly.StartRotation = NewItem.transform.rotation;

        containedItems.Add(NewItem);

        NewItem.transform.parent = transform;

        Vector3 Closest = StorageBounds.ClosestPoint(NewItem.transform.position);
        fly.EndPos = Closest;
        pickedUpItems.Add(fly);
    }
}
