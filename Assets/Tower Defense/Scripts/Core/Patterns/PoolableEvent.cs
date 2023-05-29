using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Patterns
{
    [Serializable]
    public class PoolableEvent<T> : UnityEvent<T> {}
}
