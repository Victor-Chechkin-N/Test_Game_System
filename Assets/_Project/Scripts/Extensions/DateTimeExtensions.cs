using System;

public static class DateTimeExtensions
{
	private static DateTime dateFrom = new DateTime(1970, 1, 1);
	
	public static double GetCurrentTimeInMs()
	{
		return (DateTime.UtcNow - DateTimeExtensions.dateFrom).TotalMilliseconds;
	}
	
	public static double GetCurrentTimeInS()
	{
		return Math.Floor(DateTimeExtensions.GetCurrentTimeInMs() / 1000);
	}
}