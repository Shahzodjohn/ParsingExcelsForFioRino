using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Parsing.Context;
using Parsing.Models;
using Parsing.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Parsing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<List<Products>> Import(IFormFile file)
        {
            var list = new List<Products>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    var SizeR = new List<ProductsDTO>();
                    //int rowF = 9;
                    var column = 4;
                    #region
                    //for (; ; )
                    //{
                    //    //var res = await _context.products.AddAsync(new Products
                    //    //{
                    //    //    //Size = worksheet.Cells[rowF, column].Value.ToString().Trim()
                    //    //});
                    //    //var detectGapes = res.Entity.Size.Contains("   ");


                    //    //rowF++;
                    //    //column++;
                    //    //await _context.SaveChangesAsync();
                    //    if (column > 19)
                    //        break;

                    //}
                    #endregion

                    #region CreateCategory
                    var ClassicCategory = _context.Categories.FirstOrDefault(x => x.CategoryName == "Classic");
                    if (ClassicCategory == null)
                    {
                        var rt = new Category()
                        {
                            CategoryName = "Classic",
                        };

                        ClassicCategory = _context.Categories.Add(rt).Entity;
                        _context.SaveChanges();
                    }
                    var FasterCategory = _context.Categories.FirstOrDefault(x => x.CategoryName == "Faster");
                    if (FasterCategory == null)
                    {
                        var rt = new Category()
                        {
                            CategoryName = "Faster",
                        };

                        FasterCategory = _context.Categories.Add(rt).Entity;
                        _context.SaveChanges();
                    }
                    #endregion

                    #region CreateSizes
                    if (_context.Sizes.Count() == 0)
                    {
                        List<Size> sizes = new List<Size>();
                        for (var i = 4; i < 20; i++)
                        {
                            sizes.Add(new Size
                            {
                                SizeName = worksheet.Cells[9, i].Value.ToString()
                            });
                        }
                        _context.Sizes.AddRange(sizes);
                        _context.SaveChanges();
                    }
                    #endregion

                    var indexColumn = 3;

                    //int indexRow = 10;
                    for (int row = 10; row < rowcount - 1; row += 2)
                    {
                        //define category and size
                        Category category = new Category();
                        Size size = new Size();
                        string sizeName = string.Empty;
                        int productAmount = 0;

                        var isClassicCategory = worksheet.Cells[row, indexColumn].Value == null ? false :
                                                worksheet.Cells[row, indexColumn].Value.ToString().ToLower() == "classic" ? true : false;
                        var tempColumn = indexColumn + 1;
                        
                        
                        while(isClassicCategory && tempColumn < 19)
                        {
                            var cell = worksheet.Cells[row, tempColumn].Value;

                                if (cell != null)
                                {
                                    Int32.TryParse(cell.ToString(), out productAmount);
                                    category = ClassicCategory;
                                    sizeName = (worksheet.Cells[9, tempColumn].Value == null ? "" :  worksheet.Cells[9, tempColumn].Value.ToString());
                                    size = _context.Sizes.FirstOrDefault(x => x.SizeName == sizeName);
                                    tempColumn++;
                                    var InsertProducts = await _context.products.AddAsync(new Products
                                    {
                                        SKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        //Category = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                        //Size = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                        //Size = worksheet.Cells[rowF, column].Value.ToString().Trim(),
                                        CategoryId = category.Id,
                                        Amount = productAmount,
                                        SizeId = size.Id,
                                        Razem = worksheet.Cells[row, 19].Value.ToString().Trim()
                                    });
                                }
                             
                                if (cell == null)
                                    tempColumn++;
                                if (tempColumn == 19 && cell == null)
                                    isClassicCategory = false;
                            
                        }                
                            

                        if (!isClassicCategory)
                        {
                            var isFasterCategory = worksheet.Cells[row+1, indexColumn].Value == null ? false :
                                               worksheet.Cells[row+1, indexColumn].Value.ToString().ToLower() == "faster" ? true : false;
                            tempColumn = indexColumn + 1;

                            while (isFasterCategory && tempColumn < 19)
                            {
                                var cell = worksheet.Cells[row+1, tempColumn].Value;

                                if (cell != null)
                                {
                                    Int32.TryParse(cell.ToString(), out productAmount);
                                    category = FasterCategory;
                                    sizeName = (worksheet.Cells[9, tempColumn].Value == null ? "" : worksheet.Cells[9, tempColumn].Value.ToString());
                                    size = _context.Sizes.FirstOrDefault(x => x.SizeName == sizeName);
                                    tempColumn++;
                                    var InsertProducts = await _context.products.AddAsync(new Products
                                    {
                                        SKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                        ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                        CategoryId = category.Id,
                                        Amount = productAmount,
                                        SizeId = size.Id,
                                        Razem = worksheet.Cells[row, 19].Value.ToString().Trim()
                                    });
                                }
                                if (cell == null)
                                    tempColumn++;
                                if (tempColumn == 19 && cell == null)
                                    isFasterCategory = false;
                            }
                        }
                        //create order;
                        if (category.Id == 0) continue;

                       















                        //var pagination = worksheet.Cells[indexRow, indexColumn].Value.ToString().Trim();
                        //for (int indexRow = 10; ;)
                        //{
                        //    var pagination = worksheet.Cells[indexRow, indexColumn].Value == null ?
                        //                    string.Empty : worksheet.Cells[indexRow, indexColumn].Value.ToString();
                        //    //indexRow++;
                        //    indexColumn++;
                        //    if (indexColumn > 19)
                        //        break;
                        //var verticalColumn = 3;
                        //var HorizontalColumn = 10;
                        //var Three = 4;


                        //var isClassicCategory = worksheet.Cells[10, 3].Value == null ?
                        //                string.Empty : worksheet.Cells[10, 3].Value.ToString();



                        //var isFasterCategory = worksheet.Cells[11, 3].Value == null ?
                        //                string.Empty : worksheet.Cells[11, 3].Value.ToString();



                        //if (category.Id == 0) continue;

                        //    var InsertProducts = await _context.products.AddAsync(new Products
                        //    {
                        //        SKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        //        ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                        //        //Category = worksheet.Cells[row, 3].Value.ToString().Trim(),
                        //        //Size = worksheet.Cells[row, 4].Value.ToString().Trim(),
                        //        //Size = worksheet.Cells[rowF, column].Value.ToString().Trim(),
                        //        CategoryId = category.Id,
                        //        Amount = productAmount,
                        //        SizeId = size.Id,

                        //        Razem = worksheet.Cells[row, 19].Value.ToString().Trim()
                        //    });
                        //Three++;
                        //verticalColumn++;
                        //HorizontalColumn++;

                        //if (verticalColumn > 19)
                        //    break;
                        //if (Three > 19)
                        //    break;








                        #region
                        //if (pagination == "classic")
                        //{
                        //    _context.products.Update(new Products
                        //    {
                        //        CategoryId = 1
                        //    });
                        //}

                        //if (pagination == "faster")
                        //{
                        //    _context.products.Update(new Products
                        //    {
                        //        CategoryId = 2
                        //    });
                        //}
                        #endregion
                        //var InsertProducts = await _context.products.AddAsync(new Products
                        //{
                        //    SKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        //    ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                        //    //Category = worksheet.Cells[row, 3].Value.ToString().Trim(),
                        //    //Size = worksheet.Cells[row, 4].Value.ToString().Trim(),
                        //    //Size = worksheet.Cells[rowF, column].Value.ToString().Trim(),
                        //    //CategoryId = pagination.Contains("classic") ? 1 : 2,
                        //   // CategoryId = pagination.Contains("classic") && pagination.Contains,
                        //    Razem = worksheet.Cells[row, 19].Value.ToString().Trim()
                        //});
                        //}



                        
                    
                        
                    }



                    #region
                    //for (int row = 10; row < rowcount; )
                    //{
                    //    var InsertProducts = await _context.products.AddAsync(new Products
                    //    {
                    //        SKU = worksheet.Cells[row, 1].Value.ToString().Trim(),
                    //        ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                    //        Category = worksheet.Cells[row, 3].Value.ToString().Trim(),
                    //        //Size = worksheet.Cells[row, 4].Value.ToString().Trim(),
                    //        Size = worksheet.Cells[rowF, column].Value.ToString().Trim(),
                    //        Razem = worksheet.Cells[row, 19].Value.ToString().Trim(),
                    //    });
                    //    if (column > 19)
                    //        break;
                    //    row += 2;
                    //}
                    //for (int row = 9; row <= rowcount; row++)
                    //{
                    //    list.Add(new Products
                    //    {
                    //         SKU = worksheet.Cells[row,1].Value.ToString().Trim(),
                    //          ProductName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                    //           Category = worksheet.Cells[row, 3].Value.ToString().Trim(),
                    //            //Size = worksheet.Cells[row, 4].Value.ToString().Trim(),
                    //             Razem = worksheet.Cells[row, 19].Value.ToString().Trim()
                    //    });
                    //}
                    #endregion
                    await _context.SaveChangesAsync();
                }
            }
            return list;
        }
    }
}
