using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace NotBura.Packages
{
    public static class ListRequestExtensions
    {
        public static async ValueTask<T> ToValueTask<T>(this Request<T> request)
        {
            while (request.Status == StatusCode.InProgress)
            {
                await Task.Yield();
            }

            return request.Result;
        }
    }
}
