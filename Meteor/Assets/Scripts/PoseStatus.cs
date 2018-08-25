using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class PoseStatus
{
    public static Dictionary<int, List<Pose>> ActionList = new Dictionary<int, List<Pose>>();
    static Dictionary<int, TextAsset> PosFile = new Dictionary<int, TextAsset>();
    private Entity Owner;
    private CharacterLoader characterLoader;
    private int EntityId;

    public void Init(Entity owner)
    {
        Owner = owner;
        characterLoader = owner.GetComponent<CharacterLoader>();
        EntityId = Owner == null ? 0 : Owner.EntityId;
        if (!PosFile.ContainsKey(EntityId))
        {
            int TargetIdx = EntityId >= 20 ? 0 : EntityId;
            PosFile.Add(EntityId, Resources.Load<TextAsset>("9.07" + "/P" + TargetIdx + ".pos"));
            ActionList.Add(EntityId, new List<Pose>());
            ReadPose();
        }
    }
    void ReadPose()
    {
        if (PosFile[EntityId] != null)
        {
            Pose current = null;
            PosAction curAct = null;
            AttackDes att = null;
            DragDes dra = null;
            NextPose nex = null;
            int left = 0;
            int leftAct = 0;
            int leftAtt = 0;
            int leftDra = 0;
            int leftNex = 0;
            string text = System.Text.Encoding.ASCII.GetString(PosFile[EntityId].bytes);
            string[] pos = text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pos.Length; i++)
            {
                if (current != null && current.Idx == 573)
                {
                    //Debug.Log("get");
                }
                string line = pos[i];
                string[] lineObject = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (lineObject.Length == 0)
                {
                    //Debug.Log("line i:" + i);
                    //空行跳过
                    continue;
                }
                else if (lineObject[0].StartsWith("#"))
                    continue;
                else
                if (lineObject[0] == "Pose" && left == 0 && leftAct == 0)
                {
                    Pose insert = new Pose();
                    ActionList[EntityId].Add(insert);
                    int idx = int.Parse(lineObject[1]);
                    insert.Idx = idx;
                    current = insert;
                }
                else if (lineObject[0] == "{")
                {
                    if (nex != null)
                        leftNex++;
                    else
                    if (dra != null)
                        leftDra++;
                    else
                    if (att != null)
                    {
                        leftAtt++;
                    }
                    else
                        if (curAct != null)
                        leftAct++;
                    else
                        left++;
                }
                else if (lineObject[0] == "}")
                {
                    if (nex != null)
                    {
                        leftNex--;
                        if (leftNex == 0)
                            nex = null;
                    }
                    else
                    if (dra != null)
                    {
                        leftDra--;
                        if (leftDra == 0)
                            dra = null;
                    }
                    else
                    if (att != null)
                    {
                        leftAtt--;
                        if (leftAtt == 0)
                            att = null;
                    }
                    else
                    if (curAct != null)
                    {
                        leftAct--;
                        if (leftAct == 0)
                            curAct = null;
                    }
                    else
                    {
                        left--;
                        if (left == 0)
                            current = null;
                    }

                }
                else if (lineObject[0] == "link" || lineObject[0] == "Link" || lineObject[0] == "Link\t" || lineObject[0] == "link\t")
                {
                    current.Link = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "source" || lineObject[0] == "Source")
                {
                    current.SourceIdx = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "Start" || lineObject[0] == "start")
                {
                    if (nex != null)
                    {
                        nex.Start = int.Parse(lineObject[1]);
                    }
                    else
                    if (dra != null)
                    {
                        dra.Start = int.Parse(lineObject[1]);
                    }
                    else
                    if (att != null)
                    {
                        att.Start = int.Parse(lineObject[1]);
                    }
                    else
                    if (curAct != null)
                        curAct.Start = int.Parse(lineObject[1]);
                    else
                        current.Start = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "End" || lineObject[0] == "end")
                {
                    if (nex != null)
                    {
                        nex.End = int.Parse(lineObject[1]);
                    }
                    else
                    if (dra != null)
                    {
                        dra.End = int.Parse(lineObject[1]);
                    }
                    else
                    if (att != null)
                    {
                        att.End = int.Parse(lineObject[1]);
                    }
                    else
                    if (curAct != null)
                        curAct.End = int.Parse(lineObject[1]);
                    else
                        current.End = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "Speed" || lineObject[0] == "speed")
                {
                    if (curAct != null)
                        curAct.Speed = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "LoopStart")
                {
                    current.LoopStart = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "LoopEnd")
                {
                    current.LoopEnd = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "EffectType")
                {
                    current.EffectType = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "EffectID")
                {
                    current.EffectID = lineObject[1];
                }
                else if (lineObject[0] == "Blend")
                {
                    PosAction act = new PosAction();
                    act.Type = "Blend";
                    current.ActionList.Add(act);
                    curAct = act;
                }
                else if (lineObject[0] == "Action")
                {
                    PosAction act = new PosAction();
                    act.Type = "Action";
                    current.ActionList.Add(act);
                    curAct = act;
                }
                else if (lineObject[0] == "Attack")
                {
                    att = new AttackDes();
                    att.PoseIdx = current.Idx;
                    current.Attack.Add(att);
                }
                else if (lineObject[0] == "bone")
                {
                    //重新分割，=号分割，右边的,号分割
                    lineObject = line.Split(new char[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
                    string bones = lineObject[1];
                    while (bones.EndsWith(","))
                    {
                        i++;
                        lineObject = new string[1];
                        lineObject[0] = pos[i];
                        bones += lineObject[0];
                    }
                    //bones = bones.Replace(' ', '_');
                    string[] bonesstr = bones.Split(new char[] { ',' });
                    for (int j = 0; j < bonesstr.Length; j++)
                    {
                        string b = bonesstr[j].TrimStart(new char[] { ' ', '\"' });
                        b = b.TrimEnd(new char[] { '\"', ' ' });
                        b = b.Replace(' ', '_');
                        att.bones.Add(b);
                    }
                }
                else if (lineObject[0] == "AttackType")
                {
                    att._AttackType = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "CheckFriend")
                {
                    att.CheckFriend = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "DefenseValue")
                {
                    att.DefenseValue = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "DefenseMove")
                {
                    att.DefenseMove = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetValue")
                {
                    att.TargetValue = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetMove")
                {
                    att.TargetMove = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetPose")
                {
                    att.TargetPose = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetPoseFront")
                {
                    att.TargetPoseFront = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetPoseBack")
                {
                    att.TargetPoseBack = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetPoseLeft")
                {
                    att.TargetPoseLeft = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "TargetPoseRight")
                {
                    att.TargetPoseRight = int.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "Drag")
                {
                    dra = new DragDes();
                    current.Drag = dra;
                }
                else if (lineObject[0] == "Time")
                {
                    if (nex != null)
                        nex.Time = float.Parse(lineObject[1]);
                    else
                        dra.Time = float.Parse(lineObject[1]);
                }
                else if (lineObject[0] == "Color")
                {
                    string[] rgb = lineObject[1].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    dra.Color.x = int.Parse(rgb[0]);
                    dra.Color.y = int.Parse(rgb[1]);
                    dra.Color.z = int.Parse(rgb[2]);
                }
                else if (lineObject[0] == "NextPose")
                {
                    current.Next = new NextPose();
                    nex = current.Next;
                }
                else if (lineObject[0] == "{}")
                {
                    current = null;
                    continue;
                }
                else
                {
                    Debug.Log("line :" + i + " can t understand：" + pos[i]);
                    break;
                }
            }
        }
    }

    public void ChangeAction(int idx = CommonAction.Idle, float time = 0.0f, int targetFrame = 0)
    {
        if (characterLoader != null)
        {
            characterLoader.SetPosData(ActionList[EntityId][idx], time, false, targetFrame);
        }
    }
}

public class Pose
{
    public static int FPS;
    public int Idx;
    public int SourceIdx;
    public int Start;
    public int End;
    public int LoopStart;
    public int LoopEnd;
    public int EffectType;//这个链接到特效事件表里.
    public string EffectID;
    public List<PosAction> ActionList = new List<PosAction>();
    public int Link;
    public List<AttackDes> Attack = new List<AttackDes>();
    public DragDes Drag;
    public NextPose Next;
}

//[System.Serializable]
public class AttackDes
{
    public List<string> bones = new List<string>();//攻击伤害盒
    public int PoseIdx;//伤害由哪个动作赋予，由动作可以反向查找技能，以此算伤害
    public int Start;
    public int End;
    public int _AttackType;//0普攻1破防
    public int CheckFriend;
    public float DefenseValue;//防御僵硬
    public float DefenseMove;//防御时移动.
    public float TargetValue;//攻击僵硬
    public float TargetMove;//攻击时移动
    public int TargetPose;//受击时播放动作
    public int TargetPoseFront;  //挨打倒地096
    public int TargetPoseBack;//倒地前翻   099
    public int TargetPoseLeft;//倒地右翻   098
    public int TargetPoseRight;//倒地左翻  097
}

public class DragDes
{
    public int Start;
    public int End;
    public float Time;
    public Vector3 Color;
}


public class NextPose
{
    public int Start;
    public int End;
    public float Time;
}

[System.Serializable]
public class PosAction
{
    public string Type;//"Blend/Action"
    public int Start;
    public int End;
    public float Speed;//出招速度.

}

