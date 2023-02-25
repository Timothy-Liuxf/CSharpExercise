namespace GoScript.Utils
{
    public class ExtendedBuffer<T>
    {
        private T[] buffer;

        public T this[int idx]
        {
            get => buffer[idx];
            set
            {
                if (idx < 0 || idx == int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException($"The index: {idx} must be positive and less than {int.MaxValue}.");
                }

                if (idx >= buffer.Length)
                {
                    int curLen = buffer.Length;
                    int extLen = int.MaxValue / 2 > curLen ? curLen * 2 : int.MaxValue;
                    extLen = Math.Max(extLen, idx + 1);
                    var newBuffer = new T[extLen];
                    for (int i = 0; i < curLen; ++i)
                    {
                        newBuffer[i] = buffer[i];
                    }
                    buffer = newBuffer;
                }
                buffer[idx] = value;
            }
        }

        public int Length => buffer.Length;
        public T[] Buffer => buffer;

        public ExtendedBuffer(int reserved = 0)
        {
            buffer = new T[Math.Max(reserved, 1)];
        }
    }
}
