using UnityEditor;

namespace FinalClick.Services.Editor
{
    public class ServicesProjectSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateServicesSettingsProvider()
        {
          
            var provider = new SettingsProvider("Project/FinalClick/Services", SettingsScope.Project)
            {
                label = "Services",
                guiHandler = (_) =>
                {
                    var config = ServicesProjectSettings.GetConfig();
                    UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(config);
                    editor.OnInspectorGUI();
                    ServicesProjectSettings.Save();
                }
            };

            return provider;
        }
    }
}