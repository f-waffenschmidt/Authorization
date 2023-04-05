using System.Security.Claims;
using FluentAssertions;
using Fwaqo.Authorization.Core;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Tests;

public class UnitTest1
{
    [Fact]
    public async Task AuthorizationCache_GetKey_Returns_Value()
    {
        //Arrange
        var claim = new Claim("type", "value");
        var identity = new ClaimsIdentity(new[] { claim });
        var cache = new AuthorizationCache(new MemoryCache(new MemoryCacheOptions()));
        
        //Act
        await cache.SetAsync("my-key", identity, TimeSpan.FromHours(1));
        var valueFromCache = await cache.GetAsync("my-key");
        
        //Assert
        valueFromCache.Should().NotBeNull();
        valueFromCache.Claims.Should().HaveCount(1);
        valueFromCache.Claims.First().Type.Should().Be(claim.Type);

    }
}