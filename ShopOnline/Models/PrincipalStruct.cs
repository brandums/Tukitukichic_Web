using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ShopOnline.Models
{
    public class PrincipalStruct
    {
        [Key]
        public int Id { get; set; }
        public string? Canal { get; set; } // acountnumber
        public Substruct[]? Substructs { get; set; }
        public Opiniones[]? Opiniones { get; set; }
        // otra substruct[] 
        public string[]? Data1 { get; set; } // productos
        public string[]? Data2 { get; set; } // precios
        public string[]? Data3 { get; set; } // unidad
        public string[]? Data4 { get; set; }
        public string[]? Data5 { get; set; }
        public string[]? Data6 { get; set; }
        public string[]? Data7 { get; set; }
        public string[]? Data8 { get; set; }
        public string[]? Data9 { get; set; }
        //string hasta 20
        public double[]? Data10 { get; set; }
        public double[]? Data11 { get; set; }
        public double[]? Data12 { get; set; }
        //doubles hasta el 30 total 10
        public int[]? Data13 { get; set; }
        public int[]? Data14 { get; set; }
        public int[]? Data15 { get; set; }
        public int[]? Data16 { get; set; }
        public int[]? Data17 { get; set; }
        public int[]? Data18 { get; set; }
        //int hasta 45

        public PrincipalStruct()
        {
            Canal = "Shopping Online";
            Substructs = new Substruct[1];
            Opiniones = new Opiniones[1];
            Data1 = new string[1];
            Data2 = new string[1];
            Data3 = new string[1];
            Data4 = new string[1];
            Data5 = new string[1];
            Data6 = new string[3] { "0", "0", "0" };
            Data7 = new string[1];
            Data8 = new string[1];
            Data9 = new string[1];
            Data10 = new double[1];
            Data11 = new double[1];
            Data12 = new double[1];
            Data13 = new int[1];
            Data14 = new int[1];
            Data15 = new int[1];
            Data16 = new int[1];
            Data17 = new int[1];
            Data18 = new int[1];
        }

        public void ResizeSubstruct(int index, string propiedad)
        {
            PropertyInfo[] properties = typeof(Substruct).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != propiedad)
                {
                    if (property.PropertyType == typeof(string[]))
                    {
                        string[] currentValue = (string[])property.GetValue(Substructs[index]);

                        string[] newValue = new string[currentValue.Length + 1];
                        Array.Copy(currentValue, newValue, currentValue.Length);

                        property.SetValue(Substructs[index], newValue);
                    }
                    else if (property.PropertyType == typeof(string[][]))
                    {
                        string[][] currentValue = (string[][])property.GetValue(Substructs[index]);

                        string[][] newValue = new string[currentValue.Length + 1][];
                        Array.Copy(currentValue, newValue, currentValue.Length);

                        property.SetValue(Substructs[index], newValue);
                    }
                }
            }
        }

        public void CreateNewProduct(Substruct substruct)
        {
            int tam = this.Substructs[0].Nombre.Length - 1;
            this.Substructs[0].Nombre[tam] = substruct.Nombre[0];
            this.Substructs[0].Precio[tam] = substruct.Precio[0];
            this.Substructs[0].BreveDescripcion[tam] = substruct.BreveDescripcion[0];
            this.Substructs[0].Descripcion[tam] = substruct.Descripcion[0];
            this.Substructs[0].Codigo[tam] = GetCode();
            this.Substructs[0].Color[tam] = substruct.Color[0];
            this.Substructs[0].Talla[tam] = substruct.Talla[0];
            this.Substructs[0].Categoria[tam] = substruct.Categoria[0];
            this.Substructs[0].SubCategoria[tam] = substruct.SubCategoria[0];
            this.Substructs[0].Images[tam] = substruct.Images[0];
            this.Substructs[0].Extra1[tam] = substruct.Extra1[0];
            this.Substructs[0].Extra2[tam] = substruct.Extra2[0];
            this.Substructs[0].Extra3[tam] = "1";
            this.Substructs[0].Extra4[tam] = ((int)Math.Ceiling((double)(Data2.Length + 1) / 10)).ToString();
            this.Substructs[0].Extra5[tam] = "0";
            this.Substructs[0].Extra6[tam] = "0";
            this.Substructs[0].Extra7[tam] = substruct.Extra7[0];
            this.Substructs[0].Extra8[tam] = "0";
            this.Substructs[0].TiempoOferta[tam] = "0";
            this.Substructs[0].Ventas[tam] = "0";
            this.Substructs[0].VentasBase[tam] = "0";
            this.Substructs[0].LikesBase[tam] = "0";
        }

        private string GetCode()
        {
            Random rand = new Random();
            string code;

            do
            {
                code = rand.Next(10000, 100000).ToString();
            } while (Substructs[0].Codigo.Any(u => u == code));

            return code;
        }

        public void RemoveProductFromSubstruct(int index, int productIndex, string color)
        {
            string exception = "";
            if (index != 0)
            {
                if (color == "hola")
                {
                    productIndex = Array.FindIndex(Substructs[index].Nombre, x => x == productIndex.ToString());
                }
                else
                {
                    for (int i = 0; i < Substructs[index].Nombre.Length; i++)
                    {
                        if (Substructs[index].Nombre[i] == productIndex.ToString() &&
                            Substructs[index].Color[i][0] == color)
                        {
                            productIndex = i;
                            break;
                        }
                    }
                }
                exception = "Extra7";
            }

            PropertyInfo[] properties = typeof(Substruct).GetProperties();

            if (productIndex >= 0)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != exception)
                    {
                        if (property.PropertyType == typeof(string[]))
                        {
                            string[] currentValue = (string[])property.GetValue(Substructs[index]);

                            List<string> newValueList = new List<string>(currentValue);

                            newValueList.RemoveAt(productIndex);
                            property.SetValue(Substructs[index], newValueList.ToArray());
                        }
                        else if (property.PropertyType == typeof(string[][]))
                        {
                            string[][] currentValue = (string[][])property.GetValue(Substructs[index]);

                            List<string[]> newValueList = new List<string[]>(currentValue);
                            newValueList.RemoveAt(productIndex);

                            property.SetValue(Substructs[index], newValueList.ToArray());
                        }
                    }
                }
            }
        }

        public async Task newProductToCart(int i, int productId, List<string> tallas, List<string> colors, List<string> images, int cant)
        {
            Substructs[i].Nombre[Substructs[i].Nombre.Length - 1] = productId.ToString();
            Substructs[i].Talla[Substructs[i].Nombre.Length - 1] = tallas.ToArray();
            Substructs[i].Color[Substructs[i].Nombre.Length - 1] = colors.ToArray();
            Substructs[i].Images[Substructs[i].Nombre.Length - 1] = images.ToArray();
            Substructs[i].Extra1[Substructs[i].Nombre.Length - 1] = cant.ToString();
            Substructs[i].Codigo[Substructs[i].Nombre.Length - 1] = Substructs[0].Codigo[productId];
            Substructs[i].Extra2[Substructs[i].Nombre.Length - 1] = Substructs[0].Extra2[productId];
        }

        public void refreshPages()
        {
            for (int j = 0; j < Substructs[0].Nombre.Length; j++)
            {
                var indiceProducto = Array.IndexOf(Data2, Substructs[0].Codigo[j]);

                Substructs[0].Extra4[j] = ((int)Math.Ceiling((double)(indiceProducto + 1) / 10)).ToString();
            }
        }

        public void refreshPositions()
        {
            for (int j = 0; j < Substructs[0].Nombre.Length; j++)
            {
                int indiceProducto = -1;
                if (Data2.Contains(Substructs[0].Codigo[j]))
                {
                    indiceProducto = Array.IndexOf(Data2, Substructs[0].Codigo[j]);
                    indiceProducto = int.Parse(indiceProducto.ToString().Substring(indiceProducto.ToString().Length - 1));
                }

                Substructs[0].Extra5[j] = (indiceProducto + 1).ToString();
            }
        }

        public void refreshPromo()
        {
            for (int j = 0; j < Substructs[0].Nombre.Length; j++)
            {
                var indiceProducto = Array.IndexOf(Data3, Substructs[0].Codigo[j]);

                Substructs[0].Extra6[j] = (indiceProducto != -1) ? "1" : "0";
            }
        }

        public Substruct getProducts(List<int> indexes, int userIndex)
        {
            PropertyInfo[] properties = typeof(Substruct).GetProperties();
            var filteredSubstruct = new Substruct();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string[]))
                {
                    var propertyList = new List<string>();

                    foreach (var index in indexes)
                    {
                        var value = property.GetValue(Substructs[userIndex], new object[] { });
                        var arrayValue = (string[])value;

                        propertyList.Add(arrayValue[index]);
                    }

                    var propertyArray = propertyList.ToArray();
                    property.SetValue(filteredSubstruct, propertyArray);
                }
                else if (property.PropertyType == typeof(string[][]))
                {
                    var propertyList = new List<string[]>();

                    foreach (var index in indexes)
                    {
                        var value = property.GetValue(Substructs[userIndex], new object[] { });
                        var arrayValue = (string[][])value;

                        propertyList.Add(arrayValue[index]);
                    }

                    var propertyArray = propertyList.ToArray();
                    property.SetValue(filteredSubstruct, propertyArray);
                }
            }

            return filteredSubstruct;
        }
    }
}
