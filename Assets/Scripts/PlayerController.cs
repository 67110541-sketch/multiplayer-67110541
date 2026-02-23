using UnityEngine;
using Fusion;
using static Unity.Collections.Unicode;
public class PlayerController : NetworkBehaviour
{

    [SerializeField] private NetworkCharacterController _ncc;
    [SerializeField] private Animator _animator;
    [SerializeField] private NetworkMecanimAnimator _networkAnimator;
    // ตวัช่วยซิงค์Anim อัตโนมัติ
    [Networked] public int HP { get; set; } = 100;
    [Networked] public NetworkBool IsDead { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        if (IsDead) return;
        if (GetInput(out NetworkInputData data))
        {
            // 1. การเคลื่อนที่ 3D บนพ้ืนผิว(X และ Z) normalized ทา ใหเ้ดินเฉียงไม่เร็วเกินไป
            Vector3 moveVector = new Vector3(data.direction.x, 0, data.direction.y).normalized;
            // 2. ใช้ NCC ในการเคลื่อนที่ (รองรับ Gravity 3D ในตัว)
            _ncc.Move(5 * moveVector * Runner.DeltaTime);
            // 3. การหมุนตัวละครใน 3D (Smooth Rotation)
            if (moveVector != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveVector);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,

                Runner.DeltaTime * 10f);
            }
            // 4. ส่งค่าไป Animator
            _animator.SetFloat("Speed", moveVector.magnitude);
            // เช็คการโจมตี
            if (data.buttons.WasPressed(data.buttons, NetworkInputData.BUTTON_ATTACK))
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        // สั่งใหท้ ุกเครื่องเห็นท่าโจมตีพร้อมกนั
        _networkAnimator.SetTrigger("Attack", true);
        Debug.Log("Player Attacked!");
    }
}
