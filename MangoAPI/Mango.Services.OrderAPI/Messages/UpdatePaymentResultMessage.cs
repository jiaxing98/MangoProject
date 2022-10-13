﻿using Mango.MessageBus;

namespace Mango.Service.OrderAPI.Messages
{
    public class UpdatePaymentResultMessage : BaseMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
    }
}