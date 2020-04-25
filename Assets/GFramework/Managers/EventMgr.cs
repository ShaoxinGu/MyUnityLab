using System.Collections.Generic;
using UnityEngine.Events;

namespace GFramework
{
    public interface IEventCallBack
    {

    }

    public class EventCallBack : IEventCallBack
    {
        public UnityAction actions;
        public EventCallBack(UnityAction action)
        {
            actions += action;
        }
    }

    public class EventCallBack<T> : IEventCallBack
    {
        public UnityAction<T> actions;
        public EventCallBack(UnityAction<T> action)
        {
            actions += action;
        }
    }

    public class EventMgr : Singleton<EventMgr>
    {
        private Dictionary<string, IEventCallBack> eventDic = new Dictionary<string, IEventCallBack>();

        public void AddEventListener(string eventName, UnityAction action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventCallBack).actions += action;
            }
            else
            {
                eventDic.Add(eventName, new EventCallBack(action));
            }
        }

        public void AddEventListener<T>(string eventName, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventCallBack<T>).actions += action;
            }
            else
            {
                eventDic.Add(eventName, new EventCallBack<T>(action));
            }
        }

        public void RemoveEventListener(string eventName, UnityAction action)
        {
            if (eventDic.ContainsKey(eventName))
                (eventDic[eventName] as EventCallBack).actions -= action;
        }

        public void RemoveEventListener<T>(string eventName, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(eventName))
                (eventDic[eventName] as EventCallBack<T>).actions -= action;
        }

        public void DispatchEvent(string eventName)
        {
            if (eventDic.ContainsKey(eventName))
            {
                if ((eventDic[eventName] as EventCallBack).actions != null)
                    (eventDic[eventName] as EventCallBack).actions.Invoke();
            }
        }

        public void DispatchEvent<T>(string eventName, T param)
        {
            if (eventDic.ContainsKey(eventName))
            {
                if ((eventDic[eventName] as EventCallBack<T>).actions != null)
                    (eventDic[eventName] as EventCallBack<T>).actions.Invoke(param);
            }
        }

        public void Clear()
        {
            eventDic.Clear();
        }
    }
}
