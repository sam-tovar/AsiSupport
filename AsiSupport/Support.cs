﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsiSupport.ASI;
using AsiSupport.Managers;
using PursuitLib;
using PursuitLib.Extensions;
using PursuitLib.IO;
using PursuitLib.RPH;
using Rage;

namespace AsiSupport
{
	public class Support : RPHPlugin
	{
		public static Support Instance { get; private set; }

		public Config Config { get; private set; }
		public int GameVersion { get; private set; } = -1;
		public AsiLoader Loader { get; private set; }
		public TextureManager TextureManager { get; private set; }
		public KeyboardManager KeyboardManager { get; private set; }
		private bool intialized = false;

		public Support()
		{
			Instance = this;
			this.Config = new Config();

			if(!File.Exists(this.ConfigFile))
				this.Config.Save();
		}

		protected override void Tick()
		{
			if(!Game.IsLoading && !this.intialized) //Do not load plugins while the game is loading, it will cause a crash
				this.Initialize();
		}

		private void Initialize()
		{
			Game.FadeScreenOut(0);
			GameFiber.Wait(1000);

			Log.Info("Intializing AsiSupport version " + this.Version + "...");
			Log.Info("Using PursuitLib " + typeof(Log).GetVersion());
			Log.Info("Using PursuitLib.RPH " + typeof(RPHPlugin).GetVersion());

			ResourceManager.RegisterProvider("data", new DirectoryResourceProvider(this.DataDirectory));
			this.EnsureResourcesAvailable();
			if(this.IsStopped)
				return;

			ManifestFile gameVersions = new ManifestFile(Path.Combine(this.DataDirectory, "Versions"));
			string versionStr = Game.ProductVersion.ToString();
			bool isSteamVer = File.Exists("steam_api64.dll");

			for(int i = 0; i < gameVersions.Entries.Count; i++)
			{
				string entry = gameVersions.Entries[i];
				bool isSteam = entry[0] == 's';
				string version = entry.Substring(1);

				if(isSteam == isSteamVer && versionStr == version)
				{
					this.GameVersion = i;
					break;
				}
			}

			VectorHelper.Init();
			this.Loader = new AsiLoader(Path.Combine("", "AsiPlugins"));
			this.TextureManager = new TextureManager();
			this.KeyboardManager = new KeyboardManager();
			AsiInterface.Initialize();

			Log.Info("AsiSupport initialized.");

			if(this.Config.LoadAllPluginsOnStartup)
				this.Loader.LoadAllPlugins();

			Game.FadeScreenIn(1000);
			this.intialized = true;
		}

		public override void Unload(bool canSleep)
		{
			this.Loader?.UnloadAllPlugins();

			Log.Info("Disposing KeyboardManager...");
			this.KeyboardManager?.ReleaseHandle();

			Log.Info("Disposing AsiInterface...");
			AsiInterface.Dispose();

			base.Unload(canSleep);
			Log.Info("AsiSupport unloaded successfully.");
		}
	}
}
