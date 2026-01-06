using System;
using System.Buffers;
using MemoryPack;
using Netcode.Rollback;
using UnityEngine;
using Utils;

namespace Game.Sim
{
    [MemoryPackable]
    public partial class GameState : IState<GameState>
    {
        public Frame Frame;
        public FighterState[] Fighters;
        // public HitboxState[] Hitboxes;     
        // public ProjectileState[] Projectiles; 

        public GameState()
        {
            Frame = Frame.NullFrame;
            Fighters = new FighterState[2];
        }

        public static GameState New()
        {
            GameState state = new GameState();
            state.Frame = Frame.FirstFrame;
            state.Fighters[0] = new FighterState(new Vector2(-7, -4.5f), 7f, Vector2.right);
            state.Fighters[1] = new FighterState(new Vector2(7, -4.5f), 7f, Vector2.left);
            return state;
        }

        public void Advance((GameInput input, InputStatus status)[] inputs)
        {
            if (inputs.Length >= 1)
                Fighters[0].ApplyInputs(inputs[0].input);
            if (inputs.Length >= 2)
                Fighters[1].ApplyInputs(inputs[1].input);
            Frame += 1;
        }

        [ThreadStatic]
        private static ArrayBufferWriter<byte> _writer;
        private static ArrayBufferWriter<byte> Writer
        {
            get
            {
                if (_writer == null)
                    _writer = new ArrayBufferWriter<byte>(256);
                return _writer;
            }
        }

        public ulong Checksum()
        {
            Writer.Clear();
            MemoryPackSerializer.Serialize(Writer, this);
            ReadOnlySpan<byte> bytes = Writer.WrittenSpan;

            // 64-bit FNV-1a over the serialized bytes
            const ulong OFFSET = 14695981039346656037UL;
            const ulong PRIME = 1099511628211UL;

            ulong hash = OFFSET;
            for (int i = 0; i < bytes.Length; i++)
            {
                hash ^= bytes[i];
                hash *= PRIME;
            }
            return hash;
        }
    }
}