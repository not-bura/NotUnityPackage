using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using CT = System.Threading.CancellationToken;

namespace NotBura.Packages
{
    public class AssetStream
    {
        private Dictionary<AssetIdentifier, string> m_pathTable;
        private Dictionary<AssetIdentifier, object> m_cacheTable;

        public AssetStream()
        {
            m_pathTable = new();
            m_cacheTable = new();
        }

        public void Add((AssetIdentifier, string)[] values)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                var _value = values[i];
                m_pathTable.Add(_value.Item1, _value.Item2);
            }
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>IsCanceled</returns>
        public async UniTask<bool> InitializeAsync(CT cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return true;
            }

            var (_isCanceled, _) = await Addressables
                .InitializeAsync(true)
                .ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true
                )
                .SuppressCancellationThrow();

            if (_isCanceled)
            {
                return true;
            }

            return false;
        }

        public async UniTask<(bool IsCanceled, AssetHandle<T> Handle)> LoadAsync<T>(
            AssetIdentifier key
            , CT cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return (true, default);
            }

            if (m_cacheTable.TryGetValue(key, out var _cache))
            {
                return (false, new(key, (T)_cache));
            }

            if (false == m_pathTable.TryGetValue(key, out var _path))
            {
                return (true, default);
            }

            var (_isCanceled, _result) = await Addressables
                .LoadAssetAsync<T>(_path)
                .ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true
                )
                .SuppressCancellationThrow();

            if (_isCanceled)
            {
                return (true, default);
            }

            if (false == m_cacheTable.TryAdd(key, _result))
            {
                // TODO: マルチスレッドで問題起きそうなので対策を入れる
            }

            return (false, new(key, _result));
        }

        public void Release(AssetHandle handle)
        {
            if (false == m_cacheTable.TryGetValue(handle.Id, out var _value))
            {
                // TODO: track disposed handle disposing
                return;
            }

            Addressables.Release(_value);
        }
    }
}
