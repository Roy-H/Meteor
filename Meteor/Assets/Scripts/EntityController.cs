using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour {

    private Entity mOwner;
    public Entity Owner { get { return mOwner; } }


    public void Init()
    {
        mOwner = GetComponent<Entity>();
    }
        // Update is called once per frame
        void Update () {
        CheckActionInput(Time.deltaTime);
    }
    void CheckActionInput(float deltaTime)
    {
        if (Owner == null)
            return;
        EntityBehaviour.Instance.ProcessBehaviour(Owner);
    }
}
