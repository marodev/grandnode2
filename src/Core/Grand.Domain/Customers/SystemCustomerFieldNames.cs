namespace Grand.Domain.Customers
{
    public static partial class SystemCustomerFieldNames
    {
        //Form fields
        public static string FirstName { get { return "FirstName"; } }
        public static string LastName { get { return "LastName"; } }
        public static string Gender { get { return "Gender"; } }
        public static string DateOfBirth { get { return "DateOfBirth"; } }
        public static string Company { get { return "Company"; } }
        public static string StreetAddress { get { return "StreetAddress"; } }
        public static string StreetAddress2 { get { return "StreetAddress2"; } }
        public static string ZipPostalCode { get { return "ZipPostalCode"; } }
        public static string City { get { return "City"; } }
        public static string CountryId { get { return "CountryId"; } }
        public static string StateProvinceId { get { return "StateProvinceId"; } }
        public static string Phone { get { return "Phone"; } }
        public static string Fax { get { return "Fax"; } }
        public static string VatNumber { get { return "VatNumber"; } }
        public static string VatNumberStatusId { get { return "VatNumberStatusId"; } }        
        public static string PasswordToken { get { return "PasswordToken"; } }

        //Other attributes
        public static string DiscountCoupons { get { return "DiscountCoupons"; } }
        public static string GiftVoucherCoupons { get { return "GiftVoucherCoupons"; } }
        public static string UrlReferrer { get { return "UrlReferrer"; } }
        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }
        public static string PasswordRecoveryTokenDateGenerated { get { return "PasswordRecoveryTokenDateGenerated"; } }
        public static string AccountActivationToken { get { return "AccountActivationToken"; } }
        public static string LastVisitedPage { get { return "LastVisitedPage"; } }
        public static string LastUrlReferrer { get { return "LastUrlReferrer"; } }
        public static string ImpersonatedCustomerId { get { return "ImpersonatedCustomerId"; } }
        public static string AdminAreaStoreScopeConfiguration { get { return "AdminAreaStoreScopeConfiguration"; } }
        public static string TwoFactorEnabled { get { return "TwoFactorEnabled"; } }
        public static string TwoFactorSecretKey { get { return "TwoFactorSecretKey"; } }
        public static string TwoFactorValidCode { get { return "TwoFactorValidCode"; } }
        public static string TwoFactorCodeValidUntil { get { return "TwoFactorValidCodeUntil"; } }

        public static string CurrencyId { get { return "CurrencyId"; } }
        public static string LanguageId { get { return "LanguageId"; } }
        public static string SelectedPaymentMethod { get { return "SelectedPaymentMethod"; } }
        public static string PaymentTransaction { get { return "PaymentTransaction"; } }
        public static string PaymentOptionAttribute { get { return "PaymentOptionAttribute"; } }
        public static string SelectedShippingOption { get { return "SelectedShippingOption"; } }
        //value customer chose "pick up in store" option
        public static string SelectedPickupPoint { get { return "SelectedPickupPoint"; } }
        public static string CheckoutAttributes { get { return "CheckoutAttributes"; } }
        public static string OfferedShippingOptions { get { return "OfferedShippingOptions"; } }
        public static string ShippingOptionAttributeDescription { get { return "ShippingOptionAttributeDescription"; } }
        public static string ShippingOptionAttribute { get { return "ShippingOptionAttribute"; } }
        public static string LastContinueShoppingPage { get { return "LastContinueShoppingPage"; } }
        public static string WorkingThemeName { get { return "WorkingThemeName"; } }
        public static string AdminThemeName { get { return "AdminThemeName"; } }
        public static string TaxDisplayTypeId { get { return "TaxDisplayTypeId"; } }
        public static string UseLoyaltyPointsDuringCheckout { get { return "UseLoyaltyPointsDuringCheckout"; } }
        public static string CookieAccepted { get { return "Cookie.Accepted"; } }
        public static string ConsentCookies { get { return "ConsentCookies"; } }
    }
}