using UnityEngine;

public class MassiveObject : MonoBehaviour
{
    public float mass = 1f;

    public float HalfScale { get { return 0.5f * transform.localScale.x; } }
}
