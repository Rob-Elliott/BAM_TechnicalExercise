using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace StargateAPI
{
    // Adds a global prefix to attribute-routed controllers
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _prefix;

        public RoutePrefixConvention(IRouteTemplateProvider route)
        {
            _prefix = new AttributeRouteModel(route);
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                // find selectors with attribute route model
                var hasRouteSelectors = controller.Selectors.Any(s => s.AttributeRouteModel != null);

                if (hasRouteSelectors)
                {
                    foreach (var selector in controller.Selectors.Where(s => s.AttributeRouteModel != null))
                    {
                        selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_prefix, selector.AttributeRouteModel);
                    }
                }
                else
                {
                    // no attribute routes on controller, add the prefix as the route
                    controller.Selectors.Add(new SelectorModel
                    {
                        AttributeRouteModel = _prefix
                    });
                }
            }
        }
    }
}