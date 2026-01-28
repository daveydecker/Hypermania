using MemoryPack;

namespace Game.Sim
{
    [MemoryPackable]
    public partial class LT_inputHistory
    {
        [MemoryPackInclude]
        private GameInput[] _buffer;

        [MemoryPackInclude]
        private int _front;

        [MemoryPackInclude]
        private int _count;

        // The structure of this input history follows a circular array / buffer, for constant access times to previos frames.
        // We only add on the last frame at the end, for constant O(1) time.
        public LT_inputHistory()
        {
            _buffer = new GameInput[64];
            _front = 0;
            _count = 0;
        }

        public void PushInput(GameInput input)
        {
            _buffer[_front] = input;
            _front = (_front + 1) % _buffer.Length;
            if (_count < _buffer.Length)
            {
                _count = _count + 1;
            }
        }

        public GameInput GetInput(int framesAgo)
        {
            if (framesAgo < 0 || framesAgo >= _count)
            {
                return new GameInput(InputFlags.None);
            }
            int idx = (_front - 1 - framesAgo + _buffer.Length) % _buffer.Length;
            return _buffer[idx];
        }

        // Checks if the button was pressed within the last couple of frames.
        public bool PressedRecently(InputFlags flag, int withinFrames)
        {
            if (withinFrames < 0 || withinFrames >= _count)
            {
                return false;
            }
            for (int i = 0; i < withinFrames; i++)
            {
                if (GetInput(i).HasInput(flag))
                {
                    return true;
                }
            }
            return false;
        }

        // Was the key ever pressed and then released in this frame of time?
        public bool PressedAndReleasedRecently(InputFlags flag, int withinFrames)
        {
            if (withinFrames < 0 || withinFrames >= _count)
            {
                return false;
            }
            bool beingPressed = false;
            for (int i = withinFrames - 1; i >= 0; i--)
            {
                if (GetInput(i).HasInput(flag) && !beingPressed)
                {
                    beingPressed = true;
                    continue;
                }

                if (GetInput(i).HasInput(flag) && beingPressed)
                {
                    return true;
                }
            }
            return false;
        }

        // Was an input held for a long enough period of time?
        public bool HeldRecently(InputFlags flag, int framesLong, int withinFrames)
        {
            if (withinFrames < 0 || withinFrames >= _count)
            {
                return false;
            }
            int heldCount = 0;
            for (int i = withinFrames - 1; i >= 0; i--)
            {
                if (GetInput(i).HasInput(flag))
                {
                    heldCount++;
                    if (heldCount >= framesLong)
                    {
                        return true;
                    }
                    continue;
                }
                else
                {
                    heldCount = 0;
                }
            }
            return false;
        }
    }
}
