using UnityEngine;
using System.Collections;

public class AnimationBehaviour : StateMachineBehaviour {
    float prev=0;
    int c = 4;
    public AnimationManager animationManager;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log(1 + "-" + stateInfo.fullPathHash + "-" + stateInfo.length + "-" + stateInfo.normalizedTime + "-" + (stateInfo.normalizedTime - prev));
        prev = stateInfo.normalizedTime;
        animationManager.initState(animator, stateInfo);
        AnimationEvent ae = new AnimationEvent();
        ae.functionName = "animationEvent";
        
        AnimatorTransitionInfo ati = animator.GetAnimatorTransitionInfo(layerIndex);
        
        AnimatorClipInfo aci = animator.GetCurrentAnimatorClipInfo(layerIndex)[0];
        aci.clip.AddEvent(ae);
        Debug.Log( "进入："+ aci.clip.name +"-"+ stateInfo.fullPathHash + "-" + stateInfo.length + "-" + stateInfo.normalizedTime + "-" + (stateInfo.normalizedTime - prev));
        //animator.SetInteger("idle",Random.Range(1, 3));
        //AnimationAction animationAction = AnimationManager.getAnimationAction(animator);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorTransitionInfo ati = animator.GetAnimatorTransitionInfo(layerIndex);
        
        AnimatorClipInfo []acis = animator.GetCurrentAnimatorClipInfo(layerIndex);
        string acit = "";
        foreach (AnimatorClipInfo aci in acis){
            acit += "["+aci.weight + ":" + aci.clip.name+"],";
        }
        //Debug.Log(2 + "-[" + ati.normalizedTime + "]-["+animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name+ "]-["+stateInfo.normalizedTime+"]-"+ (stateInfo.normalizedTime-prev));
        prev = stateInfo.normalizedTime;
        Debug.Log("update:" + stateInfo.fullPathHash+"-"+acit+stateInfo.normalizedTime);
        if (stateInfo.normalizedTime>0.8f)
        {
           // Debug.Log(stateInfo.fullPathHash);
            //animator.SetInteger("Attack", 0);
           //Debug.Break();
        }
    }

    //OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo aci = animator.GetCurrentAnimatorClipInfo(layerIndex)[0];
        Debug.Log("离开：" + aci.clip.name + "-" + stateInfo.fullPathHash + "-" + stateInfo.length + "-" + stateInfo.normalizedTime + "-" + (stateInfo.normalizedTime - prev));

        //Debug.Log(3 + "-" + stateInfo.fullPathHash + "-" + stateInfo.length + "-" + stateInfo.normalizedTime + "-" + (stateInfo.normalizedTime - prev));
        prev = stateInfo.normalizedTime;

    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMachineEnter is called when entering a statemachine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Debug.Log("4:" + stateMachinePathHash);
    }

    // OnStateMachineExit is called when exiting a statemachine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Debug.Log("5:" + stateMachinePathHash);
    }
}
