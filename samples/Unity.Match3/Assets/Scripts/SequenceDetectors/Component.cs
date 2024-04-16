using System;
using System.Collections.Generic;

namespace Match3
{
    public abstract class Component
    {
        public Dictionary<Type, object> SubComponents { get; set; }

        public T AddComponent<T>(T instance) where T : Component
        {
            SubComponents ??= new Dictionary<Type, object>();
            SubComponents.Add(typeof(T), instance);
            return instance;
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)SubComponents[typeof(T)];
        }
    }
}