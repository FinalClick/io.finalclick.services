using UnityEditor;

namespace FinalClick.Services.Editor
{
    public class ServicesProjectSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateServicesSettingsProvider()
        {
            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(ServicesProjectSettings.GetConfig());
            var provider = new SettingsProvider("Project/FinalClick/Services", SettingsScope.Project)
            {
                label = "Services",
                guiHandler = (_) =>
                {
                    editor.OnInspectorGUI();
                }
            };

            return provider;
        }
    }
}