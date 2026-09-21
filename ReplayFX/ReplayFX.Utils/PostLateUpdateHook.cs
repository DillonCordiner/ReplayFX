using GameManagement;
using ReplayEditor;
using System;
using System.Linq;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ReplayFX.Utils
{
    public static class PostLateUpdateHook
    {
        private static bool _installed;

        public static void Install()
        {
            if (_installed) return; // guard against double-install if your mod's Load/OnToggle fires more than once

            PlayerLoopSystem rootLoop = PlayerLoop.GetCurrentPlayerLoop();

            for (int i = 0; i < rootLoop.subSystemList.Length; i++)
            {
                if (rootLoop.subSystemList[i].type != typeof(PostLateUpdate)) continue;

                PlayerLoopSystem postLateUpdate = rootLoop.subSystemList[i];
                var newSubsystems = new PlayerLoopSystem[postLateUpdate.subSystemList.Length + 1];
                Array.Copy(postLateUpdate.subSystemList, newSubsystems, postLateUpdate.subSystemList.Length);
                newSubsystems[postLateUpdate.subSystemList.Length] = new PlayerLoopSystem
                {
                    type = typeof(PostLateUpdateHook),
                    updateDelegate = OnPostLateUpdate
                };
                postLateUpdate.subSystemList = newSubsystems;
                rootLoop.subSystemList[i] = postLateUpdate;
                break;
            }

            PlayerLoop.SetPlayerLoop(rootLoop);
            _installed = true;
            Main.Logger.Log($"[PostLateUpdateHook] Installed: {_installed} ");
        }

        public static void Uninstall()
        {
            if (!_installed) return;

            PlayerLoopSystem rootLoop = PlayerLoop.GetCurrentPlayerLoop();

            for (int i = 0; i < rootLoop.subSystemList.Length; i++)
            {
                if (rootLoop.subSystemList[i].type != typeof(PostLateUpdate)) continue;

                PlayerLoopSystem postLateUpdate = rootLoop.subSystemList[i];
                postLateUpdate.subSystemList = postLateUpdate.subSystemList.Where(s => s.type != typeof(PostLateUpdateHook)).ToArray();
                rootLoop.subSystemList[i] = postLateUpdate;
                break;
            }

            PlayerLoop.SetPlayerLoop(rootLoop);
            _installed = false;
            Main.Logger.Log($"[PostLateUpdateHook] Installed: {_installed} ");
        }

        private static void OnPostLateUpdate()
        {
            if (!_installed || Main.timelineManager == null) return;

            Main.timelineManager.LaterUpdate();
        }
    }
}