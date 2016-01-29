// Coded by Alex Danvy
// http://danvy.tv
// http://twitter.com/danvy
// http://github.com/danvy
// Licence Apache 2.0
// Use at your own risk, have fun

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KloutSharp.Lib
{
    public class Klout : IDisposable
    {
        private const string kloutUri = "http://api.klout.com/v2/";
        private static readonly string kloutIdentityUri = kloutUri + "identity.json/";
        private string key;
				private HttpClient _HttpClient;

        public Klout(string key) : this(key, CreateDefaultHttpClient())
        {
        }
				public Klout(string key, HttpClient httpClient) 
				{
					if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

					this.key = key;
					_HttpClient = httpClient;
				}
				private static HttpClient CreateDefaultHttpClient()
				{
					var handler = new System.Net.Http.HttpClientHandler();
					if (handler.SupportsAutomaticDecompression)
						handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
					
					var retVal = new HttpClient(handler);
					return retVal;
				}
				private void CheckKey()
        {
            if (string.IsNullOrEmpty(key))
                throw new KloutException("Klout key not set!");
        }
				private void CheckIsDisposed()
				{
					if (_HttpClient == null) throw new ObjectDisposedException(nameof(Klout));
				}
				private string UriAddKey(string uri)
        {
            return string.Format("{0}{2}key={1}", uri, key, uri.Contains("?") ? "&" : "?");
        }
        public async Task<KloutIdentity> IdentityAsync(string id, KloutIdentityKind kind = KloutIdentityKind.TwitterScreenName)
        {
            CheckKey();
						CheckIsDisposed();
						var parmeters = string.Empty;
            switch (kind)
            {
                case KloutIdentityKind.TwitterId:
                    {
                        parmeters = "tw/{0}";
                        break;
                    }
                case KloutIdentityKind.Google:
                    {
                        parmeters = "gp/{0}";
                        break;
                    }
                case KloutIdentityKind.Instagram:
                    {
                        parmeters = "ig/{0}";
                        break;
                    }
                case KloutIdentityKind.TwitterScreenName:
                    {
                        parmeters = "twitter?screenName={0}";
                        break;
                    }
                case KloutIdentityKind.KloutId:
                    {
                        parmeters = "klout/{0}/tw";
                        break;
                    }
            }
            var uri = UriAddKey(string.Format(kloutIdentityUri + parmeters, id));
            var res = await _HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var identity = JsonConvert.DeserializeObject<KloutIdentity>(content);
                return identity;
            }
            else
            {
                throw new KloutException(res.StatusCode);
            }
        }
        public async Task<KloutIdentity> IdentityTwitterIdAsync(string id)
        {
            return await IdentityAsync(id, KloutIdentityKind.TwitterId).ConfigureAwait(false);
        }
        public async Task<KloutIdentity> IdentityGoogle(string googleId)
        {
            return await IdentityAsync(googleId, KloutIdentityKind.Google).ConfigureAwait(false);
        }
        public async Task<KloutIdentity> IdentityInstagram(string instagramId)
        {
            return await IdentityAsync(instagramId, KloutIdentityKind.Instagram).ConfigureAwait(false);
        }
        public async Task<KloutIdentity> IdentityTwitterScreenName(string twitter)
        {
            return await IdentityAsync(twitter, KloutIdentityKind.Instagram).ConfigureAwait(false);
        }
        public async Task<KloutIdentity> IdentityKlout(string kloutId)
        {
            return await IdentityAsync(kloutId, KloutIdentityKind.KloutId).ConfigureAwait(false);
        }
        public async Task<KloutUser> UserAsync(string kloutId)
        {
            CheckKey();
						CheckIsDisposed();
						var uri = UriAddKey(string.Format(kloutUri + "user.json/{0}", kloutId));
            var res = await _HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var user = JsonConvert.DeserializeObject<KloutUser>(content);
                return user;
            }
            else
            {
                throw new KloutException(res.StatusCode);
            }
        }
        public async Task<KloutScore> ScoreAsync(string kloutId)
        {
            CheckKey();
						CheckIsDisposed();
						var uri = UriAddKey(string.Format(kloutUri + "user.json/{0}/score", kloutId));
            var res = await _HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var score = JsonConvert.DeserializeObject<KloutScore>(content);
                return score;
            }
            else
            {
                throw new KloutException(res.StatusCode);
            }
        }
        public async Task<List<KloutTopic>> TopicsAsync(string kloutId)
        {
            CheckKey();
						CheckIsDisposed();
						var uri = UriAddKey(string.Format(kloutUri + "user.json/{0}/topics", kloutId));
            var res = await _HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var topics = JsonConvert.DeserializeObject<List<KloutTopic>>(content);
                return topics;
            }
            else
            {
                throw new KloutException(res.StatusCode);
            }
        }
        public async Task<KloutInfluence> InfluenceAsync(string kloutId)
        {
            CheckKey();
						CheckIsDisposed();
						var uri = UriAddKey(string.Format(kloutUri + "user.json/{0}/influence", kloutId));
            var res = await _HttpClient.GetAsync(uri).ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var influence = JsonConvert.DeserializeObject<KloutInfluence>(content);
                return influence;
            }
            else
            {
                throw new KloutException(res.StatusCode);
            }
        }
        public void Dispose()
				{
					try
					{
						Dispose(true);
					}
					finally
					{
						GC.SuppressFinalize(this);
					}
				}
	      protected virtual void Dispose(bool isDisposing)
				{
					if (isDisposing)
					{
						if (_HttpClient != null)
						{
							_HttpClient.Dispose();
							_HttpClient = null;
						}
					}
				}
		}
}