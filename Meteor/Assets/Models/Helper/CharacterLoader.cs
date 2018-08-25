using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using ProtoBuf;

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

        AmbLoader.Ins.LoadCharacterAmb(id);
        AmbLoader.Ins.LoadCharacterAmb();
        LoadBoxDef(id);
    }

    void LoadBoxDef(int idx)
    {
        idx = 0;
        TextAsset asset = Resources.Load<TextAsset>("boxdef0");
        if (asset == null)
            return;
        MemoryStream ms = new MemoryStream(asset.bytes);
        List<BoxColliderDef> boxdef = Serializer.Deserialize<List<BoxColliderDef>>(ms);
        for (int i = 0; i < bone.Count; i++)
        {
            for (int j = 0; j < boxdef.Count; j++)
            {
                if (boxdef[j].name == bone[i].name)
                {
                    BoxCollider bodef = bone[i].gameObject.AddComponent<BoxCollider>();
                    bodef.center = boxdef[j].center;
                    bodef.size = boxdef[j].size;
                    bodef.isTrigger = true;
                    bodef.enabled = true;
                    owner.AddHitBox(bodef);//受击盒.固定的
                    bone[i].gameObject.layer = LayerMask.NameToLayer("Bone");
                }
            }
        }
    }


    int TheFirstFrame = -1;//第一个Action的第一帧，0则无
    int TheLastFrame = -1;//最后一个Action的最后一帧，0则无
    Pose po;
    int blendStart = 0;
    public int curIndex = 0;
    float moveScale = 1.0f;
    Vector3 lastDBasePos = Vector3.zero;
    public void SetActionScale(float scale)
    {
        moveScale = scale;
    }
    public void SetPosData(Pose pos, float BlendTime = 0.0f, bool singlePos = false, int targetFrame = 0)
    {
        PosAction act = null;
        var isAttackPos = false;
        for (int i = 0; i < pos.ActionList.Count; i++)
        {
            //过滤掉565，刀雷电斩的头一个 第一个混合段与整个动画一致.
            if (pos.ActionList[i].Start == pos.Start && pos.ActionList[i].End == pos.End)
                continue;
            act = pos.ActionList[i];
            break;
        }
        TheLastFrame = pos.End - 1;
        TheFirstFrame = pos.Start;
        curIndex = targetFrame != 0 ? targetFrame : (act != null ? (act.Type == "Action" ? (isAttackPos ? act.Start : pos.Start) : (isAttackPos ? act.End : act.Start)) : pos.Start);

        //部分动作混合帧比开始帧还靠前
        if (curIndex < pos.Start)
            curIndex = pos.Start;
        blendStart = curIndex;

        po = pos;
        //下一个动作的第一帧所有虚拟物体
        if (po.SourceIdx == 0)
            lastDBasePos = AmbLoader.CharCommon[curIndex].DummyPos[0];
        else if (po.SourceIdx == 1)
            lastDBasePos = AmbLoader.FrameBoneAni[CharacterIndex][curIndex].DummyPos[0];
    }
}

public class BoneStatus
{
    public int startflag;
    public SVector BonePos;//相对位置,每一帧只有首骨骼有
    public List<SVector> DummyPos;//虚拟对象相对位置
    public List<SQuaternion> BoneQuat;//相对旋转.
    public List<SQuaternion> DummyQuat;//虚拟对象相对旋转
    public void Init()
    {
        DummyPos = new List<SVector>();
        BoneQuat = new List<SQuaternion>();
        DummyQuat = new List<SQuaternion>();
    }
}
public class AmbLoader
{
    AmbLoader()
    {
    }

