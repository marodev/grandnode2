﻿using Grand.Business.Catalog.Commands.Models;
using Grand.Infrastructure.Caching;
using Grand.Infrastructure.Caching.Constants;
using Grand.Infrastructure.Extensions;
using Grand.Domain.Catalog;
using Grand.Domain.Data;
using MediatR;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grand.Business.Catalog.Commands.Handlers
{
    public class UpdateIntervalPropertiesCommandHandler : IRequestHandler<UpdateIntervalPropertiesCommand, bool>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMediator _mediator;
        private readonly ICacheBase _cacheBase;

        public UpdateIntervalPropertiesCommandHandler(IRepository<Product> productRepository, IMediator mediator, ICacheBase cacheBase)
        {
            _productRepository = productRepository;
            _mediator = mediator;
            _cacheBase = cacheBase;
        }

        public async Task<bool> Handle(UpdateIntervalPropertiesCommand request, CancellationToken cancellationToken)
        {
            if (request.Product == null)
                throw new ArgumentNullException(nameof(request.Product));

            var filter = Builders<Product>.Filter.Eq("Id", request.Product.Id);
            var update = Builders<Product>.Update
                    .Set(x => x.Interval, request.Interval)
                    .Set(x => x.IntervalUnitId, request.IntervalUnit)
                    .Set(x => x.IncBothDate, request.IncludeBothDates);

            await _productRepository.Collection.UpdateOneAsync(filter, update);

            //cache
            await _cacheBase.RemoveByPrefix(string.Format(CacheKey.PRODUCTS_BY_ID_KEY, request.Product.Id));

            //event notification
            await _mediator.EntityUpdated(request.Product);

            return true;
        }
    }
}
