namespace Idler.Common.Authorization.Models;

public class ControllerItemModel
{
    public ControllerItemModel(string Name)
    {
        this.Id = Name;
        this.Name = Name;
    }

    public string Id { get; set; }
    public string Name { get; set; }
}