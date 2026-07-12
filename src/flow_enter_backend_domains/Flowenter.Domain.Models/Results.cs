using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Flowenter.Domain.Models;

public enum Results
{
    Failure = 0,
    NotFound = 1,
    Validation = 2,
    Conflict = 3,
    AccessUnAuthorized = 4,
    AccessForbidden = 5,

    Duplicate = 6,
    ArgumentNull = 7
}

public class Error
{
    private Error(
        string code,
        string description,
        Results errorType
    )
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public string Code { get; }

    public string Description { get; }

    public Results ErrorType { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, Results.Failure);

    public static Error Duplicate(string code, string description) =>
        new(code, description, Results.Duplicate);

    public static Error Duplicate(string description) =>
        new("DB_DUPLICATE", description, Results.Duplicate);

    public static Error NotFound(string code, string description) =>
        new(code, description, Results.NotFound);
    public static Error NotFound(string description) =>
        new("NOT_FOUND_DATA", description, Results.NotFound);

    public static Error Validation(string code, string description) =>
        new(code, description, Results.Validation);

    public static Error Conflict(string code, string description) =>
        new(code, description, Results.Conflict);

    public static Error AccessUnAuthorized(string code, string description) =>
        new(code, description, Results.AccessUnAuthorized);

    public static Error AccessForbidden(string code, string description) =>
        new(code, description, Results.AccessForbidden);

    public static Error ArgumentNull(string argumentName) =>
       new("ARGUMENT_NULL", $"{argumentName} cannot be null.", Results.ArgumentNull);
}

public class Result
{
    protected Result()
    {
        IsSuccess = true;
        Error = default;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; }

    public static implicit operator Result(Error error) =>
        new(error);

    public static Result Success() =>
        new();

    public static Result Failure(Error error) =>
        new(error);
}

public sealed class ResultT<TValue> : Result
{
    private readonly TValue? _value;

    private ResultT(
        TValue value
    ) : base()
    {
        _value = value;
    }

    private ResultT(
        Error error
    ) : base(error)
    {
        _value = default;
    }

    public TValue Value =>
        IsSuccess ? _value! : throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

    public static implicit operator ResultT<TValue>(Error error) =>
        new(error);

    public static implicit operator ResultT<TValue>(TValue value) =>
        new(value);

    public static ResultT<TValue> Success(TValue value) =>
        new(value);

    public static new ResultT<TValue> Failure(Error error) =>
        new(error);

}

public static class ResultExtensions
{
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Error, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }

    public static T Match<T, TValue>(
        this ResultT<TValue> result,
        Func<TValue, T> onSuccess,
        Func<Error, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
    }

    public static T Match<T>(
        this Task<Result> result,
        Func<T> onSuccess,
        Func<Error, T> onFailure)
    {
        return result.Result.IsSuccess ? onSuccess() : onFailure(result.Result.Error!);
    }

    public static T Match<T, TValue>(
        this Task<ResultT<TValue>> result,
        Func<TValue, T> onSuccess,
        Func<Error, T> onFailure)
    {
        return result.Result.IsSuccess ? onSuccess(result.Result.Value) : onFailure(result.Result.Error!);
    }

    public static T Match<T, TValue>(
        this Task<ResultT<TValue>> result,
        Func<TValue, T> onSuccess)
    {
        return onSuccess(result.Result.Value);
    }
}