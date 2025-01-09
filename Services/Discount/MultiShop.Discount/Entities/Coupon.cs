namespace MultiShop.Discount.Entities
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get;}
        public int Rate { get;}
        public bool IsActive { get;}
        public DateTime ValidDate { get;}
    }
}
