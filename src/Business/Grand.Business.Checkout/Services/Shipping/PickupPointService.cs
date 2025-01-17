﻿using Grand.Business.Checkout.Interfaces.Shipping;
using Grand.Infrastructure.Caching;
using Grand.Infrastructure.Caching.Constants;
using Grand.Infrastructure.Extensions;
using Grand.Domain.Data;
using Grand.Domain.Shipping;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grand.Business.Checkout.Services.Shipping
{
    public class PickupPointService : IPickupPointService
    {
        #region Fields

        private readonly IRepository<PickupPoint> _pickupPointsRepository;
        private readonly IMediator _mediator;
        private readonly ICacheBase _cacheBase;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public PickupPointService(
            IRepository<PickupPoint> pickupPointsRepository,
            IMediator mediator,
            ICacheBase cacheBase)
        {
            _pickupPointsRepository = pickupPointsRepository;
            _mediator = mediator;
            _cacheBase = cacheBase;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">The pickup point identifier</param>
        /// <returns>Delivery date</returns>
        public virtual Task<PickupPoint> GetPickupPointById(string pickupPointId)
        {
            var key = string.Format(CacheKey.PICKUPPOINTS_BY_ID_KEY, pickupPointId);
            return _cacheBase.GetAsync(key, () => _pickupPointsRepository.GetByIdAsync(pickupPointId));
        }

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <returns>Warehouses</returns>
        public virtual async Task<IList<PickupPoint>> GetAllPickupPoints()
        {
            return await _cacheBase.GetAsync(CacheKey.PICKUPPOINTS_ALL, () =>
            {
                var query = from pp in _pickupPointsRepository.Table
                            orderby pp.DisplayOrder
                            select pp;
                return query.ToListAsync();
            });
        }

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <returns>Warehouses</returns>
        public virtual async Task<IList<PickupPoint>> LoadActivePickupPoints(string storeId = "")
        {
            var pickupPoints = await GetAllPickupPoints();
            return pickupPoints.Where(pp => pp.StoreId == storeId || string.IsNullOrEmpty(pp.StoreId)).ToList();
        }


        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public virtual async Task InsertPickupPoint(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _pickupPointsRepository.InsertAsync(pickupPoint);

            //clear cache
            await _cacheBase.RemoveByPrefix(CacheKey.PICKUPPOINTS_PATTERN_KEY);

            //event notification
            await _mediator.EntityInserted(pickupPoint);
        }

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        public virtual async Task UpdatePickupPoint(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _pickupPointsRepository.UpdateAsync(pickupPoint);

            //clear cache
            await _cacheBase.RemoveByPrefix(CacheKey.PICKUPPOINTS_PATTERN_KEY);

            //event notification
            await _mediator.EntityUpdated(pickupPoint);
        }

        /// <summary>
        /// Deletes a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        public virtual async Task DeletePickupPoint(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            await _pickupPointsRepository.DeleteAsync(pickupPoint);

            //clear cache
            await _cacheBase.RemoveByPrefix(CacheKey.PICKUPPOINTS_PATTERN_KEY);

            //event notification
            await _mediator.EntityDeleted(pickupPoint);
        }


        #endregion
    }
}
