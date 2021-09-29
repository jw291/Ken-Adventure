using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonsterController
{
    [System.Serializable]
    public struct Pattern
    {
        [SerializeField]
        public string PatternName;

        [SerializeField]
        public int PatternPercent;
    };

    [SerializeField]
    private List<Pattern> AttackPatterns = new List<Pattern>();

    [SerializeField]
    private float _attackTerm;

    private GameObject RangeIndicator;

    private Animator _anim;

    private Transform AttackRangeRoot;

    private bool _attacklock = false;

    private int index = 0;

    private bool _once = false;

    private string[] clipName = new string[3] {"BossSwoosh", "BossAttack2", "BossAttack2" };
    private float[] clippitch = new float[3] { 0.4f, 0.9f, 0.7f };

    public override void Init()
    {
        base.Init();

        if (_anim == null)
            _anim = GetComponent<Animator>();
    }
    protected override void UpdateAttack()
    {
        if (!_attacklock)
        {
            if (distance > _attackRange)
            {
                State = Define.State.Moving;
            }
            else
            {
                _anim.Play("Idle");
                _attacklock = true;
                int res = RandomIndexWithProbability(AttackPatterns);
                StartCoroutine(TermAttack(res));
            }
        }
    }

    protected override void UpdateLockOn()
    {
        base.UpdateLockOn();
        if (_lockTarget != null && !_once)
        {
            _once = true;
            Managers.Sound.Play("BossVoice");
        }
    }

    protected override void UpdateStun()
    {
        nma.speed = 0;
        _attacklock = false;
    }

    protected override IEnumerator DestroyByDead()
    {
        RangeIndicator.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Managers.Game.Despawn(gameObject);
    }

    private void AttackPattern(int patterns)
    {
        Vector3 dir = _lockTarget.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
        _anim.Play(AttackPatterns[patterns].PatternName);
    }

    private int RandomIndexWithProbability(List<Pattern> p)
    {
        // sum 계산

        int rand = Random.Range(0, 100);
        int current = 0;
  
        for (int i = 0; i < p.Count; i++)
        {
            current += p[i].PatternPercent;
            if (rand < current)
            {
                return i;
            }
        }
        return 0;
    }

    IEnumerator TermAttack(int pattern)
    {
        AttackPattern(pattern);
        yield return new WaitForSeconds(0.5f);
        AnimatorStateInfo animatorState = _anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(animatorState.length + _attackTerm);
        _attacklock = false;
    }

    //------Animation Event
    public void BossSoundEffectPlay(int pattern)
    {
        Managers.Sound.Play(clipName[pattern], Define.Sound.Effect, clippitch[pattern]);
    }
}
