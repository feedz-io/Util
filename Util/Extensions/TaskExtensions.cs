using System.Threading.Tasks;

namespace Feedz.Util.Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> AsTaskResult<T>(this T result)
            => Task.FromResult(result);
    }
}