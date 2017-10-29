namespace Sift.Common
{
    public class Line
    {
        public byte Index { get; }

        private Caller caller;

        public Caller Caller
        {
            get { return caller; }
            set
            {
                if (value == caller)
                    return;
                if (value == null)
                    State = LineState.Empty;
                else
                    State = LineState.Ringing;
                caller = value;
            }
        }

        public LineState State { get; set; } = LineState.Empty;

        public Line(byte index)
        {
            Index = index;
        }
    }
}
