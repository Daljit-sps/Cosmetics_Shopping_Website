using Cosmetics_Shopping_Website.GenericPattern.Models;
using Cosmetics_Shopping_Website.GenericPattern.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmetics_Shopping_Website.GenericPattern.Interfaces
{
    public interface IVariantAvailabilityServices
    {
        Task<IEnumerable<ProductVariant>> GetProductVariants();
        Task<IEnumerable<State>> GetStates();

        Task<VariantAvailabilityVM> CreateVariantAvailability(VariantAvailabilityVM objVariantAvailabilityVM, int logedUser);
        Task<VariantAvailabilityVM> GetVariantAvailabilityByIds(int id);
        Task<IEnumerable<VariantAvailabilityVM>> GetAllVariantsAvailability();

        Task<VariantAvailabilityVM> PutVariantAvailability(VariantAvailabilityVM objVariantAvailabilityVM, int logedUser);

        Task<bool> DeleteVariantAvailability(int Id, int logedUser);
    }
}
