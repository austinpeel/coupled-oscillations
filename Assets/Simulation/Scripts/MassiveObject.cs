using UnityEngine;

public class MassiveObject : MonoBehaviour
{
    [Min(0)] public float mass = 1f;
    public float HalfScale { get { return 0.5f * transform.localScale.x; } }

    public void SetMass(float mass)
    {
        transform.localScale = (1 + Mathf.Log10(Mathf.Max(0.1f, Mathf.Pow(mass, 0.33f)))) * Vector3.one;
        this.mass = mass;
    }
}
