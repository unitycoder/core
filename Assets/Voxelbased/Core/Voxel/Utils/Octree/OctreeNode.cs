using Unity.Mathematics;

public struct OctreeNode
{
    public long guid;
    public float3 position;
    public float3 voxelPosition;
    public float size;
    public byte lodLevel;
    public bool hasChildren;
    public long[] children;

    public OctreeNode
    (
        long guid,
        float3 position,
        float3 voxelPosition,
        float size,
        byte lodLevel,
        bool hasChildren
    )
    {
        this.guid = guid;
        this.position = position;
        this.voxelPosition = voxelPosition;
        this.size = size;
        this.lodLevel = lodLevel;
        this.hasChildren = hasChildren;
        children = new long[8];
    }
}
