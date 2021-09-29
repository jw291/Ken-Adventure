using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private List<WeaponVFX> SwishEffect = new List<WeaponVFX>();
    [SerializeField] private List<WeaponVFX> ShotEffect = new List<WeaponVFX>();
    //Animation Rigging TwoBoneIKConstraint Component
    [SerializeField] private TwoBoneIKConstraint[] TBIC = new TwoBoneIKConstraint[3];
    [SerializeField] private RigBuilder _rigbuilder;
    private PlayerInventory _inventory;
    private List<GameObject> Bulletlist = new List<GameObject>();
    private List<GameObject> Grenadelist = new List<GameObject>();
    private GameObject SwordVFXRoot;
    private GameObject GunVFXRoot;
    private Animator _anim;
    private int _comboStep;
    private int _bullet = 20;
    private int _grenade = 5;
    private int pos = 1;
    private float cliptime;
    private float _increaseLerpTime;
    private float _decreaseLerpTime;
    private bool _comboPossible;
    private bool _swordskill;
    private bool _vfxTrigger = true;
    private bool _attack = true;
    private bool _stop = true;

    public Weapons _weaponState;

    public GameObject _lockTarget;
    public GameObject SwordHand;
    public GameObject GunHand;
    public GameObject GrenadeHand;

    public enum Weapons
    {
        Sword,
        Gun,
    }

    public Weapons WeaponState { get { return _weaponState; } set { _weaponState = value; } }
  
    void Start()
    {
        _inventory = GetComponent<PlayerInventory>();
        _anim = GetComponent<Animator>();
        _rigbuilder = GetComponent<RigBuilder>();

        WeaponState = Weapons.Sword;

        //------Weapon OBJ
        SwordHand = Util.AllFindChild(gameObject, "Sword");
        GunHand = Util.AllFindChild(gameObject, "Gun");
        GrenadeHand = Util.AllFindChild(gameObject, "GrenadeHand");

        _inventory.DefaultItemEquip();
        GrenadeHand.SetActive(false);

        //------effect OBJ
        SwordVFXRoot = Util.AllFindChild(gameObject, "SwordVFX");
        GunVFXRoot = Util.AllFindChild(gameObject, "GunVFX");
        foreach(Transform child in SwordVFXRoot.transform)
        {
            SwishEffect[pos++] = child.gameObject.GetOrAddComponent<WeaponVFX>();
        }
        pos = 1;
        foreach(Transform child in GunVFXRoot.transform)
        {
            ShotEffect[pos++] = child.gameObject.GetOrAddComponent<WeaponVFX>();
        }

        //------Object Pool Setting
        Set_Grenade();
        Set_Bullet();

    }

    //칼 콤보공격 가능 여부와 콤보스텝 증가
    public void Sword_Attack()
    {
        if (_comboStep == 0)
        {
            _anim.Play("sword_combo1");
            Managers.Sound.Play("Sword1");
            _comboStep = 1;
            return;
        }
        if (_comboStep != 0)
        {
            if (_comboPossible)
            {
                _comboPossible = false;
                _comboStep += 1;
            }
        }
    }

    public void Sword_SkillF()
    {
        //Skill Tab에서 Set한 것으로 교체 가능
        _anim.Play("SwordSkill1");
        Managers.Sound.Play("Skill1");
    }

    public void Sword_SkillS()
    {
        _anim.Play("SwordSkill2");
        Managers.Sound.Play("Skill2");
    }

    //총 기본 공격
    public void Gun_Attack()
    {
        if (_anim.GetFloat("MoveSpeed") >= 3)
        {
            //Animation Rigging Weight값 조절 
            TBIC[1].weight = 1; //right hand weight
        }
        else
        {
            TBIC[0].weight = 1; //left hand weight
            TBIC[1].weight = 1; //right hand weight
        }

        if (_attack)
        {
            _attack = false;
            _rigbuilder.enabled = true;
            if (_rigbuilder.enabled == true)
                StartCoroutine("Gun_AttackWithReboundRoutine");
        }
    }

    private void Bullet_Fire()
    {
        GameObject _go = Managers.Resource.Instantiate("Object/Pistol/Bullet");
    }

    public void Gun_AttackStop()
    {
        if (_stop)
        {
            _stop = false;
            StartCoroutine("Gun_ResetRoutine");
        }
    }

    public void Gun_SkillG()
    {
        _anim.Play("Throw");
        GunHand.SetActive(false);
        GrenadeHand.SetActive(true);
        StartCoroutine("SkillG_ResetRoutine");
    }

    //총 기본 공격 시, 반동에 따른 Animation Rigging Weight값 조절, Skill Effect, Sound, 공격 Delay 
    IEnumerator Gun_AttackWithReboundRoutine()
    {
        yield return new WaitUntil(() => _rigbuilder.enabled == true);
        Managers.Sound.Play("NormalGunshot");
        VFX_Activate(1); // Normal VFX
        VFX_Activate(3); // Shot Light
        Bullet_Fire();
        yield return new WaitForSeconds(0.05f);
        VFX_DeActivate(3);
        float timer = 0;
        while (timer <= 1f)
        {
            yield return new WaitForSeconds(0.001f);
            timer += (20f * Time.fixedDeltaTime);
            TBIC[2].weight = timer;
        }
        yield return new WaitForSeconds(0.2f);
        VFX_DeActivate(1);
        while (timer >= 0f)
        {
            yield return new WaitForSeconds(0.0001f);
            timer -= (5f * Time.fixedDeltaTime);
            TBIC[2].weight = timer;
        }
        _attack = true;
        _stop = true;
    }

    IEnumerator Gun_ResetRoutine()
    {
        yield return new WaitUntil(() => _attack == true);
        _rigbuilder.enabled = false;
        float timer = 1f;
        while (timer >= 0f)
        {
            timer -= (5f * Time.fixedDeltaTime);
            TBIC[0].weight = timer;
            TBIC[1].weight = timer;
        }
    }

    IEnumerator SkillG_ResetRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        if (gameObject.GetComponent<PlayerController>().State != Define.State.Idle)
            gameObject.GetComponent<PlayerController>().State = Define.State.Idle;
        GunHand.SetActive(true);
        GrenadeHand.SetActive(false);
    }

    //Object Pooling 적용
    void Set_Bullet()
    {
        //ResourceManager에서 정의한 Instantiate로 Bullet 생성
        //생성과 동시에 Object Pool 생성
        for (int i = 0; i <= _bullet; i++)
        {
            GameObject go = Managers.Resource.Instantiate("Object/Pistol/Bullet", null, true);
            Bullet bullet = go.GetOrAddComponent<Bullet>();
            bullet.playerController = GetComponent<PlayerController>();
            bullet.character = gameObject;
            bullet._stat = GetComponent<PlayerStat>();
        }
        //Object Pool이 존재하는 Object의 경우에는
        //SetActive False
        foreach (GameObject obj in Bulletlist)
        {
            Managers.Resource.Destroy(obj);
        }
    }

    void Set_Grenade()
    {
        for (int i = 0; i <= _grenade; i++)
        {
            GameObject go = Managers.Resource.Instantiate("Object/Pistol/Grenade", null, true);
            Grenade grenade = go.GetOrAddComponent<Grenade>();
            grenade.character = gameObject;
            grenade._stat = GetComponent<PlayerStat>();
        }
        foreach (GameObject obj in Grenadelist)
        {
            Managers.Resource.Destroy(obj);
        }
    }

    //Animation_Event
    //----------------Sword Attack

    public void Sword_ComboPossible()
    {
        _comboPossible = true;
    }

    public void Sword_NextAttack()
    {
        if (!_swordskill)
        {
            if (_comboStep == 2)
            {
                _anim.Play("sword_combo2");
                Managers.Sound.Play("Sword23", Define.Sound.Effect,0.8f);
            }
            if (_comboStep == 3)
            {
                _anim.Play("sword_combo3");
                Managers.Sound.Play("Sword23", Define.Sound.Effect, 0.9f);
            }
            if (_comboStep == 4)
            {
                _anim.Play("sword_combo4");
                Managers.Sound.Play("Sword4", Define.Sound.Effect, 0.9f);
            }
        }
    }

    public void Sword_ResetCombo()
    {
        if (gameObject.GetComponent<PlayerController>().State != Define.State.Idle)
            gameObject.GetComponent<PlayerController>().State = Define.State.Idle;
        _comboPossible = false;
        _comboStep = 0;
    }

    public void VFX_Activate(int tmp = 1)
    {
        List<WeaponVFX> Effect = new List<WeaponVFX>();

        if (WeaponState == Weapons.Sword) Effect = SwishEffect;
        else Effect = ShotEffect;

        if (WeaponState == Weapons.Sword)
        {
            for (int i = 1; i <= Effect.Count - 1; i++)
            {
                if (Effect[i].IsActive())
                {
                    Effect[i].DeActivate();
                }
            }
            Effect[tmp].Activate();
        }
        else
            Effect[tmp].Activate();

    }

    public void VFX_DeActivate(int tmp = 1)
    {
        List<WeaponVFX> Effect = new List<WeaponVFX>();

        if (WeaponState == Weapons.Sword) Effect = SwishEffect;
        else Effect = ShotEffect;
        Effect[tmp].DeActivate();
    }

    public void Grenade_Fire()
    {
        GameObject _go = Managers.Resource.Instantiate("Object/Pistol/Grenade");
        Grenade genenade = _go.GetOrAddComponent<Grenade>();
    }

}

