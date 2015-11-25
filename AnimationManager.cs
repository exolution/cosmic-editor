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
abstract class AnimationStateParameter
{
    protected AnimatorControllerParameter parameter;
    public virtual void resolve(Animator animator,AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("Base."+parameter.name))
        {
            Debug.Log("isName:" + parameter.name);
            setValue(animator);
        }
    }   
    protected abstract void setValue(Animator animator);
}
class BoolParameterResolver : AnimationStateParameter
{
    protected override void setValue(Animator animator)
    {
        animator.SetBool(parameter.name, true);
    }
    public BoolParameterResolver(AnimatorControllerParameter parameter)
    {
        this.parameter = parameter;
    }
}
class IntegerParameterResolver : AnimationStateParameter
{
    int max;
    int prev=1;
    public override void resolve(Animator animator, AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("Base." + parameter.name+prev))
        {
            Debug.Log("isName:"+ parameter.name);
            setValue(animator);
        }
    }
    protected override void setValue(Animator animator)
    {

        int r = UnityEngine.Random.Range(1, max+1);
        prev = r;
        Debug.Log("random=" + prev + ",max=" + max);
        animator.SetInteger(parameter.name, prev);
    }
    public IntegerParameterResolver(AnimatorControllerParameter parameter,Animator animator)
    {
        this.parameter = parameter;
        max = parameter.defaultInt;
        animator.SetInteger(parameter.name, 0);
    }
}

abstract class AnimatorStateProxy
{
    public AnimatorControllerParameter stateParameter {
        get;
        protected set;
    }
    public string name
    {
        get;
        protected set;
    }
    protected const string BASE_LAYER_NAME = "Base";
    public static Dictionary<int, AnimatorStateProxy> animationStates = new Dictionary<int, AnimatorStateProxy>();
    public AnimatorStateProxy(AnimatorControllerParameter stateParameter)
    {
        this.stateParameter = stateParameter;
    }
    public abstract void onEnterState(AnimationManager animationManager, AnimatorStateInfo stateInfo);
}
class AnimatorSingleStateProxy : AnimatorStateProxy
{
    public override void onEnterState(AnimationManager animationManager, AnimatorStateInfo stateInfo)
    {
        throw new NotImplementedException();
    }
    public  AnimatorSingleStateProxy(AnimatorControllerParameter parameter) 
        : base(parameter)
    {
        for (int i = 1; i <= parameter.defaultInt; i++) {
            AnimatorStateProxy.animationStates.Add(Animator.StringToHash(BASE_LAYER_NAME + "." +parameter.name+i),this);
        }
    }
}
class AnimatorSequnceStateProxy : AnimatorStateProxy
{
    public override void onEnterState(AnimationManager animationManager, AnimatorStateInfo stateInfo)
    {
        throw new NotImplementedException();
    }
    public AnimatorSequnceStateProxy(AnimatorControllerParameter parameter) 
        : base(parameter)
    {
        for (int i = 1; i <= parameter.defaultInt; i++)
        {
            AnimatorStateProxy.animationStates.Add(Animator.StringToHash(BASE_LAYER_NAME + "." + parameter.name + i), this);
        }
        name = parameter.name.Split('.')[0];
    }
}


public class AnimationManager  {
   
    //static Dictionary<Animator, AnimationManager> animationManagerTable = new Dictionary<Animator, AnimationManager>();
    Animator animator;
    AnimationAction animationAction=new AnimationAction();
    public AnimationManager(Animator animator)
    {
        this.animator = animator;

        //set animationManager instance to every animationBehaviour;
        AnimationBehaviour[] animationBehaviour = animator.GetBehaviours<AnimationBehaviour>();
        for (int i = 0; i < animationBehaviour.Length; i++)
        {
            animationBehaviour[i].animationManager = this;
        }

        AnimatorControllerParameter []parameters = animator.parameters;
        for (int i=0;i< parameters.Length&&i<20; i++)
        {
            //animationStates.Add(Animator.StringToHash(BASE_LAYER_NAME+"."+))
            //if (parameters[i].type == AnimatorControllerParameterType.Bool)
            //{
            //    parameterResolver[i] = new BoolParameterResolver(parameters[i]);
            //}
            //else if (parameters[i].type == AnimatorControllerParameterType.Int)
            //{
            //    parameterResolver[i] = new IntegerParameterResolver(parameters[i],animator);
            //}
        }

       // animationManagerTable.Add(animator, this);
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
        animator.SetInteger("Idle.idle", 0);
        animator.CrossFade(animationName, 0.1f);
        
        animationAction.init(animationName);
        return animationAction;
    }
    public void initState(Animator animator,AnimatorStateInfo stateInfo) {
        //for(int i=0;i< parameterResolver.Length; i++)
        //{
        //    if (parameterResolver[i] == null) break;
        //    parameterResolver[i].resolve(animator, stateInfo);
           
        //}
    }
    public static void InitState(Animator animator, AnimatorStateInfo stateInfo)
    {
        AnimationManager.Get(animator).initState(animator, stateInfo);
    }
    public static AnimationManager Get(Animator animator)
    {

        return UnitManager.getUnit(animator.gameObject).animationManager;
    }
}
