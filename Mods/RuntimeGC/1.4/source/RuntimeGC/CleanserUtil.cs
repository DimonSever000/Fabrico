using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RuntimeGC
{
	// Token: 0x0200000B RID: 11
	internal static class CleanserUtil
	{
		// Token: 0x06000043 RID: 67 RVA: 0x0000554C File Offset: 0x0000374C
		public static IEnumerable<Pawn> getPawnsWithDirectRelationsWithMe(Pawn_RelationsTracker r)
		{
			return (HashSet<Pawn>)CleanserUtil.field.GetValue(r);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005570 File Offset: 0x00003770
		public static int RemoveAllBattleLogEntries()
		{
			int num2;
			try
			{
				int num = 0;
				foreach (Battle battle in Find.BattleLog.Battles)
				{
					num = battle.Entries.Count;
					bool flag = num <= 0;
					if (flag)
					{
						num = 0;
					}
					else
					{
						CleanserUtil.battles.SetValue(Find.BattleLog, null);
						CleanserUtil.battles.SetValue(Find.BattleLog, new List<Battle>(20));
						CleanserUtil.activeEntries.SetValue(Find.BattleLog, null);
					}
				}
				num2 = num;
			}
			catch (Exception ex)
			{
				string[] array = new string[12];
				array[0] = "RuntimeGC Error Start: ";
				array[1] = Environment.NewLine;
				array[2] = ex.StackTrace;
				array[3] = Environment.NewLine;
				array[4] = ex.Message;
				array[5] = Environment.NewLine;
				array[6] = ex.Source;
				array[7] = Environment.NewLine;
				int num3 = 8;
				IDictionary data = ex.Data;
				array[num3] = ((data != null) ? data.ToString() : null);
				array[9] = Environment.NewLine;
				array[10] = Environment.NewLine;
				array[11] = "RuntimeGC End.";
				Log.Message(string.Concat(array));
				num2 = 0;
			}
			return num2;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000045 RID: 69 RVA: 0x000056E8 File Offset: 0x000038E8
		public static WorldPawnCleaner GCObject
		{
			get
			{
				bool flag = CleanserUtil.gcobject == null;
				if (flag)
				{
					CleanserUtil.gcobject = new WorldPawnCleaner();
				}
				return CleanserUtil.gcobject;
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005718 File Offset: 0x00003918
		public static int DeconstructAnimalFamily()
		{
			List<Pawn> list = new List<Pawn>();
			foreach (Pawn pawn in Find.WorldPawns.AllPawnsAliveOrDead)
			{
				list.Add(pawn);
			}
			List<Pawn> list2 = new List<Pawn>();
			List<Pawn> list3 = new List<Pawn>();
			foreach (Map map in Find.Maps)
			{
				foreach (Pawn pawn2 in map.mapPawns.AllPawns)
				{
					bool flag = pawn2.records.GetAsInt(RecordDefOf.TimeAsColonistOrColonyAnimal) > 0 && !pawn2.RaceProps.Humanlike;
					if (flag)
					{
						list3.Add(pawn2);
					}
				}
			}
			foreach (Pawn pawn3 in list3)
			{
				bool flag2 = pawn3.relations != null;
				if (flag2)
				{
					foreach (Pawn pawn4 in CleanserUtil.expandRelation(pawn3))
					{
						bool flag3 = list.Contains(pawn4) && !pawn4.Spawned && !pawn4.IsPlayerControlledCaravanMember() && !PawnUtility.IsTravelingInTransportPodWorldObject(pawn4) && pawn3.Corpse == null;
						if (flag3)
						{
							list2.Add(pawn4);
							list.Remove(pawn4);
						}
					}
				}
			}
			List<Pawn> list4;
			CleanserUtil.InitUsedTalePawns(out list4);
			foreach (Pawn pawn5 in list4)
			{
				list2.Remove(pawn5);
			}
			int count = list2.Count;
			foreach (Pawn pawn6 in list2)
			{
				Find.WorldPawns.RemovePawn(pawn6);
				bool flag4 = !pawn6.Destroyed;
				if (flag4)
				{
					pawn6.Destroy(DestroyMode.Vanish);
				}
				bool flag5 = !pawn6.Discarded;
				if (flag5)
				{
					pawn6.Discard(true);
				}
			}
			Find.WindowStack.WindowOfType<UserInterface>().Notify_PawnsCountDirty();
			return count;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005A1C File Offset: 0x00003C1C
		private static IEnumerable<Pawn> expandRelation(Pawn p)
		{
			foreach (DirectPawnRelation directPawnRelation in p.relations.DirectRelations)
			{
				yield return directPawnRelation.otherPawn;
			}
			foreach (Pawn pawn in CleanserUtil.getPawnsWithDirectRelationsWithMe(p.relations))
			{
				yield return pawn;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005A2C File Offset: 0x00003C2C
		public static int RemoveFilth(Map map, bool homearea)
		{
			bool flag = map == null || map.listerFilthInHomeArea == null || map.listerThings == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				List<Thing> list;
				if (homearea)
				{
					list = map.listerFilthInHomeArea.FilthInHomeArea;
				}
				else
				{
					list = map.listerThings.ThingsInGroup(ThingRequestGroup.Filth);
				}
				num += list.Count;
				for (int i = list.Count - 1; i > -1; i--)
				{
					Thing bob = list[i];
					Filth filth = (Filth)list[i];
					filth.DeSpawn(DestroyMode.Vanish);
					bool flag2 = !filth.Destroyed;
					if (flag2)
					{
						filth.Destroy(DestroyMode.Vanish);
					}
					bool flag3 = !filth.Discarded;
					if (flag3)
					{
						Log.Warning("A thing_filth_object destroyed before is not discarded!That's wierd.");
						filth.Discard(false);
					}
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00005B18 File Offset: 0x00003D18
		public static void FixFactionRelationships()
		{
			List<Faction> allFactionsListForReading = Find.FactionManager.AllFactionsListForReading;
			foreach (Faction faction in allFactionsListForReading)
			{
				for (int i = 0; i < allFactionsListForReading.Count; i++)
				{
					bool flag = allFactionsListForReading[i] != faction;
					if (flag)
					{
						faction.TryMakeInitialRelationsWith(allFactionsListForReading[i]);
					}
				}
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005BAC File Offset: 0x00003DAC
		public static int FixFactionLeader()
		{
			int num = 0;
			foreach (Faction faction in Find.FactionManager.AllFactionsVisible)
			{
				bool flag = faction.leader == null || faction.leader.Dead || faction.leader.Destroyed;
				if (flag)
				{
					faction.TryGenerateNewLeader();
					num++;
				}
			}
			Find.WindowStack.WindowOfType<UserInterface>().Notify_PawnsCountDirty();
			return num;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005C48 File Offset: 0x00003E48
		public static int FixFactionLeader_Wrapped()
		{
			int num = CleanserUtil.FixFactionLeader();
			int num2 = CleanserUtil.FixFactionLeader();
			return num - num2;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00005C6C File Offset: 0x00003E6C
		public static int RemoveCorpses(bool rottenOnly = false)
		{
			Map currentMap = Find.CurrentMap;
			bool flag = currentMap == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				List<Thing> list = currentMap.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
				int count = list.Count;
				for (int i = list.Count - 1; i > -1; i--)
				{
					Corpse corpse = (Corpse)list[i];
                    if (rottenOnly)
                    {
						CompRottable comp = corpse.GetComp<CompRottable>();
						if (comp != null)
						{
							if (comp.Stage == RotStage.Fresh)
							{
								count--;
								continue;
							}
						}
					}
					corpse.DeSpawn(DestroyMode.Vanish);
					bool flag2 = !corpse.Destroyed;
					if (flag2)
					{
						corpse.Destroy(DestroyMode.Vanish);
					}
					bool flag3 = !corpse.Discarded;
					if (flag3)
					{
						corpse.Discard(false);
					}
				}
				Find.WindowStack.WindowOfType<UserInterface>().Notify_PawnsCountDirty();
				result = count;
			}
			return result;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00005D2C File Offset: 0x00003F2C
		public static void InitUsedTalePawns(out List<Pawn> concernedPawns)
		{
			List<Tale> list = Find.TaleManager.AllTalesListForReading.FindAll((Tale t) => t.Uses > 0 || t.def.type == TaleType.PermanentHistorical);
			List<Pawn> list2 = new List<Pawn>();
			int count = list.Count;
			int i = 0;
			foreach (Pawn pawn in Find.WorldPawns.AllPawnsAliveOrDead)
			{
				try
				{
					for (i = 0; i < count; i++)
					{
						bool flag = list[i].Concerns(pawn);
						if (flag)
						{
							list2.Add(pawn);
							break;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error("Exception in InitUsedTalePawns with Tale id=" + list[i].id.ToString() + " :\n" + ex.ToString());
					list2.Add(pawn);
				}
			}
			concernedPawns = list2;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005E48 File Offset: 0x00004048
		public static void RemoveSnow(Map map, bool homearea)
		{
			bool flag = map == null || map.snowGrid == null || map.mapDrawer == null;
			if (!flag)
			{
				if (homearea)
				{
					SnowGrid snowGrid = map.snowGrid;
					using (IEnumerator<IntVec3> enumerator = map.areaManager.Home.ActiveCells.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IntVec3 intVec = enumerator.Current;
							snowGrid.SetDepth(intVec, 0f);
						}
						return;
					}
				}
				map.snowGrid = new SnowGrid(map);
				map.mapDrawer.WholeMapChanged(MapMeshFlag.Snow);
				map.mapDrawer.WholeMapChanged(MapMeshFlag.Things);
				map.pathing.RecalculateAllPerceivedPathCosts();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005F14 File Offset: 0x00004114
		public static int RemoveIArchivable(bool removePinned)
		{
			bool flag = Find.Archive == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				List<IArchivable> list = new List<IArchivable>();
				bool flag2 = !removePinned;
				if (flag2)
				{
					list.AddRange((HashSet<IArchivable>)CleanserUtil.pinnedArchivables.GetValue(Find.Archive));
					list.SortBy((IArchivable iA) => iA.CreatedTicksGame);
				}
				int num = ((List<IArchivable>)CleanserUtil.archivables.GetValue(Find.Archive)).CountAllowNull<IArchivable>() - list.Count;
				CleanserUtil.archivables.SetValue(Find.Archive, list);
				if (removePinned)
				{
					CleanserUtil.pinnedArchivables.SetValue(Find.Archive, new HashSet<IArchivable>());
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00005FDE File Offset: 0x000041DE
		public static void UnlockNormalSpeedLimit()
		{
			CleanserUtil.forceNormalSpeedUntil.SetValue(Find.TickManager.slower, Find.TickManager.TicksGame);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00006008 File Offset: 0x00004208
		public static void OpenModSettingsPage()
		{
			Mod mod = LoadedModManager.ModHandles.FirstOrFallback((Mod m) => m is RuntimeGC, null);
			Dialog_ModSettings modSettingDialog = new Dialog_ModSettings(mod);
			Find.WindowStack.Add(modSettingDialog);
		}

		// Token: 0x04000041 RID: 65
		private static FieldInfo field = typeof(Pawn_RelationsTracker).GetField("pawnsWithDirectRelationsWithMe", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000042 RID: 66
		private static FieldInfo battles = typeof(BattleLog).GetField("battles", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000043 RID: 67
		private static FieldInfo activeEntries = typeof(BattleLog).GetField("cachedActiveEntries", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000044 RID: 68
		private static FieldInfo archivables = typeof(Archive).GetField("archivables", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000045 RID: 69
		private static FieldInfo pinnedArchivables = typeof(Archive).GetField("pinnedArchivables", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000046 RID: 70
		private static FieldInfo forceNormalSpeedUntil = typeof(TimeSlower).GetField("forceNormalSpeedUntil", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000047 RID: 71
		private static FieldInfo selMod = typeof(Dialog_ModSettings).GetField("selMod", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000048 RID: 72
		private static WorldPawnCleaner gcobject = new WorldPawnCleaner();
	}
}
