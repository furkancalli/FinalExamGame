using UnityEngine;

public class RotateObject : MonoBehaviour
{
    void Update()
    {
        // Objenin Y ekseninde saniyede 100 derece dönmesini sağlar
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}   