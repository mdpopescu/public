using System;

namespace CQRS3.Library.Helpers
{
    public class Either<L, R>
    {
        public static implicit operator Either<L, R>(L left) => new Either<L, R>(left);
        public static implicit operator Either<L, R>(R right) => new Either<L, R>(right);

        public Either(L left)
        {
            this.left = left;
            isLeft = true;
        }

        public Either(R right)
        {
            this.right = right;
            isLeft = false;
        }

        public T Match<T>(Func<L, T> leftFunc, Func<R, T> rightFunc) =>
            isLeft ? leftFunc(left) : rightFunc(right);

        public void Do(Action<L> leftAction, Action<R> rightAction)
        {
            if (isLeft)
                leftAction(left);
            else
                rightAction(right);
        }

        public void DoLeft(Action<L> leftAction)
        {
            if (isLeft)
                leftAction(left);
        }

        public void DoRight(Action<R> rightAction)
        {
            if (!isLeft)
                rightAction(right);
        }

        public L LeftOrDefault(L defValue = default(L)) => Match(l => l, r => defValue);
        public R RightOrDefault(R defValue = default(R)) => Match(l => defValue, r => r);

        //

        private readonly L left;
        private readonly R right;
        private readonly bool isLeft;
    }
}