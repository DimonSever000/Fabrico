﻿using System;
using System.Globalization;
using System.IO;
using RimWorld;
using RocketMan;
using Verse;

namespace Gagarin
{
    public class GagarinSettings : IExposable
    {
        private const string FMT = "yyyy-MM-dd HH:mm:ss.fffffff";

        private string creationDateInt = null;

        private string gameBuild;

        public GagarinSettings()
        {
        }

        public void ExposeData()
        {
            Log.Message($"b.{VersionControl.CurrentBuild}:{VersionControl.CurrentBuildDate}:{VersionControl.CurrentVersionStringWithRev}");
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                gameBuild = $"{VersionControl.CurrentBuild}:{VersionControl.CurrentBuildDate}:{VersionControl.CurrentVersionStringWithRev}";
            }
            Scribe_Values.Look(ref gameBuild, "gameBuild", null);            
            if (Scribe.mode != LoadSaveMode.Saving)
            {
                string curGameBuild = $"{VersionControl.CurrentBuild}:{VersionControl.CurrentBuildDate}:{VersionControl.CurrentVersionStringWithRev}";
                if (gameBuild == null || curGameBuild != gameBuild)
                {
                    Log.Warning($"GAGARIN: Game build changed {gameBuild} vs {curGameBuild} clearing cache");
                    Context.IsUsingCache = false;
                    gameBuild = curGameBuild;
                }
            }
            Scribe_Values.Look(ref GagarinPrefs.Enabled, "Enabled2", true);
            Scribe_Values.Look(ref GagarinPrefs.TextureCachingEnabled, "TextureCachingEnabled", false);
            Scribe_Values.Look(ref GagarinPrefs.FilterMode, "FilterMode", (int)UnityEngine.FilterMode.Trilinear);
            Scribe_Values.Look(ref GagarinPrefs.MipMapBias, "MipMapBias", float.MinValue);

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.creationDateInt = GagarinPrefs.CacheCreationTime.ToString(FMT);
            }
            Scribe_Values.Look(ref this.creationDateInt, "creationTime", DateTime.Now.ToString(FMT));            
            if (Scribe.mode != LoadSaveMode.Saving && this.creationDateInt != null)
            {
                GagarinPrefs.CacheCreationTime = DateTime.ParseExact(this.creationDateInt, FMT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
        }        

        public static void LoadSettings()
        {
            bool settingsFound = false;
            try
            {
                if (File.Exists(GagarinEnvironmentInfo.GagarinSettingsFilePath))
                {
                    Scribe.loader.InitLoading(GagarinEnvironmentInfo.GagarinSettingsFilePath);
                    try
                    {
                        Scribe_Deep.Look(ref Context.Settings, "ModSettings");
                        settingsFound = Context.Settings != null;
                        if (Context.Settings == null)
                            Context.Settings = new GagarinSettings();
                    }
                    catch (Exception er)
                    {
                        Log.Error($"GAGARIN: Error while scribing settings {er}");
                        Logger.Debug("Error while scribing settings", exception: er);
                    }
                    finally
                    {
                        Scribe.loader.FinalizeLoading();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"GAGARIN: Caught exception while loading mod settings data for {GagarinEnvironmentInfo.CacheFolderPath}. Generating fresh settings. The exception was: {ex.ToString()}");
                Context.Settings = null;
            }
            if (Context.Settings == null)
            {
                Context.Settings = new GagarinSettings();
            }
            WriteSettings();
            //if (!settingsFound)
            //{
            //    WriteSettings();
            //}
        }

        public static void WriteSettings()
        {
            if (!Directory.Exists(GagarinEnvironmentInfo.CacheFolderPath))
            {
                Directory.CreateDirectory(GagarinEnvironmentInfo.CacheFolderPath);
            }
            if (!Directory.Exists(GagarinEnvironmentInfo.TexturesFolderPath))
            {
                Directory.CreateDirectory(GagarinEnvironmentInfo.TexturesFolderPath);
            }
            if (File.Exists(GagarinEnvironmentInfo.GagarinSettingsFilePath))
            {
                File.Delete(GagarinEnvironmentInfo.GagarinSettingsFilePath);
            }
            Scribe.saver.InitSaving(GagarinEnvironmentInfo.GagarinSettingsFilePath, "SettingsBlock");
            try
            {
                Scribe_Deep.Look(ref Context.Settings, "ModSettings");
            }
            catch (Exception er)
            {
                Log.Error($"GAGARIN: Error while scribing settings {er}");
                Logger.Debug("Error while scribing settings", exception: er);
            }
            finally
            {
                Scribe.saver.FinalizeSaving();
            }
        }
    }
}
