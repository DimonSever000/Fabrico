using System;

namespace RuntimeGC
{
	// Token: 0x02000005 RID: 5
	internal enum MemoryMonitorUpdateMode
	{
		// Token: 0x04000023 RID: 35
		Debug_Flash = 1,
		// Token: 0x04000024 RID: 36
		Debug_Realtime = 15,
		// Token: 0x04000025 RID: 37
		PerSecond = 60,
		// Token: 0x04000026 RID: 38
		UltraFrequent = 150,
		// Token: 0x04000027 RID: 39
		Frequent = 300,
		// Token: 0x04000028 RID: 40
		Moderate = 600,
		// Token: 0x04000029 RID: 41
		PerMinuteQuarter = 900,
		// Token: 0x0400002A RID: 42
		PerMinuteHalf = 1800,
		// Token: 0x0400002B RID: 43
		PerMinute = 3600,
		// Token: 0x0400002C RID: 44
		Debug_Lazy = 18000,
		// Token: 0x0400002D RID: 45
		Debug_frozen = 2147483647
	}
}
