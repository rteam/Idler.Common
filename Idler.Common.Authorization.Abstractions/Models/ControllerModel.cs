using System;
using System.Collections.Generic;
using System.Linq;

namespace Idler.Common.Authorization.Models;

public class ControllerModel
{
    public ControllerModel(Type[] Types)
    {
        this.Controllers = Types.Select(t => new ControllerItemModel(t.Name.Replace("Controller", "")))
            .OrderBy(t => t.Name).ToList();
    }

    public IList<ControllerItemModel> Controllers { get; set; }
}