using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace ExpenseFlow.Application.Services.Helper;

#pragma warning disable CS0618
public class DynamicLinqCustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
{
    public DynamicLinqCustomTypeProvider() : base()
    {
    }
}
#pragma warning restore CS0618