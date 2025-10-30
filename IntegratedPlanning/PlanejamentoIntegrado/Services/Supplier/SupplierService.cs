using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Repositories;

namespace PlanejamentoIntegrado.Services;

public class SupplierService(IRepository<Supplier> supplierRepository) : ISupplierService
{
    public async Task<List<Supplier>> GetAllSuppliers()
    {
        return await supplierRepository.GetAll(
            filter: s => s.InactivatedAt == null,
            orderBy: q => q.OrderBy(s => s.SupplierName)
        );
    }
}
