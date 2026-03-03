using UnityEditor;
using Window = NotBura.Packages.MasterStringEditorWindow;

namespace NotBura.Packages
{
    public sealed class MasterStringEditorWindow
        : EditorWindow
    {
        private const string PATH = "Window/Text/MasterString";

        [MenuItem(PATH)]
        private static void Open()
        {
            var _window = GetWindow<Window>();
        }

        private void OnGUI()
        {
            
        }
    }
}
