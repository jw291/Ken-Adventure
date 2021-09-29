using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : CharacterController
{
    Stat _stat;

	protected NavMeshAgent nma;
	[SerializeField]
    protected float _scanRange = 10;

	[SerializeField]
	protected float _scanRangeLimit = 20;

    [SerializeField]
	protected float _attackRange = 2;

	[SerializeField]
	protected GameObject _lockIndicator;

	[SerializeField]
	protected GameObject _healthBarCanvas; //헬스바는 private

	[SerializeField]
	protected int _getEXp;
	[SerializeField]
	protected int _getGold;

	protected Image _healthBar;

	protected GameObject player;

	protected PlayerStat playerStat;

	protected float distance;

	private Vector3 _originalPos;

	private int _pushforce = 0;

	public bool _lock = false;

	protected override void Init()
    {
		base.Init();

		CharacterType = Define.Character.Monster;
		player = Managers.Game.GetPlayer();
		if (player == null)
			return;

		nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        _stat = gameObject.GetComponent<Stat>();
		playerStat = player.GetComponent<PlayerStat>();

		if (_healthBarCanvas == null)
			_healthBarCanvas = Util.AllFindChild(gameObject, "EnemyHealthBar");

		_originalPos = transform.position;
		HpBarInit();
	}

	protected override void UpdateIdle()
	{
		_lock = false;
		if (_lockTarget != null)
			State = Define.State.Moving;

	}

	protected override void UpdateMoving()
    {
		if (_lockTarget != null)
		{
			_destPos = _lockTarget.transform.position;
			if (distance <= _attackRange)
			{
				NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
				nma.SetDestination(transform.position);
				State = Define.State.Attack;
				return;
			}
		}

		// 이동
		Vector3 dir = _destPos - transform.position;
		Vector3 origindir = _originalPos - transform.position;
		if (dir.magnitude < 0.1f)
		{
			State = Define.State.Idle;
		}
		else
		{
			//scan범위에 들면 따라감 
			if (distance <= _scanRangeLimit)
			{
				nma.SetDestination(_destPos);
				nma.speed = _stat.MoveSpeed;

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 20 * Time.deltaTime);
			}
			else // 제자리로 되돌아감
			{
				if (origindir.magnitude < 1.0f)
				{
					State = Define.State.Idle;
				}
				nma.SetDestination(_originalPos);
				nma.speed = _stat.MoveSpeed;

				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(origindir), 30 * Time.deltaTime);
				_lockTarget = null;
				_lock = true;
			}
		}
	}

	protected override void UpdateAttack()
	{
		////공격 도중 플레이어가 멀어지면 다시 쫒아가기
		if (_lockTarget != null)
        {
            if (distance > _attackRange)
            {
                State = Define.State.Moving;
            }
        }
    }
    protected override void UpdateLockOn()
    {
		if (!_lock)
		{
			distance = (player.transform.position - transform.position).magnitude;
			if (_lockTarget == null && distance <= _scanRange)
			{
				_lockTarget = player;
				return;
			}
		}
		if (_lockTarget == null)
		{
			HpBarInit();
		}
		else
		{
			_lockIndicator.SetActive(true);
			_healthBarCanvas.gameObject.SetActive(true);
		}
	}

    protected override void UpdateStun()
    {
		if (_pushforce != 0)
			transform.Translate(Vector3.back * Time.deltaTime * _pushforce);

		nma.speed = 0;
	}


	//--------Animation Event

	public void ReturnState()
    {
		State = Define.State.Moving;
    }

	public void SpellFireBall()
    {
		Vector3 dir = _lockTarget.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation(dir);
		GameObject fireball = Managers.Resource.Instantiate("Object/Pistol/FireBallSpell");
		SpellShotStraight spellShotStraight = fireball.GetOrAddComponent<SpellShotStraight>();
		spellShotStraight.character = gameObject;
		spellShotStraight._stat = _stat;
	}

	//--------ETC
	void HpBarInit()
    {
		if(_healthBar == null)
			_healthBar = _healthBarCanvas.transform.GetChild(1).GetComponent<Image>();
		_healthBar.rectTransform.localScale = new Vector3(1f,1f,1f);
		_stat.Hp = _stat.MaxHp;
		_healthBarCanvas.gameObject.SetActive(false);
		_lockIndicator.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		//if (other.name == "CollisionRange")
		//{
		//	Debug.Log("부딪");
		//}
	}

	void Stunning()
    {
		State = Define.State.Stun;
    }

	public void GetHitAttack(object[] attackerData)
	{
		StartCoroutine(DelayGetHit(attackerData));
	}

	protected virtual IEnumerator DestroyByDead()
	{
		playerStat.Exp += _getEXp;
		playerStat.Gold += _getGold;
		_healthBar.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		SimpleQuest.SendData(gameObject.name);
		yield return new WaitForSeconds(2.5f);
		Managers.Game.Despawn(gameObject);
	}

	IEnumerator DelayLock(bool check)
    {
		yield return new WaitForSeconds(1f);
		_lock = check;
	}


	IEnumerator DelayGetHit(object[] attackerData)
    {
		int _hitnum = (int)attackerData[3];
		float _stuntimes;
		_pushforce = (int)attackerData[5];
		for (int i = 0; i < _hitnum; i++)
		{
			Managers.Sound.Play("GetHit");
			_stuntimes = (float)attackerData[4];

			if (_stuntimes != 0f)
                Stunning();

			_stat.Hp -= damageCalculator.Calculate(attackerData, _stat);
			Debug.Log(damageCalculator.Calculate(attackerData, _stat));
			if (_stat.Hp <= 0)
			{
				if(GetComponent<CapsuleCollider>().enabled == true)
					GetComponent<CapsuleCollider>().enabled = false;
				nma.speed = 0;
				_stat.Hp = 0;
				State = Define.State.Die;
				StartCoroutine("DestroyByDead");
				break;
			}
			_healthBar.rectTransform.localScale = new Vector3((float)_stat.Hp / (float)_stat.MaxHp, 1f, 1f);

			yield return new WaitForSeconds(_stuntimes);
		}
	}
}