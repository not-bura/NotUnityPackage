using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NotBura.Packages
{
    public class HeaderFunctionMenuData
    {
        private string m_name;
        private MethodInfo m_methodInfo;
        private bool m_parallel;
        private bool m_editable;
        private object[] m_arguments;

        public string Name
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_name;
        }

        public ParameterInfo[] Parameters
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_methodInfo.GetParameters();
        }

        public HeaderFunctionMenuData(MethodInfo methodInfo, HeaderFunctionAttribute attribute)
        {
            m_name = string.IsNullOrWhiteSpace(attribute.ItemName)
                ? methodInfo.Name
                : attribute.ItemName;
            m_methodInfo = methodInfo;
            m_parallel = attribute.Parallel;
            m_editable = attribute.Editable;
            m_arguments = attribute.Arguments;
        }

        public object[] GetFixedArguments(out bool openWindow)
        {
            var _parameters = m_methodInfo.GetParameters();
            var _arguments = m_arguments;

            if (_arguments == null)
            {
                // NOTE: 引数があるメソッドを引数無しで呼び出す際はウィンドウを出す
                if (_parameters.Length != 0)
                {
                    openWindow = true;
                    return null;
                }

                // NOTE: 引数無しメソッドならいらないのでNullを返す
                openWindow = false;
                return null;
            }

            if (_arguments.Length != _parameters.Length)
            {
                // NOTE: 割り当てた引数長がメソッド引数長より少ないならウィンドウを出す
                if (_arguments.Length < _parameters.Length)
                {
                    openWindow = true;
                    return null;
                }

                // NOTE: 割り当てた引数長がメソッド引数長より多いならならワーニングを出しておく
                Debug.LogWarning($"Over length arguments. Target: {m_methodInfo.DeclaringType.FullName}.{m_methodInfo.Name}");

                // NOTE: 引数無しのメソッドならいらないのでNullを返す
                if (_parameters.Length == 0)
                {
                    openWindow = false;
                    return null;
                }

                // NOTE: 無効なパラメータがあればウィンドウを出す
                if (IsParametersInvalid(_parameters, _arguments, out var _invalidIndicies))
                {
                    LogErrorInvalidParameters(m_methodInfo, _parameters, _arguments, _invalidIndicies);

                    openWindow = true;
                    return null;
                }

                // NOTE: 必要な長さだけスライスして返す
                var _results = new object[_parameters.Length];
                for (int i = 0; i < _results.Length; ++i)
                {
                    _results[i] = _arguments[i];
                }

                openWindow = false;
                return _results;
            }

            // NOTE: 編集可能を指定していればウィンドウを出す
            if (m_editable)
            {
                openWindow = true;
                return null;
            }
            else
            {
                // NOTE: 無効なパラメータがあればエラーとウィンドウを出す
                if (IsParametersInvalid(_parameters, _arguments, out var _invalidIndicies))
                {
                    LogErrorInvalidParameters(m_methodInfo, _parameters, _arguments, _invalidIndicies);

                    openWindow = true;
                    return null;
                }
            }

            // NOTE: パラメータに対して完全に有効な引数はそのまま返す
            openWindow = false;
            return _arguments;
        }

        public void Invoke(Object[] targets, object[] arguments)
        {
            var _methodInfo = m_methodInfo;

            if (_methodInfo.ReturnType == typeof(void))
            {
                VoidInvoke(_methodInfo, targets, arguments);
                return;
            }

            if (HeaderFunctionUtility.IsAsync(_methodInfo))
            {
                if (m_parallel)
                {
                    ParallelInvoke(_methodInfo, targets, arguments);
                }
                else
                {
                    AsyncInvoke(_methodInfo, targets, arguments);
                }
                return;
            }

            ReturnInvoke(_methodInfo, targets, arguments);
        }

        /// <summary>
        /// パラメータ長の分だけ引数が適切か確認する　余った分は確認しない
        /// </summary>
        private static bool IsParametersInvalid(ParameterInfo[] parameters, object[] arguments, out int[] invalidIndices)
        {
            var _results = new List<int>();

            for (int i = 0; i < parameters.Length; ++i)
            {
                var _parameter = parameters[i];
                var _type = _parameter.ParameterType;

                if (HeaderFunctionUtility.IsNullable(_type) && arguments[i] == null)
                {
                    continue;
                }

                if (_parameter.ParameterType.IsAssignableFrom(arguments[i].GetType()))
                {
                    continue;
                }

                _results.Add(i);
            }

            if (_results.Count == 0)
            {
                invalidIndices = null;
                return false;
            }
            
            invalidIndices = _results.ToArray();
            return true;
        }

        private static void LogErrorInvalidParameters(MethodInfo methodInfo, ParameterInfo[] parameters, object[] arguments, int[] invalidIndices)
        {
            var _sb = new StringBuilder();
            _sb.AppendLine($"Invalid parameter Error. Target: {methodInfo.DeclaringType.FullName}.{methodInfo.Name}");
            foreach (var i in invalidIndices)
            {
                _sb.AppendLine($"Index: {i}. ParameterType: {parameters[i].ParameterType} ArgumentType: {arguments[i].GetType()}");
            }
            Debug.LogError(_sb.ToString());
        }

        private static void VoidInvoke(MethodInfo methofInfo, Object[] targets, object[] arguments)
        {
            foreach (var target in targets)
            {
                methofInfo.Invoke(target, arguments);
            }
        }

        private static void ReturnInvoke(MethodInfo methofInfo, Object[] targets, object[] arguments)
        {
            foreach (var target in targets)
            {
                var _result = methofInfo.Invoke(target, arguments);
                HeaderFunctionMenu.ResultHandler?.Invoke(_result);
            }
        }

        private static async void AsyncInvoke(MethodInfo methodInfo, Object[] targets, object[] arguments)
        {
            var _awaitableFactory = new AwaitableFactory();

            foreach (var target in targets)
            {
                var _invokeResult = methodInfo.Invoke(target, arguments);
                var _awaitableWrapper = _awaitableFactory.GetWrapper(_invokeResult);

                await _awaitableWrapper.CompleteAsync();

                if (_awaitableFactory.IsVoidResult())
                {
                    continue;
                }

                var _result = _awaitableWrapper.GetResult();
                HeaderFunctionMenu.ResultHandler?.Invoke(_result);
            }
        }

        private static async void ParallelInvoke(MethodInfo methodInfo, Object[] targets, object[] arguments)
        {
            var _awaitableWrappers = new List<AwaitableWrapper>(targets.Length);
            var _awaitableFactory = new AwaitableFactory();

            for (int i = 0; i < targets.Length; ++i)
            {
                var target = targets[i];

                var _invokeResult = methodInfo.Invoke(target, arguments);
                var _awaitableWrapper = _awaitableFactory.GetWrapper(_invokeResult);

                _awaitableWrappers.Add(_awaitableWrapper);
            }

            while (_awaitableWrappers.Count > 0)
            {
                for (int i = 0; i < _awaitableWrappers.Count; ++i)
                {
                    var _awaitableWrapper = _awaitableWrappers[i];
                    if (_awaitableWrapper.IsCompleted())
                    {
                        if (false == _awaitableFactory.IsVoidResult())
                        {
                            var _result = _awaitableWrapper.GetResult();
                            HeaderFunctionMenu.ResultHandler?.Invoke(_result);
                        }

                        _awaitableWrappers.RemoveAt(i);
                        i--;
                    }
                }

                if (_awaitableWrappers.Count == 0)
                {
                    break;
                }

                await Task.Yield();
            }
        }
    }
}
