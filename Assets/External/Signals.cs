// ========================================================================================
// Signals - A typesafe, lightweight messaging lib for Unity.
// ========================================================================================
// 2017-2019, Yanko Oliveira  / http://yankooliveira.com / http://twitter.com/yankooliveira
// Special thanks to Max Knoblich for code review and Aswhin Sudhir for the anonymous 
// function asserts suggestion.
// ========================================================================================
// Inspired by StrangeIOC, minus the clutter.
// Based on http://wiki.unity3d.com/index.php/CSharpMessenger_Extended
// Converted to use strongly typed parameters and prevent use of strings as ids.
//
// Supports up to 3 parameters. More than that, and you should probably use a VO.
//
// Usage:
//    1) Define your class, eg:
//          ScoreSignal : ASignal<int> {}
//    2) Add listeners on portions that should react, eg on Awake():
//          Signals.Get<ScoreSignal>().AddListener(OnScore);
//    3) Dispatch, eg:
//          Signals.Get<ScoreSignal>().Dispatch(userScore);
//    4) Don't forget to remove the listeners upon destruction! Eg on OnDestroy():
//          Signals.Get<ScoreSignal>().RemoveListener(OnScore);
//    5) If you don't want to use global Signals, you can have your very own SignalHub
//       instance in your class
//
// ========================================================================================

using System;
using System.Collections.Generic;


//Z：作用：是一个事件系统，能实现发送方和接收方解耦  [因为： 发送方和接收方只依赖 “信号”，而不依赖彼此，即它们直接不直接引用]

//Z：思路：
//①提供静态 Signals 类为全局入口，简化订阅、广播、取消订阅流程
//②使用字典存储 信号类型 - 实例 映射，确保每种信号单例
//③Signals的泛型 Get 方法获取信号实例，通过委托实现通信，还能避免字符串标识错误（如拼写错事件 ID)
//④需要自定义泛型信号类，约束参数类型，避免参数不匹配
//注意：这里字符串标识错误指的是类似Messenger.AddListener("score_changed", UpdateScore);

//Z：使用步骤：定义信号类 ->添加监 ->广播 ->销毁时移除监听器

//Z：大部分时候用在点击按钮后的反应上
namespace Utils
{
    /// <summary>
    /// 定义所有 “信号” 的统一接口
    /// </summary>
    public interface ISignal
    {
        string Hash { get; }//Z：信号的唯一标识（后续通过类型字符串生成），用于通过哈希查找信号
    }
    /// <summary>
    /// 静态对象，用于外部调用（Z：作用类似单例）
    /// </summary>
    public static class Signals
    {
        //创建了一个 SignalHub 实例，作为全局的事件中心
        private static readonly SignalHub hub = new SignalHub();

        //从 SignalHub 中获取指定类型的信号实例
        public static SType Get<SType>() where SType : ISignal, new() {
            return hub.Get<SType>();
        }
    }


    /// <summary>
    /// 事件中心（用字典存储 所有信号：Type+ISignal），负责信号的注册、存储、查找
    /// </summary>
    public class SignalHub
    {
        //使用 Dictionary 存储信号类型和对应的信号实例
        private Dictionary<Type, ISignal> signals = new Dictionary<Type, ISignal>();

        /// <summary>
        /// 根据类型获取事件(如果已经注册了直接获取，如果没有注册则会 自动创建新的实例注册进去，一样使用)
        /// </summary>
        public SType Get<SType>() where SType : ISignal, new() {
            Type signalType = typeof(SType);
            ISignal signal;

            if (signals.TryGetValue(signalType, out signal))      //Z：从字典中查找该类型对应的信号实例
            {
                return (SType) signal;
            }

            return (SType) Bind(signalType);//Z：没找到则调用 Bind(signalType) 创建并注册新实例，再返回
        }

        /// <summary>
        /// 手动提供一个事件的哈希，并将其绑定到给定的监听器（如果这个Hash原本就没有被绑定，则无效）
        /// /// </summary> 
        public void AddListenerToHash(string signalHash, Action handler) {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal) {
                (signal as ASignal).AddListener(handler);
            }
        }
        
