using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld.Planet;
using Verse;

namespace Mute
{
	// Token: 0x0200000C RID: 12
	public static class Launcher
	{
		// Token: 0x06000053 RID: 83 RVA: 0x000027C3 File Offset: 0x000009C3
		public static void Add(LogEntry entry)
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006128 File Offset: 0x00004328
		public unsafe static bool DoDetour(MethodInfo source, MethodInfo destination)
		{
			if (IntPtr.Size == 8)
			{
				byte* arg_136_0 = (byte*)source.MethodHandle.GetFunctionPointer().ToInt64();
				long num = destination.MethodHandle.GetFunctionPointer().ToInt64();
				byte* ptr = arg_136_0;
				long* ptr2 = (long*)(ptr + 2);
				*ptr = 72;
				ptr[1] = 184;
				*ptr2 = num;
				ptr[10] = 255;
				ptr[11] = 224;
			}
			else
			{
				int num2 = source.MethodHandle.GetFunctionPointer().ToInt32();
				int arg_1A6_0 = destination.MethodHandle.GetFunctionPointer().ToInt32();
				byte* ptr3 = (byte*)num2;
				int* ptr4 = (int*)(ptr3 + 1);
				int num3 = arg_1A6_0 - num2 - 5;
				*ptr3 = 233;
				*ptr4 = num3;
			}
			return true;
		}
		// Token: 0x06000055 RID: 85 RVA: 0x00006204 File Offset: 0x00004404
		public static void ExposeData()
		{
			List<Battle> battles = new List<Battle>();
			Scribe_Collections.Look<Battle>(ref battles, "battles", LookMode.Deep, Array.Empty<object>());
		}

		// Token: 0x06000056 RID: 86 RVA: 0x0000622C File Offset: 0x0000442C
		public static void Launch(bool doGC, bool doBL)
		{
			BindingFlags bindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			BindingFlags bindingFlag1 = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			if (doGC)
			{
				LongEventHandler.QueueLongEvent(delegate()
				{
					Launcher.DoDetour(typeof(WorldPawnGC).GetMethod("WorldPawnGCTick", bindingFlag), typeof(Launcher).GetMethod("WorldPawnGCTick", bindingFlag1));
					Log.Message("[RuntimeGC] Detour completed: MuteGC");
				}, "Initializing", false, null, true);
			}
			if (doBL)
			{
				LongEventHandler.QueueLongEvent(delegate()
				{
					Launcher.DoDetour(typeof(BattleLog).GetMethod("Add", bindingFlag), typeof(Launcher).GetMethod("Add", bindingFlag1));
					Launcher.DoDetour(typeof(BattleLog).GetMethod("ExposeData", bindingFlag), typeof(Launcher).GetMethod("ExposeData", bindingFlag1));
					Log.Message("[RuntimeGC] Detour completed: MuteBL");
				}, "Initializing", false, null, true);
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000027C3 File Offset: 0x000009C3
		public static void WorldPawnGCTick()
		{
		}
	}
}
