using Cosmetics_Shopping_Website.GenericPattern.Interfaces;
using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Services
{
    public class VariantAvailabilityServices: IVariantAvailabilityServices
    {
        public IGenericRepository _genericRepository;

        public VariantAvailabilityServices(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<IEnumerable<ProductVariant>> GetProductVariants()
        {
            var result= await _genericRepository.GetTable<ProductVariant>();
            return result.Where(e => e.IsDelete == false).ToList();
        }

        public async Task<IEnumerable<State>> GetStates()
        {
            var result = await _genericRepository.GetTable<State>();
            return result.Where(e => e.IsDelete == false).ToList();
        }

        public async Task<VariantAvailabilityVM> CreateVariantAvailability(VariantAvailabilityVM objVariantAvailabilityVM, int logedUser)
        {
            try
            {
                VariantsAvailability variantsAvailability = new();
                variantsAvailability.StateId = objVariantAvailabilityVM.StateId;
                variantsAvailability.ProductVariantId = objVariantAvailabilityVM.ProductVariantId;
                variantsAvailability.IsAvailable = objVariantAvailabilityVM.IsAvailable;
                variantsAvailability.CreatedBy = logedUser;
                variantsAvailability.UpdatedBy = logedUser;
                variantsAvailability.CreatedOn = DateTime.Now;
                variantsAvailability.UpdatedOn = DateTime.Now;
               
                if (variantsAvailability != null)
                {
                    await _genericRepository.Post(variantsAvailability);
                    VariantAvailabilityVM result = new()
                    {
                        Id = variantsAvailability.Id,
                        StateId = variantsAvailability.StateId,
                        ProductVariantId = variantsAvailability.ProductVariantId,
                        IsAvailable = variantsAvailability.IsAvailable
                    };
                    return result;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<VariantAvailabilityVM> GetVariantAvailabilityByIds(int id)
        {
            try
            {
                if (id != 0)
                {
                    var variantAvailabilityDetails = await _genericRepository.GetByIdFromMultipleTable<VariantsAvailability>(id, e => e.State, e=>e.ProductVariant);
                    if (variantAvailabilityDetails != null && variantAvailabilityDetails.IsDelete == false)
                    {
                        VariantAvailabilityVM result = new()
                        {
                            Id = variantAvailabilityDetails.Id,
                            StateId = variantAvailabilityDetails.StateId,
                            ProductVariantId = variantAvailabilityDetails.ProductVariantId,
                            IsAvailable = variantAvailabilityDetails.IsAvailable,
                            State = variantAvailabilityDetails.State.StateName,
                            ProductVariantName = variantAvailabilityDetails.ProductVariant.VariantName
                        };
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }



        public async Task<IEnumerable<VariantAvailabilityVM>> GetAllVariantsAvailability()
        {
            try
            {
                var variantAvailabilityDetailsList = await _genericRepository.GetFromMutlipleTable<VariantsAvailability>(e => e.State, e=>e.ProductVariant);
                return variantAvailabilityDetailsList.Select(e => new VariantAvailabilityVM
                {
                    Id = e.Id,
                    StateId = e.StateId,
                    ProductVariantId = e.ProductVariantId,
                    State = e.State.StateName,
                    ProductVariantName = e.ProductVariant.VariantName,
                    IsAvailable= e.IsAvailable,
                    IsDelete = e.IsDelete
                }).Where(e => e.IsDelete == false).ToList();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<VariantAvailabilityVM> PutVariantAvailability(VariantAvailabilityVM objVariantAvailabilityVM, int logedUser)
        {
            try
            {
                var varaintAvailability = await _genericRepository.GetById<VariantsAvailability>(objVariantAvailabilityVM.Id);
                if (varaintAvailability != null && varaintAvailability.IsDelete == false)
                {
                    varaintAvailability.Id = objVariantAvailabilityVM.Id;
                    varaintAvailability.IsAvailable = objVariantAvailabilityVM.IsAvailable;
                    varaintAvailability.UpdatedOn = DateTime.Now;
                    varaintAvailability.UpdatedBy = logedUser;

                    await _genericRepository.Put(varaintAvailability);
                    VariantAvailabilityVM result = new()
                    {
                        Id = varaintAvailability.Id,
                        StateId=objVariantAvailabilityVM.StateId,
                        ProductVariantId = objVariantAvailabilityVM.ProductVariantId,
                        IsAvailable = objVariantAvailabilityVM.IsAvailable,

                    };
                    return result;

                }

            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<bool> DeleteVariantAvailability(int Id, int logedUser)
        {
            if (Id > 0)
            {
                var variantAvailabilityDetails = await _genericRepository.GetById<VariantsAvailability>(Id);
                if (variantAvailabilityDetails != null)
                {
                    variantAvailabilityDetails.IsDelete = true;
                    variantAvailabilityDetails.UpdatedBy = logedUser;
                    variantAvailabilityDetails.UpdatedOn = DateTime.Now;
                    var result = _genericRepository.Save();


                    if (result > 0)
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
