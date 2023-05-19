using System;
using UnityEngine;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000008 RID: 8
	public class RuntimeGC : Mod
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000031DE File Offset: 0x000013DE
		public RuntimeGC(ModContentPack content) : base(content)
		{
			RuntimeGC.Settings = base.GetSettings<RuntimeGCSettings>();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000031F4 File Offset: 0x000013F4
		public override void DoSettingsWindowContents(Rect inRect)
		{
			GUI.BeginGroup(inRect);
			float lblMenuTitle = UIUtil.DrawSectionLabel(inRect.x, inRect.y, "SettingsMMCategory".Translate(), inRect.xMax);
			string str = "SettingsMMTipLabel".Translate();
			float settingWindowWidth = inRect.width / 2f - 0f - 20f;
			float settingComponentHeight = Text.CalcHeight(str, settingWindowWidth);
			Rect rect = new Rect(inRect.x + 0f + 5f, lblMenuTitle, inRect.width - 0f - 10f, settingComponentHeight * 4f + 15f + (Prefs.DevMode ? (settingComponentHeight + 5f) : 0f));
			GUI.BeginGroup(rect);
			Rect rect2 = new Rect(0f, 0f, settingWindowWidth + 5f, settingComponentHeight);
			bool enableMemoryMonitorTip = RuntimeGC.Settings.EnableMemoryMonitorTip;
			Widgets.CheckboxLabeled(rect2, str, ref enableMemoryMonitorTip, false, null, null, false);
			RuntimeGC.Settings.EnableMemoryMonitorTip = enableMemoryMonitorTip;
			TooltipHandler.TipRegion(rect2, "SettingsMMTipTip".Translate());
			rect2 = new Rect(rect2.x, rect2.yMax + 5f, settingWindowWidth + 5f, settingComponentHeight);
			enableMemoryMonitorTip = RuntimeGC.Settings.EnableMemoryMonitorBar;
			Widgets.CheckboxLabeled(rect2, "SettingsMMBarLabel".Translate(), ref enableMemoryMonitorTip, false, null, null, false);
			RuntimeGC.Settings.EnableMemoryMonitorBar = enableMemoryMonitorTip;
			TooltipHandler.TipRegion(rect2, "SettingsMMBarTip".Translate());
			bool enableMemoryMonitorBar = RuntimeGC.Settings.EnableMemoryMonitorBar;
			if (enableMemoryMonitorBar)
			{
				Rect rect3 = new Rect(5f, rect2.yMax + 5f, settingWindowWidth, settingComponentHeight);
				Widgets.Label(rect3, "SettingsMMRangeLabel".Translate());
				TooltipHandler.TipRegion(rect3, "SettingsMMRangeTip".Translate());
				float single3 = Text.CalcSize("x32").x;
				Rect rect4 = new Rect(rect3.xMax - single3 * 2f - 5f - 5f, rect3.y, single3, settingComponentHeight);
				bool flag = !Mouse.IsOver(rect4);
				if (flag)
				{
					GUI.color = Color.gray;
				}
				else
				{
					GUI.color = Color.cyan;
				}
				Widgets.Label(rect4, "x32");
				Widgets.DrawLineHorizontal(rect4.x, rect4.yMax - 1f, rect4.width);
				GUI.color = Color.white;
				bool flag2 = Widgets.ButtonInvisible(rect4, true);
				if (flag2)
				{
					RuntimeGC.Settings.MemoryMonitorBarLowerBoundMb = 0;
					RuntimeGC.Settings.MemoryMonitorBarUpperBoundMb = 1024;
				}
				Rect rect5 = new Rect(rect3.xMax - single3 - 5f, rect3.y, single3, settingComponentHeight);
				bool flag3 = !Mouse.IsOver(rect5) || IntPtr.Size != 8;
				if (flag3)
				{
					GUI.color = Color.gray;
				}
				else
				{
					GUI.color = Color.cyan;
				}
				Widgets.Label(rect5, "x64");
				bool flag4 = IntPtr.Size != 8;
				if (flag4)
				{
					Widgets.DrawLine(new Vector2(rect5.x, rect5.y), new Vector2(rect5.xMax, rect5.yMax), Color.gray, 1f);
				}
				else
				{
					Widgets.DrawLineHorizontal(rect5.x, rect5.yMax - 1f, rect5.width);
					bool flag5 = Widgets.ButtonInvisible(rect5, true);
					if (flag5)
					{
						RuntimeGC.Settings.MemoryMonitorBarLowerBoundMb = 0;
						RuntimeGC.Settings.MemoryMonitorBarUpperBoundMb = 2048;
					}
				}
				GUI.color = Color.white;
				Rect rect6 = new Rect(rect3.x, rect3.yMax + 5f, settingWindowWidth, settingComponentHeight);
				IntRange intRange = new IntRange(RuntimeGC.Settings.MemoryMonitorBarLowerBoundMb, RuntimeGC.Settings.MemoryMonitorBarUpperBoundMb);
				Widgets.IntRange(rect6, 233, ref intRange, 0, 8192, null, 0);
				RuntimeGC.Settings.MemoryMonitorBarLowerBoundMb = intRange.min;
				RuntimeGC.Settings.MemoryMonitorBarUpperBoundMb = intRange.max;
			}
			Rect settingBtnContainer = new Rect(rect.width / 2f + (rect.width / 2f - 130f) / 2f, (settingComponentHeight * 2f + 5f - 35f) / 2f, 130f, 35f);
			UIUtil.MainButtonWorker.DoButton(settingBtnContainer);
			Rect rect7 = new Rect(rect.width / 2f + 10f, settingComponentHeight * 2f + 10f + (35f - settingComponentHeight) / 2f, rect.width / 2f - 5f - 125f, settingComponentHeight);
			Widgets.Label(rect7, "SettingsMMIntervalLabel".Translate());
			TooltipHandler.TipRegion(rect7, "SettingsMMIntervalTip".Translate());
			Rect rect8 = new Rect(rect.xMax - 125f - 10f, settingComponentHeight * 2f + 10f, 125f, 35f);
			bool flag6 = Widgets.ButtonText(rect8, UIUtil.MMIntervalButtonLabelCache, true, true, true, null);
			if (flag6)
			{
				FloatMenuUtil.GenerateFloatMenuGroup(FloatMenuUtil.GroupMMUpdateMode);
			}
			bool devMode = Prefs.DevMode;
			if (devMode)
			{
				Rect dev_ReplaceRuntimeWithUsageContainer = new Rect(rect.width / 2f + 10f, rect8.yMax + 5f, rect.width / 2f - 15f, settingComponentHeight);
				enableMemoryMonitorTip = RuntimeGC.Settings.DevOnScreenMemoryUsage;
				Widgets.CheckboxLabeled(dev_ReplaceRuntimeWithUsageContainer, "SettingsDevOnScreenMemoryUsageLabel".Translate(), ref enableMemoryMonitorTip, false, null, null, false);
				bool flag7 = enableMemoryMonitorTip != RuntimeGC.Settings.DevOnScreenMemoryUsage;
				if (flag7)
				{
					RuntimeGC.Settings.DevOnScreenMemoryUsage = enableMemoryMonitorTip;
					RuntimeGC.Settings.UpdateCache();
				}
			}
			GUI.EndGroup();
			UIUtil.BeginRestartCheck();
			float lblCatAuto = UIUtil.DrawSectionLabel(inRect.x, rect.yMax + settingComponentHeight, "SettingsAutoCleanupCategory".Translate(), inRect.width / 2f - 5f);
			Rect rect9 = new Rect(rect.x, lblCatAuto, settingWindowWidth, settingComponentHeight * 3f + 10f);
			GUI.BeginGroup(rect9);
			Rect chbxClearModInfoContainer = new Rect(0f, 0f, settingWindowWidth, settingComponentHeight);
			UIUtil.DrawCheckboxRestartIfApplied(chbxClearModInfoContainer, "SettingsACModMetaDataLabel".Translate(), "SettingsACModMetaDataTip".Translate(), ref RuntimeGC.Settings.AutoCleanModMetaData);
			Rect chbxClearTransContainer = new Rect(0f, settingComponentHeight + 5f, settingWindowWidth, settingComponentHeight);
			UIUtil.DrawCheckboxRestartIfApplied(chbxClearTransContainer, "SettingsACLanguageDataLabel".Translate(), "SettingsACLanguageDataTip".Translate(), ref RuntimeGC.Settings.AutoCleanLanguageData);
			Rect chbxClearDefContainer = new Rect(0f, settingComponentHeight * 2f + 10f, settingWindowWidth, settingComponentHeight);
			UIUtil.DrawCheckboxRestartIfApplied(chbxClearDefContainer, "SettingsACDefPackageLabel".Translate(), "SettingsACDefPackageTip".Translate(), ref RuntimeGC.Settings.AutoCleanDefPackage);
			GUI.EndGroup();
			float lblCatMute = UIUtil.DrawSectionLabel(rect.x + rect.width / 2f + 5f, rect.yMax + settingComponentHeight, "SettingsMuteCategory".Translate(), inRect.xMax);
			GUI.BeginGroup(new Rect(rect.x + rect.width / 2f + 10f, lblCatMute, settingWindowWidth, settingComponentHeight * 2f + 5f));
			Rect chbxReplaceVanillaGC = new Rect(0f, 0f, settingWindowWidth, settingComponentHeight);
			UIUtil.DrawCheckboxRestartIfApplied(chbxReplaceVanillaGC, "SettingsMuteGCLabel".Translate(), "SettingsMuteGCTip".Translate(), ref RuntimeGC.Settings.DoMuteGC);
			Rect chbxMuteBattleLogs = new Rect(0f, settingComponentHeight + 5f, settingWindowWidth, settingComponentHeight);
			UIUtil.DrawCheckboxRestartIfApplied(chbxMuteBattleLogs, "SettingsMuteBLLabel".Translate(), "SettingsMuteBLTip".Translate(), ref RuntimeGC.Settings.DoMuteBL);
			GUI.EndGroup();
			float lblCatGeneral = UIUtil.DrawSectionLabel(inRect.x, rect9.yMax + settingComponentHeight + (Prefs.DevMode ? (settingComponentHeight + 5f) : 0f), "SettingsGeneralCategory".Translate(), inRect.width / 2f - 5f);
			GUI.BeginGroup(new Rect(rect.x, lblCatGeneral, settingWindowWidth, settingComponentHeight * 3f + 10f));
			Rect chbxArchiveAllRuntimeGCContainer = new Rect(0f, 0f, settingWindowWidth, settingComponentHeight);
			Widgets.CheckboxLabeled(chbxArchiveAllRuntimeGCContainer, "SettingsArchiveGCLabel".Translate(), ref RuntimeGC.Settings.ArchiveGCDialog, false, null, null, false);
			TooltipHandler.TipRegion(chbxArchiveAllRuntimeGCContainer, "SettingsArchiveGCTip".Translate());
			Rect chbxArchiveWorldPawnContainer = new Rect(0f, settingComponentHeight + 5f, settingWindowWidth, settingComponentHeight);
			Widgets.CheckboxLabeled(chbxArchiveWorldPawnContainer, "SettingsArchiveGeneralLabel".Translate(), ref RuntimeGC.Settings.ArchiveMessageGeneral, false, null, null, false);
			TooltipHandler.TipRegion(chbxArchiveWorldPawnContainer, "SettingsArchiveGeneralTip".Translate());
			GUI.EndGroup();
			bool flag8 = Widgets.ButtonText(new Rect(inRect.xMax - 5f - 135f - 50f, inRect.yMax - 35f - 65f, 135f, 35f), "SettingsReset".Translate(), true, true, true, null);
			if (flag8)
			{
				RuntimeGC.Settings.ResetToDefault();
			}
			GUI.EndGroup();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003BDC File Offset: 0x00001DDC
		public override string SettingsCategory()
		{
			return "RuntimeGC";
		}

		// Token: 0x04000035 RID: 53
		public static RuntimeGCSettings Settings;
	}
}
