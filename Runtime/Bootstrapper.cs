using UnityEngine;

public class Bootstrapper {
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() {
        Object systems = Object.Instantiate(Resources.Load("Systems"));
        systems.name = "GlobalSystems";
        Object.DontDestroyOnLoad(systems);
    } 
}
