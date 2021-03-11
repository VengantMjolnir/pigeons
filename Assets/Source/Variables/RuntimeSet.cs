using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public List<T> Items;

    private void OnEnable()
    {
        Items.Clear();
    }

    public void Add(T t)
    {
        if (!Items.Contains(t)) Items.Add(t);
    }

    public void Remove(T t)
    {
        if (Items.Contains(t)) Items.Remove(t);
    }
}
