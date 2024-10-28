using System;

public static class FloatExtensions
{
	public static float Floor(this float value, int range)
	{
		int rest = (int) Math.Pow(10, range);
		return (float) (Math.Floor(value * rest) / rest);
	}
}