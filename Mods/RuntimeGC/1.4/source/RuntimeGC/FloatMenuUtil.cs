using System;
using System.Collections;
using System.Collections.Generic;
using RimWorld;
using Toolbox;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000007 RID: 7
	public static class FloatMenuUtil
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002B7C File Offset: 0x00000D7C
		static FloatMenuUtil()
		{
			FloatMenuUtil.groups = new Dictionary<string, List<string>>();
			FloatMenuUtil.items = new Dictionary<string, Action>();
			FloatMenuUtil.devOnly = new Dictionary<string, bool>();
			string groupFix = FloatMenuUtil.GroupFix;
			FloatMenuUtil.Add(groupFix, "FloatDebugLog".Translate(), delegate
			{
				bool flag = !Find.WindowStack.TryRemove(typeof(EditWindow_Log), true);
				if (flag)
				{
					Find.WindowStack.Add(new EditWindow_Log());
				}
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatAGRegen".Translate(), delegate
			{
				foreach (Map map in Find.Maps)
				{
					map.avoidGrid.Regenerate();
				}
				FloatMenuUtil.Message("MsgTextAGR".Translate(), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatFactionFixItems1".Translate(), delegate
			{
				CleanserUtil.FixFactionRelationships();
				FloatMenuUtil.Message("MsgTextFFR".Translate(), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatFactionFixItems2".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextFFL".Translate(CleanserUtil.FixFactionLeader_Wrapped()), MessageTypeDefOf.PositiveEvent);
			}, false);
			groupFix = FloatMenuUtil.GroupTools;
			FloatMenuUtil.Add(groupFix, "FloatToolsItems1".Translate(), delegate
			{
				int num = CleanserUtil.DeconstructAnimalFamily();
				Log.Message("CleanserUtil.DeconstructAnimalFamily():Round 1 completed.");
				CleanserUtil.DeconstructAnimalFamily();
				Log.Message("CleanserUtil.DeconstructAnimalFamily():Round 2 completed.");
				FloatMenuUtil.Message("MsgTextAFT".Translate(num), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRFH".Translate(CleanserUtil.RemoveFilth(Find.CurrentMap, true)), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2Dev1".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRFM".Translate(CleanserUtil.RemoveFilth(Find.CurrentMap, false)), MessageTypeDefOf.PositiveEvent);
			}, true);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2Dev2".Translate(), delegate
			{
				int num = 0;
				foreach (Map map in Find.Maps)
				{
					num += CleanserUtil.RemoveFilth(map, false);
				}
				FloatMenuUtil.Message("MsgTextRFW".Translate(num), MessageTypeDefOf.PositiveEvent);
			}, true);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2Snow".Translate(), delegate
			{
				CleanserUtil.RemoveSnow(Find.CurrentMap, true);
				FloatMenuUtil.Message("MsgTextRSH".Translate(), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2SnowDev1".Translate(), delegate
			{
				CleanserUtil.RemoveSnow(Find.CurrentMap, false);
				FloatMenuUtil.Message("MsgTextRSM".Translate(), MessageTypeDefOf.PositiveEvent);
			}, true);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems2SnowDev2".Translate(), delegate
			{
				foreach (Map map in Find.Maps)
				{
					CleanserUtil.RemoveSnow(map, false);
				}
				FloatMenuUtil.Message("MsgTextRSW".Translate(), MessageTypeDefOf.PositiveEvent);
			}, true);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems3".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRCM".Translate(CleanserUtil.RemoveCorpses()), MessageTypeDefOf.NeutralEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems6".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRCM".Translate(CleanserUtil.RemoveCorpses(true)), MessageTypeDefOf.NeutralEvent);
			}, false);
			try
			{
				FloatMenuUtil.Add(groupFix, "FloatToolsItems4".Translate(), delegate
				{
					FloatMenuUtil.Message("MsgTextRBL".Translate(CleanserUtil.RemoveAllBattleLogEntries()), MessageTypeDefOf.PositiveEvent);
				}, false);
			}
			catch
			{
				Log.Message("We're in the try catch for MsgTextRBL uh oh");
			}
			FloatMenuUtil.Add(groupFix, "FloatToolsItems5".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRAM".Translate(CleanserUtil.RemoveIArchivable(false)), MessageTypeDefOf.PositiveEvent);
			}, false);
			FloatMenuUtil.Add(groupFix, "FloatToolsItems5Dev".Translate(), delegate
			{
				FloatMenuUtil.Message("MsgTextRAM".Translate(CleanserUtil.RemoveIArchivable(true)), MessageTypeDefOf.PositiveEvent);
			}, true);
			groupFix = FloatMenuUtil.GroupQuickbar;
			FloatMenuUtil.Add(groupFix, "QuickCloseLetStack".Translate(), delegate
			{
				LetterStack letterStack = Find.LetterStack;
				bool flag = letterStack == null;
				if (!flag)
				{
					for (int i = letterStack.LettersListForReading.Count - 1; i > -1; i--)
					{
						letterStack.RemoveLetter(letterStack.LettersListForReading[i]);
					}
				}
			}, false);
			FloatMenuUtil.Add(groupFix, "QuickUnlockSpeedLimit".Translate(), delegate
			{
				CleanserUtil.UnlockNormalSpeedLimit();
				Find.WindowStack.TryRemove(typeof(UserInterface), true);
			}, false);
			FloatMenuUtil.Add(groupFix, "QuickOpenSettings".Translate(), delegate
			{
				CleanserUtil.OpenModSettingsPage();
			}, false);
			groupFix = GroupMMUpdateMode;
            foreach (MemoryMonitorUpdateMode m in Enum.GetValues(typeof(MemoryMonitorUpdateMode)))
                Add(groupFix, ("MMUpdate_" + m.ToString()).Translate(), delegate
                {
                    RuntimeGC.Settings.MemoryMonitorUpdateInterval = (int)m;
                    UIUtil.Notify_MMBtnLabelChanged();
                }, m.ToString().Contains("Debug_"));		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002F78 File Offset: 0x00001178
		public static void Add(string group, string label, Action action, bool devonly = false)
		{
			bool flag = !FloatMenuUtil.groups.ContainsKey(group);
			if (flag)
			{
				FloatMenuUtil.groups.Add(group, new List<string>());
			}
			FloatMenuUtil.groups[group].Add(label);
			FloatMenuUtil.items.Add(label, action);
			FloatMenuUtil.devOnly.Add(label, devonly);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002FD8 File Offset: 0x000011D8
		public static void GenerateFloatMenuGroup(string group)
		{
			List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();
			foreach (string item in FloatMenuUtil.groups[group])
			{
				bool flag = FloatMenuUtil.devOnly[item] && !Prefs.DevMode;
				if (!flag)
				{
					floatMenuOptions.Add(new FloatMenuOption(item, FloatMenuUtil.items[item], MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
				}
			}
			Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003088 File Offset: 0x00001288
		public static void GenerateMemoryReclaimOptions()
		{
			List<FloatMenuOption> floatMenuOptions = new List<FloatMenuOption>();
			FloatMenuOption floatMenuOption = new FloatMenuOption("FloatACModMetaData".Translate(), new Action(ModMetaDataCleaner.CleanModMetaData), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
			bool cleaned = ModMetaDataCleaner.Cleaned;
			if (cleaned)
			{
				floatMenuOption.Label = "FloatACModMetaDataCleared".Translate();
				floatMenuOption.Disabled = true;
			}
			floatMenuOptions.Add(floatMenuOption);
			floatMenuOption = new FloatMenuOption("FloatACLanguageData".Translate(), new Action(LanguageDataCleaner.CleanLanguageData), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
			bool cleaned2 = LanguageDataCleaner.Cleaned;
			if (cleaned2)
			{
				floatMenuOption.Label = "FloatACLanguageDataCleared".Translate();
				floatMenuOption.Disabled = true;
			}
			floatMenuOptions.Add(floatMenuOption);
			floatMenuOption = new FloatMenuOption("FloatACDefPackage".Translate(), new Action(DefPackageCleaner.CleanDefPackage), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
			bool cleaned3 = DefPackageCleaner.Cleaned;
			if (cleaned3)
			{
				floatMenuOption.Label = "FloatACDefPackageCleared".Translate();
				floatMenuOption.Disabled = true;
			}
			floatMenuOptions.Add(floatMenuOption);
			Find.WindowStack.Add(new FloatMenu(floatMenuOptions));
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000031C9 File Offset: 0x000013C9
		public static void Message(string str, MessageTypeDef type)
		{
			Messages.Message(str, type, RuntimeGC.Settings.ArchiveMessageGeneral);
		}

		// Token: 0x0400002E RID: 46
		private static Dictionary<string, List<string>> groups;

		// Token: 0x0400002F RID: 47
		private static Dictionary<string, Action> items;

		// Token: 0x04000030 RID: 48
		private static Dictionary<string, bool> devOnly;

		// Token: 0x04000031 RID: 49
		public static readonly string GroupTools = "tools";

		// Token: 0x04000032 RID: 50
		public static readonly string GroupFix = "fix";

		// Token: 0x04000033 RID: 51
		public static readonly string GroupQuickbar = "quickbar";

		// Token: 0x04000034 RID: 52
		public static readonly string GroupMMUpdateMode = "mmupdate";
	}
}
