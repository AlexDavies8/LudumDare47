using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeSync : MonoBehaviour
{
    [SerializeField] private float _bpm = 120;
    public static float BPM;
    public static float Beats { get; private set; }
    public static float DeltaBeat { get; private set; }

    static Dictionary<float, CallbackTimer> beatTimers;
    public static Action OnBeat { get; private set; }

    private void Awake()
    {
        beatTimers = new Dictionary<float, CallbackTimer>();
    }

    private void Update()
    {
        BPM = _bpm;

        DeltaBeat = Time.deltaTime * (_bpm / 60f);
        Beats += DeltaBeat;

        foreach (var interval in beatTimers.Keys)
        {
            beatTimers[interval].timer += DeltaBeat;
            while (beatTimers[interval].timer >= interval)
            {
                beatTimers[interval].timer -= interval;
                beatTimers[interval].callback(beatTimers[interval].timer);
            }
        }
    }

    public static void RegisterBeatTimer(float interval, Action<float> callback)
    {
        if (beatTimers.ContainsKey(interval))
        {
            beatTimers[interval].callback += callback;
        }
        else
        {
            beatTimers.Add(interval, new CallbackTimer(0, callback));
        }
    }

    class CallbackTimer
    {
        public float timer;
        public Action<float> callback;

        public CallbackTimer(float timer, Action<float> callback)
        {
            this.timer = timer;
            this.callback = callback;
        }
    }
}
