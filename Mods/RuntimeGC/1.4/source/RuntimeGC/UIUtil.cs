using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000002 RID: 2
	internal static class UIUtil
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static MainButtonWorker MainButtonWorker
		{
			get
			{
				bool flag = UIUtil.worker == null;
				if (flag)
				{
					UIUtil.worker = DefDatabase<MainButtonDef>.GetNamed("RGC_UI", true).Worker;
				}
				return UIUtil.worker;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000208A File Offset: 0x0000028A
		public static void BeginRestartCheck()
		{
			UIUtil.resetFlagPtr = 0;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002094 File Offset: 0x00000294
		public static void DrawCheckboxRestartIfApplied(Rect rect, string label, string tip, ref bool checkOn)
		{
			bool flag = (UIUtil.resetFlags & 1 << UIUtil.resetFlagPtr) > 0;
			if (flag)
			{
				checkOn = !checkOn;
				UIUtil.resetFlags ^= 1 << UIUtil.resetFlagPtr;
			}
			UIUtil.resetFlagPtr++;
			Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, false);
			TooltipHandler.TipRegion(rect, tip);
			bool flag2 = checkOn != checkOn;
			if (flag2)
			{
				int num = UIUtil.resetFlagPtr - 1;
				bool flag3 = (RuntimeGC.Settings.restartFlags & 1 << num) > 0;
				if (flag3)
				{
					RuntimeGCSettings runtimeGCSetting = RuntimeGC.Settings;
					runtimeGCSetting.restartFlags ^= 1 << num;
				}
				else
				{
					Dialog_MessageBox dialogMessageBox = new Dialog_MessageBox("DlgTextRestartNotice".Translate(), "OK".Translate(), delegate()
					{
						RuntimeGCSettings settings = RuntimeGC.Settings;
						settings.restartFlags |= 1 << num;
					}, "UndoChange".Translate(), delegate()
					{
						UIUtil.resetFlags |= 1 << num;
					}, "DlgTitleRestart".Translate(), false, null, null, WindowLayer.Dialog);
					Find.WindowStack.Add(dialogMessageBox);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021D8 File Offset: 0x000003D8
		public static float DrawSectionLabel(float x, float y, string text, float xMax)
		{
			Text.Font = GameFont.Medium;
			Vector2 vector2 = Text.CalcSize(text);
			Rect rect = new Rect(x + 0f, y + 0f, vector2.x, vector2.y);
			Widgets.Label(rect, text);
			Color _color = GUI.color;
			GUI.color = Color.gray;
			Widgets.DrawLineHorizontal(rect.xMax + 5f, rect.y + rect.height / 2f, xMax - rect.xMax - 5f);
			Text.Font = GameFont.Small;
			GUI.color = _color;
			return rect.yMax + 5f;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002284 File Offset: 0x00000484
		public static void Notify_MMBtnLabelChanged()
		{
			int memoryMonitorUpdateInterval = RuntimeGC.Settings.MemoryMonitorUpdateInterval;
			bool flag = Enum.IsDefined(typeof(MemoryMonitorUpdateMode), memoryMonitorUpdateInterval);
			TaggedString taggedString;
			if (flag)
			{
				MemoryMonitorUpdateMode memoryMonitorUpdateMode = (MemoryMonitorUpdateMode)memoryMonitorUpdateInterval;
				taggedString = ("MMUpdate_" + memoryMonitorUpdateMode.ToString()).Translate();
			}
			else
			{
				taggedString = "UITicks".Translate(memoryMonitorUpdateInterval);
			}
			UIUtil.MMIntervalButtonLabelCache = taggedString;
		}

		// Token: 0x04000001 RID: 1
		public const float MarginLarge = 0f;

		// Token: 0x04000002 RID: 2
		public const float MarginHorizontal = 5f;

		// Token: 0x04000003 RID: 3
		public const float MarginVertical = 5f;

		// Token: 0x04000004 RID: 4
		private static int resetFlags;

		// Token: 0x04000005 RID: 5
		private static int resetFlagPtr;

		// Token: 0x04000006 RID: 6
		private static MainButtonWorker worker;

		// Token: 0x04000007 RID: 7
		public static string MMIntervalButtonLabelCache;
	}
}
