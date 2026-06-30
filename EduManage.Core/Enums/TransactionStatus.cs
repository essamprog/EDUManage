using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Core.Enums
{
    public enum TransactionStatus
    {
        Pending = 1,
        Matured = 2,
        Paid = 3,
        Refunded = 4,
        Completed = 5
    }
}
