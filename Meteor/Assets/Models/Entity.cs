using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public PoseStatus posMng;
    public EntityController controller;
    public bool IsHero { get; set; }
    public int EntityId;
    private void Start()
    {
        Init();
    }
    private void Awake()
    {
        
    }

    private void Init()
    {
        

        controller = gameObject.GetComponent<EntityController>();
        if (controller == null)
            controller = gameObject.AddComponent<EntityController>();

        var charLoader = GetComponent<CharacterLoader>();
        if (charLoader == null)
        {
            charLoader = gameObject.AddComponent<CharacterLoader>();
        }

        charLoader.LoadCharactor(EntityId);
        if (posMng == null)
            posMng = new PoseStatus();
        try
        {
            posMng.Init(this);
        }
        catch
        {
            Debug.LogError("unit id:" + EntityId);
        }
        posMng.ChangeAction();
        if (controller != null)
            controller.Init();

        
    }

    //受击盒列表
    public List<BoxCollider> hitList = new List<BoxCollider>();
    public void AddHitBox(BoxCollider co)
    {
        hitList.Add(co);
    }


    #region Action
    public void Defence()
    {
        //int weapon = GetWeaponType();
        //if (weapon == (int)EquipWeaponType.Gun || weapon == (int)EquipWeaponType.Dart || weapon == (int)EquipWeaponType.Guillotines)
        //    return;
        //posMng.ChangeAction(CommonAction.Defence, 0.1f);
    }

    public void DoBreakOut()
    {
        //if (AngryValue >= 60 || (Attr.IsPlayer && GameData.gameStatus.EnableInfiniteAngry))
        //{
        //    posMng.ChangeAction(CommonAction.BreakOut);
        //    AngryValue -= Attr.IsPlayer ? (GameData.gameStatus.EnableInfiniteAngry ? 0 : 60) : 60;
        //    if (Attr.IsPlayer)
        //        FightWnd.Instance.UpdateAngryBar();
        //}
    }

    public void ChangeWeapon()
    {
        posMng.ChangeAction(CommonAction.ChangeWeapon , 0.0f);
    }
    #endregion
}
