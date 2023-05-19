using System;
using Verse;

namespace RuntimeGC
{
	// Token: 0x02000004 RID: 4
	public class RuntimeGCSettings : ModSettings
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002644 File Offset: 0x00000844
		// (set) Token: 0x0600000C RID: 12 RVA: 0x0000265C File Offset: 0x0000085C
		public bool EnableMemoryMonitorBar
		{
			get
			{
				return this.EnableMemoryUsageBar;
			}
			set
			{
				bool flag = this.EnableMemoryUsageBar != value;
				if (flag)
				{
					this.EnableMemoryUsageBar = value;
					this.UpdateCache();
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000268C File Offset: 0x0000088C
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000026A4 File Offset: 0x000008A4
		public bool EnableMemoryMonitorTip
		{
			get
			{
				return this.EnableMemoryUsageTip;
			}
			set
			{
				bool flag = this.EnableMemoryUsageTip != value;
				if (flag)
				{
					this.EnableMemoryUsageTip = value;
					this.UpdateCache();
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000026D4 File Offset: 0x000008D4
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000026EC File Offset: 0x000008EC
		public int MemoryMonitorBarLowerBoundMb
		{
			get
			{
				return this.MemoryUsageBarLowerBoundMb;
			}
			set
			{
				bool flag = this.MemoryUsageBarLowerBoundMb != value;
				if (flag)
				{
					this.MemoryUsageBarLowerBoundMb = value;
					this.UpdateCache();
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000011 RID: 17 RVA: 0x0000271C File Offset: 0x0000091C
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002734 File Offset: 0x00000934
		public int MemoryMonitorBarUpperBoundMb
		{
			get
			{
				return this.MemoryUsageBarUpperBoundMb;
			}
			set
			{
				bool flag = this.MemoryUsageBarUpperBoundMb != value;
				if (flag)
				{
					this.MemoryUsageBarUpperBoundMb = value;
					this.UpdateCache();
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002764 File Offset: 0x00000964
		// (set) Token: 0x06000014 RID: 20 RVA: 0x0000277C File Offset: 0x0000097C
		public int MemoryMonitorUpdateInterval
		{
			get
			{
				return this.MemoryUsageUpdateInterval;
			}
			set
			{
				bool flag = this.MemoryUsageUpdateInterval != value;
				if (flag)
				{
					this.MemoryUsageUpdateInterval = value;
					MainButtonWorker_RuntimeGC.Notify_UpdateIntervalChanged(value);
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000027AC File Offset: 0x000009AC
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000027C3 File Offset: 0x000009C3
		public new Mod Mod
		{
			get
			{
				return LoadedModManager.GetMod<RuntimeGC>();
			}
			set
			{
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000027C6 File Offset: 0x000009C6
		public RuntimeGCSettings()
		{
			this.Init();
			this.UpdateCache();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027E0 File Offset: 0x000009E0
		public override void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.EnableMemoryUsageBar, "EnableMemoryUsageBar", true, false);
			Scribe_Values.Look<int>(ref this.MemoryUsageBarLowerBoundMb, "MemoryUsageBarLowerBoundMb", 0, false);
			Scribe_Values.Look<int>(ref this.MemoryUsageBarUpperBoundMb, "MemoryUsageBarUpperBoundMb", 1024 * ((IntPtr.Size == 4) ? 1 : 2), false);
			Scribe_Values.Look<int>(ref this.MemoryUsageUpdateInterval, "MemoryUsageUpdateInterval", 600, false);
			Scribe_Values.Look<bool>(ref this.EnableMemoryUsageTip, "EnableMemoryUsageTip", true, false);
			Scribe_Values.Look<bool>(ref this.AutoCleanModMetaData, "AutoCleanModMetaData", true, false);
			Scribe_Values.Look<bool>(ref this.AutoCleanLanguageData, "AutoCleanLanguageData", false, false);
			Scribe_Values.Look<bool>(ref this.AutoCleanDefPackage, "AutoCleanDefPackage", false, false);
			Scribe_Values.Look<bool>(ref this.DoMuteGC, "DoMuteGC", true, false);
			Scribe_Values.Look<bool>(ref this.DoMuteBL, "DoMuteBL", false, false);
			Scribe_Values.Look<bool>(ref this.ArchiveGCDialog, "ArchiveGCDialog", true, false);
			Scribe_Values.Look<bool>(ref this.ArchiveMessageGeneral, "ArchiveMessageGeneral", false, false);
			Scribe_Values.Look<bool>(ref this.DevOnScreenMemoryUsage, "DevOnScreenMemoryUsage", false, false);
			bool flag = Scribe.mode == LoadSaveMode.LoadingVars;
			if (flag)
			{
				bool flag2 = this.MemoryUsageBarLowerBoundMb < 0;
				if (flag2)
				{
					this.MemoryUsageBarLowerBoundMb = 0;
				}
				bool flag3 = this.MemoryUsageBarUpperBoundMb > 1024 * ((IntPtr.Size == 4) ? 4 : 128);
				if (flag3)
				{
					this.MemoryUsageBarUpperBoundMb = 1024 * ((IntPtr.Size == 4) ? 4 : 128);
				}
				bool flag4 = this.MemoryUsageBarUpperBoundMb <= this.MemoryUsageBarLowerBoundMb;
				if (flag4)
				{
					this.MemoryUsageBarLowerBoundMb = 0;
					this.MemoryUsageBarUpperBoundMb = 1024 * ((IntPtr.Size == 4) ? 1 : 2);
				}
				this.UpdateCache();
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000029A0 File Offset: 0x00000BA0
		public void Init()
		{
			this.InitMemoryMonitor();
			this.AutoCleanModMetaData = true;
			this.AutoCleanLanguageData = false;
			this.AutoCleanDefPackage = false;
			this.DoMuteGC = true;
			this.DoMuteBL = false;
			this.ArchiveGCDialog = true;
			this.ArchiveMessageGeneral = false;
			this.DevOnScreenMemoryUsage = false;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000029ED File Offset: 0x00000BED
		public void InitMemoryMonitor()
		{
			this.EnableMemoryUsageBar = true;
			this.MemoryUsageBarLowerBoundMb = 0;
			this.MemoryUsageBarUpperBoundMb = 1024 * ((IntPtr.Size == 4) ? 1 : 2);
			this.MemoryUsageUpdateInterval = 600;
			this.EnableMemoryUsageTip = true;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002A28 File Offset: 0x00000C28
		public bool RequiresRestart()
		{
			return this.restartFlags != 0;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002A44 File Offset: 0x00000C44
		public void ResetToDefault()
		{
			bool flag = !this.AutoCleanModMetaData;
			if (flag)
			{
				this.restartFlags ^= 1;
			}
			bool flag2 = !this.AutoCleanLanguageData;
			if (flag2)
			{
				this.restartFlags ^= 2;
			}
			bool autoCleanDefPackage = this.AutoCleanDefPackage;
			if (autoCleanDefPackage)
			{
				this.restartFlags ^= 4;
			}
			bool flag3 = !this.DoMuteGC;
			if (flag3)
			{
				this.restartFlags ^= 8;
			}
			bool doMuteBL = this.DoMuteBL;
			if (doMuteBL)
			{
				this.restartFlags ^= 16;
			}
			this.Init();
			this.UpdateCache();
			UIUtil.Notify_MMBtnLabelChanged();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002AF4 File Offset: 0x00000CF4
		public void UpdateCache()
		{
			MainButtonWorker_RuntimeGC.UpdateSettings(this);
		}

		// Token: 0x04000014 RID: 20
		private bool EnableMemoryUsageBar;

		// Token: 0x04000015 RID: 21
		private int MemoryUsageBarLowerBoundMb;

		// Token: 0x04000016 RID: 22
		private int MemoryUsageBarUpperBoundMb;

		// Token: 0x04000017 RID: 23
		private int MemoryUsageUpdateInterval;

		// Token: 0x04000018 RID: 24
		private bool EnableMemoryUsageTip;

		// Token: 0x04000019 RID: 25
		public bool AutoCleanModMetaData;

		// Token: 0x0400001A RID: 26
		public bool AutoCleanLanguageData;

		// Token: 0x0400001B RID: 27
		public bool AutoCleanDefPackage;

		// Token: 0x0400001C RID: 28
		public bool DoMuteGC;

		// Token: 0x0400001D RID: 29
		public bool DoMuteBL;

		// Token: 0x0400001E RID: 30
		public bool ArchiveGCDialog;

		// Token: 0x0400001F RID: 31
		public bool ArchiveMessageGeneral;

		// Token: 0x04000020 RID: 32
		public bool DevOnScreenMemoryUsage;

		// Token: 0x04000021 RID: 33
		internal int restartFlags;
	}
}
