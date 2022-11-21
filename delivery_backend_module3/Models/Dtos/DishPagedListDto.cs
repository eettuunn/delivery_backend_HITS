namespace delivery_backend_module3.Models.Dtos;

public class DishPagedListDto
{
    public List<DishDto>? dishes { get; set; }
    
    public PageInfoModel pagination { get; set; }
}