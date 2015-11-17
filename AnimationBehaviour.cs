using UnityEngine;
using System.Collections;

public class AnimationBehaviour : StateMachineBehaviour {
    float prev=0;
    int c = 4;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log(1 + "-" + stateInfo.fullPathHash + "-" + stateInfo.length + "-" + stateInfo.normalizedTime + "-" + (stateInfo.normalizedTime - prev));
        prev = stateInfo.normalizedTime;
        AnimationManager.ResolveState(animator, stateInfo);
        //animator.SetInteger("idle",Random.Range(1, 3));
        //AnimationAction animationAction = AnimationManager.getAnimationAction(animator);
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AnimatorClipInfo aci = animator.GetCurrentAnimatorClipInfo(layerIndex)[0];
       
        //Debug.Log(2 + "-" + stateInfo.fullPathHash+"-"+stateInfo.length+"-"+stateInfo.normalizedTime+"-"+ (stateInfo.normalizedTime-prev));
        prev = stateInfo.normalizedTime;
        if (stateInfo.normalizedTime >= 0.9f)
        {
           
           // Debug.Break();
        }
    }

    //OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


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
