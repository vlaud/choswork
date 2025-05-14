using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnDamage(float dmg);
    bool IsLive();
    void AddAttacker(IBattle ib);
    void RemoveAttacker(IBattle ib);
    void DeadMessage(Transform tr);
}

public class BattleSystem : CharacterMovement, IBattle
{
    protected List<IBattle> myAttackers = new List<IBattle>();
    Transform _target = null;
    protected Transform myTarget
    {
        get => _target;
        set
        {
            _target = value;
            if (_target != null) _target.GetComponent<IBattle>()?.AddAttacker(this);
        }
    }

    public void AttackTarget()
    {
        if (Vector3.Distance(myTarget.position, transform.position) <= myStat.AttackRange + 0.5f)
        {
            myTarget.GetComponent<IBattle>()?.OnDamage(myStat.AP);
        }
    }
    public virtual void OnDamage(float dmg)
    {

    }
    public virtual bool IsLive()
    {
        return true;
    }
    public void AddAttacker(IBattle ib)
    {
        myAttackers.Add(ib);
    }
    public virtual void DeadMessage(Transform tr)
    {

    }
    public void RemoveAttacker(IBattle ib)
    {
        for (int i = 0; i < myAttackers.Count;)
        {
            if (ib == myAttackers[i])
            {
                myAttackers.RemoveAt(i);
                break;
            }
            ++i;
        }
    }
}
