using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Character
    {
        Unknown,
        Player,
        Monster,
    }

    public enum Sound
    {
        BGM,
        Effect,
        MaxCount,
    }

    public enum TouchEvent
    {
        PointerDown,
        PointerUp,
        Press,
        Click,
    }

    public enum CameraMode
    {
        ShoulderView,
    }

    public enum State
    {
        Die,
        Moving,
        Idle,
        SwordIdle,
        GunIdle,
        Attack,
        Roll,
        SkillF,
        SkillS,
        SkillG,
        Stun,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        PointerDown,
        PointerUp,
    }

    public enum Scene
    {
        Unknown,
        StartRoom,
        BossRoom,
    }
}
