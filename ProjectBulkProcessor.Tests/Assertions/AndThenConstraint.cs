using FluentAssertions;

namespace ProjectBulkProcessor.Tests.Assertions
{
    public class AndThenConstraint<T> : AndConstraint<T>
    {
        public AndThenConstraint(T then) : base(then)
        {
            Then = then;
        }

        public T Then { get; }
    }
}
