using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace neuranx.Infrastructure.Persistence.Extensions
{
    public static class ModelBuilderExtension
    {
        public static void ApplyAllConfiguration(this ModelBuilder modelBuilder)
        {
            var typeOfRegister = Assembly.GetExecutingAssembly().GetTypes()
                 .Where(t => t.GetInterfaces().Any(gi => gi.IsGenericType &&
                 gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))).ToList();
            foreach (var type in typeOfRegister)
            {
                dynamic instance = Activator.CreateInstance(type) ??
                                   throw new InvalidOperationException($"Unable to instance type {type.FullName}");
                modelBuilder.ApplyConfiguration(instance);
            }
        }
    }
}
