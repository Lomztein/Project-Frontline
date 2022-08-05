using UnityEngine;

namespace Util
{
    public static class VectorUtils
    {
        public static Vector3 MultiplyComponents(this Vector3 lhs, Vector3 rhs) =>
            new Vector3(
                lhs.x * rhs.x,
                lhs.y * rhs.y,
                lhs.z * rhs.z
            );

        public static float DifferenceAlongDirection(Vector3 direction, Vector3 rhs, Vector3 lhs)
        {
            float difference = Vector3.Dot(direction,
                rhs - lhs);

            return difference;
        }
    }
}