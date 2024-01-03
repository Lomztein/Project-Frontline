using System.Linq;
using UnityEngine;

public abstract class MovingWindow<T>
{
	public int Size { get; private set; }

	private int _index;
	protected T[] _array;

	protected MovingWindow(int size)
	{
		Size = size;
		_array = new T[size];
	}

	public void Register(T item)
	{
		_array[_index++ % _array.Length] = item;
	}

	public abstract T Average();
}

public class FloatMovingWindow : MovingWindow<float>
{
    public FloatMovingWindow(int size) : base(size)
    {
    }

    public override float Average()
		=> _array.Average();
}
