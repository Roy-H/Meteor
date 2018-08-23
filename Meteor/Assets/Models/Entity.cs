using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool IsHero { get; set; }
    public int EntityId;
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        var charLoader = GetComponent<CharacterLoader>();
        if (charLoader == null)
        {
            charLoader = gameObject.AddComponent<CharacterLoader>();
        }
            
        charLoader.LoadCharactor(EntityId);
    }
}
