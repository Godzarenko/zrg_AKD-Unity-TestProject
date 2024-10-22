using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReinit()
    {
        //domain reset
    }

    [Header("Eyes")]
    public Transform Eyes;
    public float MouseXSensivity = 1;
    public float MouseYSensivity = -1;

    [Header("Body")]
    public float LinearSpeed = 7;
    public Rigidbody MyRB;

    [Header("Interractions")]
    public float InterractionRange = 1.4f;
    public Transform PlayerHoldingPos;
    public float FlyTime = 0.3f;
    public AnimationCurve ItemFlyCurve;
    [HideInInspector]public PickableItem CurrentPickable;

    public LayerMask PickableFindMask;
    public LayerMask StorageFindMask;


    Usable lastUsable = null;

    FlyItem<PickableItem> pickedUpItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //mouse movement
        float mousex = Input.GetAxis("Mouse X");
        float mousey = Input.GetAxis("Mouse Y");
        if (mousex != 0 || mousey != 0)
        {
            Vector3 LocalEulerAngles = Eyes.localEulerAngles;
            LocalEulerAngles.y += mousex * MouseXSensivity;
            LocalEulerAngles.x += mousey * MouseYSensivity;
            //contain in -180 .. 180 degrees
            if (LocalEulerAngles.x < -180)
            {
                LocalEulerAngles.x += 360;
            }
            if(LocalEulerAngles.x > 180)
            {
                LocalEulerAngles.x -= 360;
            }

            //keep in -88 .. 88 degrees for projections
            if (LocalEulerAngles.x > 88)
            {
                LocalEulerAngles.x = 88;
            }
            if (LocalEulerAngles.x < -88)
            {
                LocalEulerAngles.x = -88;
            }
            Eyes.localEulerAngles = LocalEulerAngles;
        }
        //player movement
        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        if (vert != 0 || hor != 0)
        {
            Vector3 Fwd = Vector3.ProjectOnPlane(Eyes.transform.forward, transform.up).normalized;
            Vector3 Rght = Vector3.ProjectOnPlane(Eyes.transform.right, transform.up).normalized;
            Vector3 InputVector = (Fwd * vert + Rght * hor);
            if(InputVector.sqrMagnitude>1) InputVector.Normalize();
            //transform.position += InputVector * LinearSpeed * Time.deltaTime;
            Vector3 Velocity = MyRB.velocity;
            Velocity.x = InputVector.x * LinearSpeed;
            Velocity.z = InputVector.z * LinearSpeed;

            MyRB.velocity = Velocity;
        }

        //usables
        if (CurrentPickable == null)
        {
            //seek items
            RaycastHit RHIT;
            if(Physics.SphereCast(Eyes.transform.position,0.25f, Eyes.transform.forward, out RHIT, InterractionRange, PickableFindMask, QueryTriggerInteraction.Ignore))
            {
                Usable U = RHIT.collider.GetComponentInParent<Usable>();
                lastUsable = U;
            }
            else
            {
                lastUsable = null;
            }
        }
        else
        {
            //seek storage
            RaycastHit RHIT;
            if (Physics.SphereCast(Eyes.transform.position, 0.25f, Eyes.transform.forward, out RHIT, InterractionRange, StorageFindMask, QueryTriggerInteraction.Ignore))
            {
                Usable U = RHIT.collider.GetComponentInParent<Usable>();
                lastUsable = U;
            }
            else
            {
                lastUsable = null;
            }
        }

        if(lastUsable != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                lastUsable.Use(this);
            }
            //show prompt
            UI_UsePrompt.Instance.ShowPrompt(lastUsable.GetUseText(this), lastUsable.GetUseIcon(this));
        }
        else
        {
            //hide prompt
            UI_UsePrompt.Instance.HidePrompt();
        }

        if (pickedUpItem != null)
        {
            if (pickedUpItem.Item == null)
            {
                pickedUpItem = null;
            }
            else
            {
                if (pickedUpItem.t < 1)
                {
                    pickedUpItem.t += Time.deltaTime / FlyTime;
                    pickedUpItem.Item.transform.SetPositionAndRotation(
                        Vector3.Lerp(pickedUpItem.StartPos, PlayerHoldingPos.position, pickedUpItem.t) + ItemFlyCurve.Evaluate(pickedUpItem.t) * Vector3.up,
                        Quaternion.Lerp(pickedUpItem.Item.transform.rotation, PlayerHoldingPos.rotation, pickedUpItem.t)
                    );
                    if (pickedUpItem.t >= 1)
                    {
                        pickedUpItem.Item.transform.parent = PlayerHoldingPos;
                        pickedUpItem.Item.transform.localRotation = Quaternion.identity;
                        pickedUpItem.Item.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }
    }
    public void ItemRemovedFromHands()
    {
        pickedUpItem = null;
        CurrentPickable = null;
    }
    public void PlayerPickupItem(PickableItem item)
    {
        CurrentPickable = item;
        pickedUpItem = new FlyItem<PickableItem>();
        pickedUpItem.Item = item;
        pickedUpItem.StartPos = item.transform.position;
        pickedUpItem.StartRotation = item.transform.rotation;
    }
}

public class FlyItem<T>
{
    public T Item;
    public float t;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public Quaternion StartRotation;
    public Quaternion EndRotation;
}
