using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public GameObject character;
    [SerializeField] protected GameObject _hitEffect;
    [SerializeField] protected Vector3 _effectPos;
    [SerializeField] protected Vector3 _effectRot;
    [SerializeField] protected Vector3 _effectSca;

    public object[] StatStorage = new object[6];
    [SerializeField] protected int _skilldamage;
    [SerializeField] protected int _hitTimes;
    [SerializeField] protected float _stuntimes;
    [SerializeField] protected int _pushforce;
    [SerializeField] protected float _cooltime;
    [SerializeField] protected Define.Character _opponentObj;
    public Kinds _kinds;
    public Stat _stat;
    public enum Kinds
    {
        Bullet,
        NormalAttack,
        SkillS,
        SkillF,
        SkillG,
        Explosion,
    }

    public Kinds SkillKinds { get { return _kinds; } set { _kinds = value; } }

    protected virtual void OnCollisionEnter(Collision collision) { }
    protected virtual void OnTriggerEnter(Collider other) { }
    protected virtual void OnParticleCollision(GameObject other) { }
    
    void Awake()
    {
        if (SkillKinds == Kinds.Explosion || SkillKinds == Kinds.Bullet)
            character = Managers.Game.GetPlayer();
        _stat = character.GetComponent<Stat>();
    }

    public void SetStorage()
    {
        StatStorage[0] = _stat.MinAttack;
        StatStorage[1] = _stat.MaxAttack;
        StatStorage[2] = _skilldamage;
        StatStorage[3] = _hitTimes;
        StatStorage[4] = _stuntimes;
        StatStorage[5] = _pushforce;
    }

}
