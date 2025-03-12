using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopOnline.DataBaseContext;
using ShopOnline.Models;
using System.Reflection;
using System.Xml.Serialization;

namespace ShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PrincipalStructController : ControllerBase
    {
        private readonly DBaseContext _context;
        private UserController _userController;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public PrincipalStructController(DBaseContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userController = new UserController(_context, webHostEnvironment);
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrincipalStruct>>> GetPrincipalStructs()
        {
            return await _context.PrincipalStructs.ToListAsync();
        }

        [HttpGet("discount")]
        public async Task<ActionResult<List<decimal>>> GetDescuentos()
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound("No se encontró la estructura principal.");
            }

            return Ok(principalStruct.Data6);
        }

        [HttpPut("discount/{index}/{value}")]
        public async Task<IActionResult> UpdateDescuento(int index, int value)
        {
            if (index < 0 || index >= 3)
            {
                return BadRequest("Índice fuera de rango. Solo se permiten valores de 0 a 2.");
            }

            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound("No se encontró la estructura principal.");
            }

            // Actualizar solo el descuento correspondiente
            principalStruct.Data6[index] = value.ToString();
            _context.Entry(principalStruct).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(principalStruct.Data6[index]);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrincipalStruct>> GetPrincipalStruct(int id)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(id);

            if (principalStruct == null)
            {
                return NotFound();
            }

            return principalStruct;
        }

        [HttpGet("getCategory")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null || string.IsNullOrEmpty(principalStruct.Data8[0]))
            {
                return NotFound("No categories found.");
            }

            var categories = JsonConvert.DeserializeObject<List<Category>>(principalStruct.Data8[0]);
            return Ok(categories);
        }

        [HttpGet("updateCategoryName/{oldName}/{newName}")]
        public async Task<IActionResult> UpdateCategoryName(string oldName, string newName)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null || string.IsNullOrEmpty(principalStruct.Data8[0]))
            {
                return NotFound("No categories found.");
            }

            var categories = JsonConvert.DeserializeObject<List<Category>>(principalStruct.Data8[0]);

            var index = categories.FindIndex(c => c.Name == oldName);

            var categoryName = categories[index];
            if (categoryName == null)
            {
                return NotFound($"Categoría no encontrada.");
            }

            foreach (var category in categories)
            {
                if (category.SubCategories != null)
                {
                    foreach (var subCategory in category.SubCategories)
                    {
                        if (subCategory.Name.Equals(categoryName.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            subCategory.Name = newName;
                        }
                    }
                }
            }
            await ChangeSeasonNameInProducts(categoryName.Name, newName);
            categories[index].Name = newName;

            principalStruct.Data8[0] = JsonConvert.SerializeObject(categories);
            _context.PrincipalStructs.Update(principalStruct);
            await _context.SaveChangesAsync();

            return Ok(categories);
        }

        private async Task ChangeSeasonNameInProducts(string oldName, string newName)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();

            for (int i = 0; i < principalStruct.Substructs.Count(); i++)
            {
                for (int j = 0; j < principalStruct.Substructs[i].Nombre.Count(); j++)
                {
                    if (principalStruct.Substructs[i].Categoria[j] == oldName)
                    {
                        principalStruct.Substructs[i].Categoria[j] = newName;
                    }

                    if (principalStruct.Substructs[i].SubCategoria[j] == oldName)
                    {
                        principalStruct.Substructs[i].SubCategoria[j] = newName;
                    }
                }
            }
        }

        [HttpPut("updateSubCategoryStatus/{categoryName}/{subCategoryName}/{newStatus}")]
        public async Task<IActionResult> UpdateSubCategoryStatus(string categoryName, string subCategoryName, bool newStatus)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null || string.IsNullOrEmpty(principalStruct.Data8[0]))
            {
                return NotFound("No categories found.");
            }

            var categories = JsonConvert.DeserializeObject<List<Category>>(principalStruct.Data8[0]);

            var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (category == null)
            {
                return NotFound($"Categoría '{categoryName}' no encontrada.");
            }

            if (category.SubCategories == null || !category.SubCategories.Any())
            {
                return NotFound($"La categoría '{categoryName}' no tiene subcategorías.");
            }

            var subCategory = category.SubCategories.FirstOrDefault(sc => sc.Name.Equals(subCategoryName, StringComparison.OrdinalIgnoreCase));
            if (subCategory == null)
            {
                return NotFound($"Subcategoría '{subCategoryName}' no encontrada en la categoría '{categoryName}'.");
            }

            subCategory.IsEnabled = newStatus;

            principalStruct.Data8[0] = JsonConvert.SerializeObject(categories);
            _context.PrincipalStructs.Update(principalStruct);
            await _context.SaveChangesAsync();

            return Ok(category);
        }


        [HttpGet("getBySearch/{text}")]
        public async Task<ActionResult<Substruct>> GetBySearch(string text)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            var filteredSubstruct = new Substruct();
            List<int> indexes = new List<int>();

            if (principalStruct != null)
            {
                indexes = Enumerable.Range(0, principalStruct.Substructs[0].Nombre.Length)
                                        .Where(i => principalStruct.Substructs[0].Nombre[i].ToLower().Contains(text.ToLower()))
                                        .Take(10)
                                        .ToList();

                filteredSubstruct = principalStruct.getProducts(indexes, 0);

                return filteredSubstruct;
            }

            return BadRequest();
        }


        [HttpGet("substruct/{id}")]
        public async Task<ActionResult<Substruct>> GetSubStruct(int id)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);

            if (principalStruct != null)
            {
                for (int i = 0; i < principalStruct.Substructs.Count(); i++)
                {
                    if (principalStruct.Data9[i] == id.ToString())
                    {
                        Substruct substruct = principalStruct.Substructs[i];

                        if (id != 1)
                        {
                            for (int j = 0; j < substruct.Nombre.Length; j++)
                            {
                                int indiceAux = int.Parse(substruct.Nombre[j]);
                                substruct.Nombre[j] = principalStruct.Substructs[0].Nombre[indiceAux];
                                //substruct.Precio[j] = principalStruct.Substructs[0].Precio[indiceAux];
                                if (principalStruct.Substructs[0].Extra8[indiceAux] == "1")
                                {
                                    substruct.Precio[j] = principalStruct.Substructs[0].Extra7[indiceAux];
                                }
                                else if (principalStruct.Substructs[0].Extra8[indiceAux] == "0")
                                {
                                    substruct.Precio[j] = principalStruct.Substructs[0].Precio[indiceAux];
                                }

                            }
                        }

                        return substruct;
                    }
                }
            }
            return NotFound();
        }

        [HttpGet("getPurchasedProducts")]
        public async Task<ActionResult<List<Substruct>>> GetPurchasedProducts()
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            List<Substruct> purchasedProducts = new List<Substruct>();

            if (principalStruct != null)
            {
                foreach (var userData in principalStruct.Data5)
                {
                    if (userData != null && userData != "string")
                    {
                        var substructs = JsonConvert.DeserializeObject<List<Substruct>>(userData);
                        purchasedProducts.AddRange(substructs);
                    }
                }
            }
            return purchasedProducts;
        }

        [HttpGet("getProduct/{code}")]
        public async Task<ActionResult<Substruct>> GetProductByCode(string code)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            var filteredSubstruct = new Substruct();
            int index;

            if (principalStruct != null)
            {
                index = Enumerable.Range(0, principalStruct.Substructs[0].Codigo.Length)
                                        .FirstOrDefault(i => principalStruct.Substructs[0].Codigo[i] == code);

                var indexes = new List<int>();
                indexes.Add(index);
                filteredSubstruct = principalStruct.getProducts(indexes, 0);
            }

            return filteredSubstruct;
        }


        [HttpGet("getProductsPerPage/{category}/{page}/{subcategory?}")]
        public async Task<ActionResult<Substruct>> GetProductsPerPages(string category, int page, string? subcategory)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            var filteredSubstruct = new Substruct();
            var categories = JsonConvert.DeserializeObject<List<Category>>(principalStruct.Data8[0]);
            List<int> indexes = new List<int>();

            if (principalStruct != null)
            {
                if (category == "Todo")
                {
                    indexes = Enumerable.Range(0, principalStruct.Substructs[0].Extra4.Length)
                                        .Where(i => principalStruct.Substructs[0].Extra4[i] == page.ToString() && principalStruct.Substructs[0].Extra3[i] == "1")
                                        .ToList();
                }
                else if (category == categories[5].Name)
                {
                    var allMatchingIndexes = Enumerable.Range(0, principalStruct.Substructs[0].Categoria.Length)
                                               .Where(i => principalStruct.Substructs[0].Categoria[i] == category ||
                                                           principalStruct.Substructs[0].SubCategoria[i] == category &&
                                                           principalStruct.Substructs[0].Extra3[i] == "1")
                                               .ToList();

                    int inicio = (page - 1) * 10;
                    int fin = inicio + 10;
                    indexes = allMatchingIndexes.Skip(inicio).Take(fin - inicio).ToList();
                }
                else if (!string.IsNullOrEmpty(subcategory))
                {
                    var allMatchingIndexes = Enumerable.Range(0, principalStruct.Substructs[0].Categoria.Length)
                                               .Where(i => principalStruct.Substructs[0].Categoria[i] == category &&
                                                           principalStruct.Substructs[0].SubCategoria[i] == subcategory &&
                                                           principalStruct.Substructs[0].Extra3[i] == "1")
                                               .ToList();

                    int inicio = (page - 1) * 10;
                    int fin = inicio + 10;
                    indexes = allMatchingIndexes.Skip(inicio).Take(fin - inicio).ToList();
                }
                else
                {
                    var allMatchingIndexes = Enumerable.Range(0, principalStruct.Substructs[0].Categoria.Length)
                                               .Where(i => principalStruct.Substructs[0].Categoria[i] == category &&
                                                           principalStruct.Substructs[0].Extra3[i] == "1")
                                               .ToList();

                    int inicio = (page - 1) * 10;
                    int fin = inicio + 10;
                    indexes = allMatchingIndexes.Skip(inicio).Take(fin - inicio).ToList();
                }

                filteredSubstruct = principalStruct.getProducts(indexes, 0);
            }

            return filteredSubstruct;
        }



        [HttpGet("getProductsPerPromo")]
        public async Task<ActionResult<Substruct>> GetProductsPerPromo()
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            var filteredSubstruct = new Substruct();

            if (principalStruct != null)
            {
                var indexes = Enumerable.Range(0, principalStruct.Substructs[0].Extra4.Length)
                                        .Where(i => principalStruct.Substructs[0].Extra6[i] == "1")
                                        .ToList();

                PropertyInfo[] properties = typeof(Substruct).GetProperties();

                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string[]))
                    {
                        var propertyList = new List<string>();

                        foreach (var index in indexes)
                        {
                            var value = property.GetValue(principalStruct.Substructs[0], new object[] { });
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
                            var value = property.GetValue(principalStruct.Substructs[0], new object[] { });
                            var arrayValue = (string[][])value;

                            propertyList.Add(arrayValue[index]);
                        }

                        var propertyArray = propertyList.ToArray();
                        property.SetValue(filteredSubstruct, propertyArray);
                    }
                }
            }

            return filteredSubstruct;
        }

        [HttpGet("getPages/{category}/{subcategory?}")]
        public async Task<ActionResult<int>> GetPages(string category, string subcategory = null)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            var categories = JsonConvert.DeserializeObject<List<Category>>(principalStruct.Data8[0]);

            if (principalStruct != null)
            {
                if (category == "Todo")
                {
                    return int.Parse(principalStruct.Data1[0]);
                }
                else if (category == categories[5].Name)
                {
                    var indexes = Enumerable.Range(0, principalStruct.Substructs[0].Categoria.Length)
                                               .Where(i => principalStruct.Substructs[0].Categoria[i] == category ||
                                                           principalStruct.Substructs[0].SubCategoria[i] == category &&
                                                           principalStruct.Substructs[0].Extra3[i] == "1")
                                               .ToList();

                    return (int)Math.Ceiling((double)indexes.Count() / 10);
                }
                else if (!string.IsNullOrEmpty(subcategory))
                {
                    // Filtrar por subcategoría si existe
                    var indexes = Enumerable.Range(0, principalStruct.Substructs[0].Extra4.Length)
                                            .Where(i => principalStruct.Substructs[0].Categoria[i] == category &&
                                                        principalStruct.Substructs[0].SubCategoria[i] == subcategory)
                                            .ToList();

                    return (int)Math.Ceiling((double)indexes.Count() / 10);
                }
                else
                {
                    // Filtrar solo por categoría si no hay subcategoría
                    var indexes = Enumerable.Range(0, principalStruct.Substructs[0].Extra4.Length)
                                            .Where(i => principalStruct.Substructs[0].Categoria[i] == category)
                                            .ToList();

                    return (int)Math.Ceiling((double)indexes.Count() / 10);
                }
            }

            return NotFound();
        }


        [HttpGet("getPagePerCategory/{productCode}")]
        public async Task<ActionResult<int[]>> getPagePerCategory(string productCode)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
            if (principalStruct != null && productIndex != null)
            {
                var indexes = Enumerable.Range(0, principalStruct.Substructs[0].Extra4.Length)
                                        .Where(i => principalStruct.Substructs[0].Categoria[i] == principalStruct.Substructs[0].Categoria[productIndex])
                                        .ToList();

                int indice = indexes.FindIndex(i => i == productIndex);
                int page = ((int)Math.Ceiling((double)(indice + 1) / 10));
                int[] positions = new int[] { indice, page };
                return positions;
            }

            return NotFound();
        }

        [HttpGet("getImages")]
        public async Task<ActionResult<IEnumerable<string>>> GetImages()
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();

            return principalStruct.Data7;
        }

        [HttpGet("getSavedProduct/{userId}/{productCode}")]
        public async Task<ActionResult<int>> getSavedProduct(int userId, int productCode)
        {
            bool existing = false;
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
            for (int i = 0; i < principalStruct.Substructs.Count(); i++)
            {
                if (principalStruct.Data9[i] == userId.ToString())
                {
                    existing = principalStruct.Substructs[i].Extra7.Contains(productIndex.ToString());
                }
            }

            return (existing) ? 1 : 0;
        }

        [HttpGet("alreadyInCart/{userId}/{productCode}/{color}")]
        public async Task<ActionResult<int>> AlreadyInCart(int userId, int productCode, string color)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());

            for (int i = 0; i < principalStruct.Substructs.Count(); i++)
            {
                if (principalStruct.Data9[i] == userId.ToString())
                {
                    for (int j = 0; j < principalStruct.Substructs[i].Nombre.Length; j++)
                    {
                        if (principalStruct.Substructs[i].Nombre[j] == productIndex.ToString())
                        {
                            if (principalStruct.Substructs[i].Color[j][0] == color)
                            {
                                return 1;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        [HttpGet("addProductToUser/{userId}/{productCode}/{cant}/{talla}/{color}")]
        public async Task<ActionResult> AddProductToUser(int userId, int productCode, int cant, string talla, string color)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int productId = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
                int colorIndex = Array.FindIndex(principalStruct.Substructs[0].Color[productId], x => x == color) + 5;
                List<string> tallas = new List<string> { talla };
                List<string> colors = new List<string> { color };
                List<string> images = new List<string> { principalStruct.Substructs[0].Images[productId][colorIndex] };

                if (principalStruct != null)
                {
                    for (int i = 0; i < principalStruct.Substructs.Count(); i++)
                    {
                        if (principalStruct.Data9[i] == userId.ToString())
                        {
                            if (principalStruct.Substructs[i].Nombre.Length == 0)
                            {
                                principalStruct.ResizeSubstruct(i, "Extra7");
                                await principalStruct.newProductToCart(i, productId, tallas, colors, images, cant);
                            }
                            else if (principalStruct.Substructs[i].Nombre[principalStruct.Substructs[i].Nombre.Length - 1] == null)
                            {
                                if (!principalStruct.Substructs[i].Nombre.Contains(productId.ToString()))
                                {
                                    await principalStruct.newProductToCart(i, productId, tallas, colors, images, cant);
                                }
                            }
                            else
                            {
                                var matchingIndexes = principalStruct.Substructs[i].Nombre
                                    .Select((nombre, index) => new { nombre, index })
                                    .Where(x => x.nombre == productId.ToString())
                                    .Select(x => x.index)
                                    .ToList();

                                bool noMatchingColor = matchingIndexes.All(index => principalStruct.Substructs[i].Color[index][0] != color);

                                if (noMatchingColor)
                                {
                                    principalStruct.ResizeSubstruct(i, "Extra7");
                                    await principalStruct.newProductToCart(i, productId, tallas, colors, images, cant);
                                }
                            }
                            _context.Entry(principalStruct).State = EntityState.Modified;
                            await SaveChangesAndSerializeAsync();

                            return Ok();
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("getLikesPerProduct/{productCode}")]
        public async Task<ActionResult<int>> GetLikesPerProduct(int productCode)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
                List<Substruct> substruct = principalStruct.Substructs.ToList();

                int counter = substruct
                    .SelectMany(p => p.Extra7)
                    .Count(extra => extra == productIndex.ToString());

                int total = counter + int.Parse(principalStruct.Substructs[0].LikesBase[productIndex]);
                return total;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("getNumberOfSales/{productCode}")]
        public async Task<ActionResult<int>> GetNumberOfSales(int productCode)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());

                int ventas = int.Parse(principalStruct.Substructs[0].Ventas[productIndex]);
                int ventasBase = int.Parse(principalStruct.Substructs[0].VentasBase[productIndex]);

                return ventas + ventasBase;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("addFavoriteProduct/{userId}/{productCode}")]
        public async Task<ActionResult> AddFavoriteProduct(int userId, int productCode)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int productId = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
                if (principalStruct != null)
                {
                    for (int i = 0; i < principalStruct.Substructs.Count(); i++)
                    {
                        if (principalStruct.Data9[i] == userId.ToString())
                        {
                            if (principalStruct.Substructs[i].Extra7.Length == 0)
                            {
                                principalStruct.Substructs[i].Extra7 = new string[1];
                                principalStruct.Substructs[i].Extra7[principalStruct.Substructs[i].Extra7.Length - 1] = productId.ToString();
                            }
                            else if (principalStruct.Substructs[i].Extra7[principalStruct.Substructs[i].Extra7.Length - 1] == null)
                            {
                                if (!principalStruct.Substructs[i].Extra7.Contains(productId.ToString()))
                                {
                                    principalStruct.Substructs[i].Extra7[principalStruct.Substructs[i].Extra7.Length - 1] = productId.ToString();
                                }
                            }
                            else
                            {
                                if (!principalStruct.Substructs[i].Extra7.Contains(productId.ToString()))
                                {
                                    List<string> favoritos = principalStruct.Substructs[i].Extra7.ToList();
                                    favoritos.Add(productId.ToString());
                                    principalStruct.Substructs[i].Extra7 = favoritos.ToArray();
                                }
                            }
                            _context.Entry(principalStruct).State = EntityState.Modified;
                            await SaveChangesAndSerializeAsync();

                            return Ok();
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("addProduct")]
        public async Task<ActionResult> addProduct(Substruct substruct)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            principalStruct.ResizeSubstruct(0, "ninguna excepcion");
            principalStruct.CreateNewProduct(substruct);

            principalStruct.Data1[0] = ((int)Math.Ceiling((double)principalStruct.Substructs[0].Nombre.Length / 10)).ToString();
            List<string> codigos = new List<string>(principalStruct.Data2);
            codigos.Add(principalStruct.Substructs[0].Codigo[principalStruct.Substructs[0].Nombre.Length - 1]);
            principalStruct.Data2 = codigos.ToArray();

            principalStruct.refreshPages();
            principalStruct.refreshPositions();

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return NoContent();
        }

        [HttpGet("buyProducts/{userId}/{country}/{address}")]
        public async Task<ActionResult> buyProducts(int userId, string country, string address)
        {
            var user = await _context.Users.FindAsync(userId);
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            int userIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());
            var newSubstruct = new Substruct();
            var purchaseProducts = new Substruct();
            List<int> indexes = new List<int>();

            if (principalStruct != null)
            {
                purchaseProducts = principalStruct.Substructs[userIndex];

                foreach (var codigo in purchaseProducts.Codigo)
                {
                    var indices = principalStruct.Substructs[0].Codigo
                                    .Select((c, index) => new { Codigo = c, Index = index })
                                    .Where(x => x.Codigo == codigo)
                                    .Select(x => x.Index)
                                    .ToList();

                    foreach (var index in indices)
                    {
                        var ventaActual = principalStruct.Substructs[0].Ventas[index];

                        if (string.IsNullOrEmpty(ventaActual) || ventaActual == "0")
                        {
                            principalStruct.Substructs[0].Ventas[index] = "1";
                        }
                        else
                        {
                            if (int.TryParse(ventaActual, out int ventaValor))
                            {
                                ventaValor += 1;
                                principalStruct.Substructs[0].Ventas[index] = ventaValor.ToString();
                            }
                        }
                    }
                }


                for (int i = 0; i < principalStruct.Substructs[userIndex].Nombre.Length; i++)
                {
                    int indiceAux = int.Parse(purchaseProducts.Nombre[i]);

                    purchaseProducts.Date = DateTime.Now.ToString();
                    purchaseProducts.Nombre[i] = principalStruct.Substructs[0].Nombre[indiceAux];

                    if (principalStruct.Substructs[0].Extra8[indiceAux] == "1")
                    {
                        purchaseProducts.Precio[i] = principalStruct.Substructs[0].Extra7[indiceAux];
                    }
                    else if (principalStruct.Substructs[0].Extra8[indiceAux] == "0")
                    {
                        purchaseProducts.Precio[i] = principalStruct.Substructs[0].Precio[indiceAux];
                    }

                    //purchaseProducts.Color[i] = principalStruct.Substructs[userIndex].Color[i];
                    purchaseProducts.Extra3[i] = country;
                    purchaseProducts.Extra4[i] = address;
                    purchaseProducts.Extra5[i] = user.Id.ToString();
                    purchaseProducts.Extra6[i] = "0";
                    //purchaseProducts.Extra7[i] = user.Id.ToString();
                    purchaseProducts.Extra8[i] = user.Name;

                    newSubstruct.Extra7 = principalStruct.Substructs[userIndex].Extra7;
                }
                principalStruct.Substructs[userIndex] = newSubstruct;

                if (principalStruct.Data5[userIndex] == null || principalStruct.Data5[userIndex] == "string")
                {
                    List<Substruct> auxSubstruct = new List<Substruct>();
                    auxSubstruct.Add(purchaseProducts);
                    principalStruct.Data5[userIndex] = JsonConvert.SerializeObject(auxSubstruct);
                }
                else
                {
                    List<Substruct> auxSubstruct = JsonConvert.DeserializeObject<List<Substruct>>(principalStruct.Data5[userIndex]);
                    auxSubstruct.Add(purchaseProducts);

                    principalStruct.Data5[userIndex] = JsonConvert.SerializeObject(auxSubstruct);
                }

                _context.Entry(principalStruct).State = EntityState.Modified;
                await SaveChangesAndSerializeAsync();
            }

            return Redirect("https://tukitukichic.com/order-complete.html");
        }


        [HttpPost("PostPrincipalStruct")]
        public async Task<ActionResult<PrincipalStruct>> PostPrincipalStruct()
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "xmluploads");
            var filePath = Path.Combine(uploadsFolder, "PrincipalStructs.xml");

            // Verificar si el archivo XML ya existe
            if (System.IO.File.Exists(filePath))
            {
                return Conflict("El archivo PrincipalStructs.xml ya existe.");
            }

            PrincipalStruct principalStruct = new PrincipalStruct();
            principalStruct.Substructs[0] = new Substruct();
            principalStruct.Opiniones[0] = new Opiniones();

            principalStruct.Data8[0] = CreateCategories();

            principalStruct.Data7 = new string[] { "https://imagetesteo1.blob.core.windows.net/goldenwendy/1200x1000-2.jpg" };

            ActionResult<User> userActionResult = await _userController.PostUser(new User { Name = "Admin", Email = "admin123@gmail.com", CI = "000000", City = "Cochabamba", Password = "1980Bulzer" });
            if (!(userActionResult.Result is CreatedAtActionResult createdUserResult) || !(createdUserResult.Value is User createdUser))
            {
                return BadRequest("Error al crear el usuario Admin.");
            }
            principalStruct.Data9[0] = (createdUser.Id).ToString();

            _context.PrincipalStructs.Add(principalStruct);
            await SaveChangesAndSerializeAsync();

            return CreatedAtAction(nameof(GetPrincipalStruct), new { id = principalStruct.Id }, principalStruct);
        }

        private string CreateCategories()
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Women",
                    SubCategories = new List<SubCategory>
                    {
                        new SubCategory { Name = "Season", IsEnabled = true },
                        new SubCategory { Name = "T-shirt", IsEnabled = true },
                        new SubCategory { Name = "Hoodie", IsEnabled = true },
                        new SubCategory { Name = "Sueter", IsEnabled = true },
                        new SubCategory { Name = "Accessories", IsEnabled = true },
                        new SubCategory { Name = "1", IsEnabled = false },
                        new SubCategory { Name = "2", IsEnabled = false },
                        new SubCategory { Name = "3", IsEnabled = false },
                        new SubCategory { Name = "4", IsEnabled = false }
                    }
                },
                new Category
                {
                    Name = "Men",
                    SubCategories = new List<SubCategory>
                    {
                        new SubCategory { Name = "Season", IsEnabled = true },
                        new SubCategory { Name = "T-shirt", IsEnabled = true },
                        new SubCategory { Name = "Hoodie", IsEnabled = true },
                        new SubCategory { Name = "Sueter", IsEnabled = true },
                        new SubCategory { Name = "Accessories", IsEnabled = true },
                        new SubCategory { Name = "1", IsEnabled = false },
                        new SubCategory { Name = "2", IsEnabled = false },
                        new SubCategory { Name = "3", IsEnabled = false },
                        new SubCategory { Name = "4", IsEnabled = false }
                    }
                },
                new Category
                {
                    Name = "Children",
                    SubCategories = new List<SubCategory>
                    {
                        new SubCategory { Name = "Season", IsEnabled = true },
                        new SubCategory { Name = "T-shirt", IsEnabled = true },
                        new SubCategory { Name = "Hoodie", IsEnabled = true },
                        new SubCategory { Name = "Sueter", IsEnabled = true },
                        new SubCategory { Name = "Accessories", IsEnabled = true },
                        new SubCategory { Name = "1", IsEnabled = false },
                        new SubCategory { Name = "2", IsEnabled = false },
                        new SubCategory { Name = "3", IsEnabled = false },
                        new SubCategory { Name = "4", IsEnabled = false }
                    }
                },
                new Category { Name = "Gifts", SubCategories = null },
                new Category { Name = "For Home", SubCategories = null },
                new Category { Name = "Season", SubCategories = null },
                new Category { Name = "Pets", SubCategories = null },
                new Category { Name = "Others", SubCategories = null },
            };

            return JsonConvert.SerializeObject(categories);
        }

        [HttpGet("updateBase/{codigo}/{nuevaBase}")]
        public async Task<IActionResult> UpdateVentasBase(int codigo, int nuevaBase)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound("PrincipalStruct no encontrado.");
            }

            var substruct = principalStruct.Substructs.FirstOrDefault();
            if (substruct == null)
            {
                return NotFound("Substruct no encontrado.");
            }

            int index = Array.IndexOf(substruct.Codigo, codigo.ToString());
            if (index == -1)
            {
                return NotFound($"El código '{codigo}' no fue encontrado.");
            }

            substruct.VentasBase[index] = nuevaBase.ToString();

            _context.PrincipalStructs.Update(principalStruct);
            await _context.SaveChangesAsync();

            return Ok("VentasBase actualizada exitosamente.");
        }

        [HttpGet("server-time")]
        public IActionResult GetServerTime()
        {
            var serverTime = DateTime.UtcNow;
            return Ok(serverTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        [HttpGet("updateTimeOferta/{codigo}/{time}")]
        public async Task<IActionResult> UpdateTimeOferta(int codigo, int time)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound("PrincipalStruct no encontrado.");
            }

            var substruct = principalStruct.Substructs.FirstOrDefault();
            if (substruct == null)
            {
                return NotFound("Substruct no encontrado.");
            }

            int index = Array.IndexOf(substruct.Codigo, codigo.ToString());
            if (index == -1)
            {
                return NotFound($"El código '{codigo}' no fue encontrado.");
            }

            DateTime currentDateTime = DateTime.Now;
            DateTime updatedDateTime = currentDateTime.AddHours(time);

            substruct.TiempoOferta[index] = updatedDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            _context.PrincipalStructs.Update(principalStruct);
            await _context.SaveChangesAsync();

            return Ok("VentasBase actualizada exitosamente.");
        }

        [HttpGet("updateLikesBase/{codigo}/{nuevaBase}")]
        public async Task<IActionResult> UpdateLikesBase(int codigo, int nuevaBase)
        {
            var principalStruct = await _context.PrincipalStructs.FindAsync(1);
            if (principalStruct == null)
            {
                return NotFound("PrincipalStruct no encontrado.");
            }

            var substruct = principalStruct.Substructs.FirstOrDefault();
            if (substruct == null)
            {
                return NotFound("Substruct no encontrado.");
            }

            int index = Array.IndexOf(substruct.Codigo, codigo.ToString());
            if (index == -1)
            {
                return NotFound($"El código '{codigo}' no fue encontrado.");
            }

            substruct.LikesBase[index] = nuevaBase.ToString();

            _context.PrincipalStructs.Update(principalStruct);
            await _context.SaveChangesAsync();

            return Ok("Likes base actualizada exitosamente.");
        }


        [HttpPost("updateProduct/{productCode}")]
        public async Task<IActionResult> UpdateProduct(int productCode, Substruct updatedSubstruct)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            int id = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());

            if (principalStruct == null)
            {
                return NotFound();
            }

            principalStruct.Substructs[0].Nombre[id] = updatedSubstruct.Nombre[0];
            principalStruct.Substructs[0].Precio[id] = updatedSubstruct.Precio[0];
            principalStruct.Substructs[0].BreveDescripcion[id] = updatedSubstruct.BreveDescripcion[0];

            if (updatedSubstruct.Descripcion[0] != "")
            {
                principalStruct.Substructs[0].Descripcion[id] = updatedSubstruct.Descripcion[0];
            }

            principalStruct.Substructs[0].Color[id] = updatedSubstruct.Color[0];
            principalStruct.Substructs[0].Talla[id] = updatedSubstruct.Talla[0];
            principalStruct.Substructs[0].Categoria[id] = updatedSubstruct.Categoria[0];
            principalStruct.Substructs[0].SubCategoria[id] = updatedSubstruct.SubCategoria[0];
            principalStruct.Substructs[0].Images[id] = updatedSubstruct.Images[0];
            principalStruct.Substructs[0].Extra1[id] = updatedSubstruct.Extra1[0];
            principalStruct.Substructs[0].Extra2[id] = updatedSubstruct.Extra2[0];
            principalStruct.Substructs[0].Extra7[id] = updatedSubstruct.Extra7[0];

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return Ok();
        }

        [HttpPost("updateProductState/{id}/{state}")]
        public async Task<IActionResult> UpdateProductState(int id, string state)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            principalStruct.Substructs[0].Extra3[id] = state;
            List<string> codigos = new List<string>(principalStruct.Data2);
            List<string> codigosPromo = new List<string>(principalStruct.Data3);

            if (state == "1")
            {
                if (!codigos.Contains(principalStruct.Substructs[0].Codigo[id]))
                {
                    codigos.Add(principalStruct.Substructs[0].Codigo[id]);
                }
            }
            else if (state == "0")
            {
                if (codigos.Contains(principalStruct.Substructs[0].Codigo[id]))
                {
                    codigos.Remove(principalStruct.Substructs[0].Codigo[id]);
                }
                if (codigosPromo.Contains(principalStruct.Substructs[0].Codigo[id]))
                {
                    codigosPromo.Remove(principalStruct.Substructs[0].Codigo[id]);
                }
            }

            principalStruct.Data2 = codigos.ToArray();
            principalStruct.Data3 = codigosPromo.ToArray();
            principalStruct.refreshPages();
            principalStruct.refreshPositions();
            principalStruct.refreshPromo();
            principalStruct.Data1[0] = ((int)Math.Ceiling((double)principalStruct.Substructs[0].Nombre.Length / 10)).ToString();

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return NoContent();
        }

        [HttpPost("updateProductPage/{code}/{newPage}")]
        public async Task<IActionResult> UpdateProductPage(string code, int newPage)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            var productIndex = Array.IndexOf(principalStruct.Data2, code);

            var currentPage = productIndex / 10 + 1;
            if (currentPage == newPage)
            {
                return Ok();
            }

            var newData2 = principalStruct.Data2.Where((val, idx) => idx != productIndex).ToArray();

            var newIndex = (newPage - 1) * 10;
            principalStruct.Data2 = newData2.Take(newIndex).Concat(new[] { code }).Concat(newData2.Skip(newIndex)).ToArray();

            principalStruct.refreshPages();
            principalStruct.refreshPositions();

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return Ok(principalStruct);
        }

        [HttpPost("updateProductPosition/{code}/{newPosition}")]
        public async Task<IActionResult> UpdateProductPosition(string code, int newPosition)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            var productIndex = Array.IndexOf(principalStruct.Data2, code).ToString();

            int newIndex = int.Parse(productIndex.Substring(0, productIndex.Length - 1) + (newPosition - 1));

            var newData2 = principalStruct.Data2.Where((val, idx) => idx != int.Parse(productIndex)).ToArray();
            principalStruct.Data2 = newData2.Take(newIndex).Concat(new[] { code }).Concat(newData2.Skip(newIndex)).ToArray();

            principalStruct.refreshPages();
            principalStruct.refreshPositions();

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return Ok(principalStruct);
        }

        [HttpPost("updatePurchasedProducts/{userId}/{productIndex}")]
        public async Task<ActionResult> UpdatePurchasedProducts(int userId, int productIndex)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int userIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());

                if (principalStruct != null)
                {
                    List<Substruct> purchasedProducts = JsonConvert.DeserializeObject<List<Substruct>>(principalStruct.Data5[userIndex]);
                    purchasedProducts[productIndex].Extra6[0] = "1";

                    principalStruct.Data5[userIndex] = JsonConvert.SerializeObject(purchasedProducts);

                    _context.Entry(principalStruct).State = EntityState.Modified;
                    await SaveChangesAndSerializeAsync();

                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Error al actualizar los productos comprados.");
            }
        }

        [HttpPost("updateProductPromo/{id}/{promo}")]
        public async Task<IActionResult> UpdateProductPromo(int id, string promo)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            principalStruct.Substructs[0].Extra6[id] = promo;
            List<string> codigosPromo = new List<string>(principalStruct.Data3);

            if (promo == "1")
            {
                if (!codigosPromo.Contains(principalStruct.Substructs[0].Codigo[id]))
                {
                    codigosPromo.Insert(0, principalStruct.Substructs[0].Codigo[id]);
                    if (codigosPromo.Count > 7)
                    {
                        codigosPromo.RemoveAt(codigosPromo.Count - 1);
                    }
                }
            }
            else if (promo == "0")
            {
                if (codigosPromo.Contains(principalStruct.Substructs[0].Codigo[id]))
                {
                    codigosPromo.Remove(principalStruct.Substructs[0].Codigo[id]);
                }
            }

            principalStruct.Data3 = codigosPromo.ToArray();
            principalStruct.refreshPromo();

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return NoContent();
        }

        [HttpPost("updateProductDiscount/{id}/{discount}")]
        public async Task<IActionResult> updateProductDiscount(int id, string discount)
        {
            var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
            if (principalStruct == null)
            {
                return NotFound();
            }

            principalStruct.Substructs[0].Extra8[id] = discount;

            _context.Entry(principalStruct).State = EntityState.Modified;
            await SaveChangesAndSerializeAsync();

            return NoContent();
        }


        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile image, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No se proporcionó ninguna imagen o la imagen estaba vacía.");
            }

            //var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "uploads");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            //var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var uploadAzure = new UploadAzure();

            using (var stream = image.OpenReadStream())
            {
                await uploadAzure.UploadBlobAsync("goldenwendy", uniqueFileName, stream);
            }
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await image.CopyToAsync(stream);
            //}

            var imageUrl = "https://imagetesteo1.blob.core.windows.net/goldenwendy/" + uniqueFileName;
            //var imageUrl = "C:/Users/brran/OneDrive/Escritorio/ShopOnline/ShopOnline/uploads/" + uniqueFileName;
            return Ok(new { imageUrl });
        }



        [HttpGet("RemoveProduct/{userId}/{productCode}/{color}")]
        public async Task<ActionResult> RemoveProductFromUser(int userId, int productCode, string color)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FirstOrDefaultAsync();
                if (principalStruct != null)
                {
                    int substructIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());
                    int productIndex = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
                    if (substructIndex != -1)
                    {
                        if (userId == 1)
                        {
                            List<string> codigos = new List<string>(principalStruct.Data2);
                            codigos.Remove(principalStruct.Substructs[0].Codigo[productIndex]);
                            for (int i = 1; i < principalStruct.Substructs.Count(); i++)
                            {
                                principalStruct.RemoveProductFromSubstruct(i, productIndex, color);
                                await DeleteFavoriteProduct(i, productCode);
                            }
                            principalStruct.Data2 = codigos.ToArray();
                        }

                        principalStruct.RemoveProductFromSubstruct(substructIndex, productIndex, color);

                        principalStruct.Data1[0] = ((int)Math.Ceiling((double)principalStruct.Substructs[0].Nombre.Length / 10)).ToString();
                        principalStruct.refreshPages();
                        principalStruct.refreshPositions();

                        _context.Entry(principalStruct).State = EntityState.Modified;
                        await SaveChangesAndSerializeAsync();
                        return Ok();
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("deleteFavoriteProduct/{userId}/{productCode}")]
        public async Task<ActionResult> DeleteFavoriteProduct(int userId, int productCode)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int productId = Array.FindIndex(principalStruct.Substructs[0].Codigo, x => x == productCode.ToString());
                if (principalStruct != null)
                {
                    for (int i = 0; i < principalStruct.Substructs.Count(); i++)
                    {
                        if (principalStruct.Data9[i] == userId.ToString())
                        {
                            List<string> favorites = principalStruct.Substructs[i].Extra7.ToList();
                            favorites.Remove(productId.ToString());
                            principalStruct.Substructs[i].Extra7 = favorites.ToArray();

                            _context.Entry(principalStruct).State = EntityState.Modified;
                            await SaveChangesAndSerializeAsync();

                            return Ok();
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("deletePurchasedProducts/{userId}/{productIndex}")]
        public async Task<ActionResult> DeletePurchasedProducts(int userId, int productIndex)
        {
            try
            {
                var principalStruct = await _context.PrincipalStructs.FindAsync(1);
                int userIndex = Array.FindIndex(principalStruct.Data9, x => x == userId.ToString());

                if (principalStruct != null)
                {
                    List<Substruct> purchasedProducts = JsonConvert.DeserializeObject<List<Substruct>>(principalStruct.Data5[userIndex]);
                    purchasedProducts.RemoveAt(productIndex);

                    principalStruct.Data5[userIndex] = JsonConvert.SerializeObject(purchasedProducts);

                    _context.Entry(principalStruct).State = EntityState.Modified;
                    await SaveChangesAndSerializeAsync();

                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private bool PrincipalStructExists(int id)
        {
            return _context.PrincipalStructs.Any(e => e.Id == id);
        }

        private async Task SaveChangesAndSerializeAsync()
        {
            await _context.SaveChangesAsync();
            await SerializeToXmlAsync();
        }

        private async Task SerializeToXmlAsync()
        {
            var principalStructs = await _context.PrincipalStructs.ToListAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(List<PrincipalStruct>));
            var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "xmluploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, "PrincipalStructs.xml");

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, principalStructs);
            }
        }
    }
}
