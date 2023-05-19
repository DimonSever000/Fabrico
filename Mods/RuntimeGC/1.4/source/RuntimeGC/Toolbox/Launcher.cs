using System;
using Verse;

namespace Toolbox
{
	// Token: 0x0200000F RID: 15
	public static class Launcher
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00006570 File Offset: 0x00004770
		public static void Launch(bool modMetaData, bool languageData, bool defPackage)
		{
			if (modMetaData)
			{
				LongEventHandler.QueueLongEvent(new Action(ModMetaDataCleaner.CleanModMetaData), "Reclaiming Memory", false, null, true);
			}
			if (languageData)
			{
				LongEventHandler.QueueLongEvent(new Action(LanguageDataCleaner.CleanLanguageData), "Reclaiming Memory", false, null, true);
			}
			if (defPackage)
			{
				LongEventHandler.QueueLongEvent(new Action(DefPackageCleaner.CleanDefPackage), "Reclaiming Memory", false, null, true);
			}
		}
	}
}
