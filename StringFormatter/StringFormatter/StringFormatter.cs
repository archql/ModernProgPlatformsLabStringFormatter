namespace lab5StringFormatter.Core
{
    public class StringFormatter : IStringFormatter
    {
        private enum State
        {
            A, // first { readed
            B, // first } readed
            C, // readed any char after {
            D, // readed any char (start state)
            E, // exception
            F  // readed } after word or empty
        }
        static private readonly IReadOnlyDictionary<State, Dictionary<char, State>> _states = new Dictionary<State, Dictionary<char, State>>()
        {
            {State.A, new Dictionary<char, State> { 
                { '}', State.F },
                { '{', State.D },
                { 'A', State.C }
            } },
            {State.B, new Dictionary<char, State> {
                { '}', State.D },
                { 'A', State.E },
                { '{', State.E }
            } },
            {State.C, new Dictionary<char, State> {
                { '}', State.F },
                { '{', State.E },
                { 'A', State.C }
            } },
            {State.D, new Dictionary<char, State> {
                { '{', State.A },
                { '}', State.B },
                { 'A', State.D }
            } },
            {State.F, new Dictionary<char, State> {
                { '{', State.D },
                { '}', State.D },
                { 'A', State.D }
            } }
        };

        public static readonly StringFormatter Shared = new StringFormatter();
        public string Format(string template, object target)
        {
            // parse template string
            string result = "", member = "";
            State state = State.D;
            int counter = -1, memStart = -1;
            foreach (var chr in template)
            {
                // count amount of chars processed
                counter++;
                // 
                char ctrl = (chr != '{' && chr != '}') ? 'A' : chr;
                state = _states[state][ctrl];
                switch(state)
                {
                    case State.D: result += chr; break;
                    case State.A: memStart = counter; break;
                    case State.F: member = template.Substring(memStart, counter - memStart - 1); break; // and analyse member here
                    case State.E: throw new ArgumentException($"template string has unbalanced brackets! (stopped at: {counter})");
                }
            }
            return result;
        }
    }
}