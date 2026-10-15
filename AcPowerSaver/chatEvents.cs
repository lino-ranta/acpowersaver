using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Text.RegularExpressions;

namespace AcPowerSaver
{
    public partial class FilterCore
    {
        private static Regex cmdRegex = new Regex(@"^/powersaver (?<speed>[0-9])$");

        private bool chatEventsInitialized = false;

        private int MessageColor = 5;
        private void initChatEvents()
        {
            if (!chatEventsInitialized && Core.CharacterFilter != null)
            {
                Core.CharacterFilter.Login += new EventHandler<LoginEventArgs>(CharacterFilter_Login);
                Core.CharacterFilter.LoginComplete += new EventHandler(CharacterFilter_LoginComplete);
                Core.CharacterFilter.Logoff += new EventHandler<LogoffEventArgs>(CharacterFilter_Logoff);

                // Initialize the outgoing chat/command message event handler
                Core.CommandLineText += new EventHandler<Decal.Adapter.ChatParserInterceptEventArgs>(Core_CommandLineText);
                chatEventsInitialized = true;
            }
        }

        void CharacterFilter_Login(object sender, LoginEventArgs e)
        {
            initSettings();
        }

        void CharacterFilter_LoginComplete(object sender, EventArgs e)
        {
            initSettings(true);
        }

        void CharacterFilter_Logoff(object sender, LogoffEventArgs e)
        {
            initSettings();
        }

        void Core_CommandLineText(object sender, Decal.Adapter.ChatParserInterceptEventArgs e)
        {
            try
            {
                if (cmdRegex.IsMatch(e.Text) && pluginSettings != null)
                {
                    e.Eat = true;
                    pluginSettings.sleep = Int32.Parse(cmdRegex.Match(e.Text).Result("${speed}").Trim());
                    saveSettings();
                    initSettings(loggedIn);
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }

        void Core_ChatBoxMessage(object sender, Decal.Adapter.ChatTextInterceptEventArgs e)
        {
            //TODO: incoming chat handling code
        }
        private void destroyChatEvents()
        {
            if (chatEventsInitialized)
            {
                Core.CommandLineText -= new EventHandler<Decal.Adapter.ChatParserInterceptEventArgs>(Core_CommandLineText);

                if (Core.CharacterFilter != null)
                {
                    Core.CharacterFilter.Login -= new EventHandler<LoginEventArgs>(CharacterFilter_Login);
                    Core.CharacterFilter.LoginComplete -= new EventHandler(CharacterFilter_LoginComplete);
                    Core.CharacterFilter.Logoff -= new EventHandler<LogoffEventArgs>(CharacterFilter_Logoff);
                }
            }
        }

        private void WriteToChat(string message)
        {
            try
            {
                Host.Actions.AddChatText(string.Format("[{0}] {1}", PLUGIN, message), MessageColor, 1);
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
    }
}