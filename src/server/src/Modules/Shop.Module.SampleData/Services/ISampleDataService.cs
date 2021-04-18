using Shop.Module.SampleData.ViewModels;
using System.Threading.Tasks;

namespace Shop.Module.SampleData.Services
{
    public interface ISampleDataService
    {
        Task ResetToSampleData(SampleDataOption model);
    }
}