        public void RemoveListenerFromHash(string signalHash, Action handler) {
            ISignal signal = GetSignalByHash(signalHash);
            if (signal != null && signal is ASignal) {
                (signal as ASignal).RemoveListener(handler);
            }
        }

        //根据信号类型  没有注册测创建并注册信号实例（利用反射创建实例+放入字典中），若已注册则输出错误信息
        private ISignal Bind(Type signalType) {
            ISignal signal;
            if (signals.TryGetValue(signalType, out signal)) {
                UnityEngine.Debug.LogError(string.Format("Signal already registered for type {0}",
                    signalType.ToString()));
                return signal;
            }

            signal = (ISignal) Activator.CreateInstance(signalType);//用于动态创建对象实例的方法 [Z：反射中，提供快速创建对象实例的方法]
            signals.Add(signalType, signal);//将新创建的信号实例添加到signals字典中
            return signal;
        }

        private ISignal Bind<T>() where T : ISignal, new() {
            return Bind(typeof(T));
        }

        private ISignal GetSignalByHash(string signalHash) {
            foreach (ISignal signal in signals.Values) {
                if (signal.Hash == signalHash) {
                    return signal;
                }
            }

            return null;
        }
    }


    /// <summary>
    /// 信号事件的抽象类
    /// </summary>
    public abstract class ABaseSignal : ISignal
    {
        protected string _hash;

        /// <summary>
        /// 本身的哈希值
        /// </summary>
        public string Hash {
            get {
                if (string.IsNullOrEmpty(_hash)) {
                    _hash = this.GetType().ToString();
                }

                return _hash;
            }
        }
    }

    /// <summary>
    /// 无参的事件（事件的具体实现）[其实就是能向其内部委托中添加事件]
    /// </summary>
    public abstract class ASignal : ABaseSignal
    {
        private Action callback;

        /// <summary>
        /// 事件加监听
        /// </summary>
        public void AddListener(Action handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// 事件的移除监听
        /// </summary>
        public void RemoveListener(Action handler) {
            callback -= handler;
        }

        /// <summary>
        /// 广播事件
        /// </summary>
        public void Dispatch() {
            if (callback != null) {
                callback();
            }
        }
    }

    /// <summary>
    /// 带一个参数的事件
    /// </summary>
    public abstract class ASignal<T> : ABaseSignal
    {
        private Action<T> callback;

        /// <summary>
        /// 事件加监听
        /// </summary>
        public void AddListener(Action<T> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveListener(Action<T> handler) {
            callback -= handler;
        }

        /// <summary>
        /// 广播事件，带一个参数
        /// </summary>
        public void Dispatch(T arg1) {
            if (callback != null) {
                callback(arg1);
            }
        }
    }

    /// <summary>
    /// 同理，2个参数的事件
    /// </summary>
    public abstract class ASignal<T, U> : ABaseSignal
    {
        private Action<T, U> callback;
        
        public void AddListener(Action<T, U> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }
        
        public void RemoveListener(Action<T, U> handler) {
            callback -= handler;
        }
        
        public void Dispatch(T arg1, U arg2) {
            if (callback != null) {
                callback(arg1, arg2);
            }
        }
    }

    /// <summary>
    /// 同理三个参数的事件
    /// </summary>
    public abstract class ASignal<T, U, V> : ABaseSignal
    {
        private Action<T, U, V> callback;
        
        public void AddListener(Action<T, U, V> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(
                handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),
                    inherit: false).Length == 0,
                "Adding anonymous delegates as Signal callbacks is not supported (you wouldn't be able to unregister them later).");
#endif
            callback += handler;
        }
        
        public void RemoveListener(Action<T, U, V> handler) {
            callback -= handler;
        }
        
        public void Dispatch(T arg1, U arg2, V arg3) {
            if (callback != null) {
                callback(arg1, arg2, arg3);
            }
        }
    }
}