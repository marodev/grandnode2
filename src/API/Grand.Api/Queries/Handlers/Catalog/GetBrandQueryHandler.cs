﻿using Grand.Api.DTOs.Catalog;
using Grand.Api.Queries.Models.Common;
using Grand.Domain.Data;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grand.Api.Queries.Handlers.Common
{
    public class GetBrandQueryHandler : IRequestHandler<GetQuery<BrandDto>, IMongoQueryable<BrandDto>>
    {
        private readonly IMongoDBContext _mongoDBContext;

        public GetBrandQueryHandler(IMongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }
        public Task<IMongoQueryable<BrandDto>> Handle(GetQuery<BrandDto> request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Id))
                return Task.FromResult(
                    _mongoDBContext.Database()
                    .GetCollection<BrandDto>
                    (typeof(Domain.Catalog.Brand).Name)
                    .AsQueryable());
            else
                return Task.FromResult(_mongoDBContext.Database()
                    .GetCollection<BrandDto>(typeof(Domain.Catalog.Brand).Name)
                    .AsQueryable()
                    .Where(x => x.Id == request.Id));
        }
    }
}
