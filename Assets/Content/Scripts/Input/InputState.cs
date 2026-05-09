using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour, IInputState
{
    private readonly Dictionary<LockType, HashSet<string>> _locks = new();
    public enum LockType { Move, Camera, Cursor, Menu }

    public bool GetLockState(LockType type)
    {
         return _locks.TryGetValue(type, out var set) && set.Count > 0;
    }

    public void AddLock(LockType type, string key)
    {
        if (!_locks.TryGetValue(type, out var set))
        {
            set = new HashSet<string>();
            _locks[type] = set;
        }

        set.Add(key);
    }

    public void RemoveLock(LockType type, string key)
    {
        if (_locks.TryGetValue(type, out var set))
            set.Remove(key);
    }
}
