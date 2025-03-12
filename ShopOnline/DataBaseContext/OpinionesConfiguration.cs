using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopOnline.Models;
using System.Text.Json;

namespace ShopOnline.DataBaseContext
{
    public class OpinionesConfiguration : IEntityTypeConfiguration<Opiniones>
    {
        public void Configure(EntityTypeBuilder<Opiniones> builder)
        {
            builder.Property(p => p.Codigo)
                .HasColumnName("Codigo")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Calificacion)
                .HasColumnName("Calificacion")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Comentario)
                .HasColumnName("Comentario")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Data1)
                .HasColumnName("Data1")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Data2)
                .HasColumnName("Data2")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Data3)
                .HasColumnName("Data3")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Data4)
                .HasColumnName("Data4")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );
        }
    }
}
