using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RuntimeGC
{
	// Token: 0x0200000A RID: 10
	public class WorldPawnCleaner
	{
		// Token: 0x06000039 RID: 57 RVA: 0x000045B4 File Offset: 0x000027B4
		private void addFlag(Pawn pawn, WorldPawnCleaner.Flags flag)
		{
			bool flag4 = this.allFlags.ContainsKey(pawn);
			if (flag4)
			{
				Dictionary<Pawn, WorldPawnCleaner.Flags> pawns = this.allFlags;
				pawns[pawn] |= flag;
			}
			else
			{
				this.allFlags.Add(pawn, flag);
			}
			bool flag5 = this.verbose;
			if (flag5)
			{
				bool flag6 = this.allPawnsCounter.ContainsKey(pawn);
				if (flag6)
				{
					Dictionary<Pawn, int> pawns2 = this.allPawnsCounter;
					pawns2[pawn]++;
				}
				else
				{
					this.allPawnsCounter.Add(pawn, 1);
				}
				foreach (WorldPawnCleaner.Flags flag2 in this.splitFlag(flag))
				{
					Dictionary<WorldPawnCleaner.Flags, int> flags = this.allFlagsCounter;
					WorldPawnCleaner.Flags flag3 = flag2;
					flags[flag3]++;
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000046B8 File Offset: 0x000028B8
		private bool containsFlag(Pawn pawn, WorldPawnCleaner.Flags flag)
		{
			return this.containsFlag(this.getFlag(pawn), flag);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000046D8 File Offset: 0x000028D8
		private bool containsFlag(WorldPawnCleaner.Flags f1, WorldPawnCleaner.Flags f2)
		{
			return (f1 & f2) > WorldPawnCleaner.Flags.None;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000046F0 File Offset: 0x000028F0
		private void DiagnoseMapPawns(out List<Pawn> mapPawnEntryPoints)
		{
			List<Pawn> pawns = new List<Pawn>();
			foreach (Map map in Find.Maps)
			{
				foreach (Pawn allPawn in map.mapPawns.AllPawns)
				{
					bool isColonist = allPawn.IsColonist;
					if (isColonist)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)129);
					}
					bool isPrisonerOfColony = allPawn.IsPrisonerOfColony;
					if (isPrisonerOfColony)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)130);
					}
					bool flag = PawnUtility.IsFactionLeader(allPawn);
					if (flag)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)72);
					}
					bool flag2 = PawnUtility.IsKidnappedPawn(allPawn);
					if (flag2)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)144);
					}
					bool flag3 = PawnUtility.IsTravelingInTransportPodWorldObject(allPawn);
					if (flag3)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)144);
					}
					bool flag4 = allPawn.Corpse != null;
					if (flag4)
					{
						this.addFlag(allPawn, (WorldPawnCleaner.Flags)68);
					}
					bool humanlike = allPawn.RaceProps.Humanlike;
					if (humanlike)
					{
						this.addFlag(allPawn, WorldPawnCleaner.Flags.RelationLvl1);
					}
					bool flag5 = !this.allFlags.ContainsKey(allPawn);
					if (!flag5)
					{
						pawns.Add(allPawn);
						bool flag6 = !this.verbose;
						if (!flag6)
						{
							Log.Message("[mapPawn] " + allPawn.LabelShort + " [flag] " + this.markedFlagsString(allPawn));
						}
					}
				}
			}
			mapPawnEntryPoints = Enumerable.ToList<Pawn>(pawns);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000048DC File Offset: 0x00002ADC
		public void DisposeTmpForSystemGC()
		{
			this.reference = null;
			this.allPawnsCounter.Clear();
			this.allFlags.Clear();
			this.allFlagsCounter.Clear();
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000490C File Offset: 0x00002B0C
		private void expandRelation(Pawn p, WorldPawnCleaner.Flags flag)
		{
			bool flag2 = p.relations == null;
			if (!flag2)
			{
				bool flag3 = this.debug;
				if (flag3)
				{
					foreach (Pawn familyByBlood in p.relations.FamilyByBlood)
					{
						foreach (PawnRelationDef relation in p.GetRelations(familyByBlood))
						{
							Log.Message(string.Concat(new string[]
							{
								"(Family) ",
								p.LabelShort,
								" <",
								relation.label,
								"> ",
								familyByBlood.LabelShort
							}));
						}
					}
				}
				foreach (DirectPawnRelation directRelation in p.relations.DirectRelations)
				{
					bool flag4 = !this.reference.Contains(directRelation.otherPawn);
					if (!flag4)
					{
						this.addFlag(directRelation.otherPawn, flag);
						bool flag5 = !this.verbose;
						if (!flag5)
						{
							Log.Message(string.Concat(new string[]
							{
								"(Relation) ",
								p.LabelShort,
								" <",
								directRelation.def.label,
								"> ",
								directRelation.otherPawn.LabelShort
							}));
						}
					}
				}
				foreach (Pawn pawnsWithDirectRelationsWithMe in CleanserUtil.getPawnsWithDirectRelationsWithMe(p.relations))
				{
					bool flag6 = !this.reference.Contains(pawnsWithDirectRelationsWithMe) || Enumerable.Count<PawnRelationDef>(pawnsWithDirectRelationsWithMe.GetRelations(p)) <= 0;
					if (!flag6)
					{
						this.addFlag(pawnsWithDirectRelationsWithMe, flag);
						bool flag7 = !this.verbose;
						if (!flag7)
						{
							Log.Message(string.Concat(new string[]
							{
								"(Reflexed) <",
								pawnsWithDirectRelationsWithMe.LabelShort,
								",",
								p.LabelShort,
								">"
							}));
						}
					}
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004BB4 File Offset: 0x00002DB4
		public int GC(bool verbose = false)
		{
			bool flag = Current.ProgramState != ProgramState.Playing;
			int result;
			if (flag)
			{
				Log.Error("You must be kidding me...GC a save without loading one?");
				result = 0;
			}
			else
			{
				Log.Message("[GC Log] Pre-Initializing GC...");
				this.reference = Enumerable.ToList<Pawn>(Find.WorldPawns.AllPawnsAliveOrDead);
				this.allFlags.Clear();
				if (verbose)
				{
					this.allFlagsCounter.Clear();
					this.allFlagsCounter.Add(WorldPawnCleaner.Flags.None, 0);
					for (int i = 0; i < WorldPawnCleaner.FlagsCountNotNull; i++)
					{
						this.allFlagsCounter.Add((WorldPawnCleaner.Flags)(1 << i), 0);
					}
				}
				Log.Message("[GC Log] Generating EntryPoints from Map Pawns...");
				List<Pawn> pawns;
				this.DiagnoseMapPawns(out pawns);
				if (verbose)
				{
					Log.Message("[GC Log][Verbose] " + Enumerable.Count<KeyValuePair<Pawn, int>>(this.allPawnsCounter).ToString() + " Map Pawns marked during diagnosis");
				}
				this.allPawnsCounter.Clear();
				if (verbose)
				{
					this.allFlagsCounter.Clear();
					this.allFlagsCounter.Add(WorldPawnCleaner.Flags.None, 0);
					for (int j = 0; j < WorldPawnCleaner.FlagsCountNotNull; j++)
					{
						this.allFlagsCounter.Add((WorldPawnCleaner.Flags)(1 << j), 0);
					}
				}
				Log.Message("[GC Log] Collecting Pawns concerned by Used Tales...");
				List<Pawn> pawns2;
				CleanserUtil.InitUsedTalePawns(out pawns2);
				Log.Message("[GC Log] Running diagnosis on WorldPawns...");
				foreach (Pawn pawn in this.reference)
				{
					bool flag2 = pawn == null;
					if (flag2)
					{
						Log.Message("Encountered a null pawn.");
					}
					else
					{
						bool isColonist = pawn.IsColonist;
						if (isColonist)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)129);
						}
						bool isPrisonerOfColony = pawn.IsPrisonerOfColony;
						if (isPrisonerOfColony)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)130);
						}
						bool flag3 = PawnUtility.IsFactionLeader(pawn);
						if (flag3)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)88);
						}
						bool flag4 = PawnUtility.IsKidnappedPawn(pawn);
						if (flag4)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)144);
						}
						bool flag5 = pawn.Corpse != null;
						if (flag5)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)68);
						}
						bool flag6 = pawns2.Contains(pawn);
						if (flag6)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)288);
						}
						bool inContainerEnclosed = pawn.InContainerEnclosed;
						if (inContainerEnclosed)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)48);
						}
						bool spawned = pawn.Spawned;
						if (spawned)
						{
							this.addFlag(pawn, WorldPawnCleaner.Flags.RelationLvl0);
						}
						bool flag7 = pawn.IsPlayerControlledCaravanMember();
						if (flag7)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)144);
						}
						bool flag8 = PawnUtility.IsTravelingInTransportPodWorldObject(pawn);
						if (flag8)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)144);
						}
						bool flag9 = PawnUtility.ForSaleBySettlement(pawn);
						if (flag9)
						{
							this.addFlag(pawn, (WorldPawnCleaner.Flags)544);
						}
						bool flag10 = !verbose;
						if (!flag10)
						{
							Log.Message("[worldPawn] " + pawn.LabelShort + " [flag] " + this.markedFlagsString(pawn));
						}
					}
				}
				if (verbose)
				{
					Log.Message("[GC Log][Verbose] " + Enumerable.Count<KeyValuePair<Pawn, int>>(this.allPawnsCounter).ToString() + " World Pawns marked during diagnosis");
				}
				Log.Message("[GC Log] Expanding Relation networks through Map Pawn Entry Points...");
				for (int k = pawns.Count - 1; k > -1; k--)
				{
					bool flag11 = this.containsFlag(pawns[k], WorldPawnCleaner.Flags.RelationLvl2);
					if (flag11)
					{
						this.expandRelation(pawns[k], WorldPawnCleaner.Flags.RelationLvl1);
						pawns.RemoveAt(k);
					}
				}
				for (int k = pawns.Count - 1; k > -1; k--)
				{
					bool flag12 = this.containsFlag(pawns[k], WorldPawnCleaner.Flags.RelationLvl1);
					if (flag12)
					{
						this.expandRelation(pawns[k], WorldPawnCleaner.Flags.RelationLvl0);
						pawns.RemoveAt(k);
					}
				}
				Log.Message("[GC Log] Expanding Relation networks on marked World Pawns...");
				for (int k = this.reference.Count - 1; k > -1; k--)
				{
					bool flag13 = this.containsFlag(this.reference[k], WorldPawnCleaner.Flags.RelationLvl2);
					if (flag13)
					{
						this.expandRelation(this.reference[k], WorldPawnCleaner.Flags.RelationLvl1);
						this.reference.RemoveAt(k);
					}
				}
				for (int k = this.reference.Count - 1; k > -1; k--)
				{
					bool flag14 = this.containsFlag(this.reference[k], WorldPawnCleaner.Flags.RelationLvl1);
					if (flag14)
					{
						this.expandRelation(this.reference[k], WorldPawnCleaner.Flags.RelationLvl0);
						this.reference.RemoveAt(k);
					}
				}
				for (int k = this.reference.Count - 1; k > -1; k--)
				{
					bool flag15 = this.containsFlag(this.reference[k], WorldPawnCleaner.Flags.RelationLvl0);
					if (flag15)
					{
						this.reference.RemoveAt(k);
					}
				}
				int count = 0;
				if (verbose)
				{
					foreach (KeyValuePair<Pawn, int> keyValuePair in this.allPawnsCounter)
					{
						count += keyValuePair.Value;
					}
					Log.Message("[GC Log][Verbose] " + Enumerable.Count<KeyValuePair<Pawn, int>>(this.allPawnsCounter).ToString() + " World Pawns marked during Expanding");
					bool flag16 = this.debug;
					if (flag16)
					{
						Log.Message("addFlag() called " + count.ToString() + " times");
					}
				}
				Log.Message("[GC Log] Excluding Pawns concerned by Used Tales...");
				foreach (Pawn pawn2 in pawns2)
				{
					this.reference.Remove(pawn2);
				}
				List<Thing> list = Enumerable.ToList<Thing>(Enumerable.Distinct<Thing>(Enumerable.Select<GlobalTargetInfo, Thing>(Enumerable.Where<GlobalTargetInfo>(Enumerable.SelectMany<Quest, GlobalTargetInfo>(Enumerable.Where<Quest>(Find.QuestManager.QuestsListForReading, (Quest q) => q.State == QuestState.NotYetAccepted), (Quest q) => q.QuestLookTargets), (GlobalTargetInfo x) => x.Thing != null), (GlobalTargetInfo x) => x.Thing)));
				Log.Message(string.Format("[GC Log] Excluding Quest Pawns: {0}", this.reference.RemoveAll((Pawn p) => list.Contains(p))));
				Log.Message("[GC Log] Disposing World Pawns...");
				count = this.reference.Count;
				for (int k = this.reference.Count - 1; k > -1; k--)
				{
					Pawn item = this.reference[k];
					Find.WorldPawns.RemovePawn(item);
					bool flag17 = !item.Destroyed;
					if (flag17)
					{
						item.Destroy(DestroyMode.Vanish);
					}
					bool flag18 = !item.Discarded;
					if (flag18)
					{
						item.Discard(true);
					}
				}
				if (verbose)
				{
					string str = "[GC Log][Verbose] Flag calls stat:";
					this.allFlagsCounter.Remove(WorldPawnCleaner.Flags.None);
					foreach (KeyValuePair<WorldPawnCleaner.Flags, int> keyValuePair2 in this.allFlagsCounter)
					{
						string[] array = new string[5];
						array[0] = str;
						array[1] = "\n  ";
						string[] strArrays = array;
						strArrays[2] = keyValuePair2.Key.ToString();
						strArrays[3] = " : ";
						strArrays[4] = keyValuePair2.Value.ToString();
						str = string.Concat(strArrays);
					}
					Log.Message(str);
				}
				Log.Message("[GC Log] GC() completed with " + count.ToString() + " World Pawns disposed");
				result = count;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005464 File Offset: 0x00003664
		private WorldPawnCleaner.Flags getFlag(Pawn pawn)
		{
			bool flag = !this.allFlags.ContainsKey(pawn);
			WorldPawnCleaner.Flags result;
			if (flag)
			{
				result = WorldPawnCleaner.Flags.None;
			}
			else
			{
				result = this.allFlags[pawn];
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000549C File Offset: 0x0000369C
		private string markedFlagsString(Pawn p)
		{
			List<WorldPawnCleaner.Flags> list = Enumerable.ToList<WorldPawnCleaner.Flags>(this.splitFlag(this.getFlag(p)));
			string str = "";
			bool flag = list != null;
			if (flag)
			{
				int i;
				for (i = 0; i < list.Count - 2; i++)
				{
					str += list[i].ToString();
					str += "|";
				}
				str += list[i].ToString();
			}
			return str;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00005535 File Offset: 0x00003735
		private IEnumerable<WorldPawnCleaner.Flags> splitFlag(WorldPawnCleaner.Flags flag)
		{
			int f = 1;
			int num;
			for (int i = 0; i < WorldPawnCleaner.FlagsCountNotNull; i = num + 1)
			{
				bool flag2 = this.containsFlag(flag, (WorldPawnCleaner.Flags)(f <<= 1));
				if (flag2)
				{
					yield return (WorldPawnCleaner.Flags)f;
				}
				num = i;
			}
			yield return WorldPawnCleaner.Flags.None;
			yield break;
		}

		// Token: 0x04000039 RID: 57
		private static string CopyrightStr = "RuntimeGC for 1.0,user19990313,Baidu Tieba&Ludeon forum";

		// Token: 0x0400003A RID: 58
		private Dictionary<Pawn, int> allPawnsCounter = new Dictionary<Pawn, int>();

		// Token: 0x0400003B RID: 59
		private Dictionary<WorldPawnCleaner.Flags, int> allFlagsCounter = new Dictionary<WorldPawnCleaner.Flags, int>();

		// Token: 0x0400003C RID: 60
		private bool verbose;

		// Token: 0x0400003D RID: 61
		private bool debug;

		// Token: 0x0400003E RID: 62
		private static int FlagsCountNotNull = Enum.GetNames(typeof(WorldPawnCleaner.Flags)).Length - 1;

		// Token: 0x0400003F RID: 63
		private List<Pawn> reference;

		// Token: 0x04000040 RID: 64
		private Dictionary<Pawn, WorldPawnCleaner.Flags> allFlags = new Dictionary<Pawn, WorldPawnCleaner.Flags>();

		// Token: 0x02000015 RID: 21
		private enum Flags
		{
			// Token: 0x04000064 RID: 100
			None,
			// Token: 0x04000065 RID: 101
			Colonist,
			// Token: 0x04000066 RID: 102
			Prisoner,
			// Token: 0x04000067 RID: 103
			CorpseOwner = 4,
			// Token: 0x04000068 RID: 104
			FactionLeader = 8,
			// Token: 0x04000069 RID: 105
			KeptWorldPawn = 16,
			// Token: 0x0400006A RID: 106
			RelationLvl0 = 32,
			// Token: 0x0400006B RID: 107
			RelationLvl1 = 64,
			// Token: 0x0400006C RID: 108
			RelationLvl2 = 128,
			// Token: 0x0400006D RID: 109
			TaleEntryOwner = 256,
			// Token: 0x0400006E RID: 110
			OnSale = 512,
			// Token: 0x0400006F RID: 111
			Animal = 1024
		}
	}
}
