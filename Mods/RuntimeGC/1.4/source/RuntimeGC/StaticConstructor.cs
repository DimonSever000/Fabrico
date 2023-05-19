using System;
using UnityEngine;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000006 RID: 6
	[StaticConstructorOnStartup]
	internal class StaticConstructor
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002B00 File Offset: 0x00000D00
		static StaticConstructor()
		{
			bool flag = GameObject.Find("RuntimeGCInstance") == null;
			if (flag)
			{
				UnityEngine.Object.DontDestroyOnLoad(new GameObject("RuntimeGCInstance"));
			}
			else
			{
				Log.Warning("[RuntimeGC] More than one RuntimeGC instance is running!");
			}
			MainButtonWorker_RuntimeGC.TabDescriptionTranslated = "MainTabWindowDescription".Translate();
			MainButtonWorker_RuntimeGC.MMTipTranslated = "MMTip".Translate();
			UIUtil.Notify_MMBtnLabelChanged();
		}
	}
}
