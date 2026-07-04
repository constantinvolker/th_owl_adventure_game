using UnityEngine;

public static class Initialize
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Debug.Log("Loaded by the Persistent Objects from the initializer script");
        Object.DontDestroyOnLoad(
            Object.Instantiate(Resources.Load("PERSISTOBJECTS"))
        );
    }
}