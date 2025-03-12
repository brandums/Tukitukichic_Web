using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopOnline.Models;
using System.Text.Json;

namespace ShopOnline.DataBaseContext
{
    public class PrincipalStructConfiguration : IEntityTypeConfiguration<PrincipalStruct>
    {
        public void Configure(EntityTypeBuilder<PrincipalStruct> builder)
        {
            builder.Property(p => p.Opiniones)
            .HasColumnName("OpinionesData")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<Opiniones[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Substructs)
            .HasColumnName("SubstructsData")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<Substruct[]>(v, new JsonSerializerOptions { })
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

            builder.Property(p => p.Data5)
            .HasColumnName("Data5")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data6)
            .HasColumnName("Data6")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data7)
            .HasColumnName("Data7")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data8)
            .HasColumnName("Data8")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data9)
            .HasColumnName("Data9")
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data10)
            .HasColumnName("Data10")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<double[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data11)
            .HasColumnName("Data11")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<double[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data12)
            .HasColumnName("Data12")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<double[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data13)
            .HasColumnName("Data13")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data14)
            .HasColumnName("Data14")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data15)
            .HasColumnName("Data15")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data16)
            .HasColumnName("Data16")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data17)
            .HasColumnName("Data17")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );

            builder.Property(p => p.Data18)
            .HasColumnName("Data18")
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                v => JsonSerializer.Deserialize<int[]>(v, new JsonSerializerOptions { })
            );
        }
    }
}
