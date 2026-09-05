using System;
using System.Linq;
using System.Reflection;
using Chat.Domain.Common;
using Chat.Domain.Rooms;

namespace Chat.ArchitectureTests;

public sealed class LayeringTests
{
    private static readonly Assembly DomainAssembly = typeof(ChatRoom).Assembly;
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Chat.Application");

    [Fact]
    public void Domain_ShouldNotDependOn_OtherLayers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Chat.Application",
                "Chat.Infrastructure",
                "Chat.Api",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore"
            ).GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrEFCore()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Chat.Infrastructure",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore"
            )
            .GetResult();
        
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_ShouldBe_Sealed()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .BeSealed()
            .GetResult();
        
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEntities_ShouldHaveNoPublicSetters()
    {
        var entityTypes = DomainAssembly.GetTypes()
            .Where(IsDomainEntity);

        var offenders = entityTypes
            .SelectMany(t => t.GetProperties(
                BindingFlags.Public | BindingFlags.Instance))
            .Where(p => p.SetMethod is { IsPublic: true })
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name}")
            .ToList();
        
        offenders.Should().BeEmpty();
    }

    private static bool IsDomainEntity(Type type)
    {
        for (Type? t = type.BaseType; t is not null; t = t.BaseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Entity<>))
            {
                return true;
            }
        }
        
        return false;
    }
}