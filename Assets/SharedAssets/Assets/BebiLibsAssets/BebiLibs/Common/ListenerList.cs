using System.Collections;
using System.Collections.Generic;

public class ListenerList<T> : IList<T> where T : class
{
    private List<T> _listenersList = new List<T>();

    public int Count => _listenersList.Count;
    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => _listenersList[index];
        set => _listenersList[index] = value;
    }

    public void Add(T listener)
    {
        if (!_listenersList.Contains(listener))
        {
            _listenersList.Add(listener);
        }
    }

    public void Remove(T listener) => _listenersList.Remove(listener);
    public void Clear() => _listenersList.Clear();
    public int IndexOf(T item) => _listenersList.IndexOf(item);
    public void Insert(int index, T item) => _listenersList.Insert(index, item);
    public void RemoveAt(int index) => _listenersList.RemoveAt(index);
    public bool Contains(T item) => _listenersList.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _listenersList.CopyTo(array, arrayIndex);
    bool ICollection<T>.Remove(T item) => _listenersList.Remove(item);
    public IEnumerator<T> GetEnumerator() => _listenersList.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _listenersList.GetEnumerator();
}