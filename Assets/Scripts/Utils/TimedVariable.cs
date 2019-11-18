using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedVariable {

    public float time;
    public float currentTime { get; private set; }

    public float progress {
        get {
            return Mathf.Min(currentTime / time, 1.0f);
        }
    }
    public bool finished { get; private set; }

    public Action triggerAction;

    public TimedVariable(float time = 1f) {
        this.time = time;
    }

    public void StepFrame() {
        Step(Time.fixedDeltaTime);
    }

    public void Step(float stepTime) {
        currentTime += stepTime;
        if (currentTime >= time && !finished) {
            finished = true;
            if (triggerAction != null) triggerAction();
        }
    }

    public void SkipToEnd() {
        Step(time);
    }

    public void Reset() {
        currentTime = 0;
        finished = false;
    }
}