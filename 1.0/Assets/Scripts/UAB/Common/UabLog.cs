using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UabLog {
    public static bool LogSwitch = true;

    public enum LogPrefix {
        Hotfix,
        Manager,
    }
    
    private static string dump(LogPrefix prefix, params object[] args) {
        List<string> outs = new List<string>();
        outs.Add(prefix.ToString());
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == null) {
                outs.Add("null");
            } else {
                outs.Add(args[i].ToString());
            }
        }

        return string.Join("|", outs);
    }
    public static void D(LogPrefix prefix, params object[] args) {
        if (LogSwitch) {
            Debug.Log(dump(prefix, args));
        }
    }

    public static void E(LogPrefix prefix, params object[] args) {
        Debug.LogError(dump(prefix, args));
    }
}