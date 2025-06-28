using UnityEngine;

namespace Ozurah.Utils.Unity
{
    public class HelloUnity
    {
        public static void HelloWorld()
        {
            Debug.Log("Hello from Ozurah.Utils !");
        }

        public static Vector2 V3to2(Vector3 v3)
        {
            Vector2 v2 = new Vector2(v3.x, v3.y);
            return v2;
        }
    }
}
