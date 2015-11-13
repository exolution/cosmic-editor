using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 一系列异步过程的抽象 
/// 一个Action代表一个有序的动作集合
/// </summary>
public interface Action
{
    bool isDone();
    void pause();
    void resumue();
    void abort();
}

public class ActionManager : MonoBehaviour {

    /// <summary>
    /// 核心类 action执行器
    /// 为了递归,使得action执行器也可以被yield, 所以本身也是一个action
    /// Action是包含了一系列enumerator的栈式层级结构 enumerator其实就是一系列步骤的描述
    /// 原理很简单 
    /// 每帧不断执行这一个系列的enumerator，每次执行到一个yield return 监测yield出的值。
    /// yield值是同步值 则立即执行后续代码到下一个yield 
    /// yield值是异步值（一个Action） 则一直检测Action.isDone();true则执行后续代码到下一个yield
    /// yield值是一个enumerator 当前enumerator入栈 上下文切换到enumerator执行上述操作 
    /// 当enumerator执行完毕 检查栈中是否还有enumerator 如果有则出栈继续执行 如果没有 整个操作结束 设置本身的isDone 为true 
    /// </summary>
    class ActionExecutor :Action
    {
        IEnumerator enumerator;
        ActionExecutor parent;
		bool done=false;
        bool aborted = false;
        public ActionExecutor(IEnumerator enumerator,ActionExecutor parent=null)
        {
            this.enumerator = enumerator;
            this.parent = parent;
        }
        public bool run()
        {
            
            bool flag;
            if (aborted) return false;
            object current = this.enumerator.Current;
            if(current is Action)
            {
                if((current as Action).isDone())
                {                   
                    flag = this.enumerator.MoveNext();                    
                }
                else
                {
                    return true;
                }
            }
            else if(current is IEnumerator)
            {
                this.parent = new ActionExecutor(this.enumerator, this.parent);
                this.enumerator = current as IEnumerator;
                return this.run();
                
            }
           else
            {
               
                flag = this.enumerator.MoveNext();
            }
            if (!flag&&this.parent!=null)
            {
                this.enumerator = this.parent.enumerator;
                this.parent = this.parent.parent;              
                return this.enumerator.MoveNext();
            }
            return flag;           
        }
		public bool isDone(){
			return done;
		}

        public void pause(){

		}
        public void resumue(){

		}
        public void finish(){
			done = true;
		}

        public void abort()
        {
            
        }
    }


    public static ActionManager Instance
    {
        get;
        private set;
    }
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("ActionManager must be added to just only one gameobject");
        }
    }
    List<ActionExecutor> executorList = new List<ActionExecutor>(); 
	// Use this for initialization
	void Start () {
	
	}
	void Update()
    {
      
    }

	void LateUpdate () {

        for(int i = 0; i < executorList.Count; i++)
        {
            if (!executorList[i].run()) {
				executorList[i].finish();
                executorList.RemoveAt(i);
                i--;
            };
        }	
	}
    public static Action excuteAction(IEnumerator enumerator)
    {
        ActionExecutor actionExecutor = new ActionExecutor(enumerator);
		Instance.executorList.Add (actionExecutor);
		return actionExecutor;
    }

}
