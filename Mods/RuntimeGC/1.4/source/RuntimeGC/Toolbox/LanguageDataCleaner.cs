using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;

namespace Toolbox
{
	// Token: 0x02000010 RID: 16
	public static class LanguageDataCleaner
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600005E RID: 94 RVA: 0x000065E4 File Offset: 0x000047E4
		public static bool Cleaned
		{
			get
			{
				return LanguageDataCleaner.cacheLanguageDataCount >= Enumerable.Count<LoadedLanguage>(LanguageDatabase.AllLoadedLanguages);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00006614 File Offset: 0x00004814
		public static void CleanLanguageData()
		{
			FieldInfo field = typeof(LanguageDatabase).GetField("languages", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			List<LoadedLanguage> value = (List<LoadedLanguage>)field.GetValue(null);
			int num = 0;
			int count = 0;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = value.Count - 1; i > -1; i--)
			{
				bool flag = value[i] == LanguageDatabase.activeLanguage || value[i] == LanguageDatabase.defaultLanguage;
				if (flag)
				{
					count += value[i].defInjections.Count;
					value[i].defInjections = new List<DefInjectionPackage>();
				}
				else
				{
					stringBuilder.AppendWithComma(value[i].FriendlyNameNative);
					value.RemoveAt(i);
					num++;
				}
			}
			Log.Message(string.Concat(new string[]
			{
				"[LanguageDataCleaner] Removed ",
				num.ToString(),
				" LoadedLanguages and cleaned ",
				count.ToString(),
				" DefInjectionPackages.\nRemoved Languages: ",
				stringBuilder.ToString()
			}));
			bool flag2 = Current.ProgramState == ProgramState.Playing;
			if (flag2)
			{
				Messages.Message("MsgLanguageDataCleaned".Translate(num, count), MessageTypeDefOf.PositiveEvent, false);
			}
			field.SetValue(null, value);
			LanguageDataCleaner.cacheLanguageDataCount = value.Count;
			GC.Collect(2);
		}

		// Token: 0x0400004B RID: 75
		private static int cacheLanguageDataCount = -1;
	}
}
