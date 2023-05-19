using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Toolbox
{
	// Token: 0x0200000D RID: 13
	public static class DefPackageCleaner
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00006294 File Offset: 0x00004494
		public static bool Cleaned
		{
			get
			{
				bool flag = DefPackageCleaner.coreMod == null;
				return !flag && Enumerable.Count<Def>(DefPackageCleaner.coreMod.AllDefs) == 0;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000062CC File Offset: 0x000044CC
		public static void CleanDefPackage()
		{
			FieldInfo field = typeof(ModContentPack).GetField("defs", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			int count = 0;
			foreach (ModContentPack runningMod in LoadedModManager.RunningMods)
			{
				bool isCoreMod = runningMod.IsCoreMod;
				if (isCoreMod)
				{
					DefPackageCleaner.coreMod = runningMod;
				}
				count += ((List<Def>)field.GetValue(runningMod)).Count;
				field.SetValue(runningMod, new List<Def>());
			}
			Log.Message("[DefPackageCleaner] Cleaned " + count.ToString() + " DefPackages.");
			bool flag = Current.ProgramState == ProgramState.Playing;
			if (flag)
			{
				Messages.Message("MsgDefPackageCleaned".Translate(count), MessageTypeDefOf.PositiveEvent, false);
			}
			GC.Collect(2);
		}

		// Token: 0x04000049 RID: 73
		private static ModContentPack coreMod;
	}
}
