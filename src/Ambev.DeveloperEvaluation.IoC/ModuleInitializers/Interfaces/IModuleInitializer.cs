using Microsoft.AspNetCore.Builder;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers.Interfaces;

public interface IModuleInitializer
{
    void Initialize(
        WebApplicationBuilder builder);
}