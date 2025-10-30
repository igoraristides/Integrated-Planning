using PlanejamentoIntegrado.Models;

namespace PlanejamentoIntegrado.Services;

public interface ISupplierService
{
    Task<List<Supplier>> GetAllSuppliers();
}
