using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RuntimeGC
{
    // Token: 0x02000009 RID: 9
    public class UserInterface : MainTabWindow
    {
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00003BF4 File Offset: 0x00001DF4
        public int PawnsAliveCount
        {
            get
            {
                bool flag = this.pawnsCountDirty;
                if (flag)
                {
                    this.pawnsCountDirty = false;
                    this.pawnsAliveCount = Enumerable.Count<Pawn>(Find.WorldPawns.AllPawnsAlive);
                    this.pawnsDeadCount = Enumerable.Count<Pawn>(Find.WorldPawns.AllPawnsDead);
                }
                return this.pawnsAliveCount;
            }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000029 RID: 41 RVA: 0x00003C4C File Offset: 0x00001E4C
        public int PawnsDeadCount
        {
            get
            {
                bool flag = this.pawnsCountDirty;
                if (flag)
                {
                    this.pawnsCountDirty = false;
                    this.pawnsAliveCount = Enumerable.Count<Pawn>(Find.WorldPawns.AllPawnsAlive);
                    this.pawnsDeadCount = Enumerable.Count<Pawn>(Find.WorldPawns.AllPawnsDead);
                }
                return this.pawnsDeadCount;
            }
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600002A RID: 42 RVA: 0x00003CA4 File Offset: 0x00001EA4
        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(350f, 500f);
            }
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00003CD8 File Offset: 0x00001ED8
        private List<ListableOption> returnFloatMenuOptions()
        {
            List<ListableOption> finalMenuOptions = new List<ListableOption>();
            List<ListableOption> list = new List<ListableOption>();
            list.Add(new ListableOption("BtnTextGCWP".Translate(), delegate ()
            {
                this.GC_wrapped(false);
            }, null));
            list.Add(new ListableOption("BtnTextAddOn".Translate(), delegate ()
            {
                FloatMenuUtil.GenerateFloatMenuGroup(FloatMenuUtil.GroupTools);
            }, null));
            list.Add(new ListableOption("BtnTextFix".Translate(), delegate ()
            {
                FloatMenuUtil.GenerateFloatMenuGroup(FloatMenuUtil.GroupFix);
            }, null));
            list.Add(new ListableOption("BtnTextQB".Translate(), delegate ()
            {
                FloatMenuUtil.GenerateFloatMenuGroup(FloatMenuUtil.GroupQuickbar);
            }, null));
            list.Add(new ListableOption("BtnTextSysGC".Translate(), delegate ()
            {
                bool shift = Event.current.shift;
                if (shift)
                {
                    FloatMenuUtil.GenerateMemoryReclaimOptions();
                }
                CleanserUtil.GCObject.DisposeTmpForSystemGC();
                GC.Collect();
                Messages.Message("MsgTextSysGC".Translate(), MessageTypeDefOf.PositiveEvent, true);
            }, null));
            List<ListableOption> devOption = list;
            finalMenuOptions = devOption;
            return finalMenuOptions;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00003F68 File Offset: 0x00002168
        private List<ListableOption> returnFloatMenuHelp()
        {
            List<ListableOption> finalMenuHelpOptions = new List<ListableOption>();
            List<ListableOption> list = new List<ListableOption>();
            list.Add(new ListableOption("?", delegate ()
            {
                Find.WindowStack.Add(new Dialog_MessageBox("HelpTextGCWP".Translate(), null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }, null));
            list.Add(new ListableOption("?", delegate ()
            {
                Find.WindowStack.Add(new Dialog_MessageBox("HelpTextAddOn".Translate(), null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }, null));
            list.Add(new ListableOption("?", delegate ()
            {
                Find.WindowStack.Add(new Dialog_MessageBox("HelpTextFix".Translate(), null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }, null));
            list.Add(new ListableOption("?", delegate ()
            {
                Find.WindowStack.Add(new Dialog_MessageBox("HelpTextQB".Translate(), null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }, null));
            list.Add(new ListableOption("?", delegate ()
            {
                Find.WindowStack.Add(new Dialog_MessageBox("HelpTextSysGC".Translate(), null, null, null, null, null, false, null, null, WindowLayer.Dialog));
            }, null));
            List<ListableOption> devOptions = list;
            finalMenuHelpOptions = devOptions;
            return finalMenuHelpOptions;
        }

        // Token: 0x0600002E RID: 46 RVA: 0x000041C4 File Offset: 0x000023C4
        public void DoComponents(Rect rect)
        {
            GUI.BeginGroup(rect);
            Rect componentAreaContainer = default(Rect);
            bool devMode = Prefs.DevMode;
            if (devMode)
            {
                componentAreaContainer = new Rect(0f, 0f, 130f, rect.height);
            }
            else
            {
                componentAreaContainer = new Rect(0f, 0f, 160f, rect.height);
            }
            Text.Font = GameFont.Small;
            OptionListingUtility.DrawOptionListing(componentAreaContainer, this.returnFloatMenuOptions());
            GUI.EndGroup();
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00004244 File Offset: 0x00002444
        public void DoHelpContents(Rect rect)
        {
            GUI.BeginGroup(rect);
            Rect rect2 = new Rect(0f, 0f, 35f, rect.height);
            Text.Font = GameFont.Small;
            OptionListingUtility.DrawOptionListing(rect2, this.returnFloatMenuHelp());
            GUI.EndGroup();
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00004290 File Offset: 0x00002490
        public override void DoWindowContents(Rect canvas)
        {
            canvas.height = 450f;
            GUI.BeginGroup(canvas);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(canvas);
            Text.Font = GameFont.Medium;
            listingStandard.Label("RuntimeGCTitle".Translate(), -1f, null);
            Text.Font = GameFont.Small;
            listingStandard.Label("RuntimeGCVer".Translate("1.4"), -1f, null);
            listingStandard.Label("By user19990313 - Updated by Trinity", -1f, null);
            listingStandard.Gap(12f);
            listingStandard.Label(string.Concat(new object[]
            {
                "Pawns Alive:",
                this.PawnsAliveCount,
                " Pawns Dead:",
                this.PawnsDeadCount
            }), -1f, null);
            listingStandard.Gap(12f);
            float curHeight = listingStandard.CurHeight;
            listingStandard.End();
            this.DoComponents(new Rect(canvas.x + 35f, canvas.y + curHeight, canvas.width, canvas.height - curHeight));
            this.DoHelpContents(new Rect(canvas.x, canvas.y + curHeight, canvas.width, canvas.height - curHeight));
            GUI.EndGroup();
        }

        // Token: 0x06000031 RID: 49 RVA: 0x000043E8 File Offset: 0x000025E8
        public void GC_wrapped(bool verbose = false)
        {
            int a = PawnsAliveCount;
            int b = PawnsDeadCount;
            int i = CleanserUtil.GCObject.GC(verbose);
            Notify_PawnsCountDirty();
            int j = a + b - PawnsAliveCount - PawnsDeadCount;

            string str = "DlgTextGC".Translate(a, PawnsAliveCount,
                                               b, PawnsDeadCount,
                                               j);
            Find.WindowStack.Add(new Dialog_MessageBox(str + "\n\n" + (i == j ?
                                                                "DlgTextGCAdvice1".Translate() : "DlgTextGCAdvice2".Translate(i - j))
                                                               + (verbose ? "\n\n" + "DlgTextGCV".Translate() : "")
            ));
            if (RuntimeGC.Settings.ArchiveGCDialog)
                Find.Archive.Add(new ArchivedDialog(str, "DlgArchiveTitle".Translate(), null));
        }

        // Token: 0x06000032 RID: 50 RVA: 0x0000452B File Offset: 0x0000272B
        public void Notify_PawnsCountDirty()
        {
            this.pawnsCountDirty = true;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00004535 File Offset: 0x00002735
        public override void PreOpen()
        {
            base.PreOpen();
            this.forcePause = true;
            this.Notify_PawnsCountDirty();
        }

        // Token: 0x04000036 RID: 54
        private int pawnsAliveCount;

        // Token: 0x04000037 RID: 55
        private int pawnsDeadCount;

        // Token: 0x04000038 RID: 56
        private bool pawnsCountDirty = true;
    }
}
