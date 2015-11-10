using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public enum SkillSelectMode
{
    None, Unit, Ground, All
}
public class SkillEvent
{

}
public class SkillTarget
{
    Unit selectTargetUnit;
    Vector3 selectTargetPoint;
    List<Unit> targetList=new List<Unit>();
    public SkillTarget(Unit selectTargetUnit, Vector3 selectTargetPoint)
    {
        this.selectTargetUnit = selectTargetUnit;
        this.selectTargetPoint = selectTargetPoint;
    }
    public SkillTarget(Unit selectTargetUnit)
    {
        this.selectTargetUnit = selectTargetUnit;
        
    }
    public SkillTarget(Vector3 selectTargetPoint)
    {
       
        this.selectTargetPoint = selectTargetPoint;
    }
    virtual public void catchTarget(Skill skill) {
        targetList.Add(selectTargetUnit);
    }
    public List<Unit> getAllTarget() {
        if (targetList.Count == 0)
        {
            catchTarget(null);
        }
        return targetList;
    }
    public Unit getTarget()
    {
        if (targetList.Count == 0)
        {
            catchTarget(null);
        }
        return targetList[0];
    }
    

    
}
public delegate void SkillEventHandler(SkillEvent skillEvent);
public class Skill
{

    protected Unit source;
    protected SkillSelectMode skillSelectMode = SkillSelectMode.Unit;
    protected SkillTarget skillTarget;
    protected int repeat = 0;
    protected bool allowCast;
    public event SkillEventHandler eventPreCast;
    public event SkillEventHandler eventSelect;
    public event SkillEventHandler eventSpellStart;
    public event SkillEventHandler eventSpelling;
    public event SkillEventHandler eventSpellDone;
    public event SkillEventHandler eventSpellAbort;
    //event SkillEventHandler eventCastCheck;
    public event SkillEventHandler eventBeforeCast;
    public event SkillEventHandler eventCast;
    public event SkillEventHandler eventHit;
    public event SkillEventHandler eventAfterCast;
    
    protected virtual IEnumerator preCast()
    {
        return null;
    }
    protected virtual IEnumerator spell()
    {
        return null;
    }
    protected virtual bool castCheck()
    {
        return true;
    }
    protected virtual IEnumerator cast()
    {
        return null;
    }
    protected virtual IEnumerator hit()
    {

        return null;
    }
    protected virtual float doDamage(Unit target)
    {
        return 0;
    }

    IEnumerator skillAction()
    {
        yield return preCast();
        if (eventPreCast != null) eventPreCast(new SkillEvent());
        if (skillSelectMode != SkillSelectMode.None && skillTarget == null)
        {
            yield return PlayerInteractor.SkillSelect(skillSelectMode);
            if (eventSelect != null) eventSelect(new SkillEvent());
        }
        if (eventSpellStart != null) eventSpellStart(new SkillEvent());
        yield return spell();
        if (eventSpellDone != null) eventSpellDone(new SkillEvent());
        allowCast = castCheck();
        if (allowCast)
        {
            if (eventBeforeCast != null) eventBeforeCast(new SkillEvent());
            yield return cast();
            if (eventAfterCast != null) eventAfterCast(new SkillEvent());
            yield return hit();

        }

    }
    IEnumerator skillActionRepeat()
    {
        yield return preCast();
        if (eventPreCast != null) eventPreCast(new SkillEvent());
        if (skillSelectMode != SkillSelectMode.None && skillTarget == null)
        {
            yield return PlayerInteractor.SkillSelect(skillSelectMode);
            if (eventSelect != null) eventSelect(new SkillEvent());
        }
        while (repeat-- > 0)
        {
            if (eventSpellStart != null) eventSpellStart(new SkillEvent());
            yield return spell();
            if (eventSpellDone != null) eventSpellDone(new SkillEvent());
            allowCast = castCheck();
            if (allowCast)
            {
                if (eventBeforeCast != null) eventBeforeCast(new SkillEvent());
                yield return cast();
                if (eventAfterCast != null) eventAfterCast(new SkillEvent());
                yield return hit();

            }
        }
    }
    public IEnumerator Cast()
    {
       
        if (repeat > 0)
        {
            yield return ActionManager.excuteAction(skillActionRepeat());
        }
        else
        {
            yield return ActionManager.excuteAction(skillAction());
        }
    }
    public IEnumerator Cast(SkillTarget skillTarget = null, int repeat = 0)
    {
        this.skillTarget = skillTarget;
        this.repeat = repeat;
        return Cast();
    }
    

}



public class Skill_Attack:Skill
{
    protected override IEnumerator cast()
    {
        skillTarget.catchTarget(this);

        yield return source.followUnit(skillTarget.getTarget());
        yield return source.animationManager.playAnimationAtFrame("attack", 10);
    }
}