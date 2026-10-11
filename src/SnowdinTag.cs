// Snowdin Tag (beta) - BepInEx plugin for Gorilla Tag.
// Plays music from the player's OWN Undertale install. Melty passes the folder
// through the SNOWDIN_UNDERTALE_DIR environment variable; no Undertale file is shipped.
// UNTESTED SOURCE: type/member names in Hooks are unverified (see sheets/hooks.json).
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;

[BepInPlugin("com.snowdintag.beta", "Snowdin Tag", "0.1.0")]
public class SnowdinTag : BaseUnityPlugin
{
    string undertaleDir;
    AudioSource audio;
    string current = "";
    string hud = "";
    float hudUntil;

    void Awake()
    {
        undertaleDir = Environment.GetEnvironmentVariable("SNOWDIN_UNDERTALE_DIR");
        if (string.IsNullOrEmpty(undertaleDir) || !Directory.Exists(undertaleDir))
        {
            Logger.LogWarning("Undertale folder not provided; mod stays silent.");
            enabled = false;
            return;
        }
        audio = gameObject.AddComponent<AudioSource>();
        audio.spatialBlend = 0f;
        Logger.LogInfo("Snowdin Tag ready. Undertale folder: " + undertaleDir);
    }

    // Reflection keeps a wrong guess about Gorilla Tag's types from crashing the game.
    string ReadState()
    {
        var tagger = FindType("GorillaTagger");
        var mgr = FindType("GorillaTagManager");
        if (tagger == null || mgr == null) return "waiting";
        var inst = tagger.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        if (inst == null) return "waiting";
        return "survivor"; // TODO: confirm infected check against the real game (hooks.json: tag_manager)
    }

    static Type FindType(string name)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            var t = asm.GetType(name);
            if (t != null) return t;
        }
        return null;
    }

    void Update()
    {
        string state = ReadState();
        if (state != current)
        {
            current = state;
            string file = state == "waiting" ? "mus_town.ogg" :
                          state == "infected" ? "mus_dogsong.ogg" : "mus_snowy.ogg";
            StartCoroutine(Play(file, true));
            hud = state == "infected" ? "* You were tagged. Stay determined..." : "";
            hudUntil = Time.time + 4f;
        }
    }

    IEnumerator Play(string file, bool loop)
    {
        string path = Path.Combine(undertaleDir, file);
        if (!File.Exists(path)) { Logger.LogWarning("Missing in Undertale folder: " + file); yield break; }
        using (var req = UnityWebRequestMultimedia.GetAudioClip("file:///" + path.Replace('\\', '/'), AudioType.OGGVORBIS))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success) { Logger.LogWarning(req.error); yield break; }
            audio.clip = DownloadHandlerAudioClip.GetContent(req);
            audio.loop = loop;
            audio.volume = 0.5f;
            audio.Play();
        }
    }

    void OnGUI()
    {
        if (Time.time < hudUntil && hud.Length > 0)
            GUI.Label(new Rect(20, Screen.height - 60, 700, 40), hud);
    }
}
