using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EventSubscription : MonoBehaviour
{
    //Each effect's index needs to be assgned respectively in the Functions
    public VisualEffect[] effects;

    void Start()
    {
        //putting this at Start() won't re-enable VFX event after disabled during gameplay
        VisualEventManager.AccumalatedSpeed += PassVfxShootStar;
        Debug.Log("ShootStar Subscribed");

        VisualEventManager.VelocityThreshold0 += PassVfxWave;
        Debug.Log("Wave Subscribed");

        VisualEventManager.VelocityThreshold1 += PassVfxWave2;
        Debug.Log("Wave2 Subscribed");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Create functions to subscribe to Events
    private void PassVfxEvent(VisualEffect vfx, string eventName)
    {
        vfx.SendEvent(eventName);
        //Debug.Log(eventName + "sent");
    }
    private void PassVfxShootStar()
    {
        if (effects[0].isActiveAndEnabled)
        {
            PassVfxEvent(effects[0], "ShootStar");
        }
        else
        {
            VisualEventManager.AccumalatedSpeed -= PassVfxShootStar;
            Debug.Log("ShootStar Unsubscribed");
        }
    }
    private void PassVfxWave()
    {
        if (effects[1].isActiveAndEnabled)
        {
            PassVfxEvent(effects[1], "Wave");
        }
        else
        {
            VisualEventManager.VelocityThreshold0 -= PassVfxShootStar;
            Debug.Log("Wave Unsubscribed");
        }
    }

    private void PassVfxWave2()
    {
        if (effects[2].isActiveAndEnabled)
        {
            PassVfxEvent(effects[2], "Wave");
        }
        else
        {
            VisualEventManager.VelocityThreshold1 -= PassVfxShootStar;
            Debug.Log("Wave Unsubscribed");
        }
    }
}
