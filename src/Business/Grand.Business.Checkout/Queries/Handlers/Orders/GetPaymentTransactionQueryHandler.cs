﻿using Grand.Business.Checkout.Queries.Models.Orders;
using Grand.Domain.Data;
using Grand.Domain.Payments;
using MediatR;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grand.Business.Checkout.Queries.Handlers.Orders
{
    public class GetPaymentTransactionQueryHandler : IRequestHandler<GetPaymentTransactionQuery, IMongoQueryable<PaymentTransaction>>
    {
        private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;

        public GetPaymentTransactionQueryHandler(IRepository<PaymentTransaction> paymentTransactionRepository)
        {
            _paymentTransactionRepository = paymentTransactionRepository;
        }

        public Task<IMongoQueryable<PaymentTransaction>> Handle(GetPaymentTransactionQuery request, CancellationToken cancellationToken)
        {
            var query = _paymentTransactionRepository.Table;

            if (!string.IsNullOrEmpty(request.StoreId))
                query = query.Where(rr => request.StoreId == rr.StoreId);

            if (!string.IsNullOrEmpty(request.CustomerEmail))
                query = query.Where(rr => request.CustomerEmail == rr.CustomerEmail.ToLowerInvariant());

            if (request.OrderGuid.HasValue)
                query = query.Where(rr => request.OrderGuid == request.OrderGuid.Value);

            if (request.Ts.HasValue)
                query = query.Where(rr => rr.TransactionStatus == request.Ts.Value);

            if (request.CreatedFromUtc.HasValue)
                query = query.Where(rr => request.CreatedFromUtc.Value <= rr.CreatedOnUtc);

            if (request.CreatedToUtc.HasValue)
                query = query.Where(rr => request.CreatedToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            return Task.FromResult(query);
        }
    }
}
