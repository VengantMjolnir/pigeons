using UnityEngine;

[CreateAssetMenu]
public class Vector3Variable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public Vector3 Value;
}
