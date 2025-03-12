using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.DataBaseContext;
using ShopOnline.Models.StripeHelpers;
using Stripe;
using Stripe.Checkout;

namespace ShopOnline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class PaymentController : ControllerBase
    {
        private readonly DBaseContext _context;
        private UserController userController;
        private PrincipalStructController principalStructController;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PaymentController(DBaseContext context)
        {
            _context = context;
            userController = new UserController(_context, _webHostEnvironment);
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            var shipping = new CheckoutItem
            {
                Name = "Shipping",
                Price = request.Shipping.ToString(),
                Quantity = "1"
            };
            request.Items.Add(shipping);

            var discountString = await userController.GetUserDate(request.UserId);

            int discountValue = 0;
            if (discountString.Result is OkObjectResult okResult)
            {
                if (int.TryParse(okResult.Value.ToString(), out int parsedDiscount))
                {
                    discountValue = parsedDiscount;
                }
            }
            else
            {
                return BadRequest("No se pudo obtener el descuento.");
            }

            var lineItems = request.Items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = int.Parse(item.Price) * 100,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name,
                    },
                },
                Quantity = int.Parse(item.Quantity),
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"https://server.com/api/PrincipalStruct/buyProducts/{request.UserId}/{Uri.EscapeDataString(request.Country)}/{Uri.EscapeDataString(request.Address)}",
                CancelUrl = "https://server.com/shoping-cart.html",
            };

            if (discountValue > 0)
            {
                var couponOptions = new CouponCreateOptions
                {
                    PercentOff = discountValue,
                    Duration = "once"
                };
                var couponService = new CouponService();
                var coupon = await couponService.CreateAsync(couponOptions);

                options.Discounts = new List<SessionDiscountOptions>
                {
                    new SessionDiscountOptions
                    {
                        Coupon = coupon.Id
                    }
                };
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return new JsonResult(new { id = session.Id });
        }

    }
}
