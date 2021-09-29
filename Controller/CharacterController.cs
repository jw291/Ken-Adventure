using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
	[SerializeField]
	protected Vector3 _destPos;

	[SerializeField]
	protected DamageCalculator damageCalculator;

	[SerializeField]
	protected Define.State _state = Define.State.Idle;

	[SerializeField]
	public GameObject _lockTarget;

	public Define.Character CharacterType { get; protected set; } = Define.Character.Unknown;

	public virtual Define.State State
	{
		get { return _state; }
		set
		{
			_state = value;

			Animator anim = GetComponent<Animator>();
			switch (_state)
			{

				case Define.State.Idle:
					anim.CrossFade("Idle", 0.1f);
					break;
				case Define.State.Moving:
					anim.CrossFade("Moving", 0.3f);
					break;
				case Define.State.Die:
					anim.CrossFade("Die",0.5f);
					break;
				case Define.State.Attack:
					anim.CrossFade("Attack",0.5f);
					break;
				case Define.State.SkillF:
					anim.CrossFade("Skill",0.1f);
					StartSkillF();
					break;
				case Define.State.SkillS:
					anim.CrossFade("Skill", 0.1f);
					StartSkillS();
					break;
				case Define.State.SkillG:
					anim.CrossFade("Skill",0.1f);
					StartSkillG();
					break;
				case Define.State.Stun:
					anim.CrossFade("GetHit",0.1f);
					break;
			}
		}
	}

	private void Start()
	{
		Init();
	}

	void Update()
	{
		switch (State)
		{
			case Define.State.Die:
				UpdateDie();
				break;
			
			case Define.State.Idle:
				UpdateIdle();
				UpdateGunAttack();
				break;
			case Define.State.Stun:
				UpdateStun();
				break;
		}
	}

	void FixedUpdate()
    {
		switch (State)
		{

			case Define.State.Moving:
				UpdateMoving();
				UpdateGunAttack();
				break;

			case Define.State.Attack:
				UpdateAttack();
				break;
		}
	}

	void LateUpdate()
    {
		UpdateLockOn();
	}




	protected virtual void Init() { damageCalculator = new DamageCalculator(); }

	protected virtual void UpdateDie() { }
	protected virtual void UpdateMoving() { }
	protected virtual void UpdateIdle() { }
	protected virtual void UpdateAttack() { }
	protected virtual void UpdateGunAttack() { }
	protected virtual void UpdateStun() { }
	protected virtual void UpdateLockOn() { }
	protected virtual void StartSkillF() { }
	protected virtual void StartSkillS() { }
	protected virtual void StartSkillG() { }
}
