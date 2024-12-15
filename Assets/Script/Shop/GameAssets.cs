using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    public static GameAssets i
    {
        get
        {
            if (instance == null)
                instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return instance;
        }
    }
    public Sprite s_HP_1;
    public Sprite s_MP_1;
}
