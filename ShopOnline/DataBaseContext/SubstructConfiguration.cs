using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopOnline.Models;
using System.Text.Json;

namespace ShopOnline.DataBaseContext
{
    public class SubstructConfiguration : IEntityTypeConfiguration<Substruct>
    {
        public void Configure(EntityTypeBuilder<Substruct> builder)
        {
            builder.Property(p => p.Nombre)
                .HasColumnName("Nombre")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Precio)
                .HasColumnName("Precio")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.BreveDescripcion)
                .HasColumnName("BreveDescripcion")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Descripcion)
                .HasColumnName("Descripcion")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Codigo)
                .HasColumnName("Codigo")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Color)
                .HasColumnName("Color")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[][]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Talla)
                .HasColumnName("Talla")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[][]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Categoria)
                .HasColumnName("Categoria")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.SubCategoria)
                .HasColumnName("SubCategoria")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Images)
                .HasColumnName("Images")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[][]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra1)
                .HasColumnName("Extra1")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra2)
                .HasColumnName("Extra2")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra3)
                .HasColumnName("Extra3")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra4)
                .HasColumnName("Extra4")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra5)
                .HasColumnName("Extra5")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra6)
                .HasColumnName("Extra6")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra7)
                .HasColumnName("Extra7")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Extra8)
                .HasColumnName("Extra8")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.TiempoOferta)
                .HasColumnName("TiempoOferta")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.Ventas)
                .HasColumnName("Ventas")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.VentasBase)
                .HasColumnName("VentasBase")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );

            builder.Property(p => p.LikesBase)
                .HasColumnName("LikesBase")
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { }),
                    v => JsonSerializer.Deserialize<string[]>(v, new JsonSerializerOptions { })
                );
        }
    }
}
