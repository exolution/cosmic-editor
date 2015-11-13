using UnityEngine;
using System.Collections.Generic;
using System;
public delegate void AtAnimationFrame();
public class AnimationAction : Action
{
    public String animationName { get; private set; }
    int repeat;
    bool random;
    bool done=true;
    public Dictionary<int, AtAnimationFrame> frameHandler = new Dictionary<int, AtAnimationFrame>();


    public AnimationAction()
    {

    }
    public void init(string animationName)
    {
        done = false;
        this.animationName = animationName;
        frameHandler.Clear();
    }
    public void abort()
    {
        throw new NotImplementedException();
    }

    public bool isDone()
    {
        throw new NotImplementedException();
    }

    public void pause()
    {
        throw new NotImplementedException();
    }

    public void resumue()
    {
        throw new NotImplementedException();
    }
}
public class AnimationManager  {
    static Dictionary<Animator, AnimationAction> animationActionTable = new Dictionary<Animator, AnimationAction>();
    Animator animator;
    AnimationAction animationAction=new AnimationAction();
    public AnimationManager(Animator animator)
    {
        this.animator = animator;
        animationActionTable.Add(animator, animationAction);
    }
    public Action playAnimationAtFrame(string animationName,int whichFrame, AtAnimationFrame atAnimationFrame)
    {
        animator.CrossFade(animationName, 0.0f);
        animationAction.init(animationName);
        animationAction.frameHandler.Add(whichFrame, atAnimationFrame);
        return animationAction;
    }
    public Action playAnimation(string animationName)
    {
        animator.CrossFade(animationName, 0.02f);
        animationAction.init(animationName);
        return animationAction;
    }
    public static AnimationAction getAnimationAction(Animator animator)
    {

        return animationActionTable[animator];
    }
}
