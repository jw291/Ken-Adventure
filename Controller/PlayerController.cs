using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : CharacterController
{
    [SerializeField] private float _movepower = 5.0f;
    [SerializeField] private ParticleSystem _gethitPs;
    private PlayerStat _stat;
    private WeaponController _weapon;
    private UI_Player _playerui;

    // 총 공격 시, 타겟과 플레이어가 바라보는 각도가 -85 ~ 85도 이내라면 true
    private bool _Targetting = false;

    private SkinnedMeshRenderer _renderer; 
    private Color _matcolor; //타격 시 material color
    private Transform playerChestTr; // 상체 Bone
    private Vector3 ChestOffset; //상체 회전 보정
    private bool _shotfire = false;
    private Animator anim;
    private GameObject Joystick_background; //1. 이동 UI : 조이스틱
    private Rigidbody rb;
    private  Vector3 _direction;

    private  float sec = 0.2f;
    private bool cool = false;

    public new Define.State State
    {
        get { return base._state; }
        set
        {
            base._state = value;

            anim = GetComponent<Animator>();
            switch (base._state)
            {

                case Define.State.Idle:
                    ((CharacterController)this).State = Define.State.Idle;
                    break;
                case Define.State.Moving:
                    ((CharacterController)this).State = Define.State.Moving;
                    break;
                case Define.State.SkillF:
                    ((CharacterController)this).State = Define.State.SkillF;
                    break;
                case Define.State.SkillS:
                    ((CharacterController)this).State = Define.State.SkillS;
                    break;
                case Define.State.SkillG:
                    ((CharacterController)this).State = Define.State.SkillG;
                    break;
                case Define.State.Die:
                    ((CharacterController)this).State = Define.State.Die;
                    break;
                case Define.State.Stun:
                    ((CharacterController)this).State = Define.State.Stun;
                    break;
                case Define.State.Roll:
                    anim.CrossFade("Roll", 0.01f);
                    StartRoll();
                    break;
            }
        }
    }

    public override void Init()
    {
        base.Init();

        CharacterType = Define.Character.Player;

        _playerui = Managers.UI.ShowPopupUI<UI_Player>("PlayerControl_UI");//UI생성 및 컴포넌트 추가
        _playerui.speedData = _movepower;
        _playerui._player = gameObject;

        _stat = GetComponent<PlayerStat>();
        _weapon = GetComponent<WeaponController>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _renderer = transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();

        _matcolor =_renderer.material.color;
        playerChestTr = anim.GetBoneTransform(HumanBodyBones.Spine);

    }

    protected override void UpdateIdle()
    {
        //State Update
        _playerui._swordlock = false;
        _playerui._lock = false;
        anim.SetFloat("MoveSpeed", 0f);
        anim.SetBool("lockonMove", false);
        ChestOffset = new Vector3(0, 65, 0);
    }
    protected override void UpdateDie() { }

    protected override void UpdateMoving()
    {
        ChestOffset = new Vector3(0, 0, 0);
        //Joystick Data를 참조하여 움직인다.
        //velocity : 질량 상관 없이 일정 속도
        //Addforce : F = ma 질량과 가속도에 따른 힘 적용(점프에 적당함)
        //MovePosition : 지정된 위치/속도로 이동하는 힘을 가함(물리적영향을 받지 않음)
        _direction = new Vector3(_playerui.direction.x, 0f, _playerui.direction.y);
        _direction = Camera.main.transform.TransformDirection(_direction); // Camera Direction을 기준으로 회전 방향을 정함
        _direction.y = 0f;
        
        rb.velocity = (_direction * _movepower);

        float _movingspeed = rb.velocity.magnitude;

        //총 공격 시, 타겟과 플레이어가 바라보는 각도가 -85 ~ 85도 이내라면 최대 이동 속도가 제한된다
        if (_Targetting)
        {
            anim.SetBool("lockonMove", true);
            _movepower = 2f;
            if (_movingspeed >= 2.5)
            {
                _movingspeed = 2.5f;
                anim.SetFloat("MoveSpeed", _movingspeed);
            }else anim.SetFloat("MoveSpeed", _movingspeed);
        }
        else
        {
            anim.SetBool("lockonMove", false);
            _movepower = 5.0f;
            anim.SetFloat("MoveSpeed", _movingspeed);
            if (_movingspeed >= 4.5f)
            {
                _movingspeed = 4.5f;
                anim.SetFloat("MoveSpeed", _movingspeed);
            }
        }

        if (_direction != Vector3.zero)
            rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), 20 * Time.deltaTime);
    }

    protected override void UpdateAttack()
    {
        //칼공격과 무기공격을 나누기 위해서 여기서 애니메이션 컨트롤
        if (_weapon.WeaponState == WeaponController.Weapons.Sword)
        {
            _weapon.Sword_Attack();
        }
    }

    protected override void UpdateGunAttack()
    {
        if (_shotfire)
            _weapon.Gun_Attack();
    }

    protected override void StartSkillF()
    {
        _weapon.Sword_SkillF();
    }

    protected override void StartSkillS()
    {
        _weapon.Sword_SkillS();
    }

    protected override void StartSkillG()
    {
        _weapon.Gun_SkillG();
    }

    protected override void UpdateLockOn()
    {

        if (_lockTarget == null) //_lockTaget은 Bullet Class에 의해서도 결정된다.
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, 10.0f, 1 << 8); // PlayerStat scanRange
            if (colls.Length != 0)
            {
                _lockTarget = colls[0].gameObject;
            }
            _Targetting = false;
        }
        else
        { 
            Vector3 Dir = _lockTarget.transform.position - playerChestTr.position;
            if (_shotfire)
            {
                //상체 회전
                Dir.y = 0;
                Quaternion q = Quaternion.LookRotation(Dir);
                Quaternion origin = Quaternion.LookRotation(_direction);
                float angle = Quaternion.Angle(q, origin);
                if (-85f <= angle && angle <= 85f)
                {
                    playerChestTr.rotation = q * Quaternion.Euler(ChestOffset);
                    _Targetting = false;
                }
                else
                {
                    transform.rotation = q;
                    _Targetting = true;
                }
            }
            else
            {
                _Targetting = false;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //    Gizmos.DrawWireSphere(transform.position, 10.0f);
    //}

    void StartRoll()
    {
        StartCoroutine("DelayRoll");
    }

    public void ShotFire()
    {
        if (!_shotfire)
            _shotfire = true;
    }

    public void ShotFireStop()
    {
        if (_shotfire)
            _shotfire = false;
    }

    public void WeaponSwitch()
    {
        if (_weapon.WeaponState == WeaponController.Weapons.Sword)
        {
            anim.SetTrigger("SwitchBtoA");
            StartCoroutine(PerformAnimRoutine(0, 0.7f));
        }
        else if (_weapon.WeaponState == WeaponController.Weapons.Gun)
        {
            anim.SetTrigger("SwitchAtoB");
            StartCoroutine(PerformAnimRoutine(1, 0.19f));
        }
    }

    public void GetHitAttack(object[] arr)
    {
        StartCoroutine(DelayGetHit(arr));
    }

    void Stunning(float _pushforce)
    {
        State = Define.State.Stun;
        rb.AddForce(-transform.forward * 35f, ForceMode.Impulse);
        StartCoroutine("DelayBackRoll");
    }

    //attacker로부터 Sendmessage를 통해 attacker의 Data를 전달받음
    //해당 데이터를 통해 데미지를 계산하여 플레이어의 Hp를 감소시킴
    IEnumerator DelayGetHit(object[] attackerData)
    {
        int _hitnum = (int)attackerData[3];
        float _stuntimes;
        float _pushforce = (int)attackerData[5];
        for (int i = 0; i < _hitnum; i++)
        {
            StartCoroutine("DelayMatReturn");
            _gethitPs.Play();
            _stuntimes = (float)attackerData[4];

            if (_stuntimes != 0f)
                Stunning(_pushforce);

            _stat.Hp -= damageCalculator.Calculate(attackerData, _stat);
            Debug.Log(damageCalculator.Calculate(attackerData, _stat));
            if (_stat.Hp <= 0)
            {
                _stat.Hp = 0;
                State = Define.State.Die;
                StartCoroutine("DestroyByDead");
                break;
            }

            yield return new WaitForSeconds(_stuntimes);
        }
    }

    protected IEnumerator DestroyByDead()
    {
        yield return new WaitForSeconds(2.5f);
    }

    //Sword -> Gun
    //Gun -> Sword Switching
    //애니메이션에 따라 약간의 딜레이 적용
    private IEnumerator PerformAnimRoutine(int value, float sec)
    {
        yield return new WaitForSeconds(sec);
        if (value == 0)
        {
            _weapon.SwordHand.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _weapon.GunHand.gameObject.SetActive(true);
            Managers.Sound.Play("GunSwitching");
            _weapon.WeaponState = WeaponController.Weapons.Gun;
        }
        else if (value == 1)
        {
            Managers.Sound.Play("SwordSwitching");
            _weapon.GunHand.gameObject.SetActive(false);
            _weapon.SwordHand.gameObject.SetActive(true);
            _weapon.WeaponState = WeaponController.Weapons.Sword;
        }
    }

    private IEnumerator DelayRoll()
    {
        yield return new WaitForSeconds(0.2f);
        rb.AddForce(transform.forward * 35f, ForceMode.Impulse);
    }

    private IEnumerator DelayBackRoll()
    {
        yield return new WaitForSeconds(1.0f);
        rb.AddForce(-transform.forward * 25f, ForceMode.Impulse);
    }

    private IEnumerator DelayMatReturn()
    {
        _renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _renderer.material.color = _matcolor;
    }

    public void StateReturn()
    {
        if (_direction != Vector3.zero)
            State = Define.State.Moving;
        else
            State = Define.State.Idle;
    }


}