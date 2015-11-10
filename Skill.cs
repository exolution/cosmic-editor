using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public enum SkillSelectMode
{
    None, Unit, Ground, AOE
}
public class SkillEvent
{

}
public class SkillTarget
{
    internal Unit selectTargetUnit;
    internal Vector3 selectTargetPoint;
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
//todo 冷却时间 
public class Skill
{
    //施法者
    protected Unit source;
    //技能选择模式 点/单位/aoe/无需选择
    protected SkillSelectMode skillSelectMode = SkillSelectMode.Unit;
    //目标捕捉器 每个技能根据技能的情况单独实现目标捕捉器
    public SkillTarget skillTarget;
    //技能重复释放次数
    protected int repeat = 0;
    #region 技能各阶段事件
    public event SkillEventHandler eventPreCast;
    public event SkillEventHandler eventSelect;
    public event SkillEventHandler eventSpellStart;
    public event SkillEventHandler eventSpelling;
    public event SkillEventHandler eventSpellDone;
    public event SkillEventHandler eventSpellAbort;
    public event SkillEventHandler eventBeforeCast;
    public event SkillEventHandler eventCast;
    public event SkillEventHandler eventHit;
    public event SkillEventHandler eventAfterCast;
    #endregion
    //预释放阶段
    protected virtual IEnumerator preCast()
    {
        return null;
    }
    //吟唱阶段
    protected virtual IEnumerator spell()
    {
        return null;
    }
    //释放条件检查器
    protected virtual bool castCheck()
    {
        return true;
    }
    //开始释放
    protected virtual IEnumerator cast()
    {
        return null;
    }
    //命中过程
    protected virtual IEnumerator hit()
    {

        return null;
    }
    //计算伤害函数
    protected virtual float doDamage(Unit target)
    {
        return 0;
    }
    //定义整个技能过程
    IEnumerator skillAction()
    {
        yield return preCast();
        if (eventPreCast != null) eventPreCast(new SkillEvent());
        if (skillSelectMode != SkillSelectMode.None && skillTarget == null)
        {
            yield return PlayerInteractor.SkillSelect(this,skillSelectMode);
            if (eventSelect != null) eventSelect(new SkillEvent());
        }
        if (eventSpellStart != null) eventSpellStart(new SkillEvent());
        yield return spell();
        if (eventSpellDone != null) eventSpellDone(new SkillEvent());
        bool allowCast = castCheck();
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
            yield return PlayerInteractor.SkillSelect(this,skillSelectMode);
            if (eventSelect != null) eventSelect(new SkillEvent());
        }
        while (repeat-- > 0)
        {
            if (eventSpellStart != null) eventSpellStart(new SkillEvent());
            yield return spell();
            if (eventSpellDone != null) eventSpellDone(new SkillEvent());
            bool allowCast = castCheck();
            if (allowCast)
            {
                if (eventBeforeCast != null) eventBeforeCast(new SkillEvent());
                yield return cast();
                if (eventAfterCast != null) eventAfterCast(new SkillEvent());
                yield return hit();

            }
        }
    }
    public virtual void setSkillTarget(Unit selectTargetUnit, Vector3 selectTargetPoint)
    {
        if (skillTarget == null)
        {
            skillTarget = new SkillTarget(selectTargetUnit, selectTargetPoint);
        }
        else
        {
            skillTarget.selectTargetUnit = selectTargetUnit;
            skillTarget.selectTargetPoint = selectTargetPoint;
        }
    }
    //释放方法
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
    public IEnumerator Cast(SkillTarget skillTarget )
    {
        this.skillTarget = skillTarget;
       
        return Cast();
    }
    public IEnumerator Cast( int repeat)
    {
        
        this.repeat = repeat;
        return Cast();
    }


}



public class Skill_Attack:Skill
{
    float skillRange = 100;
    protected override IEnumerator cast()
    {   
       
        
        //通过玩家选择的技能目标 获取实际的目标单位
        skillTarget.catchTarget(this);
        
        //如果离目标过远则跟随目标直到技能范围满足
        yield return source.followUnit(skillTarget.getTarget(),skillRange);
        
        //播放角色的攻击动画 至伤害关键帧（俗称前摇）
        yield return source.animationManager.playAnimationAtFrame("attack", 10);
        
        //调用命中过程 并计算伤害
        yield return hit();
    }
}