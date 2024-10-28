using UnityEngine;

namespace _Project
{
	public class Timer
	{
		private double waitUntil;
		private double delayTime;
		private bool useRealTime;
		
		private bool isPaused;
		private double hashLeftTimeInMs;
		
		public Timer()
		{
			this.Wait(0);
		}
		
		public Timer(bool useRealTime)
		{
			this.Wait(0, useRealTime);
		}
		
		public Timer Wait(int value, bool useRealTime = false)
		{
			this.delayTime = value;
			this.useRealTime = useRealTime;
			this.waitUntil = this.useRealTime
					? DateTimeExtensions.GetCurrentTimeInMs() / 1000 + value
					: Time.time + value;
			
			if (this.IfPaused())
			{
				this.hashLeftTimeInMs = this.GetLeftTime();
			}
			
			return this;
		}
	
		public Timer Wait(float value, bool useRealTime = false)
		{
			this.delayTime = value;
			this.useRealTime = useRealTime;
			this.waitUntil = this.useRealTime
					? DateTimeExtensions.GetCurrentTimeInMs() / 1000 + value
					: Time.time + value;
			
			if (this.IfPaused())
			{
				this.hashLeftTimeInMs = this.GetLeftTime();
			}
			
			return this;
		}
		
		public Timer Wait(double value, bool useRealTime = false)
		{
			this.delayTime = value;
			this.useRealTime = useRealTime;
			this.waitUntil = this.useRealTime
					? DateTimeExtensions.GetCurrentTimeInMs() / 1000 + value
					: Time.time + value;
			
			if (this.IfPaused())
			{
				this.hashLeftTimeInMs = this.GetLeftTime();
			}
			
			return this;
		}
		
		public Timer Pause()
		{
			if (!this.isPaused)
			{
				this.hashLeftTimeInMs = this.GetLeftTime();
				
				this.isPaused = true;
			}
			
			return this;
		}
		
		public Timer Resume()
		{
			if (this.isPaused)
			{
				this.Wait(this.hashLeftTimeInMs);
				
				this.isPaused = false;
			}
			
			return this;
		}
		
		public int GetLeftTimeInSeconds()
		{
			if (this.IfPaused())
			{
				return (int) this.hashLeftTimeInMs;
			}
			
			if (this.useRealTime)
			{
				return (int) (this.waitUntil - DateTimeExtensions.GetCurrentTimeInS());
			}
			
			return (int) (this.waitUntil - Time.time);
		}
		
		public double GetLeftTime()
		{
			if (this.IfPaused())
			{
				return this.hashLeftTimeInMs;
			}
			
			if (this.useRealTime)
			{
				return this.waitUntil - DateTimeExtensions.GetCurrentTimeInMs() / 1000;
			}
			
			return this.waitUntil - Time.time;
		}
		
		public double GetPassedTime()
		{
			if (this.IfPaused())
			{
				return this.delayTime - this.hashLeftTimeInMs;
			}
			
			if (this.useRealTime)
			{
				return (this.delayTime - (this.waitUntil - DateTimeExtensions.GetCurrentTimeInMs() / 1000));
			}
			
			return this.delayTime - (this.waitUntil - Time.time);
		}
		
		public double GetPassedTimeInPercent()
		{
			if (this.IfPaused())
			{
				return (this.delayTime - this.hashLeftTimeInMs) / this.delayTime;
			}
			
			if (this.useRealTime)
			{
				return (this.delayTime - (this.waitUntil - DateTimeExtensions.GetCurrentTimeInMs() / 1000)) / this.delayTime;
			}
			
			return (this.delayTime - (this.waitUntil - Time.time)) / this.delayTime;
		}
		
		public bool IfPaused()
		{
			return this.isPaused;
		}
		
		public bool IfReady()
		{
			if (this.isPaused)
			{
				return this.hashLeftTimeInMs <= 0;
			}
			
			if (this.useRealTime)
			{
				return DateTimeExtensions.GetCurrentTimeInMs() / 1000 >= this.waitUntil;
			}
			
			return Time.time >= this.waitUntil;
		}
	}
}