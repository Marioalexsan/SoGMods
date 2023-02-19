using EventInput;
using Grindless;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SoG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marioalexsan.GrindeaQoL.Patches
{
    internal class AttachedTextInputData
    {
        public int Cursor { get; set; } = 0;
    }

    [HarmonyPatch]
    internal static class BetterTextInput
    {
        private static readonly Dictionary<TextInput, AttachedTextInputData> _attachedTextData = new Dictionary<TextInput, AttachedTextInputData>();

        static AttachedTextInputData GetDataAndSetupIfNeeded(TextInput input)
        {
            if (!_attachedTextData.ContainsKey(input))
            {
                _attachedTextData[input] = new AttachedTextInputData();
                EventInput.EventInput.KeyDown += (s, e) => HandleKeyDown(input, e);
            }

            return _attachedTextData[input];
        }

        static void HandleKeyDown(TextInput input, KeyEventArgs e)
        {
            var data = GetDataAndSetupIfNeeded(input);

            if (e.KeyCode == Keys.Left)
                data.Cursor--;

            else if (e.KeyCode == Keys.Right)
                data.Cursor++;

            // Normalize cursor
            data.Cursor = Math.Max(data.Cursor, 0);
            data.Cursor = Math.Min(data.Cursor, input.sInput.Length);
        }

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.FetchInput))]
        [HarmonyPostfix]
        public static void ClearCursor(TextInput __instance)
        {
            var data = GetDataAndSetupIfNeeded(__instance);
            data.Cursor = 0;
        }

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.SendChatInput))]
        [HarmonyPostfix]
        public static void ClearCursor2(TextInput __instance)
        {
            var data = GetDataAndSetupIfNeeded(__instance);
            data.Cursor = 0;
        }

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.CharEntered))]
        [HarmonyPrefix]
        public static bool CharEntered(TextInput __instance, CharacterEventArgs e)
        {
            var data = GetDataAndSetupIfNeeded(__instance);

            if (!__instance.bIsActive || __instance.bInputWaiting)
            {
                // Just skip the original method entirely
                return false;
            }

            if (e.Character == '\b')
            {
                if (__instance.sInput.Length > 0 && data.Cursor > 0)
                {
                    __instance.sInput = __instance.sInput.Remove(data.Cursor - 1, 1);
                    data.Cursor--;
                }
            }
            else if (e.Character == '\r')
            {
                AccessTools.Method(typeof(TextInput), "HandleInput").Invoke(__instance, null);
            }
            else if (e.Character != '\u001b')
            {
                if (__instance.sInput.Length < __instance.iMaxLength && !Globals.Game.xInput_Menu.KeyDown(Keys.LeftControl))
                {
                    __instance.sInput = __instance.sInput.Insert(data.Cursor, e.Character.ToString());
                    data.Cursor++;
                }
            }

            // Normalize cursor
            data.Cursor = Math.Max(data.Cursor, 0);
            data.Cursor = Math.Min(data.Cursor, __instance.sInput.Length);

            // Just skip the original method entirely
            return false;
        }

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.RenderMarker), typeof(SpriteBatch), typeof(Vector2), typeof(SpriteFont))]
        [HarmonyPrefix]
        static bool RenderMarker(TextInput __instance, SpriteBatch spriteBatch, Vector2 v2PosOfTextinput, SpriteFont font)
        {
            var data = GetDataAndSetupIfNeeded(__instance);

            v2PosOfTextinput.X += font.MeasureString(__instance.sInput.Substring(0, data.Cursor)).X;
            v2PosOfTextinput.Y += font.MeasureString("I").Y * 0.1f;
            spriteBatch.Draw(RenderMaster.txNoTex, new Rectangle((int)v2PosOfTextinput.X, (int)v2PosOfTextinput.Y, 1, (int)(font.MeasureString("I").Y * 0.8f)), Color.White);

            return false;
        }

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.RenderMarker), typeof(SpriteBatch), typeof(Vector2), typeof(SpriteFont), typeof(Color), typeof(uint))]
        [HarmonyPrefix]
        static bool RenderMarker(TextInput __instance, SpriteBatch spriteBatch, Vector2 v2PosOfTextinput, SpriteFont font, Color col, uint iMarkerInterval)
        {
            var data = GetDataAndSetupIfNeeded(__instance);

            if (__instance.iExistFrames % (iMarkerInterval * 2) <= iMarkerInterval)
            {
                v2PosOfTextinput.X += font.MeasureString(__instance.sInput.Substring(0, data.Cursor)).X;
                v2PosOfTextinput.Y += font.MeasureString("I").Y * 0.1f;
                spriteBatch.Draw(RenderMaster.txNoTex, new Rectangle((int)v2PosOfTextinput.X, (int)v2PosOfTextinput.Y, 1, (int)(font.MeasureString("I").Y * 0.8f)), col);
            }

            return false;
        }
    }
}
