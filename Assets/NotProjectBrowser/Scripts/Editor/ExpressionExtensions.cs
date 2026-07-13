using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NotBura.Packages
{
    public readonly struct StaticROField<T>
    {
        private readonly Func<T> m_getter;

        public StaticROField(FieldInfo source)
        {
            var _field = Expression.Field(null, source);

            m_getter = Expression
                .Lambda<Func<T>>(
                    // NOTE: T直呼びできるものはConvertを通さない
                    _field.Type == typeof(T)
                        ? _field
                        : Expression.Convert(_field, typeof(T))
                )
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            return m_getter();
        }
    }

    public readonly struct ROField<TInstance, T>
    {
        private readonly Func<TInstance, T> m_getter;

        public ROField(FieldInfo source)
        {
            var _parameter = Expression.Parameter(typeof(TInstance));

            m_getter = Expression
                .Lambda<Func<TInstance, T>>(
                    Expression.Field(
                        // NOTE: TInstancfe直呼びできるものはConvertを通さない
                        _parameter.Type == source.DeclaringType
                            ? _parameter
                            : Expression.Convert(_parameter, source.DeclaringType),
                        source
                    ),
                    _parameter
                )
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(TInstance instance)
        {
            return m_getter(instance);
        }
    }

    public readonly struct RWField<TInstance, T>
    {
        private readonly Func<TInstance, T> m_getter;
        private readonly Action<TInstance, T> m_setter;

        public RWField(FieldInfo source)
        {
            var _parameter = Expression.Parameter(typeof(TInstance));

            var _field = Expression.Field(
                Cast(_parameter, source.DeclaringType),
                source
            );

            m_getter = Expression
                .Lambda<Func<TInstance, T>>(
                    Cast(_field, typeof(T)),
                    _parameter
                )
                .Compile();

            var _argument = Expression.Parameter(typeof(T));

            m_setter = Expression
                .Lambda<Action<TInstance, T>>(
                    Expression.Assign(
                        _field,
                        Cast(_argument, _field.Type)
                    ),
                    _parameter, _argument
                )
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(TInstance instance)
        {
            return m_getter(instance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(TInstance instance, T value)
        {
            m_setter(instance, value);
        }


        private static Expression Cast(Expression source, Type destination)
        {
            return source.Type == destination
                ? source
                : Expression.Convert(source, destination);
        }
    }

    public readonly struct StaticMethod<TReturn, T1>
    {
        private readonly Func<T1, TReturn> m_method;

        public StaticMethod(MethodInfo source)
        {
            var _argument1 = Expression.Parameter(typeof(T1));

            var _parameters = source.GetParameters();

            var _result = Cast(
                Expression.Call(
                    null,
                    source,
                    Cast(_argument1, _parameters[0].ParameterType)
                ),
                typeof(TReturn)
            );

            m_method = Expression
                .Lambda<Func<T1, TReturn>>(_result, _argument1)
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn Invoke(T1 argument1)
        {
            return m_method.Invoke(argument1);
        }

        private static Expression Cast(Expression source, Type destination)
        {
            return source.Type == destination
                ? source
                : Expression.Convert(source, destination);
        }
    }

    public readonly struct InstanceVoidMethod<TInstance, T1>
    {
        private readonly Action<TInstance, T1> m_method;

        public InstanceVoidMethod(MethodInfo source)
        {
            var _parameter = Expression.Parameter(typeof(TInstance));
            var _argument1 = Expression.Parameter(typeof(T1));

            var _parameters = source.GetParameters();

            var _result = Expression.Call(
                Cast(_parameter, source.DeclaringType),
                source,
                Cast(_argument1, _parameters[0].ParameterType)
            );

            m_method = Expression
                .Lambda<Action<TInstance, T1>>(_result, _parameter, _argument1)
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TInstance instance, T1 argument1)
        {
            m_method(instance, argument1);
        }

        private static Expression Cast(Expression source, Type destination)
        {
            return source.Type == destination
                ? source
                : Expression.Convert(source, destination);
        }
    }

    public readonly struct InstanceVoidMethod<TInstance, T1, T2>
    {
        private readonly Action<TInstance, T1, T2> m_method;

        public InstanceVoidMethod(MethodInfo source)
        {
            var _parameter = Expression.Parameter(typeof(TInstance));
            var _argument1 = Expression.Parameter(typeof(T1));
            var _argument2 = Expression.Parameter(typeof(T2));

            var _parameters = source.GetParameters();

            var _result = Expression.Call(
                Cast(_parameter, source.DeclaringType),
                source,
                Cast(_argument1, _parameters[0].ParameterType),
                Cast(_argument2, _parameters[1].ParameterType)
            );

            m_method = Expression
                .Lambda<Action<TInstance, T1, T2>>(_result, _parameter, _argument1, _argument2)
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TInstance instance, T1 argument1, T2 argument2)
        {
            m_method(instance, argument1, argument2);
        }

        private static Expression Cast(Expression source, Type destination)
        {
            return source.Type == destination
                ? source
                : Expression.Convert(source, destination);
        }
    }

    public readonly struct InstanceMethod<TInstance, TReturn>
    {
        private readonly Func<TInstance, TReturn> m_method;

        public InstanceMethod(MethodInfo source)
        {
            var _parameter = Expression.Parameter(typeof(TInstance));

            var _result = Cast(
                Expression.Call(
                    Cast(_parameter, source.DeclaringType),
                    source
                ),
                typeof(TReturn)
            );

            m_method = Expression
                .Lambda<Func<TInstance, TReturn>>(_result, _parameter)
                .Compile();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TReturn Invoke(TInstance instance)
        {
            return m_method(instance);
        }

        private static Expression Cast(Expression source, Type destination)
        {
            return source.Type == destination
                ? source
                : Expression.Convert(source, destination);
        }
    }

    public static class ExpressionExtensions
    {
        // TODO: FieldInfoを中で扱い外のBindingFlagsがInstance | Staticできないようにする

        public static ROField<TInstance, T> GetInstanceROField<TInstance, T>(this FieldInfo source)
        {
            return new(source);
        }

        public static ROField<TInstance, T> GetPublicInstanceROField<TInstance, T>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var _fieldInfo = type.GetField(name, _bindingFlags);

            return new(_fieldInfo);
        }

        public static ROField<TInstance, T> GetNonPublicInstanceROField<TInstance, T>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var _fieldInfo = type.GetField(name, _bindingFlags);

            return new(_fieldInfo);
        }

        public static RWField<TInstance, T> GetInstanceRWField<TInstance, T>(this FieldInfo source)
        {
            return new(source);
        }

        public static RWField<TInstance, T> GetPublicInstanceRWField<TInstance, T>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            var _fieldInfo = type.GetField(name, _bindingFlags);

            return new(_fieldInfo);
        }

        public static RWField<TInstance, T> GetNonPublicInstanceRWField<TInstance, T>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var _fieldInfo = type.GetField(name, _bindingFlags);

            return new(_fieldInfo);
        }

        public static StaticROField<T> GetPublicStaticField<T>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Static | BindingFlags.Public;
            var _fieldInfo = type.GetField(name, _bindingFlags);

            return new(_fieldInfo);
        }

        public static StaticROField<T> GetStaticROField<T>(this FieldInfo source)
        {
            return new(source);
        }

        public static StaticMethod<TReturn, T1> GetNonPublicStaticMethod<TReturn, T1>(this Type type, string name, Type returnType)
        {
            var _bindFlags = BindingFlags.Static | BindingFlags.NonPublic;

            var _methodInfos = type.GetMethods(_bindFlags);

            foreach (var _methodInfo in _methodInfos)
            {
                if (_methodInfo.ReturnType != returnType || _methodInfo.Name != name)
                {
                    continue;
                }

                var _parameters = _methodInfo.GetParameters();
                if (_parameters.Length != 1)
                {
                    continue;
                }

                if (_parameters[0].ParameterType != typeof(T1))
                {
                    continue;
                }

                return new(_methodInfo);
            }

            throw new Exception("メソッドが見つかりません");
        }

        public static InstanceVoidMethod<TInstance, T1> GetPublicInstanceVoidMethod<TInstance, T1>(this Type type, string name, Type argument1)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            return GetInstanceVoidMethod<TInstance, T1>(type, name, _bindingFlags, argument1);
        }

        public static InstanceVoidMethod<TInstance, T1> GetNonPublicInstanceVoidMethod<TInstance, T1>(this Type type, string name, Type argument1)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            return GetInstanceVoidMethod<TInstance, T1>(type, name, _bindingFlags, argument1);
        }

        public static InstanceVoidMethod<TInstance, T1> GetInstanceVoidMethod<TInstance, T1>(this Type type, string name, BindingFlags bindingFlags, Type argument1)
        {
            var _fieldInfos = type.GetMethods(bindingFlags);

            foreach (var _fieldInfo in _fieldInfos)
            {
                if (_fieldInfo.ReturnType != typeof(void) || _fieldInfo.Name != name)
                {
                    continue;
                }

                var _parameters = _fieldInfo.GetParameters();
                if (_parameters.Length != 1)
                {
                    continue;
                }

                if (_parameters[0].ParameterType != argument1)
                {
                    continue;
                }

                return new(_fieldInfo);
            }

            throw new Exception("メソッドが見つかりません");
        }

        public static InstanceVoidMethod<TInstance, T1, T2> GetNonPublicInstanceVoidMethod<TInstance, T1, T2>(this Type type, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var _fieldInfos = type.GetMethods(_bindingFlags);

            foreach (var _fieldInfo in _fieldInfos)
            {
                if (_fieldInfo.ReturnType != typeof(void) || _fieldInfo.Name != name)
                {
                    continue;
                }

                var _parameters = _fieldInfo.GetParameters();
                if (_parameters.Length != 2)
                {
                    continue;
                }

                if (_parameters[0].ParameterType != typeof(T1))
                {
                    continue;
                }

                if (_parameters[1].ParameterType != typeof(T2))
                {
                    continue;
                }

                return new(_fieldInfo);
            }

            throw new Exception("メソッドが見つかりません");
        }

        public static InstanceMethod<TInstance, TReturn> GetInstanceMethod<TInstance, TReturn>(this MethodInfo source)
        {
            return new(source);
        }

        public static InstanceMethod<TInstance, TReturn> GetNonPublicInstanceMethod<TInstance, TReturn>(this Type source, string name)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            return GetInstanceMethod<TInstance, TReturn>(source, name, _bindingFlags);
        }

        public static InstanceMethod<TInstance, TReturn> GetNonPublicInstanceMethod<TInstance, TReturn>(this Type source, string name, Type returnType)
        {
            var _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            return GetInstanceMethod<TInstance, TReturn>(source, name, _bindingFlags, returnType);
        }

        public static InstanceMethod<TInstance, TReturn> GetInstanceMethod<TInstance, TReturn>(this Type source, string name, BindingFlags bindingFlags, Type returnType)
        {
            var _fieldInfos = source.GetMethods(bindingFlags);

            foreach (var _fieldInfo in _fieldInfos)
            {
                if (_fieldInfo.ReturnType != returnType || _fieldInfo.Name != name)
                {
                    continue;
                }

                return new(_fieldInfo);
            }

            throw new Exception("メソッドが見つかりません");
        }

        public static InstanceMethod<TInstance, TReturn> GetInstanceMethod<TInstance, TReturn>(this Type source, string name, BindingFlags bindingFlags)
        {
            var _fieldInfos = source.GetMethods(bindingFlags);

            foreach (var _fieldInfo in _fieldInfos)
            {
                if (_fieldInfo.ReturnType != typeof(TReturn) || _fieldInfo.Name != name)
                {
                    continue;
                }

                return new(_fieldInfo);
            }

            throw new Exception("メソッドが見つかりません");
        }
    }
}
