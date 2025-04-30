using System.Collections.Concurrent;

namespace StudyFlowArena.API.Services
{
    public class TokenBlackListService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void BlackListToken(string token, DateTime expiry)
        {
            _blacklistedTokens.TryAdd(token, expiry);
        }

        public bool IsTokenBlacklisted(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out var expiry))
            {
                // Remove if the token expires
                if (DateTime.UtcNow > expiry)
                {
                    _blacklistedTokens.TryRemove(token, out _);
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
