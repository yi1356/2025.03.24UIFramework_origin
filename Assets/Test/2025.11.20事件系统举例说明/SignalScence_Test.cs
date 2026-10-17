using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;

//Signal事件系统使用案例：


//第一步：构建信号类
//构建一个无参的的信号类
public class No_ParameterToTestSignal : ASignal { }

//构建有一个参数的信号类
public class One_ParameterToTestSignal : ASignal<string> { }

//构建有两个参数的信号类
public class Two_ParameterToTestSignal: ASignal<string,string> { }



public class SignalScence_Test : MonoBehaviour
{
    private void Awake()
    {
        //第二步：为目标信号类添加监听
        Signals.Get<No_ParameterToTestSignal>().AddListener(No_ParameterToTestFunc);
        Signals.Get<One_ParameterToTestSignal>().AddListener(One_ParameterToTestFunc01);
        Signals.Get<One_ParameterToTestSignal>().AddListener(One_ParameterToTestFunc02);
        Signals.Get<Two_ParameterToTestSignal>().AddListener(Two_ParameterToTestFunc);
    }
    // Start is called before the first frame update
    void Start()
    {

        //第三步：目标信号类广播事件（执行事件）
        Signals.Get<No_ParameterToTestSignal>().Dispatch();

        Signals.Get<One_ParameterToTestSignal>().Dispatch("A");

        Signals.Get<Two_ParameterToTestSignal>().Dispatch("A","B");
    }

   private void No_ParameterToTestFunc()
    {
        Debug.Log("执行了没有参数的方法");
    }

    private void One_ParameterToTestFunc01(string A)
    {
        Debug.Log("执行了有一个参数的方法,"+"第一个参数是"+A);
    }
    private void One_ParameterToTestFunc02(string A)
    {
        Debug.Log("第二个方法执行了有一个参数的方法," + "第一个参数是" + A);
    }

    private void Two_ParameterToTestFunc(string A, string B)
    {
        Debug.Log("执行了有两个参数的方法," + "第一个参数是" + A +"----"+"第二个参数是"+B);
    }


    private void OnDestroy()
    {
        //第四步：销毁时需要移除监听
        Signals.Get<No_ParameterToTestSignal>().RemoveListener(No_ParameterToTestFunc);
        Signals.Get<One_ParameterToTestSignal>().RemoveListener(One_ParameterToTestFunc01);
        Signals.Get<One_ParameterToTestSignal>().RemoveListener(One_ParameterToTestFunc02);
        Signals.Get<Two_ParameterToTestSignal>().RemoveListener(Two_ParameterToTestFunc);
    }
}
