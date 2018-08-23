using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    //public
    public List<Transform> bone;
    
    //root position and rotation
    public Transform rootBone;
    public Vector3 rootPosition;
    public Quaternion rootQuat;
    public GameObject Skin;
    //private
    SkinnedMeshRenderer rend; //renderer mesh and uv bone weight
    private int CharacterIndex = 0;
    List<Transform> dummy;
    private Entity owner;


    public void LoadCharactor(int id)
    {
        owner = GetComponent<Entity>();

        CharacterIndex = id;
        Skin = new GameObject();
        Skin.transform.SetParent(transform);
        Skin.transform.localRotation = Quaternion.identity;
        Skin.transform.localScale = Vector3.one;
        Skin.transform.localPosition = Vector3.zero;

        //load skin
        SkcFile skc = SkcLoader.Instance.Load(id);

        Skin.name = skc.Skin;
        rend = Skin.AddComponent<SkinnedMeshRenderer>();
        BncFile bnc = BncLoader.Instance.Load(id);

        rend.localBounds = skc.mesh.bounds;
        rend.materials = skc.Material(id);
        rend.sharedMesh = skc.mesh;
        rend.sharedMesh.RecalculateBounds();

        bone = new List<Transform>();
        dummy = new List<Transform>();

        List<Matrix4x4> bindPos = new List<Matrix4x4>();

        if (owner.IsHero)
            Skin.layer = LayerMask.NameToLayer("LocalPlayer");
        else
            Skin.layer = LayerMask.NameToLayer("Monster");

        bnc.GenerateBone(transform, ref bone, ref dummy, ref bindPos, ref rootBone);

        rend.bones = bone.ToArray();
        rend.sharedMesh.bindposes = bindPos.ToArray();
        rend.rootBone = rootBone;
        rootPosition = rootBone.localPosition;
        rootQuat = rootBone.localRotation;
    }
}
