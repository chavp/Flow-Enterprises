using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Flowenter.Domain.Models;

public abstract class Maybe<T> : IEquatable<Maybe<T>>, IStructuralEquatable
{
    public static explicit operator Maybe<T>(T value) =>
        Some(value);

    public static Maybe<T> Some(T value) =>
        new Choices.Some(value);

    public static Maybe<T> None { get; } = new Choices.None();

    public abstract R Match<R>(Func<T, R> someFunc, Func<R> noneFunc);

    public abstract void Iter(Action<T> someAction, Action noneAction);

    public Maybe<R> Map<R>(Func<T, R> map) =>
        Match(
            v => Maybe<R>.Some(map(v)),
            () => Maybe<R>.None);

    public R Fold<R>(Func<R, T, R> foldFunc, R seed) =>
        Match(t => foldFunc(seed, t), () => seed);

    public R GetOrElse<R>(Func<T, R> foldFunc, R seed) =>
        Fold((_, t) => foldFunc(t), seed);

    public T GetOrDefault(T defaultValue) =>
        Fold((_, t) => t, defaultValue);

    public static Maybe<T> Return(T value) =>
        Some(value);

    public Maybe<R> Bind<R>(Func<T, Maybe<R>> map) =>
        Match(
            v => map(v).Match(
                r => Maybe<R>.Some(r),
                () => Maybe<R>.None),
            () => Maybe<R>.None);

    #region Value Semantics
    public static bool operator ==(Maybe<T> x, Maybe<T> y) =>
        x.Equals(y);

    public static bool operator !=(Maybe<T> x, Maybe<T> y) =>
        !(x == y);

    bool IEquatable<Maybe<T>>.Equals(Maybe<T> other) =>
        Equals(other as object);

    public abstract bool Equals(object other, IEqualityComparer comparer);

    public abstract int GetHashCode(IEqualityComparer comparer);
    #endregion

    private Maybe() { }

    private static class Choices
    {
        public sealed class Some : Maybe<T>
        {
            private T Value { get; }

            public Some(T value) =>
                Value = value;

            public override R Match<R>(Func<T, R> someFunc, Func<R> noneFunc) =>
                someFunc(Value);

            public override void Iter(Action<T> someAction, Action noneAction) =>
                someAction(Value);

            public override string ToString() =>
                $"Some ({Value})";

            #region Value Semantics
            public override bool Equals(object obj) =>
                obj is Some s
                    ? Value.Equals(s.Value)
                    : false;

            public override bool Equals(object other, IEqualityComparer comparer) =>
                other is Some s
                    ? comparer.Equals(Value, s.Value)
                    : false;

            public override int GetHashCode() =>
                "Some ".GetHashCode() ^ Value.GetHashCode();

            public override int GetHashCode(IEqualityComparer comparer) =>
                "Some ".GetHashCode() ^ comparer.GetHashCode(Value);
            #endregion
        }

        public sealed class None : Maybe<T>
        {
            public override R Match<R>(Func<T, R> someFunc, Func<R> noneFunc) =>
                noneFunc();

            public override void Iter(Action<T> someAction, Action noneAction) =>
                noneAction();

            public override string ToString() =>
                "None";

            #region Value Semantics
            public override bool Equals(object obj) =>
                obj is None;

            public override int GetHashCode() =>
                "None".GetHashCode();

            public override bool Equals(object other, IEqualityComparer comparer) =>
                Equals(other);

            public override int GetHashCode(IEqualityComparer comparer) =>
                GetHashCode();
            #endregion
        }
    }
}


public static class MaybeExtensions
{
    public static R Match<R, T>(this Task<Maybe<T>> maybe,
        Func<T, R> someFunc, Func<R> noneFunc) =>
        maybe.Result.Match(someFunc, noneFunc);

}