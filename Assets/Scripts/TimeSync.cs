using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeSync : MonoBehaviour
{
    [SerializeField] private float _bpm = 120;
    [SerializeField] private int _beatsPerBar = 2;
    public static float BPM;
    public static float Beats { get; private set; }
    public static float DeltaBeat { get; private set; }

    static Dictionary<float, CallbackTimer> beatTimers;
    public static Action OnBeat { get; private set; }
    public float Bpm { get => _bpm; set => _bpm = value; }

    float _prevDSPTime;

    private void Awake()
    {
        beatTimers = new Dictionary<float, CallbackTimer>();
    }

    private void Update()
    {
        float deltaTime = (float)AudioSettings.dspTime - _prevDSPTime;

        BPM = Bpm / _beatsPerBar;

        DeltaBeat = deltaTime * (BPM / 60f);
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

        _prevDSPTime = (float)AudioSettings.dspTime;
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
