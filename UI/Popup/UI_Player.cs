using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Player : UI_Popup
{
    public GameObject _player;
    private PlayerController playerController;
    private WeaponController weaponController;
    private PlayerStat _stat;

    [SerializeField] private RectTransform Joy_background;
    [SerializeField] private RectTransform Joy_handle;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera camera;

    private GameObject JoyStick;
    private GameObject JoyHandle;
    private GameObject SwordAttack;
    private GameObject Roll;
    private GameObject SwordSkill1;
    private GameObject SwordSkill2;
    private GameObject GunAttack;
    private GameObject GunSkill;
    private GameObject WeaponSwitch;
    private GameObject Inventory;
    private GameObject Store;
    private GameObject Exit;
    private Image HealthBar;
    private Image ExpBar;
    private Image Skill1cooltimeImg;
    private Image Skill2cooltimeImg;
    private Image GunSkillcooltimeImg;
    private Image RollcooltimeImg;
    private Image SwitchcooltimeImg;
    private Text Level;

    public bool _lock = false;
    public bool _swordlock = false;
    public float distance;
    private float movespeed = 0f;
    private float radius;
    private float Vx;
    private float Vy;
    private float timer;
    private bool[] check = { false, false, false, false};
    private Vector3 movePosition;
    public Vector2 direction { set; get; }
    public float speedData { get { return movespeed; } set { movespeed = value; } }

    public Define.State playerstate { get; set; }

    public override void Init()
    {
        base.Init();//binding

        playerController = _player.GetComponent<PlayerController>();
        weaponController = _player.GetComponent<WeaponController>();
        _stat = _player.GetComponent<PlayerStat>();

        canvas = Managers.UI.canvas;
        camera = Camera.main;

        JoyStick = GetObject((int)GameObjects.JoyStick).gameObject;
        JoyHandle = GetObject((int)GameObjects.JoyStick_handle).gameObject;
        SwordAttack = GetButton((int)Buttons.SwordAttack).gameObject;
        Roll = GetButton((int)Buttons.Roll).gameObject;
        SwordSkill1 = GetButton((int)Buttons.SwordSkill1).gameObject;
        SwordSkill2 = GetButton((int)Buttons.SwordSkill2).gameObject;
        GunAttack = GetButton((int)Buttons.GunAttack).gameObject;
        GunSkill = GetButton((int)Buttons.GunSkill).gameObject;
        WeaponSwitch = GetButton((int)Buttons.WeaponSwitch).gameObject;
        Inventory = GetButton((int)Buttons.Inventory).gameObject;
        Store = GetButton((int)Buttons.Store).gameObject;
        Exit = GetButton((int) Buttons.Exit).gameObject;
        HealthBar = GetImage((int)Images.Healthbar).gameObject.GetComponent<Image>();
        ExpBar = GetImage((int)Images.Expbar).gameObject.GetComponent<Image>();
        Level = GetText((int)Texts.Level).gameObject.GetComponent<Text>();
        Skill1cooltimeImg = SwordSkill1.transform.GetChild(0).GetComponent<Image>();
        Skill2cooltimeImg = SwordSkill2.transform.GetChild(0).GetComponent<Image>();
        GunSkillcooltimeImg = GunSkill.transform.GetChild(0).GetComponent<Image>();
        RollcooltimeImg = Roll.transform.GetChild(0).GetComponent<Image>();
        SwitchcooltimeImg = WeaponSwitch.transform.GetChild(0).GetComponent<Image>();


        Joy_background = JoyStick.GetComponent<RectTransform>();
        Joy_handle = JoyHandle.GetComponent<RectTransform>();

        JoyStick.BindEvent(JoyStickDragging, Define.UIEvent.Drag);
        JoyStick.BindEvent(JoyStickPointerDown, Define.UIEvent.PointerDown);
        JoyStick.BindEvent(JoyStickPointerUp, Define.UIEvent.PointerUp);

        SwordAttack.BindEvent(SwordAttackStart, Define.UIEvent.PointerDown);
        SwordAttack.BindEvent(SwordAttackStop, Define.UIEvent.PointerUp);
        Roll.BindEvent(Rolling);
        SwordSkill1.BindEvent(SwordSkillFirst);
        SwordSkill2.BindEvent(SwordSkillSecond);
        GunAttack.BindEvent(GunAttackStart, Define.UIEvent.PointerDown);
        GunAttack.BindEvent(GunAttackStop, Define.UIEvent.PointerUp);
        GunSkill.BindEvent(GunSkillStart);
        WeaponSwitch.BindEvent(WeaponSwitchStart);
        Inventory.BindEvent(InventoryOpen);
        Store.BindEvent(StoreOpen);
        Exit.BindEvent(ExitOpen);

        radius = Joy_background.rect.width / 2;
        Debug.Log("radius : " + radius);

        GunAttack.SetActive(false);
        GunSkill.SetActive(false);

    }

    private void Update()
    {
        StartCoroutine("Optimizer");
    }

    IEnumerator Optimizer()
    {
        yield return new WaitForSeconds(0.05f);
        HealthBar.fillAmount = _stat.Hp / (float)_stat.MaxHp;
        ExpBar.fillAmount = _stat.Exp / (float)_stat.TotalEXP;
        Level.text = _stat.Level.ToString();
    }

    private void JoyStickDragging(PointerEventData eventData)
    {
        Vector2 value = eventData.position - (Vector2)this.Joy_background.position;//조이스틱 기준에서의 터치좌표
        value = Vector2.ClampMagnitude(value, radius); //최대길이를 반지름 만큼으로 제한한다.
        Joy_handle.localPosition = value;
        distance = Vector2.Distance(this.Joy_background.position, Joy_handle.position) / this.radius;

        direction = value.normalized * distance; // distance를 이용한 가속도
    }

    private void JoyStickPointerDown(PointerEventData eventData)
    {
        if (!_lock && !_swordlock)
        {
            if(playerController.State != Define.State.Stun)
                playerController.State = Define.State.Moving;
            JoyStickDragging(eventData);
        }

    }

    private void JoyStickPointerUp(PointerEventData eventData)
    {
        Joy_handle.localPosition = Vector2.zero;
        direction = Vector2.zero;
        if (!_lock && !_swordlock)
        {
            playerController.State = Define.State.Idle;
        }
    }

    private void SwordAttackStart(PointerEventData eventData) //sword는 따로 둬야할듯.
    {
        if (!_lock)
        {
            playerController.State = Define.State.Attack;
            _swordlock = true;
        }
    }

    private void SwordAttackStop(PointerEventData eventData)
    {
        if (!_lock)
        {
            playerController.State = Define.State.AttackStop;
            _swordlock = true;
            //playerController.GunShotStop();
            //StateReturn(); // State를 안바꾸면 계속 Attack이라서 공격하고
            //State를 바꾸면 바로 CrossFade Idle이라서 애니메이션이 동작을 안하고...
        }
    }

    private void GunAttackStart(PointerEventData eventData)
    {
        if (!_lock)
        {
            playerController.ShotFire();
        }

    }

    private void GunAttackStop(PointerEventData eventData)
    {
        if (!_lock)
        {
            playerController.ShotFireStop();
        }
    }

    private void GunSkillStart(PointerEventData eventData)
    {
        float _cooltime = 10f;
        int check_index = 2;
        if (!_lock && !check[2])
        {
            playerController.State = Define.State.SkillG;
            _lock = true;
            GunSkillcooltimeImg.fillAmount = 0;
            StartCoroutine(Timer(GunSkillcooltimeImg, _cooltime, check_index));
        }
    }

    private void SwordSkillFirst(PointerEventData eventData)
    {
        float _cooltime = 10f;
        int check_index = 0;
        if (!_lock && !check[0])
        {
            playerController.State = Define.State.SkillF;
            _lock = true;
            Skill1cooltimeImg.fillAmount = 0;
            StartCoroutine(Timer(Skill1cooltimeImg, _cooltime, check_index));
        }
    }

    private void SwordSkillSecond(PointerEventData eventData)
    {
        float _cooltime = 10f;
        int check_index = 1;
        if (!_lock && !check[1])
        {
            playerController.State = Define.State.SkillS;
            _lock = true;
            Skill2cooltimeImg.fillAmount = 0;
            StartCoroutine(Timer(Skill2cooltimeImg, _cooltime, check_index));
        }
    }

    private void Rolling(PointerEventData eventData)
    {
        if (!_lock && !_swordlock)
        {
            playerController.State = Define.State.Roll;
            _lock = true;
            StartCoroutine(unLocked_Default(1.0f));
        }
    }

    private void WeaponSwitchStart(PointerEventData eventData)
    {
        float _cooltime = 2f;
        int check_index = 3;
        if (!_lock && !_swordlock && !check[check_index])
        {
            playerController.WeaponSwitch();
            _lock = false;

            if(weaponController.WeaponState == WeaponController.Weapons.Sword)
            {
                SwordAttack.SetActive(false);
                SwordSkill1.SetActive(false);
                SwordSkill2.SetActive(false);

                GunAttack.SetActive(true);
                GunSkill.SetActive(true);
            }else
            {
                GunAttack.SetActive(false);
                GunSkill.SetActive(false);

                SwordAttack.SetActive(true);
                SwordSkill1.SetActive(true);
                SwordSkill2.SetActive(true);
            }
            SwitchcooltimeImg.fillAmount = 0;
            StartCoroutine(Timer(SwitchcooltimeImg, _cooltime, check_index));
        }
    }

    private void InventoryOpen(PointerEventData eventData)
    {
        Managers.UI.ShowPopupUI<UI_inventory>("Inventory_UI");
        Managers.Sound.Play("UIOpen");
    }

    private void StoreOpen(PointerEventData eventData)
    {
        Managers.UI.ShowPopupUI<UI_Store>("Store_UI");
        Managers.Sound.Play("UIOpen");
    }

    private void ExitOpen(PointerEventData eventData)
    {
        Managers.UI.ShowPopupUI<UI_Pause>("Pause_UI");
        Managers.Sound.Play("UIOpen");
    }

    private IEnumerator unLocked_Default(float sec) //animation Event
    {
        yield return new WaitForSeconds(sec);
        StateReturn();
        _lock = false;
    }

    public void StateReturn()
    {
        if (Joy_handle.anchoredPosition != new Vector2(0, 0))
            playerController.State = Define.State.Moving;
        else
            playerController.State = Define.State.Idle;
    }

    IEnumerator Timer(Image img, float _cooltimelimit, int num)
    {
        check[num] = true;
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);
        float time = 0;
        while(time < _cooltimelimit)
        {
            yield return waitForSeconds;
            time += 0.1f;
            img.fillAmount = time/ _cooltimelimit;
        }
        check[num] = false;
    }
    //private void Jumping(PointerEventData eventData)
    //{
    //    if (!_lock)
    //    {
    //        playerController.State = Define.State.Jump;
    //        _lock = true;
    //        StartCoroutine(unLocked(1.0f));
    //    }
    //}

}
