﻿using AutoMapper;
using Grand.Infrastructure.Mapper;
using Widgets.Slider.Domain;
using Widgets.Slider.Models;

namespace Widgets.Slider.Infrastructure.Mapper
{
    public class SliderMapperConfiguration : Profile, IAutoMapperProfile
    {
        protected string SetObjectEntry(SlideModel model)
        {
            if (model.SliderTypeId == (int)SliderType.HomePage)
                return "";

            if (model.SliderTypeId == (int)SliderType.Category)
                return model.CategoryId;

            if (model.SliderTypeId == (int)SliderType.Collection)
                return model.CollectionId;

            return "";
        }
        protected string GetCategoryId(PictureSlider pictureSlider)
        {
            if (pictureSlider.SliderType == SliderType.Category)
                return pictureSlider.ObjectEntry;

            return "";
        }

        protected string GetCollectionId(PictureSlider pictureSlider)
        {
            if (pictureSlider.SliderType == SliderType.Collection)
                return pictureSlider.ObjectEntry;

            return "";
        }

        public SliderMapperConfiguration()
        {
            CreateMap<SlideModel, PictureSlider>()
                .ForMember(dest => dest.ObjectEntry, mo => mo.MapFrom(x => SetObjectEntry(x)))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<PictureSlider, SlideModel>()
                .ForMember(dest => dest.CategoryId, mo => mo.MapFrom(x => GetCategoryId(x)))
                .ForMember(dest => dest.CollectionId, mo => mo.MapFrom(x => GetCollectionId(x)))
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            CreateMap<SlideListModel, PictureSlider>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.Stores, mo => mo.Ignore());

            CreateMap<PictureSlider, SlideListModel>()
                .ForMember(dest => dest.UserFields, mo => mo.Ignore())
                .ForMember(dest => dest.PictureUrl, mo => mo.Ignore())
                .ForMember(dest => dest.ObjectType, mo => mo.MapFrom(y => y.SliderType.ToString()));
        }
        public int Order
        {
            get { return 0; }
        }
    }
}
