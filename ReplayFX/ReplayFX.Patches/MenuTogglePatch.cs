using HarmonyLib;
using UnityEngine.EventSystems;

[HarmonyPatch(typeof(MenuToggle), nameof(MenuToggle.OnMove))]
public static class MenuTogglePatch
{
    public static bool Prefix(MenuToggle __instance, AxisEventData eventData)
    {
        if (__instance.GetComponent<ButtonMarker>() != null)
        {
            if (eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right)
            {
                return false;
            }
        }
        return true;
    }
}