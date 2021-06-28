using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreApp.Enums
{
    public enum Size
    {
        [Display(Name="36")]
        _36 = 1,
        [Display(Name="38")]
        _38 = 2,
        [Display(Name="40")]
        _40 = 3 
    }
}