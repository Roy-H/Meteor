using UnityEngine;
[System.Serializable]
[ProtoBuf.ProtoContract]
public struct SVector
{
    public SVector(float a, float b, float c)
    {
        x = a;
        y = b;
        z = c;
    }
    [ProtoBuf.ProtoMember(1)]
    public float x;
    [ProtoBuf.ProtoMember(2)]
    public float y;
    [ProtoBuf.ProtoMember(3)]
    public float z;

    public static implicit operator Vector3(SVector a)
    {
        return new Vector3(a.x, a.y, a.z);
    }

    public static implicit operator SVector(Vector3 a)
    {
        return new SVector(a.x, a.y, a.z);
    }

    public static Vector3 operator -(SVector a, SVector b)
    {
        return new Vector3(a.x, a.y, a.z) - new Vector3(b.x, b.y, b.z);
    }
}
[System.Serializable]
[ProtoBuf.ProtoContract]
public struct SVector2
{
    public SVector2(float a, float b)
    {
        x = a;
        y = b;
    }
    [ProtoBuf.ProtoMember(1)]
    public float x;
    [ProtoBuf.ProtoMember(2)]
    public float y;

    public static implicit operator Vector2(SVector2 a)
    {
        return new Vector3(a.x, a.y);
    }

    public static implicit operator SVector2(Vector2 a)
    {
        return new SVector2(a.x, a.y);
    }
}
[System.Serializable]
[ProtoBuf.ProtoContract]
public struct SQuaternion
{
    public SQuaternion(float w1, float x1, float y1, float z1)
    {
        w = w1;
        x = x1;
        y = y1;
        z = z1;
    }
    [ProtoBuf.ProtoMember(1)]
    public float w;
    [ProtoBuf.ProtoMember(2)]
    public float x;
    [ProtoBuf.ProtoMember(3)]
    public float y;
    [ProtoBuf.ProtoMember(4)]
    public float z;
    public static implicit operator Quaternion(SQuaternion a)
    {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }

    public static implicit operator SQuaternion(Quaternion a)
    {
        return new SQuaternion(a.w, a.x, a.y, a.z);
    }
}