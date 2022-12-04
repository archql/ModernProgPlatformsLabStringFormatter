using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace lab5StringFormatter.Core
{
    public class StringFormatter : IStringFormatter
    {
        private enum State
        {
            A, // first { readed
            B, // first } readed
            C, // readed any char after {
            D, // readed any char (start state, terminator state)
            E, // exception (terminator state)
            F  // readed } after word or empty (identical to D except member postprocessing) (terminator state)
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
                { '{', State.A },
                { '}', State.B },
                { 'A', State.D }
            } }
        };

        public static readonly StringFormatter Shared = new StringFormatter();
        public string Format(string template, object target)
        {
            // parse template string
            string result = "";
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
                    case State.A: memStart = counter + 1; break;
                    case State.F: result += getValueByMember(template.Substring(memStart, counter - memStart), target); break; 
                    case State.E: throw new ArgumentException($"template string has unbalanced brackets! (stopped at: {counter})");
                }
            }
            if (state != State.D && state != State.F) // if state machine ended in not terminator stages
                throw new ArgumentException($"template string has unbalanced brackets! (stopped at: {counter})");
            return result;
        }

        private ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, string>>> _cache 
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, string>>>();

        public int getChacheCountForType(Type t)
        {
            if (_cache.TryGetValue(t, out var count))
                return count.Count;
            else
                return 0;
        }

        private string getValueByMember(string member, object target)
        {
            if (member == "")
                return member;
            if (target == null) { 
                throw new ArgumentNullException(nameof(target));
            }
            
            // create chache
            var type = target.GetType();
            var typedCache = _cache.GetOrAdd(type, new ConcurrentDictionary<string, Func<object, string>>());
            return _cache[type].GetOrAdd(member, createGetFunc(member, target))(target);
        }
        private Func<object, string> createGetFunc(string member, object target)
        {
            // prepare input
            var memberStrParts = member.Split(new char[] { '[', ']' });
            var memberName = memberStrParts[0];
            var memberId = (memberStrParts.Length > 1) ? memberStrParts[1] : null;
            // check if member exists in target
            var propertyInfo = target.GetType().GetProperty(memberName);
            var fieldInfo = target.GetType().GetField(memberName);
            if (fieldInfo == null && propertyInfo == null)
            {
                throw new ArgumentException("field \"" + memberName + "\" in object \"" + nameof(target) + "\" does not exist or not accessable!");
            }
            // create expr tree
            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression expr = Expression.PropertyOrField(Expression.TypeAs(obj, target.GetType()), memberName);
            if (memberId != null) // there is index
            {
                expr = Expression.ArrayAccess(expr, Expression.Constant(int.Parse(memberId), typeof(int)));
            }
            var toString = Expression.Call(expr, "ToString", null, null);
            var res = Expression.Lambda<Func<object, string>>(toString, obj);

            return res.Compile();
        }
    }
}