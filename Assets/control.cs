using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class DroneController : MonoBehaviour
{
    [Header("飞行速度")]
    public float moveSpeed = 8f;
    public float verticalSpeed = 5f;

    [Header("音效设置")]
    public AudioClip flyingSound;
    public AudioClip collisionSound;
    public AudioClip successSound;

    [Header("碰撞粒子效果")]
    public ParticleSystem collisionEffect;
    public float effectLifetime = 3f;

    [Header("碰撞设置")]
    public LayerMask obstacleLayers;
    public float collisionSoundCooldown = 0.5f;

    private Rigidbody rb;
    private AudioSource audioSource;

    private float horizontal;
    private float forward;
    private float vertical;

    private float lastCollisionSoundTime = -1f;

    // 手机虚拟按键状态
    private bool mobileForward;
    private bool mobileBackward;
    private bool mobileLeft;
    private bool mobileRight;
    private bool mobileAscend;
    private bool mobileDescend;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // 防止飞行器翻转
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 飞行器不受重力影响
        rb.useGravity = false;

        // 改善高速运动时的碰撞检测
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        audioSource.playOnAwake = false;
        audioSource.loop = true;

        if (flyingSound != null)
        {
            audioSource.clip = flyingSound;
            audioSource.Play();
        }
    }

void Update()
    {
        // 1. 前后左右：原有的 Horizontal / Vertical 默认自动支持 Joy-Con 左摇杆，无需修改！
        horizontal = Input.GetAxisRaw("Horizontal")
            + (mobileRight ? 1f : 0f)
            - (mobileLeft ? 1f : 0f);

        forward = Input.GetAxisRaw("Vertical")
            + (mobileForward ? 1f : 0f)
            - (mobileBackward ? 1f : 0f);

        vertical = 0f;

        // 2. 上升/下降：保留键盘 J/K，同时增加手柄 R 键 (Button5) 和 L 键 (Button4)
        // 备用兼容：部分安卓设备的 ZR/ZL 会映射到 Button7/Button6，这里一并加上防呆
        if (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.JoystickButton7)) 
        {
            vertical += 1f; // 上升
        }
        
        if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.JoystickButton6)) 
        {
            vertical -= 1f; // 下降
        }

        // 手机虚拟按键高度控制
        if (mobileAscend) vertical += 1f;
        if (mobileDescend) vertical -= 1f;

        // 限制输入值在 -1 到 1 之间
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        forward = Mathf.Clamp(forward, -1f, 1f);
        vertical = Mathf.Clamp(vertical, -1f, 1f);
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(horizontal, 0f, forward);
        movement = Vector3.ClampMagnitude(movement, 1f);

        Vector3 velocity =
            movement * moveSpeed +
            Vector3.up * vertical * verticalSpeed;

        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 只处理指定图层上的障碍物
        int otherLayer = collision.gameObject.layer;

        if ((obstacleLayers.value & (1 << otherLayer)) == 0)
        {
            return;
        }

        // 获取实际碰撞位置
        ContactPoint contact = collision.GetContact(0);
        Vector3 hitPosition = contact.point;

        // 生成粒子效果
        if (collisionEffect != null)
        {
            ParticleSystem effect = Instantiate(
                collisionEffect,
                hitPosition,
                Quaternion.identity
            );

            effect.Play();
            Destroy(effect.gameObject, effectLifetime);
        }

        // 播放碰撞音效
        if (collisionSound != null &&
            Time.time - lastCollisionSoundTime
            >= collisionSoundCooldown)
        {
            audioSource.PlayOneShot(collisionSound);
            lastCollisionSoundTime = Time.time;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            if (successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }

            Debug.Log("成功通过检查点！");
        }
    }

    // 供手机按钮调用
    public void SetMobileInput(string direction, bool pressed)
    {
        switch (direction)
        {
            case "Forward":  mobileForward = pressed; break;
            case "Backward": mobileBackward = pressed; break;
            case "Left":     mobileLeft = pressed; break;
            case "Right":    mobileRight = pressed; break;
            case "Ascend":   mobileAscend = pressed; break;
            case "Descend":  mobileDescend = pressed; break;
        }
    }
}