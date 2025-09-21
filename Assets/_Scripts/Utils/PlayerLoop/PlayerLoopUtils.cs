﻿using System.Collections.Generic;
using UnityEngine.LowLevel;

public static class PlayerLoopUtils
{
    public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
    {
        if (loop.type != typeof(T)) return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

        var playerLoopSystemList = new List<PlayerLoopSystem>();
        if (loop.subSystemList != null) playerLoopSystemList.AddRange(loop.subSystemList);
        playerLoopSystemList.Insert(index,systemToInsert);
        loop.subSystemList = playerLoopSystemList.ToArray();

        return true;
    }

    static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index)
    {
        if (loop.subSystemList == null) return false;

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) continue;
            return true;
        }

        return false;
    }

    public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
    {
        if (loop.subSystemList == null) return;

        var playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
        for (int i = 0; i < playerLoopSystemList.Count; i++)
        {
            if (playerLoopSystemList[i].type == systemToRemove.type && playerLoopSystemList[i].updateDelegate == systemToRemove.updateDelegate)
            {
                playerLoopSystemList.Remove(playerLoopSystemList[i]);
                loop.subSystemList = playerLoopSystemList.ToArray();
            }
        }

        HandleSubSystemLoopForRemoval<T>(ref loop, in systemToRemove);

    }

    static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToRemove)
    {
        if(loop.subSystemList==null) return;

        for (int i = 0; i < loop.subSystemList.Length; i++)
        {
            RemoveSystem<T>(ref loop.subSystemList[i],in systemToRemove);
        }
    }
}