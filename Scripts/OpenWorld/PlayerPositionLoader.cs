using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    void Awake() // Awake kullanmak Start'tan daha hızlı ışınlanma sağlar
    {
        if (PlayerPrefs.GetInt("HasSavedPosition", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("SavedX");
            float y = PlayerPrefs.GetFloat("SavedY");
            float z = PlayerPrefs.GetFloat("SavedZ");

            transform.position = new Vector3(x, y, z);
            Debug.Log("Eski konuma dönüldü.");
        }
    }
}