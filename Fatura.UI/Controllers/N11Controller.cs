using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Fatura.UI.N11CategoryService;
using Fatura.UI.N11OrderService;
using Fatura.UI.N11ProductService;
using Fatura.UI.N11ProductStockService;

namespace Fatura.UI.Controllers
{
    public class N11Controller : Controller
    {
        // GET: N11
        public ActionResult Index()
        {
            //LoadCategoryList(1);
            return View();
        }
        public List<SelectListItem> LoadCategoryList(long CategoryID)
        {
            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            var authentication = new N11CategoryService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz


            CategoryServicePortClient proxy = new CategoryServicePortClient();
            var request = new GetTopLevelCategoriesRequest();
            request.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var categories = proxy.GetTopLevelCategories(request);

            List<SelectListItem> CatItem = new List<SelectListItem>();

            CatItem.Add(new SelectListItem { Text = "*Seçiniz*", Value = "" });
            foreach (var item in categories.categoryList)
            {
                bool SelectedState = false;
                if (CategoryID == item.id) { SelectedState = true; }
                CatItem.Add(new SelectListItem { Text = "-" + item.name, Selected = SelectedState, Value = item.id.ToString() });

            }
            ViewBag.CategoryList = CatItem.OrderBy(t => t.Text);
            return CatItem;
        }
        public List<SelectListItem> LoadSubCategoryList(long CategoryId)
        {
            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            //string strAppKey = keys.N11AppKey;
            //string strAppSecret = keys.N11AppSecretKey;
            var authentication = new N11CategoryService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz


            CategoryServicePortClient proxy = new N11CategoryService.CategoryServicePortClient();
            var request = new N11CategoryService.GetSubCategoriesRequest();
            request.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            request.categoryId = CategoryId;


            var SubCategories = proxy.GetSubCategories(request);


            List<SelectListItem> SubCatItem = new List<SelectListItem>();

            //SubCatItem.Add(new SelectListItem { Text = "Seçiniz", Value = "" });
            if (SubCategories.category[0].subCategoryList != null)
            {
                foreach (var item in SubCategories.category[0].subCategoryList)
                {
                    bool SelectedState = false;
                    if (CategoryId == item.id) { SelectedState = true; }
                    SubCatItem.Add(new SelectListItem { Text = "-" + item.name, Selected = SelectedState, Value = item.id.ToString() });
                }
            }
            if (SubCategories.result.status == "success")
            {
                ViewBag.SubCategoryList = SubCatItem.OrderBy(t => t.Text);
                return SubCatItem;
            }
            else
            {
                return SubCatItem;
            }
        }
        public JsonResult GetSubCategory(int categoryIdValue)
        {

            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            //string strAppKey = keys.N11AppKey;
            //string strAppSecret = keys.N11AppSecretKey;
            var authentication = new N11CategoryService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz


            CategoryServicePortClient proxy = new N11CategoryService.CategoryServicePortClient();
            var request = new N11CategoryService.GetSubCategoriesRequest();
            request.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            request.categoryId = categoryIdValue;


            var SubCategories = proxy.GetSubCategories(request);


            List<SelectListItem> SubCatItem = new List<SelectListItem>();

            //SubCatItem.Add(new SelectListItem { Text = "Seçiniz", Value = "" });
            foreach (var item in SubCategories.category)
            {
                bool SelectedState = false;
                if (categoryIdValue == item.id) { SelectedState = true; }
                SubCatItem.Add(new SelectListItem { Text = "-" + item.name, Selected = SelectedState, Value = item.id.ToString() });
            }


            if (SubCategories.result.status == "success")
            {


                return Json(SubCategories.category.FirstOrDefault().subCategoryList, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
        public List<SelectListItem> GetOrderList()
        {
            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            var authentication = new N11OrderService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz

            OrderServicePortClient proxy = new OrderServicePortClient();

            var request1 = new OrderListRequest();
            request1.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var orders = proxy.OrderList(request1);

            //OrderServicePortClient proxy = new OrderServicePortClient();

            //var request2 = new OrderDetailRequest();
            //request2.auth = authentication;
            //request2.orderRequest.id = orders.orderList[99].id;
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //var orderdetail = proxy.OrderDetail(request2);

            //OrderServicePortClient proxy = new OrderServicePortClient();

            var request = new DetailedOrderListRequest();
            request.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var detailedorders = proxy.DetailedOrderList(request);

            List<SelectListItem> CatItem = new List<SelectListItem>();

            CatItem.Add(new SelectListItem { Text = "*Seçiniz*", Value = "" });
            if (detailedorders.orderList != null)
            {
                foreach (var item in detailedorders.orderList)
                {
                    bool SelectedState = false;
                    //if (CategoryID == item.id) { SelectedState = true; }
                    CatItem.Add(new SelectListItem { Text = "-" + item.id, Selected = SelectedState, Value = item.id.ToString() });

                }
            }
            ViewBag.CategoryList = CatItem.OrderBy(t => t.Text);
            return CatItem;
        }
        public List<SelectListItem> GetProductList()
        {
            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            var authentication = new Fatura.UI.N11ProductService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz

            ProductServicePortClient proxy = new ProductServicePortClient();

            var request = new GetProductListRequest();
            request.auth = authentication;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var products = proxy.GetProductList(request);
            
            //var product = proxy.GetProductByProductId(products.products[20].id);

            List<SelectListItem> CatItem = new List<SelectListItem>();

            CatItem.Add(new SelectListItem { Text = "*Seçiniz*", Value = "" });
            foreach (var item in products.products)
            {
                bool SelectedState = false;
                //if (CategoryID == item.id) { SelectedState = true; }
                CatItem.Add(new SelectListItem { Text = "-" + item.id, Selected = SelectedState, Value = item.id.ToString() });

            }
            ViewBag.CategoryList = CatItem.OrderBy(t => t.Text);
            return CatItem;
        }
        public List<SelectListItem> GetProductStockList()
        {
            //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
            string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
            string strAppSecret = "SpzrFSGJje8cJ4IW";
            var authentication = new Fatura.UI.N11ProductStockService.Authentication();
            authentication.appKey = strAppKey; //api anahtarınız
            authentication.appSecret = strAppSecret;//api şifeniz

            ProductStockServicePortClient proxy = new ProductStockServicePortClient();

            var request = new GetProductStockByProductIdRequest();
            request.auth = authentication;
            request.productId = 1;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var products = proxy.GetProductStockByProductId(request);

            //var request1 = new SearchProductsRequest();
            //request1.auth = authentication;
            //var products1 = proxy.SearchProducts(request1);


            //var product = proxy.GetProductByProductId(products.products[20].id);

            List<SelectListItem> CatItem = new List<SelectListItem>();

            CatItem.Add(new SelectListItem { Text = "*Seçiniz*", Value = "" });
            foreach (var item in products.stockItems)
            {
                bool SelectedState = false;
                //if (CategoryID == item.id) { SelectedState = true; }
                CatItem.Add(new SelectListItem { Text = "-" + item.id, Selected = SelectedState, Value = item.id.ToString() });

            }
            ViewBag.CategoryList = CatItem.OrderBy(t => t.Text);
            return CatItem;
        }
        public ActionResult ProductN11Save(string[] CategoryList, string productid, string productSellerCode, string subtitle, string preparingDay, string shipmentTemplate, string quantity, string attributeName, string condition)
        {
            try
            {

                int id = Convert.ToInt32(productid);

                //Data.DAL.Entity.ProductInfo pi = ProductInfoRepo.FirstOrDefault(t => t.ProductID == id);
                string title = "Deneme";
                string description = "Deneme ürünü";

                //Data.DAL.Entity.Product p = ProductRepo.FirstOrDefault(t => t.ID == id);
                int price = 13;
                int currencyType = 1;
                string stockCode = "st001";

                //List<Data.DAL.Entity.ProductImage> pimg = ProductImageRepo.GetList().Where(t => t.ProductID == id).OrderBy(t => t.SortNumber).ToList();

                //Data.DAL.Entity.Integration keys = IntegrationRepo.GetList().FirstOrDefault();
                //string strAppKey = keys.N11AppKey;
                //string strAppSecret = keys.N11AppSecretKey;
                string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
                string strAppSecret = "SpzrFSGJje8cJ4IW";

                int quantityValue = Convert.ToInt32(quantity);
                string brand = attributeName;

                var authentication = new N11ProductService.Authentication();
                authentication.appKey = strAppKey; //api anahtarınız
                authentication.appSecret = strAppSecret;//api şifeniz

                List<N11ProductService.ProductImage> productImageList = new List<ProductImage>();
                //foreach (var item in pimg)
                //{

                //    string imageUrl = System.Configuration.ConfigurationManager.AppSettings["ImageLink"] + "/upload/buyuk/" + item.ImageName;
                //    N11ProductService.ProductImage proimg = new N11ProductService.ProductImage();
                //    proimg.order = item.SortNumber.ToString();
                //    proimg.url = imageUrl;
                //    productImageList.Add(proimg);
                //}

                var stockItems = new List<ProductSkuRequest>();

                var sku1 = new ProductSkuRequest();
                sku1.sellerStockCode = stockCode; //stokkodu;
                sku1.quantity = quantityValue.ToString(); //Stokmiktar;
                sku1.optionPrice = price; // Stok Fiyatı
                stockItems.Add(sku1);



                int categoryId = 0;
                for (int i = 0; i < CategoryList.Count(); i++)
                {
                    if (CategoryList[i] != "0")
                    {
                        categoryId = Convert.ToInt32(CategoryList[i]);
                    }
                }

                var categoryRequest = new N11ProductService.CategoryRequest();
                categoryRequest.id = categoryId;// Ürünü bağlayacağınız kategoriId


                ProductRequest productRequest = new ProductRequest();
                productRequest.productSellerCode = productSellerCode;
                productRequest.title = title;
                productRequest.subtitle = subtitle;
                productRequest.description = description;
                productRequest.category = categoryRequest;
                productRequest.price = price;//Ürün Fiyatı
                productRequest.currencyType = currencyType.ToString();//Döviztipi
                productRequest.images = productImageList.ToArray();
                productRequest.preparingDay = preparingDay; //Ürün Hazırlık Süresi;
                productRequest.stockItems = stockItems.ToArray();
                productRequest.productCondition = condition; // 1 Yeni Ürün | 2 İkinci El
                productRequest.shipmentTemplate = shipmentTemplate;//Kargo Şablon adı;


                var attr = new ProductAttributeRequest();
                attr.name = "Marka";
                attr.value = brand;

                var attList = new List<ProductAttributeRequest>();
                attList.Add(attr);
                productRequest.attributes = attList.ToArray();

                var saveProductRequest = new SaveProductRequest();
                saveProductRequest.auth = authentication;
                saveProductRequest.product = productRequest;
                // ProductServicePort port = new ProductServicePortService().getProductServicePortSoap11();
                var port = new ProductServicePortClient();
                SaveProductResponse saveProductResponse = port.SaveProduct(saveProductRequest);

                Response.Redirect(@Url.Action("Index", "nonbir"));
                return null;
            }
            catch (Exception e)
            {
                Response.Redirect(@Url.Action("Error", "Home"));
                return null;
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(string title, string description, string price, string productSellerCode, string quantity, string productID, string SellerStockCode, string OptionPrice, string stockID, string image)
        {
            try
            {
                int id = Convert.ToInt32(productID);
                string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
                string strAppSecret = "SpzrFSGJje8cJ4IW";

                var authentication = new N11ProductService.Authentication();
                authentication.appKey = strAppKey; //api anahtarınız
                authentication.appSecret = strAppSecret;//api şifeniz


                ProductUpdateSkuBasicRequest productUpdateSkuBasicRequest = new ProductUpdateSkuBasicRequest();
                productUpdateSkuBasicRequest.sellerStockCode = "deneme";
                productUpdateSkuBasicRequest.optionPrice = (Convert.ToDecimal(price));
                productUpdateSkuBasicRequest.id = Convert.ToInt64(stockID);
                productUpdateSkuBasicRequest.quantity = quantity;


                List<ProductUpdateSkuBasicRequest> productUpdateSkuBasicRequests = new List<ProductUpdateSkuBasicRequest>();
                productUpdateSkuBasicRequests.Add(productUpdateSkuBasicRequest);

                var request = new UpdateProductBasicRequest();
                //request.productSellerCode = "deneme2";
                request.price = Convert.ToDecimal(price);
                request.description = description;
                request.stockItems = productUpdateSkuBasicRequests.ToArray();
                request.auth = authentication;
                request.productId = Convert.ToInt64(productID);

                //if (image == "1")
                //{
                //    List<Data.DAL.Entity.ProductImage> pimg = ResimListesi;
                //    List<N11ProductService.ProductImage> productImageList = new List<N11ProductService.ProductImage>();
                //    foreach (var item in pimg)
                //    {
                //        string imageUrl = ImagePath;
                //        com.n11.api.ProductImage proimg = new com.n11.api.ProductImage();
                //        proimg.order = item.SortNumber.ToString();
                //        proimg.url = imageUrl;
                //        productImageList.Add(proimg);
                //    }
                //    request.images = productImageList.ToArray();
                //}



                var port = new ProductServicePortClient();
                UpdateProductBasicResponse response = port.UpdateProductBasic(request);

                Response.Redirect(@Url.Action("Index", "nonbir"));
                return null;
            }
            catch (Exception e)
            {
                Response.Redirect(@Url.Action("Error", "Home"));
                return null;

            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ListOrders(string title, string description, string price, string productSellerCode, string quantity, string productID, string SellerStockCode, string OptionPrice, string stockID, string image)
        {
            try
            {
                int id = Convert.ToInt32(productID);
                string strAppKey = "209ec80a-fe19-4224-b9dc-a118cb9e8e8e";
                string strAppSecret = "SpzrFSGJje8cJ4IW";

                var authentication = new N11ProductService.Authentication();
                authentication.appKey = strAppKey; //api anahtarınız
                authentication.appSecret = strAppSecret;//api şifeniz


                ProductUpdateSkuBasicRequest productUpdateSkuBasicRequest = new ProductUpdateSkuBasicRequest();
                productUpdateSkuBasicRequest.sellerStockCode = "deneme";
                productUpdateSkuBasicRequest.optionPrice = (Convert.ToDecimal(price));
                productUpdateSkuBasicRequest.id = Convert.ToInt64(stockID);
                productUpdateSkuBasicRequest.quantity = quantity;


                List<ProductUpdateSkuBasicRequest> productUpdateSkuBasicRequests = new List<ProductUpdateSkuBasicRequest>();
                productUpdateSkuBasicRequests.Add(productUpdateSkuBasicRequest);

                var request = new UpdateProductBasicRequest();
                //request.productSellerCode = "deneme2";
                request.price = Convert.ToDecimal(price);
                request.description = description;
                request.stockItems = productUpdateSkuBasicRequests.ToArray();
                request.auth = authentication;
                request.productId = Convert.ToInt64(productID);

                //if (image == "1")
                //{
                //    List<Data.DAL.Entity.ProductImage> pimg = ResimListesi;
                //    List<N11ProductService.ProductImage> productImageList = new List<N11ProductService.ProductImage>();
                //    foreach (var item in pimg)
                //    {
                //        string imageUrl = ImagePath;
                //        com.n11.api.ProductImage proimg = new com.n11.api.ProductImage();
                //        proimg.order = item.SortNumber.ToString();
                //        proimg.url = imageUrl;
                //        productImageList.Add(proimg);
                //    }
                //    request.images = productImageList.ToArray();
                //}



                var port = new ProductServicePortClient();
                UpdateProductBasicResponse response = port.UpdateProductBasic(request);

                Response.Redirect(@Url.Action("Index", "nonbir"));
                return null;
            }
            catch (Exception e)
            {
                Response.Redirect(@Url.Action("Error", "Home"));
                return null;

            }
        }
    }
}