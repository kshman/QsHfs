using System.Text;

namespace QsHfs;

internal class PathString
{
	private readonly byte[] _u8;
	private readonly string _s;
	private readonly uint _hash;

	public PathString()
	{
		_u8 = Array.Empty<byte>();
		_s = string.Empty;
		_hash = 0;
	}

	public PathString(string s)
	{
		_u8 = Encoding.UTF8.GetBytes(s);
		_s = s;
		_hash = CalcHash();
	}

	public PathString(byte[] b)
	{
		_u8 = b;
		_s = Encoding.UTF8.GetString(b);
		_hash = CalcHash();
	}

	public byte[] ToBytes()
	{
		return _u8;
	}

	public byte[] ToBytes(int size)
	{
		var b = new byte[size];
		Array.Copy(_u8, b, Math.Min(size - 1, _u8.Length));
		return b;
	}

	public override string ToString()
	{
		return _s;
	}

	public bool Equals(PathString other)
	{
		if (other is null)
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return _s.Equals(other._s, StringComparison.OrdinalIgnoreCase);
	}

	public bool Equals(string other)
	{
		if (other is null)
			return false;
		return _s.Equals(other, StringComparison.OrdinalIgnoreCase);
	}

	public int Length => _u8.Length;

	public uint HashCode => _hash;

	public (ushort len, uint hash) LenAndHash => ((ushort)_u8.Length, _hash);

	private uint CalcHash()
	{
		uint hash = 0, cnt = 0;
		var len = Math.Min(_u8.Length, 256);
		for (var i = 0; i < len; i++, cnt++)
			hash = (hash << 5) - hash + _u8[i];
		hash = (hash << 5) - hash + cnt - 1;
		return hash;
	}

	public static implicit operator PathString(string s) => new(s);
}
