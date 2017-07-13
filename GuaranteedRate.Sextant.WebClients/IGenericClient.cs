using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GuaranteedRate.Sextant.WebClients
{
    public interface IGenericClient
    {
        string BaseAddress { get; set; }
        T Get<T>(string resource);
        Task<T> GetAsync<T>(string resource, CancellationToken? cancellationToken = null);
        TOut Post<TOut>(string resource);
        void Post<TIn>(string resource, TIn request);
        TOut Post<TIn, TOut>(string resource, TIn request);
        Task<TOut> PostAsync<TOut>(string resource, CancellationToken? cancellationToken = null);
        Task<TOut> PostAsync<TIn, TOut>(string resource, TIn request, CancellationToken? cancellationToken = null);
        TOut Put<TOut>(string resource);
        TOut Put<TIn, TOut>(string resource, TIn request);
        Task<TOut> PutAsync<TOut>(string resource, CancellationToken? cancellationToken = null);
        Task<TOut> PutAsync<TIn, TOut>(string resource, TIn request, CancellationToken? cancellationToken = null);
        T Delete<T>(string resource);
        Task<T> DeleteAsync<T>(string resource, CancellationToken? cancellationToken = null);
    }
}
