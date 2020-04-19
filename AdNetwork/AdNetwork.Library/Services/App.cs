using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdNetwork.Library.Contracts;
using AdNetwork.Library.Models;

namespace AdNetwork.Library.Services
{
    public class App
    {
        public void Register(IAdService service)
        {
            services.Add(service);
        }

        public async Task<string> Get(Criteria criteria)
        {
            var tasks = services.Select(it => InvokeWithTimeout(it.GetOffer(criteria), Constants.MAX_DELAY));
            var responses = await Task.WhenAll(tasks).ConfigureAwait(false);
            return responses
                .Where(it => it != null)
                .OrderBy(it => it.Amount)
                .Select(it => it.Html)
                .DefaultIfEmpty(Constants.DEFAULT_HTML)
                .First();
        }

        //

        private readonly List<IAdService> services = new List<IAdService>();

        // based on https://stackoverflow.com/a/11191070/31793
        private static async Task<T> InvokeWithTimeout<T>(Task<T> task, int timeout) =>
            await Task.WhenAny(task, GetDelay<T>(timeout)).Unwrap().ConfigureAwait(false);

        private static async Task<T> GetDelay<T>(int delay)
        {
            await Task.Delay(delay).ConfigureAwait(false);
            return default;
        }
    }
}