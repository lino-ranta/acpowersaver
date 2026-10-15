/* Plugin created by lino 2010 */

using System;
using System.IO;
using System.Xml.Serialization;

namespace AcPowerSaver
{
    public partial class FilterCore
    {
        public PluginSettings pluginSettings;

        private void loadSettings()
        {
            pluginSettings = PluginSettings.load(settingsFile, errorLogFile);
        }

        private void saveSettings()
        {
            pluginSettings.save(settingsFile, errorLogFile);
        }

        private void initSettings()
        {
            initSettings(false);
        }

        private void initSettings(Boolean verbose)
        {
            if (pluginSettings == null)
            {
                loadSettings();
            }

            if (pluginSettings != null)
            {
                initRenderPreUI();

                if (verbose && loggedIn) {
                    WriteToChat(string.Format("Throttling sleep from {0} to {1} while logged in.", LOGINSCREEN_SLEEP,  pluginSettings.sleep));
                }
            }            
        }
    }

    public class PluginSettings
    {
        public int sleep = 5;

        public PluginSettings()
        {
            /* SET DEFAULTS FOR THE PROPERTIES HERE */
        }

        public static PluginSettings load(string file, string errorLogFile)
        {
            try
            {
                if (File.Exists(file))
                {
                    using (FileStream myFileStream = new FileStream(file, FileMode.Open))
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(PluginSettings));
                        PluginSettings mySettings = (PluginSettings)mySerializer.Deserialize(myFileStream);
                        myFileStream.Close();
                        return mySettings;
                    }

                }

            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
            return new PluginSettings();
        }

        public void save(string file, string errorLogFile)
        {
            try
            {
                using (StreamWriter myWriter = new StreamWriter(file))
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(PluginSettings));
                    myWriter.AutoFlush = true;
                    mySerializer.Serialize(myWriter, this);
                    myWriter.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorLogging.LogError(errorLogFile, ex);
            }
        }
    }
}