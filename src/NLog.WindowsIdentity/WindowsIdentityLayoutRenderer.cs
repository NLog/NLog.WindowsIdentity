//
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
//
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Jaroslaw Kowalski nor the names of its
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//

namespace NLog.LayoutRenderers
{
    using System.Security.Principal;
    using System.Text;

    /// <summary>
    /// Thread Windows identity information (username).
    /// </summary>
    [LayoutRenderer("windows-identity")]
    public class WindowsIdentityLayoutRenderer : LayoutRenderer
    {
#if !NET35
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, CachedIdentity> _identityNameCache = new();
        private static long _cacheVersion;
#endif

        private string _defaultIdentityName = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether domain name should be included.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool Domain { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether username should be included.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public bool UserName { get; set; } = true;

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            using (var identity = WindowsIdentity.GetCurrent(ifImpersonating: false))
            {
                _defaultIdentityName = identity?.Name ?? string.Empty;
            }

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var identityName = GetIdentityName();
            if (UserName && Domain)
            {
                builder.Append(identityName);
            }
            else if (UserName)
            {
                int pos = identityName.LastIndexOf('\\');
                if (pos >= 0)
                    builder.Append(identityName, pos + 1, identityName.Length - (pos + 1));
                else
                    builder.Append(identityName);
            }
            else if (Domain)
            {
                int pos = identityName.IndexOf('\\');
                if (pos >= 0)
                    builder.Append(identityName, 0, pos);
                else
                    builder.Append(identityName);
            }
        }

        private string GetIdentityName()
        {
            // Fast path: most applications never impersonate.
            using var identity = WindowsIdentity.GetCurrent(ifImpersonating: true);
            if (identity is null)
                return _defaultIdentityName;

#if !NET35
            string? sid = identity.User?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(sid))
            {
                if (_identityNameCache.TryGetValue(sid, out var cachedIdentity))
                    return cachedIdentity.Name;

                string identityName = identity.Name;
                long insertVersion = System.Threading.Interlocked.Increment(ref _cacheVersion);
                cachedIdentity = _identityNameCache.GetOrAdd(sid, new CachedIdentity(identityName, insertVersion));
                if (_identityNameCache.Count > 128)
                    TrimCache(insertVersion - 64);
                return cachedIdentity.Name;
            }
#endif
            return identity.Name;            
        }

#if !NET35
        private static void TrimCache(long minimumVersion)
        {
            foreach (var entry in _identityNameCache)
            {
                if (entry.Value.Version < minimumVersion)
                {
                    _identityNameCache.TryRemove(entry.Key, out _);
                }
            }
        }


        private readonly struct CachedIdentity
        {
            public CachedIdentity(string name, long version)
            {
                Name = name;
                Version = version;
            }

            public string Name { get; }

            public long Version { get; }
        }
#endif
    }

}
