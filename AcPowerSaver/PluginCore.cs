/* Plugin template by Timo 'lino' Kissing <http://ac.ranta.info/DecalDev> */
/* Original template by Lonewolf <http://www.the-lonewolf.com> */

using Decal.Adapter;
using Decal.Interop.Core;
using System;
using System.IO;

namespace AcPowerSaver
{
    public partial class FilterCore : FilterBase
    {

        private static string DIR_SEP = "\\";
        private static string PLUGIN = "AcPowerSaver";

        private static string FILENAME_ERRORLOG = "errorlog.txt";
        private static string FILENAME_SETTINGS = "settings.xml";

        private static int LOGINSCREEN_SLEEP = 20;

        private bool renderHookActive = false;

        public string settingsFolder
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + DIR_SEP + "Decal Plugins" + DIR_SEP + PLUGIN;
            }
        }

        public string settingsFile { get { return settingsFolder + DIR_SEP + FILENAME_SETTINGS; } }

        public string errorLogFile { get { return settingsFolder + DIR_SEP + FILENAME_ERRORLOG; } }

        protected override void Startup()
        {
            try
            {
                initPath();

                Core.PluginInitComplete += new EventHandler<EventArgs>(Core_PluginInitComplete);

                loadSettings();
                initSettings();
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }

        void Core_PluginInitComplete(object sender, EventArgs e)
        {
            try
            {
                initChatEvents();
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }        

        void Hooks_RenderPreUI()
        {
            if (pluginSettings.sleep > 0)
            {
                System.Threading.Thread.Sleep(loggedIn ? pluginSettings.sleep : LOGINSCREEN_SLEEP);
            }
            else
            {
                destroyRenderPreUI();
            }
        }

        protected bool loggedIn { get { return Core.CharacterFilter != null && Core.CharacterFilter.Id != 0; } }

        protected void initRenderPreUI()
        {
            if (pluginSettings != null && !renderHookActive)
            {
                Host.Underlying.Hooks.RenderPreUI += new IACHooksEvents_RenderPreUIEventHandler(Hooks_RenderPreUI);
                renderHookActive = true;
            }
        }

        protected void destroyRenderPreUI()
        {
            if (renderHookActive)
            {
                Host.Underlying.Hooks.RenderPreUI -= new IACHooksEvents_RenderPreUIEventHandler(Hooks_RenderPreUI);
                renderHookActive = false;
            }
        }

        protected override void Shutdown()
        {
            try
            {
                destroyRenderPreUI();
                destroyChatEvents();
                Core.PluginInitComplete -= new EventHandler<EventArgs>(Core_PluginInitComplete);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }

        protected void initPath()
        {
            if (!Directory.Exists(settingsFolder))
            {
                try
                {
                    Directory.CreateDirectory(settingsFolder);
                }
                catch (Exception ex)
                {
                    ErrorLogging.LogError("c:\\AcPowerSaver-errorlog.txt", ex);
                }

            }
        }

    }
}