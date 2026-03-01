using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    // Oyuncunun hızı 15'ti. Engeller 5 hızıyla gitsin ki onlara yetişebilelim.
    public float moveSpeed = 5f; 

    void Update()
    {
        // Engeller sürekli ileri (Z ekseninde) kendi hızlarında ilerler
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}