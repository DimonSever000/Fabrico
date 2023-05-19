using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorld;
using Verse;

namespace Toolbox
{
	// Token: 0x0200000E RID: 14
	public static class ModMetaDataCleaner
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600005A RID: 90 RVA: 0x000063B8 File Offset: 0x000045B8
		public static bool Cleaned
		{
			get
			{
				return ModMetaDataCleaner.cacheMetaDataCount >= Enumerable.Count<ModMetaData>(ModLister.AllInstalledMods);
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000063E8 File Offset: 0x000045E8
		public static void CleanModMetaData()
		{
			FieldInfo field = typeof(ModLister).GetField("mods", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			List<ModMetaData> value = (List<ModMetaData>)field.GetValue(null);
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = value.Count - 1; i > -1; i--)
			{
				bool active = value[i].Active;
				if (active)
				{
					stringBuilder2.AppendWithComma(value[i].Name);
					value[i].UnsetPreviewImage();
					num2++;
				}
				else
				{
					stringBuilder.AppendWithComma(value[i].Name);
					value.RemoveAt(i);
					num++;
				}
			}
			Log.Message(string.Concat(new string[]
			{
				"[ModMetaDataCleaner] Removed ",
				num.ToString(),
				" Metadata and cleaned ",
				num2.ToString(),
				" PreviewImage.\nRemoved: ",
				stringBuilder.ToString(),
				"\nCleaned: ",
				stringBuilder2.ToString()
			}));
			bool flag = Current.ProgramState == ProgramState.Playing;
			if (flag)
			{
				Messages.Message("MsgModMetaDataCleaned".Translate(num, num2), MessageTypeDefOf.PositiveEvent, false);
			}
			field.SetValue(null, value);
			ModMetaDataCleaner.cacheMetaDataCount = value.Count;
			object obj = null;
			stringBuilder2 = (StringBuilder)obj;
			stringBuilder = (StringBuilder)obj;
			GC.Collect(2);
		}

		// Token: 0x0400004A RID: 74
		private static int cacheMetaDataCount = -1;
	}
}
