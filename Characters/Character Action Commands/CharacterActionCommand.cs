﻿using UnityEngine;

namespace CommandPattern
{
    public abstract class CharacterActionCommand
    {
        public abstract void Execute(int actorID, GameObject target, CharacterAction actionInfo);
        public abstract void Stop();
        protected Coroutine CurrentActionCoroutine { get; set; } = null;
    }
}