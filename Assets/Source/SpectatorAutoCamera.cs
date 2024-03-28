using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpectatorAutoCamera : MonoBehaviour
{
    public float SwitchTime;

    public PlayerHandler PlayerHandler;
    public CameraSelector CameraSelector;
    public CameraControl CameraControl;

    public State CurrentState;
    public enum State { Overview, Frontline, FrontlineWide, Defense, Offense, Unit }
    public int[] CameraForState;

    private void Start()
    {
        enabled = PlayerHandler.PlayerCommander == null;
        if (enabled)
        {
            StartCoroutine(DelayedInit());
        }
    }

    private IEnumerator DelayedInit()
    {
        yield return null;
        yield return null;

        SetState(CurrentState);
        InvokeRepeating(nameof(NextState), SwitchTime, SwitchTime);
    }

    private void NextState()
    {
        State nextState = SelectRandomStateExcept(CurrentState);
        while (!IsStateValid(nextState))
        {
            nextState = SelectRandomStateExcept(CurrentState);
        }
        SetState(nextState);
    }

    private State SelectRandomStateExcept(State state)
    {
        int numStates = Enum.GetValues(typeof(State)).Length;
        return (State)(((int)state + UnityEngine.Random.Range(1, numStates)) % numStates);
    }

    private bool IsStateValid(State state)
    {
        if (state == State.Overview)
            return false;
        if (state == State.FrontlineWide)
            return !Team.AllCommanders.Any(x => x.DefenseFactor > Mathf.Epsilon);
        return true;
    }

    private void SetState(State state)
    {
        int camera = CameraForState[(int)state];
        CameraSelector.SelectCamera(camera);
        CurrentState = state;
        if (CameraSelector.CurrentIs(out ISettableCameraController settable))
        {
            settable.LookAt(Vector3.zero);
        }
    }
}
