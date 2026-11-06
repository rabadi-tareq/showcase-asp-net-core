namespace ConfigureProblemDetailsGlobally.Infrastructure.ErrorHandling
{
    public static class ExceptionHandlerExtensions
    {
        public static IServiceCollection AddApiExceptionHandlers(this IServiceCollection services)
        {
            // Order is critical
            
            services.AddExceptionHandler<DomainExceptionHandler>();
            services.AddExceptionHandler<TimeoutExceptionHandler>();
            services.AddExceptionHandler<GlobalCrashExceptionHandler>();

            return services;
        }
    }
}
