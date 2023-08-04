using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Remotely.Shared.Primitives;

/// <summary>
/// A simple, lock-based implementation of a thread-safe List<T>.
/// Note that a copy is returned when enumerating the list.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentList<T> : IList<T>
{

    private readonly List<T> _list = new();
    private readonly object _lock = new();

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _list.Count;
            }
        }
    }

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            lock (_lock)
            {
                return _list[index];
            }
        }
        set
        {
            lock (_lock)
            {
                _list[index] = value;
            }
        }
    }
    public void Add(T item)
    {
        lock (_lock)
        {
            _list.Add(item);
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        lock (_lock)
        {
            _list.AddRange(collection);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _list.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _list.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_lock)
        {
            _list.CopyTo(array, arrayIndex);
        }
    }

    public bool Exists(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.Exists(predicate);
        }
    }

    public List<T> FindAll(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.FindAll(predicate);
        }
    }

    public T? Find(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.Find(predicate);
        }
    }

    public int FindIndex(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.FindIndex(predicate);
        }
    }
    public T? FindLast(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.FindLast(predicate);
        }
    }

    public int FindLastIndex(Predicate<T> predicate)
    {
        lock (_lock)
        {
            return _list.FindLastIndex(predicate);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            return _list.ToList().GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        lock (_lock)
        {
            return _list.ToList().GetEnumerator();
        }
    }

    public int IndexOf(T item)
    {
        lock (_lock)
        {
            return _list.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_lock)
        {
            _list.Insert(index, item);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _list.Remove(item);
        }
    }

    public void RemoveAll(Predicate<T> predicate)
    {
        lock (_lock)
        {
            _list.RemoveAll(predicate);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_lock)
        {
            _list.RemoveAt(index);
        }
    }
    public void RemoveRange(int index, int count)
    {
        lock (_lock)
        {
            _list.RemoveRange(index, count);
        }
    }
}