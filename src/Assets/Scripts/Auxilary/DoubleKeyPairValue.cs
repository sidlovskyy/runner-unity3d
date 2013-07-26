//from: https://github.com/noocyte/Util.DoubleKeyDictionary
public class DoubleKeyPairValue<K, T, V>
{
	public DoubleKeyPairValue(K key1, T key2, V value)
	{
		Key1 = key1;
		Key2 = key2;
		Value = value;
	}

	public K Key1
	{
		get;
		set;
	}

	public T Key2
	{
		get;
		set;
	}

	public V Value
	{
		get;
		set;
	}

	public override string ToString()
	{
		return Key1 + " - " + Key2 + " - " + Value;
	}
}