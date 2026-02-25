using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NotBura.Scraps
{
    public class GamingEditor
    {
        private const string PATH = "NotBura/GamingEditor";
        private const string STYLE_SHEETS_FOLDER_PATH = "Assets/NotBura/Editor/StyleSheets/Extensions";
        private const string DARK_SHEETS_NAME = "dark.uss";
        private const string LIGHT_SHEETS_NAME = "light.uss";

        private static bool s_enabled = false;
        private const float SPEED = 0.000000001f;

        private enum ThemeState
        {
            None,
            Light,
            Dark,
        }

        [MenuItem(PATH)]
        private static void ExecuteMenuCommand()
        {
            s_enabled = !s_enabled;
            Menu.SetChecked(PATH, s_enabled);

            if (s_enabled)
            {
                EnableGamingEditor();
            }
            else
            {
                DisableGamingEditor();
            }
        }

        private static void EnableGamingEditor()
        {
            EditorApplication.update -= UpdateGamingEditor;

            CheckFiles();

            EditorApplication.update += UpdateGamingEditor;
        }

        private static void DisableGamingEditor()
        {
            EditorApplication.update -= UpdateGamingEditor;
            DeleteFiles();
        }

        private static ThemeState CheckFiles()
        {
            if (false == AssetDatabase.IsValidFolder(STYLE_SHEETS_FOLDER_PATH))
            {
                Directory.CreateDirectory(STYLE_SHEETS_FOLDER_PATH);
                AssetDatabase.ImportAsset(STYLE_SHEETS_FOLDER_PATH);
            }

            var _themeIsLight = 0 == EditorPrefs.GetInt("UserSkin");

            // NOTE: STYLE_SHEETS_FOLDER_PATH + "/" + bool ? light : darkでもいいが文字列定数の方がパフォーマンス良さそうな気がする
            // NOTE: あくまで気がするだけ
            var _path = _themeIsLight
                ? STYLE_SHEETS_FOLDER_PATH + "/" + LIGHT_SHEETS_NAME
                : STYLE_SHEETS_FOLDER_PATH + "/" + DARK_SHEETS_NAME;

            if (false == AssetDatabase.AssetPathExists(_path))
            {
                File.Create(_path);
                AssetDatabase.ImportAsset(_path);
            }

            return _themeIsLight
                ? ThemeState.Light
                : ThemeState.Dark;
        }

        private static void DeleteFiles()
        {
            {
                const string THEME_PATH = STYLE_SHEETS_FOLDER_PATH + "/" + LIGHT_SHEETS_NAME;
                if (AssetDatabase.AssetPathExists(THEME_PATH))
                {
                    AssetDatabase.DeleteAsset(THEME_PATH);
                }
            }

            {
                const string THEME_PATH = STYLE_SHEETS_FOLDER_PATH + "/" + DARK_SHEETS_NAME;
                if (AssetDatabase.AssetPathExists(THEME_PATH))
                {
                    AssetDatabase.DeleteAsset(THEME_PATH);
                }
            }
        }

        private static void UpdateGamingEditor()
        {
            var _state = CheckFiles();

            var _path = _state switch
            {
                ThemeState.Light => STYLE_SHEETS_FOLDER_PATH + "/" + LIGHT_SHEETS_NAME,
                ThemeState.Dark => STYLE_SHEETS_FOLDER_PATH + "/" + DARK_SHEETS_NAME,
                _ => throw new NotSupportedException(),
            };

            var _duration = Stopwatch.GetTimestamp();
            var _tween = (_duration * SPEED) % 1.0f;

            var _color = GetColor(_tween);

            WriteStreamWriter(in _color, _path);
            AssetDatabase.ImportAsset(_path);
        }

        // NOTE: 最適化の余地はあるがファイル操作の方がネック
        private static Color32 GetColor(float tween)
        {
            var _color = Mathf.Clamp01(tween) == 1.0f
                ? 0xFF_FF_FF
                : (int)(tween * 0xFF_FF_FF);

            var _r = (byte)((_color >> 8 * 2) & 0xFF);
            var _g = (byte)((_color >> 8 * 1) & 0xFF);
            var _b = (byte)((_color >> 8 * 0) & 0xFF);

            return new(_r, _g, _b, 0);
        }

        private static void WriteStreamWriter(in Color32 color, string path)
        {
            var _colorText = $"{color.r}, {color.g} {color.b}";

            var _sb = new StringBuilder();

            _sb.AppendLine(":root {");

            {
                const string TEXT = "    --unity-theme-background-color-";
                for (int i = 1; i <= 17; ++i)
                {
                    _sb
                        .Append(TEXT)
                        .Append(i)
                        .Append(": rgb(")
                        .Append(_colorText)
                        .AppendLine(");");
                }
            }

            {
                const string TEXT = "    --unity-theme-border-color-";
                for (int i = 1; i <= 22; ++i)
                {
                    _sb
                        .Append(TEXT)
                        .Append(i)
                        .Append(": rgb(")
                        .Append(_colorText)
                        .AppendLine(");");
                }
            }

            _sb.AppendLine("}");

            using (var sw = new StreamWriter(path))
            {
                sw.Write(_sb.ToString());
            }
        }
    }
}
