using FluentAssertions;

namespace ProjectUpgrade.Tests.Assertions
{
    public class ThenConstraint<T> : AndConstraint<T>
    {
        public ThenConstraint(T then) : base(then)
        {
            Then = then;
        }

        public T Then { get; }
    }
}
