using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public static class SD
    {
        //uloge za korisnika
        public const string Role_Customer = "Customer"; //register and place order
        public const string Role_Company = "Company"; //special user, ne mora da izvrsi placanje odmah(ima 30 dana nakon pravljenja porudzbine)
        public const string Role_Admin = "Admin"; //moze da upravlja svim operacijama i content managementom
        public const string Role_Employee= "Employee"; //moze da modifuje shipping info and details

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusRefunded= "Refunded";
        public const string StatusCancelled = "Cancelled";
        public const string StatusShiped = "Shiped";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        public const string SessionCart = "SessionShoppingCart";

    }
}
