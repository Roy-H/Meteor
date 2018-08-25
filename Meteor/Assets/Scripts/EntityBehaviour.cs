using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : Singleton<EntityBehaviour>
{
    //defance, change weapon, boost handle
    public bool ProcessNormalAction(Entity Owner)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Owner.Defence();
            return true;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Owner.ChangeWeapon();
            return true;
        }
        else if (Input.GetKey(KeyCode.B))
        {
            Owner.DoBreakOut();
            return true;
        }
        return false;

    }


    public void ProcessBehaviour(Entity target)
    {
        PoseStatus posMng = target.posMng;
        
        Entity Owner = target;
        if (ProcessNormalAction(Owner))
        {
            return;
        }
    }
}
