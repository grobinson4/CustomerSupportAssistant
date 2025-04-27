using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerSupportAssistant.Domain.Entities;

namespace CustomerSupportAssistant.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Property(u => u.HashedPassword).IsRequired();
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
    }
}
