using System;
using System.Buffers.Binary;
using MemoryPack;
using UnityEngine;
using Utils;

namespace Game.Sim
{
    public enum FighterMode
    {
        Neutral,
        Airborne,
        Attacking,
        Hitstun,
        Blockstun,
        Knockdown,
    }

    [MemoryPackable]
    public partial struct FighterState
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Speed;

        [MemoryPackIgnore]
        public FighterMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _modeT = 0;
            }
        }

        [MemoryPackInclude]
        private FighterMode _mode;
        [MemoryPackInclude]
        private int _modeT;

        public Vector2 FacingDirection;

        public FighterState(Vector2 position, float speed, Vector2 facingDirection)
        {
            Position = position;
            Velocity = Vector2.zero;
            Speed = speed;
            _mode = FighterMode.Neutral;
            _modeT = 0;
            FacingDirection = facingDirection;
        }

        public void ApplyInputs(GameInput input)
        {
            // Horizontal movement
            Velocity.x = 0;
            if (input.Flags.HasFlag(InputFlags.Left))
                Velocity.x = -Speed;
            if (input.Flags.HasFlag(InputFlags.Right))
                Velocity.x = Speed;

            // Vertical movement only if grounded
            if (input.Flags.HasFlag(InputFlags.Up) && Position.y <= Globals.GROUND)
            {
                Velocity.y = Speed * 1.5f;
            }
            UpdatePhysics();
        }

        void UpdatePhysics()
        {
            // Apply gravity if not grounded
            if (Position.y > Globals.GROUND || Velocity.y > 0)
            {
                Velocity.y += Globals.GRAVITY * 1 / 60;
            }

            // Update Position
            Position += Velocity * 1 / 60;

            // Floor collision
            if (Position.y <= Globals.GROUND)
            {
                Position.y = Globals.GROUND;

                if (Velocity.y < 0)
                    Velocity.y = 0;
            }
        }
    }
}