    static AmbLoader _Ins;
    public static AmbLoader Ins
    {
        get
        {
            if (_Ins == null)
                _Ins = new AmbLoader();
            return _Ins;
        }
    }
    //所有角色公用的招式.
    public static Dictionary<int, BoneStatus> CharCommon = new Dictionary<int, BoneStatus>();
    //角色ID-角色动画帧编号-骨骼状态.
    public static Dictionary<int, Dictionary<int, BoneStatus>> FrameBoneAni = new Dictionary<int, Dictionary<int, BoneStatus>>();
    //加载个人自身的动作
    public void LoadCharacterAmb(int idx)
    {
        if (FrameBoneAni.ContainsKey(idx))
            return;
        if (idx == 24)
        {
            if (!FrameBoneAni.ContainsKey(24))
                FrameBoneAni.Add(24, FrameBoneAni[16]);
            return;
        }
        //大于20的是新角色，新角色只读skc其他男性角色读0号位数据 女性角色读1号位数据
        if (idx > 19)
        {
            if (FrameBoneAni.ContainsKey(idx))
                return;
            FrameBoneAni.Add(idx, FrameBoneAni[0]);
            return;
        }

        if (idx == 11)
            idx = 9;
        Dictionary<int, BoneStatus> ret = LoadAmb("p" + idx + ".amb");
        FrameBoneAni.Add(idx, ret);
        //9号文件和11号一样，复用
        if (idx == 9)
            FrameBoneAni.Add(11, ret);
    }
    //加载通用动作
    public void LoadCharacterAmb()
    {
        CharCommon = LoadAmb("characteramb");
    }

    //人物自身动作，0帧为TPose
    //招式通用动作，从1帧开始，没有0帧
    public Dictionary<int, BoneStatus> LoadAmb(string file)
    {
        long s1 = System.DateTime.Now.Ticks;
        TextAsset asset = Resources.Load<TextAsset>(file);
        if (asset == null)
        {
            Debug.LogError("amb file:" + file + " can not found");
            return null;
        }
        MemoryStream ms = new MemoryStream(asset.bytes);
        BinaryReader binRead = new BinaryReader(ms);
        binRead.BaseStream.Seek(5, SeekOrigin.Begin);
        int bone = binRead.ReadInt32();
        int dummy = binRead.ReadInt32();
        int frames = binRead.ReadInt32();
        int unknown = binRead.ReadInt32();
        Dictionary<int, BoneStatus> innerValue = new Dictionary<int, BoneStatus>();
        for (int i = 1; i <= frames; i++)
        {
            BoneStatus status = new BoneStatus();
            status.Init();
            status.startflag = binRead.ReadInt32();
            if (status.startflag != -1)
                Debug.LogError("frame:" + i + " startflag:" + status.startflag);
            int frameindex = binRead.ReadInt32();
            float x = binRead.ReadSingle();
            float y = binRead.ReadSingle();
            float z = binRead.ReadSingle();
            status.BonePos = new Vector3(x, z, y);//首骨骼的相对坐标.
            for (int j = 0; j < bone; j++)
            {
                float w = binRead.ReadSingle();
                float xx = -binRead.ReadSingle();
                float zz = -binRead.ReadSingle();
                float yy = -binRead.ReadSingle();
                Quaternion quat = new Quaternion(xx, yy, zz, w);
                status.BoneQuat.Add(quat);
            }
            for (int k = 0; k < dummy; k++)
            {
                binRead.BaseStream.Seek(5, SeekOrigin.Current);
                float dx = binRead.ReadSingle();
                float dy = binRead.ReadSingle();
                float dz = binRead.ReadSingle();
                float dw = binRead.ReadSingle();
                float dxx = -binRead.ReadSingle();
                float dzz = -binRead.ReadSingle();
                float dyy = -binRead.ReadSingle();
                status.DummyPos.Add(new Vector3(dx, dz, dy));
                status.DummyQuat.Add(new Quaternion(dxx, dyy, dzz, dw));
            }

            innerValue.Add(frameindex, status);
        }

        //豪微秒 10^-7秒
        //Debug.Log(string.Format("{0}", (double)(System.DateTime.Now.Ticks - s1) / 10000000.0));
        return innerValue;
    }
}

[ProtoContract]
public class BoxColliderDef
{
    [ProtoMember(1)]
    public string name;
    [ProtoMember(2)]
    public SVector center = new Vector3(0, 0, 0);
    [ProtoMember(3)]
    public SVector size = new Vector3(0, 0, 0);
}


