using UnityEngine;
using System.Collections;

public class OnlyOne<T> : MonoBehaviour where T:OnlyOne<T>
{
    public static T Instance;
	protected virtual void Awake () 
    {
        Done();
	}

    void Done()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Debug.LogWarning("There is another [" + Instance.GetType() + "] in  current scene. Destroy this [" + Instance.GetType() + "] on GameObject:[" + gameObject.name + "].");
            DestroyImmediate(this);
        }
    }
}
