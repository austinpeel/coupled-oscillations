using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spring))]
public class SpringEditor : Editor
{
    private Spring.Type type = default;
    private float springConstant = 0;
    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    public override void OnInspectorGUI()
    {
        var spring = target as Spring;

        if (type != spring.type)
        {
            spring.Redraw();
            type = spring.type;
        }

        if (springConstant != spring.springConstant)
        {
            spring.Redraw();
            springConstant = spring.springConstant;
        }

        if (point1 != spring.point1 || point2 != spring.point2)
        {
            spring.Redraw();
            point1 = spring.point1;
            point2 = spring.point2;
        }

        if (radius != spring.radius)
        {
            spring.Redraw();
            radius = spring.radius;
        }

        DrawDefaultInspector();
    }
}
