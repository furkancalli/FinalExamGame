using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ayak Sesi Ayarları")]
    public AudioSource footstepAudioSource; 
    public AudioClip footstepSound;    
    public float footstepDelay = 0.5f; 
    private float footstepTimer;

    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float upSpeedMultiplier = 1.2f;
    public float downSpeedMultiplier = 1.1f;

    [Header("Referanslar")]
    private Animator anim;
    private CharacterController controller;

    private Vector3 velocity;
    private float gravity = -9.81f;

    // Yön hafızası
    private float lastX = 0f;
    private float lastY = -0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. GİRDİLERİ AL
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = new Vector3(h, 0, v).normalized;

        bool isMoving = moveInput.magnitude > 0.1f;

        // 2. ANIMASYON KONTROLÜ
        if (anim != null)
        {
            if (isMoving)
            {
                anim.SetFloat("MoveX", h);
                anim.SetFloat("MoveY", v);
                lastX = h * 0.1f;
                lastY = v * 0.1f;
            }
            else
            {
                anim.SetFloat("MoveX", lastX);
                anim.SetFloat("MoveY", lastY);
            }
        }

        // 3. FİZİKSEL HAREKET (Sildiğin kısım burasıydı!)
        if (isMoving)
        {
            // Yukarı/Aşağı giderken hız çarpanlarını uygula
            float verticalMult = (v > 0) ? upSpeedMultiplier : (v < 0 ? downSpeedMultiplier : 1f);
            Vector3 processedMove = new Vector3(moveInput.x, 0, moveInput.z * verticalMult);

            controller.Move(processedMove * moveSpeed * Time.deltaTime);
        }

        // 4. AYAK SESİ SİSTEMİ
        if (isMoving && controller.isGrounded)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstepSound();
                footstepTimer = footstepDelay; 
            }
        }
        else if (!isMoving)
        {
            footstepTimer = 0; 
        }

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    public void PlayFootstepSound()
    {
        if (footstepAudioSource != null && footstepSound != null)
        {
            footstepAudioSource.pitch = Random.Range(0.8f, 1.1f);
            footstepAudioSource.PlayOneShot(footstepSound);
        }
    }
}