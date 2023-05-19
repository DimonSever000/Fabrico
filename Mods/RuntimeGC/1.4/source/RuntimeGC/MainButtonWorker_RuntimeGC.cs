using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000003 RID: 3
	public class MainButtonWorker_RuntimeGC : MainButtonWorker_ToggleTab
	{
		// Token: 0x06000008 RID: 8 RVA: 0x0000233C File Offset: 0x0000053C
		public override void DoButton(Rect rect)
		{
			Text.Font = GameFont.Small;
			string labelCap = this.def.LabelCap;
			float labelCapWidth = this.def.LabelCapWidth;
			bool flag = labelCapWidth > rect.width - 2f;
			if (flag)
			{
				labelCap = this.def.ShortenedLabelCap;
				labelCapWidth = this.def.ShortenedLabelCapWidth;
			}
			bool flag2 = MainButtonWorker_RuntimeGC.enableBar || MainButtonWorker_RuntimeGC.enableTip || MainButtonWorker_RuntimeGC.onScreenMemUsage;
			if (flag2)
			{
				MainButtonWorker_RuntimeGC.updatetick++;
				bool flag3 = MainButtonWorker_RuntimeGC.updatetick > MainButtonWorker_RuntimeGC.updateInterval;
				if (flag3)
				{
					MainButtonWorker_RuntimeGC.updatetick = 0;
					long totalMemory = GC.GetTotalMemory(false) / 1024L;
					float single = (float)totalMemory / 1024f;
					bool flag4 = MainButtonWorker_RuntimeGC.enableTip;
					if (flag4)
					{
						MainButtonWorker_RuntimeGC.tipCache = string.Format(MainButtonWorker_RuntimeGC.MMTipTranslated, single);
					}
					bool flag5 = MainButtonWorker_RuntimeGC.enableBar;
					if (flag5)
					{
						MainButtonWorker_RuntimeGC.progress = Mathf.Clamp01((single - (float)MainButtonWorker_RuntimeGC.memoryBarLowerMb) / (float)MainButtonWorker_RuntimeGC.memoryBarStepMb);
					}
					bool flag6 = MainButtonWorker_RuntimeGC.onScreenMemUsage;
					if (flag6)
					{
						MainButtonWorker_RuntimeGC.labelCache = string.Format("{0:F2} Mb\n{1} Kb", single, totalMemory);
					}
				}
			}
			bool _width = labelCapWidth > 0.85f * rect.width - 1f;
			bool flag7 = this.def.Icon != null;
			string empty;
			if (flag7)
			{
				empty = string.Empty;
			}
			else
			{
				empty = (MainButtonWorker_RuntimeGC.onScreenMemUsage ? MainButtonWorker_RuntimeGC.labelCache : labelCap);
			}
			string str = empty;
			float single2 = (!_width) ? -1f : 2f;
			Color? nullable = null;
			bool flag8 = Widgets.ButtonTextSubtle(rect, str, MainButtonWorker_RuntimeGC.progress, single2, SoundDefOf.Mouseover_Category, new Vector2(-1f, -1f), nullable, false) && Current.ProgramState == ProgramState.Playing;
			if (flag8)
			{
				this.InterfaceTryActivate();
			}
			TooltipHandler.TipRegion(rect, MainButtonWorker_RuntimeGC.TabDescriptionTranslated + MainButtonWorker_RuntimeGC.tipCache);
			bool flag9 = this.def.Icon != null;
			if (flag9)
			{
				Vector2 _center = rect.center;
				float settingComponentHeight = 16f;
				bool flag10 = Mouse.IsOver(rect);
				if (flag10)
				{
					_center += new Vector2(2f, -2f);
				}
				GUI.DrawTexture(new Rect(_center.x - settingComponentHeight, _center.y - settingComponentHeight, 32f, 32f), this.def.Icon);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000025BC File Offset: 0x000007BC
		internal static void Notify_UpdateIntervalChanged(int newint)
		{
			MainButtonWorker_RuntimeGC.updateInterval = newint;
			MainButtonWorker_RuntimeGC.updatetick = 32767;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000025D0 File Offset: 0x000007D0
		internal static void UpdateSettings(RuntimeGCSettings settings)
		{
			MainButtonWorker_RuntimeGC.enableBar = settings.EnableMemoryMonitorBar;
			MainButtonWorker_RuntimeGC.enableTip = settings.EnableMemoryMonitorTip;
			MainButtonWorker_RuntimeGC.memoryBarLowerMb = settings.MemoryMonitorBarLowerBoundMb;
			MainButtonWorker_RuntimeGC.memoryBarStepMb = settings.MemoryMonitorBarUpperBoundMb - MainButtonWorker_RuntimeGC.memoryBarLowerMb;
			MainButtonWorker_RuntimeGC.updateInterval = settings.MemoryMonitorUpdateInterval;
			MainButtonWorker_RuntimeGC.onScreenMemUsage = settings.DevOnScreenMemoryUsage;
			MainButtonWorker_RuntimeGC.progress = 0f;
			MainButtonWorker_RuntimeGC.tipCache = "";
			MainButtonWorker_RuntimeGC.updatetick = 32767;
		}

		// Token: 0x04000008 RID: 8
		private static bool enableBar;

		// Token: 0x04000009 RID: 9
		private static int memoryBarLowerMb;

		// Token: 0x0400000A RID: 10
		private static int memoryBarStepMb;

		// Token: 0x0400000B RID: 11
		private static bool enableTip;

		// Token: 0x0400000C RID: 12
		public static int updateInterval = 600;

		// Token: 0x0400000D RID: 13
		public static bool onScreenMemUsage = false;

		// Token: 0x0400000E RID: 14
		internal static string TabDescriptionTranslated;

		// Token: 0x0400000F RID: 15
		internal static string MMTipTranslated;

		// Token: 0x04000010 RID: 16
		private static float progress = 0f;

		// Token: 0x04000011 RID: 17
		private static string tipCache = "";

		// Token: 0x04000012 RID: 18
		private static int updatetick = 32767;

		// Token: 0x04000013 RID: 19
		private static string labelCache = "";
	}
}
