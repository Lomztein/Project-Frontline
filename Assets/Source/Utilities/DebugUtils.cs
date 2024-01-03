using UnityEngine;

public static class DebugUtils
{
    public static void DebugDrawTrajectory(Vector3 start, Vector3 dir, float speed, float gravity)
    {
        int iters = 50000;
        float distPerIter = 0.1f;
        float timePerIter = distPerIter / speed;

        Vector3 pos = start;
        Vector3 vel = dir * speed;
        Vector3 prevPos = pos;

        while (true && iters-- > 0)
        {
            vel += gravity * timePerIter * Vector3.down;
            pos += vel * timePerIter;
            Debug.DrawLine(pos, prevPos);
            if (pos.y < 0f)
                break;
            prevPos = pos;
        }
    }

    public static Vector3 Flat(this Vector3 vec)
        => new Vector3(vec.x, 0f, vec.z);
}
